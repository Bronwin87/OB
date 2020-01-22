using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Security.Claims;

namespace Shop.UI.Infrastructure
{
    public class BasePage : PageModel
    {
        public string GetUserType() =>
            User.Claims.FirstOrDefault(x => x.Type == "type")?.Value;

        public string GetUserEmail() =>
            HttpContext.User.Identity.Name;

        public (string userId, string sessionId) GetCartUserMark() =>
            (GetUserId(), GetSessionId());

        public string GetUserId() =>
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
