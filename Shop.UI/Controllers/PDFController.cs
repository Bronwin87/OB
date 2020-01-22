using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Application.Services.Emails;
using Shop.Application.Services.PDF;
using Shop.Application.Services.Quotes;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.EmailTemplates;
using Shop.UI.Infrastructure;
using Shop.UI.Models;
using Shop.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Controllers
{
    public class PDFController : BaseController
    {
        private IConfiguration _config;
        private PDFService _pdfService;
        private ApplicationDbContext _context;
        private IViewRenderService _viewRenderService;
        private IOptions<Discounts> _discounts;
        private EmailSender _emailSender;

        public PDFController(IConfiguration config, PDFService pDFService, IViewRenderService viewRenderService, ApplicationDbContext context, IOptions<Discounts> discounts, EmailSender emailSender)
        {
            _config = config;
            _pdfService = pDFService;
            _viewRenderService = viewRenderService;
            _context = context;
            _discounts = discounts;
            _emailSender = emailSender;
        }


        public async Task<IActionResult> CartDownloadAsPDfAsync()
        {
            CartViewModel model = new CartViewModel
            {
                Name = "Pro-forma Invoice",
                Account = (await _context.AccountUsers
                                  .Include(x => x.Account)
                                  .ThenInclude(x => x.Address)
                                  .FirstOrDefaultAsync(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active))
                                  .Account,
                CartItems = new GetCart(_context, _discounts).Do(GetCartUserMark()).ToList()
            };

            var result = await _viewRenderService.RenderToStringAsync("/Views/Cart/Default.cshtml", model);
            var file = await _pdfService.GetCartAsPDF(result);
            //return File(file.FileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.FileName);
            return File(file.FileBytes, "application/pdf");
        }

        public async Task<IActionResult> QuoteDownloadAsPdf(int id)
        {
            CartViewModel model = new CartViewModel()
            {
                Name = "Quote",
                
                Account = (await _context.AccountUsers
                                  .Include(x => x.Account)
                                  .ThenInclude(x => x.Address)
                                  .FirstOrDefaultAsync(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active))
                                  .Account,
                CartItems = new QuotesService(_context).GetQuote(id).QuoteProducts.Select(qp => new GetCart.Response()
                {
                    Name = qp.Product.Name,
                    Code = qp.Product.ExternalId,
                    Unit = qp.Product.Unit,
                    Value = qp.Product.Price ?? 0m,
                    Price = "R " + (qp.Product.Price ?? 0m).ToString("N2"),
                    Qty = qp.Qty,
                    Total = "R " + ((qp.Product.Price ?? 0m) * qp.Qty).ToString("N2")
                }).ToList()
            };

            var result = await _viewRenderService.RenderToStringAsync("/Views/Cart/Default.cshtml", model);
            var file = await _pdfService.GetCartAsPDF(result);
            return File(file.FileBytes, "application/pdf");
        }

        /// <summary>
        /// todo - ???? what is this?
        /// </summary>
        public async void SendProductPDF()
        {
            var product = new GetProduct(_context, _discounts);
            var result = await _viewRenderService.RenderToStringAsync("/Pages/Shop/Product.cshtml", product);
            var file = await _pdfService.GetProductAsPdf(result);
            _emailSender.SendProductPDF(new EmailSender.Request { EmailAddress = "bronwinbergstedt@gmail.com" });
        }

        [HttpPost]
        public async Task<IActionResult> EmailCartPdf(string email, string message)
        {
            CartViewModel model = new CartViewModel
            {
                Name = "Pro-forma Invoice",
                Account = _context.AccountUsers
                                  .Include(x => x.Account)
                                  .ThenInclude(x => x.Address)
                                  .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                                  .Account,
                CartItems = new GetCart(_context, _discounts).Do(GetCartUserMark()).ToList()
            };

            var result = await _viewRenderService.RenderToStringAsync("/Views/Cart/Default.cshtml", model);
            var file = await _pdfService.GetCartAsPDF(result);

            var me = await _context.Users.FindAsync(GetUserId());

            Dictionary<string, string> vars = new Dictionary<string, string> { };
            vars.Add("sendername", string.Format("{0} {1}", me.FirstName, me.LastName));
            vars.Add("type", "Pro-forma Invoice");
            vars.Add("sendermessage", message.Replace(Environment.NewLine, "<br/>"));

            Message m = new Message
            {
                FromEmail = "orders@officebox.co.za",
                FromName = "Officebox",
                Subject = "Email Quotation",
                To = new To[]
                {
                      new To {  Email = email}
                },
                Attachments = new EmailAttachhment[]
                {
                    new EmailAttachhment()
                    {
                        FileType = "application/pdf",
                        FileName = "Quote.pdf",
                        FileContent = Convert.ToBase64String(file.FileBytes)
                    }
                }
            };

            _emailSender.SendEmailByTemplate(vars, "mail-quotation", m);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> EmailQuotePdf(int id, string email, string message)
        {
            CartViewModel model = new CartViewModel
            {
                Name = "Quote",
                Account = _context.AccountUsers
                                 .Include(x => x.Account)
                                 .ThenInclude(x => x.Address)
                                 .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                                 .Account,
                CartItems = new QuotesService(_context).GetQuote(id).QuoteProducts.Select(qp => new GetCart.Response()
                {
                    Name = qp.Product.Name,
                    Code = qp.Product.ExternalId,
                    Unit = qp.Product.Unit,
                    Value = qp.Product.Price ?? 0m,
                    Price = "R " + (qp.Product.Price ?? 0m).ToString("N2"),
                    Qty = qp.Qty,
                    Total = "R " + ((qp.Product.Price ?? 0m) * qp.Qty).ToString("N2")
                }).ToList()
            };

            var result = await _viewRenderService.RenderToStringAsync("/Views/Cart/Default.cshtml", model);
            var file = await _pdfService.GetCartAsPDF(result);

            var me = await _context.Users.FindAsync(GetUserId());

            Dictionary<string, string> vars = new Dictionary<string, string> { };
            vars.Add("sendername", string.Format("{0} {1}", me.FirstName, me.LastName));
            vars.Add("type", "Quote");
            vars.Add("sendermessage", message.Replace(Environment.NewLine, "<br/>"));

            Message m = new Message
            {
                FromEmail = "orders@officebox.co.za",
                FromName = "Officebox",
                Subject = "Email Quotation",
                To = new To[]
                {
                      new To {  Email = email}
                },
                Attachments = new EmailAttachhment[]
                {
                    new EmailAttachhment()
                    {
                        FileType = "application/pdf",
                        FileName = "Quote.pdf",
                        FileContent = Convert.ToBase64String(file.FileBytes)
                    }
                }
            };

            _emailSender.SendEmailByTemplate(vars, "mail-quotation", m);
            return Ok();
        }
    }
}