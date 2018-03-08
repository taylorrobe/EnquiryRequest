using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EnquiryRequest.Models;

namespace EnquiryRequest2.Models
{
    public class EnquiryRequestContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public EnquiryRequestContext() : base("name=EnquiryRequestContext")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Contact>().ToTable("Users");
        }
        public System.Data.Entity.DbSet<Boundary> Boundaries { get; set; }

        public System.Data.Entity.DbSet<Contact> Contacts { get; set; }

        public System.Data.Entity.DbSet<Enquiry> Enquiries { get; set; }

        public System.Data.Entity.DbSet<Invoice> Invoices { get; set; }

        public System.Data.Entity.DbSet<InvoiceReminder> InvoiceReminders { get; set; }

        public System.Data.Entity.DbSet<InvoiceReminderType> InvoiceReminderTypes { get; set; }

        public System.Data.Entity.DbSet<LrcInfo> LrcInfoes { get; set; }

        public System.Data.Entity.DbSet<Organisation> Organisations { get; set; }

        public System.Data.Entity.DbSet<Quote> Quotes { get; set; }

        public System.Data.Entity.DbSet<SearchType> SearchTypes { get; set; }
    }
}
