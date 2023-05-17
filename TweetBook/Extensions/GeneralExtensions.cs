using Microsoft.AspNetCore.SignalR.Protocol;
using System.Runtime.CompilerServices;

namespace TweetBook.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.SingleOrDefault(x => x.Type == "id").Value;
        }
    }
}
