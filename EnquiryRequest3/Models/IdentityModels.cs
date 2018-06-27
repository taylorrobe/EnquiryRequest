using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EnquiryRequest3.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    [MetadataType(typeof(ContactMetaData))]
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole,
    CustomUserClaim>
    {
        public string Forename { get; set; }

        public string Surname { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                if(Organisation != null)
                {
                    return string.Format("{0} {1} - {2}", Forename, Surname, Organisation.Name);
                }
                return string.Format("{0} {1}", Forename, Surname);
            }
        }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string PostCode { get; set; }

        public int? OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public string DefaultInvoicingEmail { get; set; }

        [MaxLength(256)]
        public string UnConfirmedEmail { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole,
    int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.Boundary> Boundaries { get; set; }
        
        public System.Data.Entity.DbSet<EnquiryRequest3.Models.Organisation> Organisations { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.Enquiry> Enquiries { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.Invoice> Invoices { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.InvoiceReminder> InvoiceReminders { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.InvoiceReminderType> InvoiceReminderTypes { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.LrcInfo> LrcInfoes { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.Quote> Quotes { get; set; }

        public System.Data.Entity.DbSet<EnquiryRequest3.Models.SearchType> SearchTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // one-to-zero or one relationship between ApplicationUser and Customer
            // UserId column in Customers table will be foreign key
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasOptional(m => m.Contact)
            //    .WithRequired(m => m.ApplicationUser);
        }

    }
    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }
    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}