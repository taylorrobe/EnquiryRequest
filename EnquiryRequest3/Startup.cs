using EnquiryRequest3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using System.Web;

[assembly: OwinStartupAttribute(typeof(EnquiryRequest3.Startup))]
namespace EnquiryRequest3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createUsersAndRoles();
        }



        public void createUsersAndRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<CustomRole, int>(new RoleStore<CustomRole, int, CustomUserRole>(context));
            var UserManager = new UserManager<ApplicationUser
                , int>(new UserStore<ApplicationUser
                , CustomRole
                , int
                , CustomUserLogin
                , CustomUserRole
                , CustomUserClaim>(context));

            // create Admin Role     
            if (!roleManager.RoleExists("Admin"))
            { 
                var role = new CustomRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }
            // create EnquiryManager role     
            if (!roleManager.RoleExists("EnquiryManager"))
            { 
                var role = new CustomRole();
                role.Name = "EnquiryManager";
                roleManager.Create(role);
            }
            // create EnquiryUser role     
            if (!roleManager.RoleExists("EnquiryUser"))
            {
                var role = new CustomRole();
                role.Name = "EnquiryUser";
                roleManager.Create(role);
            }
            //Here we create the Admin super user who will maintain the website     
            if (UserManager.FindByName("Admin") == null)
            {
                var user = new ApplicationUser() {
                    UserName = "Admin",
                    Email = "itofficer@record-lrc.co.uk",
                    EmailConfirmed = true,
                    //PhoneNumberConfirmed = false,
                    //TwoFactorEnabled = false,
                    //LockoutEnabled = false,
                    //AccessFailedCount = 0,
                    Forename = "it",
                    Surname = "officer",
                    Address1 = "record office",
                    PostCode = "CH2 1LH"
                };
                string userPWD = "Recordsyzygy12*";
                var chkUser = UserManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}
