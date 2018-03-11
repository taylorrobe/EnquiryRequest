using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace EnquiryRequest3.Controllers
{
    public static class IdentityExtensions
    {
        public static string GetUserEmailAdress(this IIdentity identity)
        {
            var userId = identity.GetUserId<int>();
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                return user.Email;
            }
        }
    }
}