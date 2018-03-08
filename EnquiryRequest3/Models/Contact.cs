using EnquiryRequest3.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnquiryRequest3.Models
{
    [MetadataType(typeof(ContactMetaData))]
    public class Contact
    {
        //[Key, ForeignKey("ApplicationUser")]
        [Key]
        public int ContactId { get; set; }

        //public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Forename { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Address line 3")]
        public string Address3 { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Post code")]
        public string PostCode { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public int? OrganisationId { get; set; }

        [Display(Name = "Organisation")]
        public virtual Organisation Organisation { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Default Invoicing Email Address")]
        public string DefaultInvoicingEmail { get; set; }
        
    }
}