using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public static class IdentityExtensions
    {
        public static Guid GetUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                throw new InvalidOperationException("This extension method can only be used with ClaimsIdentity type");
            }

            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

            return userId;
        }

        public static string GetId(this IPrincipal principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return string.Empty;
            }

            return principal.Identity.GetUserId().ToString();
        }

        public static string GetId(this ClaimsPrincipal principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return string.Empty;
            }

            return principal.Identity.GetUserId().ToString();
        }

        public static IEnumerable<Claim> GetRoles(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity.FindAll(ClaimTypes.Role);
        }
    }
}
