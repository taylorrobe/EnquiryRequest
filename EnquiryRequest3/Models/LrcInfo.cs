using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace EnquiryRequest3.Models
{
    [MetadataType(typeof(AddressMetaData))]
    public class LrcInfo
    {
        [Key]
        public int LrcInfoId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Name { get; set; }

        [Required]
        public DbGeometry Area { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string CompanyNumber { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string CharityNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]

        public string Address1 { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Address2 { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Address3 { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string PostCode { get; set; }

        [Phone]
        [StringLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Email { get; set; }

        [Url]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Website { get; set; }

        [Required]
        public string PaymentTerms { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string BankAccountName { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{2}-[0-9]{2}-[0-9]{2}$",
         ErrorMessage = "Should be in format 00-00-00.")]
        public string BankAccountSortCode { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{8}$",
         ErrorMessage = "Should be in format 00000000.")]
        public string BankAccountNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string InformationRequested { get; set; }

        [DataType(DataType.MultilineText)]
        public string Declaration { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}