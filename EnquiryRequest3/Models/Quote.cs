using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnquiryRequest3.Models
{
    public class Quote
    {
        [Key]
        [Column("QuoteId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuoteId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        //auto generate quote date
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Quoted Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime QuotedDate { get; set; }

        //only one quote linked to an enquiry can have an accepted date
        [DisplayName("Accepted Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime? AcceptedDate { get; set; }

        [Required]
        public int EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}