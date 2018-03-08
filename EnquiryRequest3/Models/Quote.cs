using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnquiryRequest3.Models
{
    public class Quote
    {
        [Key]
        public int QuoteId { get; set; }

        [Required]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        //auto generate quote date
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime QuotedDate { get; set; }

        //only one quote linked to an enquiry can have an accepted date
        public DateTime AcceptedDate { get; set; }

        [Required]
        public int EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }
    }
}