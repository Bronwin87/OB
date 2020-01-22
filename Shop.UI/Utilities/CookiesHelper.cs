using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace Shop.UI.Utilities
{
    public class CookiesHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookiesHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }        

        public string Get(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }

        public void Set(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.UtcNow.AddDays(expireTime.Value);
            else
                option.Expires = DateTime.UtcNow.AddMilliseconds(10);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }

        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);

           
        }
    }
}
