using Mandrill;
using Mandrill.Model;
using Microsoft.Extensions.Options;
using RestSharp;
using Shop.Domain.Models;
using Shop.Domain.Models.EmailTemplates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Application.Services.Emails
{
    public class EmailSender
    {
        private readonly EmailSettings _emailSettings;
        public EmailSender(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }
        public class Request
        {
            public string EmailAddress { get; set; }
        }

        public void SendEmailByTemplate(Dictionary<string, string> vars, string templateName, Message message)
        {
            List<TemplateContent> templateContents = new List<TemplateContent>();
            foreach (var x in vars)
            {
                TemplateContent contents = new TemplateContent
                {
                    Name = x.Key,
                    Content = x.Value
                };

                templateContents.Add(contents);
            }

            //foreach(var item in message.To)
            //{
            //    item.Email = "a.carel.g.nel@gmail.com";
            //}

            var emailRequestObject = new ContactUs
            {
                Key = "Xs_R1QEOVN4L4OLuVtF_Jg",
                TemplateName = templateName,
                Message = message,
                TemplateContent = templateContents.ToArray()
            };

            var client = new RestClient("https://mandrillapp.com/api/1.0/messages/send-template.json");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var jsonBody = emailRequestObject.ToJson();

            request.AddJsonBody(jsonBody);

            IRestResponse response = client.Execute(request);
        }

        //public async Task sendForgotPasswordEmail(string recipient, Dictionary<string, string> vars, string templateName, Message message)
        //{
        //    List<TemplateContent> templateContents = new List<TemplateContent>();

        //    foreach (var x in vars)
        //    {
        //        TemplateContent contents = new TemplateContent();
        //        contents.Name = x.Key;
        //        contents.Content = x.Value;

        //        templateContents.Add(contents);
        //    }

        //    var emailRequestObject = new ContactUs
        //    {
        //        Key = "Xs_R1QEOVN4L4OLuVtF_Jg",
        //        TemplateName = templateName,
        //        TemplateContent = templateContents.ToArray()
        //    };

        //    var client = new RestClient("https://mandrillapp.com/api/1.0/messages/send-template.json");
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");

        //    var jsonBody = emailRequestObject.ToJson();

        //    request.AddJsonBody(jsonBody);

        //    IRestResponse response = client.Execute(request);
        //}

        //public async Task sendEFTBusinessRegisterEmail(string recipient, Dictionary<string, string> vars, string templateName, Message message)
        //{
        //    List<TemplateContent> templateContents = new List<TemplateContent>();

        //    foreach (var x in vars)
        //    {
        //        TemplateContent contents = new TemplateContent();
        //        contents.Name = x.Key;
        //        contents.Content = x.Value;

        //        templateContents.Add(contents);
        //    }

        //    var emailRequestObject = new ContactUs
        //    {
        //        Key = "Xs_R1QEOVN4L4OLuVtF_Jg",
        //        TemplateName = templateName,
        //        Message = message,
        //        TemplateContent = templateContents.ToArray()
        //    };


        //    var api = new MandrillApi("Xs_R1QEOVN4L4OLuVtF_Jg");
        //    var mandrillMessageandrillmessage = new MandrillMessage();

        //    var client = new RestClient("https://mandrillapp.com/api/1.0/messages/send-template.json");
        //    var request = new RestRequest(Method.POST);
        //    request.AddHeader("Content-Type", "application/json");

        //    var jsonBody = Domain.Models.EmailTemplates.Serialize.ToJson(emailRequestObject);

        //    request.AddJsonBody(jsonBody);

        //     IRestResponse response =  client.Execute(request);

        //}

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="request"></param>
        public async void SendProductPDF(Request request)
        {
            var api = new MandrillApi("rdwM7taGZOP5nh6rmtyM1w");
            var message = new MandrillMessage("support@officebox.co.za", request.EmailAddress,
                            "EmailTest", "Test product pdf Email for DEV");
            var result = await api.Messages.SendAsync(message);

        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="email"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public async Task<bool> SendProductEmail(string email, string link)
        {
            var api = new MandrillApi("rdwM7taGZOP5nh6rmtyM1w");
            var message = new MandrillMessage("support@officebox.co.za", email,
                            "EmailTest", $"link: {link}");
            var result = await api.Messages.SendAsync(message);
            return true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> SendTransactionalEmail(string email)
        {
            var api = new MandrillApi("rdwM7taGZOP5nh6rmtyM1w");
            var message = new MandrillMessage("support@officebox.co.za", email,
                            "EmailTest", "Test Registration Email for DEV");
            var result = await api.Messages.SendAsync(message);
            return true;
        }
    }
}