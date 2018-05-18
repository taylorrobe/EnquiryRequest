using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnquiryRequest3.Models
{
    public class InvoiceReminderType
    {
        [Key]
        public int InvoiceReminderTypeId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string ReminderSubject { get; set; }

        [Required]
        public string ReminderBody { get; set; }

        public virtual ICollection<InvoiceReminder> InvoiceReminders { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}