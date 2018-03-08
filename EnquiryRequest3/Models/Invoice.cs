using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnquiryRequest.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Code { get; set; }

        [Required]
        public double Amount { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string PONumber { get; set; }

        //will be auto timestamped
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime InvoiceDate { get; set; }

        public DateTime PaidDate { get; set; }

        public int? PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }

        [RequiredIf("PaymentMethod == PaymentMethod.CHEQUE", ErrorMessage = "Must enter a Cheque number")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string ChequeNumber { get; set; }

        [RequiredIf("PaymentMethod == PaymentMethod.CHEQUE", ErrorMessage = "Must enter an inslip number")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string InSlipNumber { get; set; }

        [RequiredIf("PaymentMethod == PaymentMethod.BANK_TRANSFER", ErrorMessage = "Must enter a remittance reference")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string RemittanceReference { get; set; }

        public virtual ICollection<InvoiceReminder> InvoiceReminders { get; set; }
    }
}