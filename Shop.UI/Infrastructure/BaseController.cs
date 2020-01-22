using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Shop.UI.Infrastructure
{
    public class BaseController : Controller
    {
        public (string userId, string sessionId) GetCartUserMark() =>
           (GetUserId(), GetSessionId());

        public string GetUserId() =>
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string GetUserEmail() =>
            HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        public string GetSessionId()
        {
            var SessionId = HttpContext.Session.GetString("SessionId");

            if (!String.IsNullOrEmpty(SessionId))
                return SessionId;

            SessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("SessionId", SessionId);

            return SessionId;
        }
    }
}
