using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Principal;

namespace EnquiryRequest3.Controllers
{
    public static class IdentityExtensions
    {
        public static string GetUserEmailAddress(this IIdentity identity)
        {
            var userId = identity.GetUserId<int>();
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                return user.Email;
            }
        }
        public static string GetUserDefaultInvoicingEmail(this IIdentity identity)
        {
            var userId = identity.GetUserId<int>();
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                return user.DefaultInvoicingEmail;
            }
        }
        public static int GetIntUserId(this IIdentity identity)
        {
            return identity.GetUserId<int>();
        }
        public static ApplicationUser GetAppUser(this IIdentity identity)
        {
            var userId = identity.GetUserId<int>();
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                return user;
            }
        }

    }
}