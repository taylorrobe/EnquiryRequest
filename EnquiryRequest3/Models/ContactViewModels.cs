using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EnquiryRequest3.Models
{
    [MetadataType(typeof(ContactMetaData))]
    public class ContactForUserViewModel
    {
        [Key]
        public int ContactId { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string PostCode { get; set; }

        public string PhoneNumber { get; set; }

        public int? OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public string DefaultInvoicingEmail { get; set; }

    }
}