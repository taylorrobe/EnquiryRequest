using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnquiryRequest3.Models
{
    public class InvoiceReminder
    {
        [Key]
        public int InvoiceReminderId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime InvoiceReminderDate { get; set; }

        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        [Required]
        public int InvoiceReminderTypeId { get; set; }
        public virtual  InvoiceReminderType InvoiceReminderType { get; set; }
    }
}