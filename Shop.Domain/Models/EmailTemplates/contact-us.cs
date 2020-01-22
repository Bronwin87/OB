using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Shop.Domain.Models.EmailTemplates
{
    public partial class ContactUs
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("template_name")]
        public string TemplateName { get; set; }

        [JsonProperty("template_content")]
        public TemplateContent[] TemplateContent { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("from_email")]
        public string FromEmail { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("to")]
        public To[] To { get; set; }

        [JsonProperty("attachments")]
        public EmailAttachhment[] Attachments { get; set; }
    }

    public partial class EmailAttachhment
    {
        [JsonProperty("type")]
        public string FileType { get; set; }
        [JsonProperty("name")]
        public string FileName { get; set; }
        [JsonProperty("content")]
        public string FileContent { get; set; }
    }

    public partial class To
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class TemplateContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public partial class ContactUs
    {
        public static ContactUs FromJson(string json) => JsonConvert.DeserializeObject<ContactUs>(json, Shop.Domain.Models.EmailTemplates.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ContactUs self) => JsonConvert.SerializeObject(self, Shop.Domain.Models.EmailTemplates.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
