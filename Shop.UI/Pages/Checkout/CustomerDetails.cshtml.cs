using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Cart;
using Shop.Application.Oders;
using Shop.Application.Payment;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.EmailTemplates;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Checkout
{
    public class CustomerDetailsModel : BasePage
    {
        private readonly ApplicationDbContext _ctx;
        private readonly OneOffPayment _payment;
        private readonly SetCartAddress _setCartAddress;
        private readonly CreateOrder _createOrder;
        private readonly EmailSender _emailSender;

        public CustomerDetailsModel(
            ApplicationDbContext ctx,
            OneOffPayment payment,
            SetCartAddress setCartAddress,
            CreateOrder createOrder,
            EmailSender emailSender)
        {
            _ctx = ctx;
            _payment = payment;
            _setCartAddress = setCartAddress;
            _createOrder = createOrder;
            _emailSender = emailSender;
        }

        public List<Location> Locations { get; private set; }
        public bool NeedsRef { get; private set; }

        public Location SelectedLocation { get; set; }

        [BindProperty]
        public CustomerDetailViewModel Input { get; set; }

        [BindProperty]
        public CustomerDetailAddressViewModel AddressInput { get; set; }
        public bool IsBusiness { get; private set; }
        public string TypeOfUser { get; set; }
        public Account account { get; set; }
        public bool is30dayAccount { get; set; }

        public async Task OnGet()
        {
            var user = GetUserId();

            if (User.Identity.IsAuthenticated)
            {
                var userType = GetUserType();
                IsBusiness = userType != "user";
                NeedsRef = userType == "authorizer" || userType == "superuser";
                TypeOfUser = userType;

                Input = new CustomerDetailViewModel()
                {
                    UserType = userType
                };

                if (IsBusiness)
                {
                    var accountId = _ctx.AccountUsers.FirstOrDefault(x => x.UserId == user);

                    account = _ctx.Accounts.FirstOrDefault(x => x.Id == accountId.AccountId);
                    is30dayAccount = account.ThirtyDayTermApproved;

                    await LoadLocationsCostCenters(userType);
                }
            }
        }

        public async Task<IActionResult> OnPostUser()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = _ctx.Users.FirstOrDefault(x => x.Id == GetUserId());

            user.PhoneNumber = AddressInput.PhoneNumber;

            var address = new Address
            {
                Address1 = AddressInput.Address1,
                Address2 = AddressInput.Address2,
                City = AddressInput.City,
                Country = AddressInput.Country,
                PostCode = AddressInput.PostCode
            };

            user.Addresses.Add(address);

            await _ctx.SaveChangesAsync();

            var mark = GetCartUserMark();

            await _setCartAddress.Do(new SetCartAddress.Request
            {
                AddressId = address.Id,
                UserMark = mark
            });

            return RedirectToPage("/Checkout/Payment");
        }

        public async Task<IActionResult> OnPostBusiness()
        {
            //var userType = GetUserType();
            //NeedsRef = userType == "superuser" || userType == "authorizer";

            if (/*NeedsRef && */string.IsNullOrEmpty(Input.OrderReference))
            {
                ModelState.AddModelError("Input.OrderReference", "Order Reference is required.");
            }
            if (Input.AddressId == 0)
            {
                ModelState.AddModelError("Input.AddressId", "Address is required.");
            }

            if (!ModelState.IsValid)
            {
                IsBusiness = Input.UserType != "user";
                await LoadLocationsCostCenters(Input.UserType);
                return Page();
            }

            var activeAccount = _ctx.AccountUsers
                .Include(x => x.Account)
                .FirstOrDefault(x => x.UserId == GetUserId() && x.Active);

            //if (Input.AdHoc)
            //    if (!ModelState.IsValid)
            //        return Page();
            //    else
            //        await SetAdhocAddress();

            if (!activeAccount.Account.TermAccount || !activeAccount.Account.ThirtyDayTermApproved)
            {
                await _setCartAddress.Do(new SetCartAddress.Request
                {
                    UserMark = GetCartUserMark(),
                    AddressId = Input.AddressId,
                    CostCenterId = Input.CostCenterId
                });

                return RedirectToPage("/Checkout/Payment");
            }

            bool placed = await PlaceOrder(activeAccount.AccountId);

            return RedirectToPage("/Checkout/Confirmation", new { placed });
        }

        private async Task LoadLocationsCostCenters(string userType)
        {
            var userId = GetUserId();
            var accountUser = await _ctx.AccountUsers
            .Include(x => x.Account)
                .ThenInclude(x => x.Locations)
                .ThenInclude(x => x.Address)
            .Include(x => x.Account)
                .ThenInclude(x => x.Locations)
                .ThenInclude(x => x.LocationAuths)
            .Include(x => x.Account)
                .ThenInclude(x => x.Locations)
                .ThenInclude(x => x.CostCenters)
                .ThenInclude(x => x.CostCenterAuth)
            .FirstOrDefaultAsync(x => x.UserId == GetUserId() && x.Active);

            Locations = new List<Location>();
            foreach (var location in accountUser.Account.Locations)
            {
                List<CostCenter> costCenters = new List<CostCenter>();
                if (userType == "businessuser")
                {
                    costCenters = location.CostCenters.Where(c => c.CostCenterAuth.Any(ca => ca.Userid == userId)).ToList();
                    if (location.LocationAuths.Any(la => la.Userid == userId) || costCenters.Any())
                    {
                        Locations.Add(location);
                    }
                }
                else if (userType == "authorizer")
                {
                    costCenters = location.CostCenters.Where(c => c.AuthorizerId == userId).ToList();
                    if (location.AuthorizerId == userId)
                    {
                        Locations.Add(location);
                    }
                    else if (costCenters.Any())
                    {
                        // Add location by default for costcenter's authorizer
                        Locations.Add(location);
                    }
                }
                else if (userType == "superuser")
                {
                    Locations.Add(location);
                }

                if (costCenters.Any())
                {
                    location.CostCenters = costCenters;
                    //CostCenters.AddRange(costCenters);
                }
            }
        }

        private async Task SetAdhocAddress()
        {
            var address = new Address
            {
                Address1 = AddressInput.Address1,
                Address2 = AddressInput.Address2,
                City = AddressInput.City,
                Country = AddressInput.Country,
                PostCode = AddressInput.PostCode
            };

            _ctx.Addresses.Add(address);

            await _ctx.SaveChangesAsync();

            await _setCartAddress.Do(new SetCartAddress.Request
            {
                AddressId = address.Id,
                UserMark = GetCartUserMark()
            });
        }

        /// <summary>
        /// Returns true if the order is approved
        /// This means the order was placed by either an authoriser or a super user
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private async Task<bool> PlaceOrder(int accountId)
        {
            DateTime now = DateTime.UtcNow;
            var mark = GetCartUserMark();
            bool approved = true;
            Location location = await _ctx.Locations
                                          .Include(l => l.Address)
                                          .Include(l => l.Authorizer)
                                          .FirstOrDefaultAsync(l => l.Id == Input.LocationId);
            ApplicationUser authorizer = null;

            if (Input.UserType == "businessuser")
            {
                if (Input.CostCenterId != 0)
                {
                    var costCenter = await _ctx.CostCenters
                                               .Include(c => c.Authorizer)
                                               .FirstOrDefaultAsync(c => c.Id == Input.CostCenterId);

                    if (!string.IsNullOrEmpty(costCenter.AuthorizerId))
                    {
                        approved = false;
                        authorizer = costCenter.Authorizer;
                    }
                }
                else
                {
                    //var location = await _ctx.Locations
                    //                         .Include(l => l.Authorizer)
                    //                         .FirstOrDefaultAsync(l => l.Id == Input.LocationId);

                    if (!string.IsNullOrEmpty(location.AuthorizerId))
                    {
                        approved = false;
                        authorizer = location.Authorizer;
                    }
                }
            }

            await _setCartAddress.Do(new SetCartAddress.Request
            {
                UserMark = mark,
                AddressId = Input.AddressId,
                CostCenterId = Input.CostCenterId
            });

            var orderRequest = new CreateOrder.Request
            {
                UserMark = mark,
                AccountId = accountId,
                OrderReference = Input.OrderReference,
                Approved = approved,
                Term = true,
                LocationId = Input.LocationId
            };

            if (Input.CostCenterId != 0)
            {
                orderRequest.CostCenterId = Input.CostCenterId;
            }

            var order = await _createOrder.Do(orderRequest);

            var me = await _ctx.Users.FindAsync(GetUserId());
            if (authorizer != null)
            {

                CostCenter costCenter = new CostCenter();
                if (Input.CostCenterId != 0)
                {
                    costCenter = await _ctx.CostCenters.FindAsync(Input.CostCenterId);
                }

                //string orderUrl = Url.Page("/BusinessProfile/Orders", null, new { redirect = true }, Request.Scheme);
                string orderUrl = Url.Action("Orders", "BusinessProfile", null, Request.Scheme);

                Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", authorizer.FirstName) },
                    { "greetingdescription", string.Format("Please note {0} {1} has placed an order which requires your approval.", me.FirstName, me.LastName) },
                    { "orderlink", string.Format("<a href='{0}'>Click here</a>", orderUrl) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", now.ToString("dd MMM yyyy") },
                    { "ordertime", now.ToString("hh:mm:ss") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", location.Name, location.Address.Address1, location.Address.Address2, location.Address.City, location.Address.PostCode) },
                    { "costcentre", costCenter.Name ?? "No cost center." }
                };

                Message m = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Order {0} - Pending Approval", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = authorizer.Email, Name = authorizer.FirstName }
                    }
                };

                _emailSender.SendEmailByTemplate(vars, "authoriser", m);

                Dictionary<string, string> vars2 = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", me.FirstName) },
                    { "greetingdescription", string.Format("Your requested order {0} has been sent for approval and we will notify you as soon as it's been approved.", order.OrderNumber) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", now.ToString("dd MMM yyyy") },
                    { "ordertime", now.ToString("hh:mm:ss") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", location.Name, location.Address.Address1, location.Address.Address2, location.Address.City, location.Address.PostCode) },
                    { "costcentre", costCenter.Name ?? "No cost center." }
                };

                Message m2 = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Order {0} - Your order has been sent for approval", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = me.Email, Name = me.FirstName }
                    }
                };

                _emailSender.SendEmailByTemplate(vars2, "userscenario2", m);
            }

            if (orderRequest.Approved)
            {
                string cmsLink = Url.Action("SingleOrder", "Admin", new { id = order.OrderNumber }, Request.Scheme);

                Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", now.ToString("dd MMM yyyy") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", location.Name, location.Address.Address1, location.Address.Address2, location.Address.City, location.Address.PostCode) },
                    { "cmslink", string.Format("<a href='{0}'>{0}</a>", cmsLink) },
                };

                Message m = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("OfficeBox Order Placed - {0}", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = "orders@officebox.co.za", Name = "OfficeBox" }
                    }
                };

                _emailSender.SendEmailByTemplate(vars, "internal-order-confirmation", m);

                Dictionary<string, string> vars2 = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", me.FirstName) },
                    { "greetingdescription", string.Format("Your order {0} has been confirmed and is being processed accordingly.", order.OrderNumber) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", now.ToString("dd MMM yyyy") },
                    { "ordertime", now.ToString("hh:mm tt") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", location.Name, location.Address.Address1, location.Address.Address2, location.Address.City, location.Address.PostCode) },
                };

                Message m2 = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Awesome - We’ve received the order - {0}", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = me.Email, Name = me.FirstName }
                    }
                };

                _emailSender.SendEmailByTemplate(vars2, "finalcomfirmation", m2);
            }

            return approved;
        }
    }

    public class CustomerDetailViewModel
    {
        public string UserType { get; set; }
        public string OrderReference { get; set; }
        public int AddressId { get; set; }
        public int LocationId { get; set; }
        public int CostCenterId { get; set; }
        public bool AdHoc { get; set; }
    }

    public class CustomerDetailAddressViewModel
    {
        //[Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        //[Required]
        public string City { get; set; }
        public string Country { get; set; } = "South Africa";
        //[Required]
        public string PostCode { get; set; }
        //[Required]
        public string PhoneNumber { get; set; }
    }
}