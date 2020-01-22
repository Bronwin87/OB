using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.Timezone;
using Shop.Database;
using Shop.UI.Models;
using Shop.UI.Utilities;

namespace Shop.UI.Controllers
{
    public class DataTableController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DataTableController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> GetData(DataTablesPageRequest model, string dataSource)
        {
            // this datasource thing is here because I'm thinking of figuring out how to make the whole iqueryable part
            // completely dynamic, then changing the "GetOrders" action to a "GetData" or (or whatever relevant name) action
            switch (dataSource)
            {
                case "Orders":
                    Page<OrderViewModel> ordersPage = SearchOrdersQuery(model);

                    var ordersPageResponse = new
                    {
                        model.draw,
                        recordsTotal = ordersPage.TotalItems,
                        recordsFiltered = ordersPage.TotalDisplayItems,
                        data = ordersPage.Items.ToArray()
                    };

                    return Json(ordersPageResponse);

                case "Accounts":
                    Page<AccountViewModel> accountsPage = SearchAccountsQuery(model);

                    var accountsPageResponse = new
                    {
                        model.draw,
                        recordsTotal = accountsPage.TotalItems,
                        recordsFiltered = accountsPage.TotalDisplayItems,
                        data = accountsPage.Items.ToArray()
                    };

                    return Json(accountsPageResponse);

                case "Users":
                    Page<UserViewModel> usersPage = SearchUsersQuery(model);

                    var usersPageResponse = new
                    {
                        model.draw,
                        recordsTotal = usersPage.TotalItems,
                        recordsFiltered = usersPage.TotalDisplayItems,
                        data = usersPage.Items.ToArray()
                    };

                    return Json(usersPageResponse);
            }

            return Json("Not Implemented Exception");
        }

        private Page<OrderViewModel> SearchOrdersQuery(DataTablesPageRequest model)
        {
            IQueryable<OrderViewModel> query = from order in _context.Orders
                                               join address in _context.Addresses on order.AddressId equals address.Id
                                               join user in _context.Users on order.UserId equals user.Id
                                               select new OrderViewModel()
                                               {
                                                   Id = order.Id,
                                                   Created = order.Created.ConvertTimeToLocal(),
                                                   OrderNumber = order.OrderNumber,
                                                   User = user.Email,
                                                   Location = address.City,
                                                   Total = order.SubTotal,
                                                   Cin7Id = order.Cin7ID,
                                                   Action = "<a class='button is-block secondary' style='height: 24px;' href='/Admin/SingleOrder/" + order.OrderNumber + "'>View</a>",
                                               };

            Page<OrderViewModel> page = new Page<OrderViewModel>
            {
                TotalItems = query.Count()
            };

            string orderByName = string.Empty;
            Order orderBy = new Order();
            if (model.order != null)
            {
                orderBy = model.order.First();
                orderByName = model.columns[orderBy.column].name;
            }

            // Apply OR filters to IQueryable
            #region ApplyFilters

            string searchTerm = model.search.value;

            Predicate<OrderViewModel> globalPredicate = PredicateExtensions.False<OrderViewModel>();
            Predicate<OrderViewModel> idPredicate = PredicateExtensions.False<OrderViewModel>();
            Predicate<OrderViewModel> createdPredicate = PredicateExtensions.False<OrderViewModel>();
            Predicate<OrderViewModel> orderNumberPredicate = PredicateExtensions.False<OrderViewModel>();
            Predicate<OrderViewModel> userPredicate = PredicateExtensions.False<OrderViewModel>();
            Predicate<OrderViewModel> locationPredicate = PredicateExtensions.False<OrderViewModel>();
            Predicate<OrderViewModel> totalPredicate = PredicateExtensions.False<OrderViewModel>();

            bool anyGlobal = false,
                 anyId = false,
                 anyCreated = false,
                 anyOrderNumber = false,
                 anyUser = false,
                 anyLocation = false,
                 anyTotal = false;

            foreach (var column in model.columns)
            {
                string columnSearchTerm = column.search.value;

                switch (column.name)
                {
                    case "Id":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Id);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Id);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(OrderViewModel o) => o.Id.ToString().Contains(searchTerm);
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(OrderViewModel o) => o.Id.ToString().Contains(columnSearchTerm);
                                idPredicate = idPredicate.Or(colPredicate);
                                anyId = true;
                            }
                        }

                        break;

                    case "Created":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Created);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Created);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(OrderViewModel o) => o.Created.ToString().Contains(searchTerm);
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(OrderViewModel o) => o.Created.ToString().Contains(columnSearchTerm);
                                createdPredicate = createdPredicate.Or(colPredicate);
                                anyCreated = true;
                            }
                        }

                        break;

                    case "OrderNumber":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.OrderNumber);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.OrderNumber);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(OrderViewModel o) => !string.IsNullOrEmpty(o.OrderNumber) ? o.OrderNumber.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(OrderViewModel o) => !string.IsNullOrEmpty(o.OrderNumber) ? o.OrderNumber.ToUpper().Contains(columnSearchTerm.ToUpper()) : false;
                                orderNumberPredicate = orderNumberPredicate.Or(colPredicate);
                                anyOrderNumber = true;
                            }
                        }

                        break;

                    case "User":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.User);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.User);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(OrderViewModel o) => !string.IsNullOrEmpty(o.User) ? o.User.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(OrderViewModel o) => !string.IsNullOrEmpty(o.User) ? o.User.ToUpper().Contains(columnSearchTerm.ToUpper()) : false;
                                userPredicate = userPredicate.Or(colPredicate);
                                anyUser = true;
                            }
                        }

                        break;

                    case "Location":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Location);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Location);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(OrderViewModel o) => !string.IsNullOrEmpty(o.Location) ? o.Location.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(OrderViewModel o) => !string.IsNullOrEmpty(o.Location) ? o.Location.ToUpper().Contains(columnSearchTerm.ToUpper()) : false;
                                locationPredicate = locationPredicate.Or(colPredicate);
                                anyLocation = true;
                            }
                        }

                        break;

                    case "Total":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Total);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Total);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(OrderViewModel o) => o.Total.ToString().Contains(searchTerm);
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(OrderViewModel o) => o.Total.ToString().Contains(columnSearchTerm);
                                totalPredicate = totalPredicate.Or(colPredicate);
                                anyTotal = true;
                            }
                        }

                        break;
                }
            }

            if (anyGlobal)
            {
                bool funcOr(OrderViewModel l) => globalPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyId)
            {
                bool funcOr(OrderViewModel l) => idPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyCreated)
            {
                bool funcOr(OrderViewModel l) => createdPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyOrderNumber)
            {
                bool funcOr(OrderViewModel l) => orderNumberPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyUser)
            {
                bool funcOr(OrderViewModel l) => userPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyLocation)
            {
                bool funcOr(OrderViewModel l) => locationPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyTotal)
            {
                bool funcOr(OrderViewModel l) => totalPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }

            #endregion

            var startPage = (model.length == 0) ? 1 : model.start / model.length;

            page.CurrentPage = startPage;
            page.ItemsPerPage = model.length;
            page.TotalPages = query.Count() / page.ItemsPerPage;
            page.TotalDisplayItems = query.Count();

            page.Items = query.Skip(startPage * model.length).Take(model.length).ToList();

            return page;
        }

        private Page<AccountViewModel> SearchAccountsQuery(DataTablesPageRequest model)
        {
            IQueryable<AccountViewModel> query = from account in _context.Accounts
                                                 join address in _context.Addresses on account.AddressId equals address.Id
                                                 select new AccountViewModel()
                                                 {
                                                     Id = account.Id,
                                                     CompanyName = account.CompanyName ?? "",
                                                     RegistrationNumber = account.RegistrationNumber ?? "",
                                                     VatNumber = account.VatNumber ?? "",
                                                     PhoneNumber = account.PhoneNumber ?? "",
                                                     Email = account.PhoneNumber ?? "",
                                                     Address = string.Format("{0} {1}, {2}, {3}",
                                                                              address.Address1,
                                                                              address.Address2,
                                                                              address.City,
                                                                              address.PostCode),
                                                     Limit = account.Limit,
                                                     Action = account.TermAccount ? account.ThirtyDayTermApproved
                                                             ? "<a class='button is-danger' href='/Admin/AccountManager?id=" + account.Id + "&handler=Disable'>Disable</a>"
                                                             : "<a class='button is-success' href='/Admin/AccountManager?id=" + account.Id + "&handler=Approve'>Approve</a>"
                                                             : "",
                                                     AccountLink = "<a class='button is-primary' href='/Admin/AccountView?id=" + account.Id + "'>View</a>"
                                                 };

            Page<AccountViewModel> page = new Page<AccountViewModel>
            {
                TotalItems = query.Count()
            };

            string orderByName = string.Empty;
            Order orderBy = new Order();
            if (model.order != null)
            {
                orderBy = model.order.First();
                orderByName = model.columns[orderBy.column].name;
            }

            // Apply OR filters to IQueryable
            #region ApplyFilters

            string searchTerm = model.search.value;

            Predicate<AccountViewModel> globalPredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> idPredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> companyNamePredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> registrationNumberPredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> vatNumbnerPredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> phoneNumberPredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> emailPredicate = PredicateExtensions.False<AccountViewModel>();
            Predicate<AccountViewModel> addressPredicate = PredicateExtensions.False<AccountViewModel>();

            bool anyGlobal = false,
                 anyId = false,
                 anyCompanyName = false,
                 anyRegistrationNumber = false,
                 anyVatNumber = false,
                 anyPhoneNumber = false,
                 anyEmail = false,
                 anyAddress = false;

            foreach (var column in model.columns)
            {
                string columnSearchTerm = column.search.value;

                switch (column.name)
                {
                    case "Id":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Id);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Id);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => o.Id.ToString().Contains(searchTerm);
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => o.Id.ToString().Contains(columnSearchTerm);
                                idPredicate = idPredicate.Or(colPredicate);
                                anyId = true;
                            }
                        }

                        break;

                    case "CompanyName":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.CompanyName);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.CompanyName);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => !string.IsNullOrEmpty(o.CompanyName) ? o.CompanyName.ToUpper().ToString().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => !string.IsNullOrEmpty(o.CompanyName) ? o.CompanyName.ToUpper().ToString().Contains(columnSearchTerm.ToUpper()) : false;
                                companyNamePredicate = companyNamePredicate.Or(colPredicate);
                                anyCompanyName = true;
                            }
                        }

                        break;

                    case "RegistrationNumber":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.RegistrationNumber);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.RegistrationNumber);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => !string.IsNullOrEmpty(o.RegistrationNumber) ? o.RegistrationNumber.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => !string.IsNullOrEmpty(o.RegistrationNumber) ? o.RegistrationNumber.ToUpper().Contains(columnSearchTerm.ToUpper()) : false;
                                registrationNumberPredicate = registrationNumberPredicate.Or(colPredicate);
                                anyRegistrationNumber = true;
                            }
                        }

                        break;

                    case "VatNumber":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.VatNumber);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.VatNumber);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => !string.IsNullOrEmpty(o.VatNumber) ? o.VatNumber.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => !string.IsNullOrEmpty(o.VatNumber) ? o.VatNumber.ToUpper().Contains(columnSearchTerm.ToUpper()) : false;
                                vatNumbnerPredicate = vatNumbnerPredicate.Or(colPredicate);
                                anyVatNumber = true;
                            }
                        }

                        break;

                    case "PhoneNumber":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.PhoneNumber);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.PhoneNumber);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => o.PhoneNumber.Contains(searchTerm);
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => o.PhoneNumber.Contains(columnSearchTerm);
                                phoneNumberPredicate = phoneNumberPredicate.Or(colPredicate);
                                anyPhoneNumber = true;
                            }
                        }

                        break;

                    case "Email":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Email);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Email);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => !string.IsNullOrEmpty(o.Email) ? o.Email.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => !string.IsNullOrEmpty(o.Email) ? o.Email.ToUpper().Contains(columnSearchTerm.ToUpper()) : false;
                                emailPredicate = emailPredicate.Or(colPredicate);
                                anyEmail = true;
                            }
                        }

                        break;

                    case "Address":
                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(AccountViewModel o) => !string.IsNullOrEmpty(o.Address) ? o.Address.ToUpper().Contains(searchTerm.ToUpper()) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(AccountViewModel o) => o.Address.ToUpper().Contains(columnSearchTerm.ToUpper());
                                addressPredicate = addressPredicate.Or(colPredicate);
                                anyAddress = true;
                            }
                        }

                        break;
                }
            }

            if (anyGlobal)
            {
                bool funcOr(AccountViewModel l) => globalPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyId)
            {
                bool funcOr(AccountViewModel l) => idPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyCompanyName)
            {
                bool funcOr(AccountViewModel l) => companyNamePredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyRegistrationNumber)
            {
                bool funcOr(AccountViewModel l) => registrationNumberPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyVatNumber)
            {
                bool funcOr(AccountViewModel l) => vatNumbnerPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyPhoneNumber)
            {
                bool funcOr(AccountViewModel l) => phoneNumberPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyEmail)
            {
                bool funcOr(AccountViewModel l) => emailPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyAddress)
            {
                bool funcOr(AccountViewModel l) => addressPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }

            #endregion

            var startPage = (model.length == 0) ? 1 : model.start / model.length;

            page.CurrentPage = startPage;
            page.ItemsPerPage = model.length;
            page.TotalPages = query.Count() / page.ItemsPerPage;
            page.TotalDisplayItems = query.Count();

            page.Items = query.Skip(startPage * model.length).Take(model.length).ToList();

            return page;
        }

        private Page<UserViewModel> SearchUsersQuery(DataTablesPageRequest model)
        {
            IQueryable<UserViewModel> query = from user in _context.Users
                                              from accountUser in _context.AccountUsers.Where(au => au.UserId == user.Id).DefaultIfEmpty()
                                              from account in _context.Accounts.Where(a => a.Id == accountUser.AccountId).DefaultIfEmpty()
                                                  //join accountUser in _context.AccountUsers on user.Id equals accountUser.UserId
                                                  //join account in _context.Accounts on accountUser.AccountId equals account.Id
                                              select new UserViewModel()
                                              {
                                                  Id = user.Id,
                                                  FirstName = user.FirstName,
                                                  LastName = user.LastName,
                                                  Email = user.Email,
                                                  Username = user.UserName,
                                                  Account = account != null ? account.CompanyName : string.Empty,
                                                  //Account = "",
                                                  Type = user.Type.ToString(),
                                                  UserLink = "<a class='button is-primary' href='/Admin/UserView?id=" + user.Id + "'>View</a>"
                                              };

            Page<UserViewModel> page = new Page<UserViewModel>
            {
                TotalItems = query.Count()
            };

            string orderByName = string.Empty;
            Order orderBy = new Order();
            if (model.order != null)
            {
                orderBy = model.order.First();
                orderByName = model.columns[orderBy.column].name;
            }

            // Apply OR filters to IQueryable
            #region ApplyFilters
            string searchTerm = model.search.value;

            Predicate<UserViewModel> globalPredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> idPredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> firstNamePredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> lastNamePredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> emailPredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> usernamePredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> accountPredicate = PredicateExtensions.False<UserViewModel>();
            Predicate<UserViewModel> typePredicate = PredicateExtensions.False<UserViewModel>();

            bool anyGlobal = false,
                 anyId = false,
                 anyFirstName = false,
                 anyLastName = false,
                 anyEmail = false,
                 anyUsername = false,
                 anyAccount = false,
                 anyType = false;

            foreach (var column in model.columns)
            {
                string columnSearchTerm = column.search.value;

                switch (column.name)
                {
                    case "Id":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Id);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Id);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => o.Id.ToString().Contains(searchTerm);
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => o.Id.ToString().Contains(columnSearchTerm);
                                idPredicate = idPredicate.Or(colPredicate);
                                anyId = true;
                            }
                        }

                        break;

                    case "FirstName":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.FirstName);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.FirstName);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => !string.IsNullOrEmpty(o.FirstName) ? (o.FirstName.ToUpper().ToString().Contains(searchTerm.ToUpper())) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => !string.IsNullOrEmpty(o.FirstName) ? (o.FirstName.ToUpper().ToString().Contains(columnSearchTerm.ToUpper())) : false;
                                firstNamePredicate = firstNamePredicate.Or(colPredicate);
                                anyFirstName = true;
                            }
                        }

                        break;

                    case "LastName":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.LastName);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.LastName);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => !string.IsNullOrEmpty(o.LastName) ? (o.LastName.ToUpper().Contains(searchTerm.ToUpper())) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => !string.IsNullOrEmpty(o.LastName) ? (o.LastName.ToUpper().Contains(columnSearchTerm.ToUpper())) : false;
                                lastNamePredicate = lastNamePredicate.Or(colPredicate);
                                anyLastName = true;
                            }
                        }

                        break;

                    case "Email":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Email);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Email);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => !string.IsNullOrEmpty(o.Email) ? (o.Email.ToUpper().Contains(searchTerm.ToUpper())) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => !string.IsNullOrEmpty(o.Email) ? (o.Email.ToUpper().Contains(columnSearchTerm.ToUpper())) : false;
                                emailPredicate = emailPredicate.Or(colPredicate);
                                anyEmail = true;
                            }
                        }

                        break;

                    case "Username":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Username);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Username);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => !string.IsNullOrEmpty(o.Username) ? (o.Username.ToUpper().Contains(searchTerm.ToUpper())) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => !string.IsNullOrEmpty(o.Username) ? (o.Username.ToUpper().Contains(columnSearchTerm.ToUpper())) : false;
                                usernamePredicate = usernamePredicate.Or(colPredicate);
                                anyUsername = true;
                            }
                        }

                        break;

                    case "Account":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Account);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Account);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => !string.IsNullOrEmpty(o.Account) ? (o.Account.ToUpper().Contains(searchTerm.ToUpper())) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => !string.IsNullOrEmpty(o.Account) ? (o.Account.ToUpper().Contains(columnSearchTerm.ToUpper())) : false;
                                accountPredicate = accountPredicate.Or(colPredicate);
                                anyAccount = true;
                            }
                        }

                        break;

                    case "Type":
                        if (column.name == orderByName)
                        {
                            if (orderBy?.dir == "asc")
                            {
                                query = query.OrderBy(q => q.Type);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Type);
                            }
                        }

                        if (column.searchable)
                        {
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool globalp(UserViewModel o) => string.IsNullOrEmpty(o.Type) ? (o.Type.ToUpper().Contains(searchTerm.ToUpper())) : false;
                                globalPredicate = globalPredicate.Or(globalp);
                                anyGlobal = true;
                            }

                            if (!string.IsNullOrEmpty(columnSearchTerm))
                            {
                                bool colPredicate(UserViewModel o) => string.IsNullOrEmpty(o.Type) ? (o.Type.ToUpper().Contains(columnSearchTerm.ToUpper())) : false;
                                typePredicate = typePredicate.Or(colPredicate);
                                anyType = true;
                            }
                        }

                        break;
                }
            }

            if (anyGlobal)
            {
                bool funcOr(UserViewModel l) => globalPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyId)
            {
                bool funcOr(UserViewModel l) => idPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyFirstName)
            {
                bool funcOr(UserViewModel l) => firstNamePredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyLastName)
            {
                bool funcOr(UserViewModel l) => lastNamePredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyEmail)
            {
                bool funcOr(UserViewModel l) => emailPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyUsername)
            {
                bool funcOr(UserViewModel l) => usernamePredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyAccount)
            {
                bool funcOr(UserViewModel l) => accountPredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }
            if (anyType)
            {
                bool funcOr(UserViewModel l) => typePredicate(l);
                query = query.Where(funcOr).AsQueryable();
            }

            #endregion

            var startPage = (model.length == 0) ? 1 : model.start / model.length;

            page.CurrentPage = startPage;
            page.ItemsPerPage = model.length;
            page.TotalPages = query.Count() / page.ItemsPerPage;
            page.TotalDisplayItems = query.Count();

            page.Items = query.Skip(startPage * model.length).Take(model.length).ToList();

            return page;
        }


        public class OrderViewModel
        {
            public int Id { get; set; }
            public DateTime Created { get; set; }
            public string OrderNumber { get; set; }
            public string User { get; set; }
            public string Location { get; set; }
            public decimal Total { get; set; }
            public bool IsCin7 { get; set; }
            public int? Cin7Id { get; set; }
            public string Action { get; set; }
        }

        public class AccountViewModel
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string RegistrationNumber { get; set; }
            public string VatNumber { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public double Limit { get; set; }

            public string Action { get; set; }
            public string AccountLink { get; set; }
        }

        public class UserViewModel
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Account { get; set; }
            public string Type { get; set; }
            public string UserLink { get; set; }
        }
    }
}