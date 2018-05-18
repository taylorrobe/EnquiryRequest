using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnquiryRequest3.Models
{
    public class Organisation
    {
        [Key]
        public int OrganisationId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Organisation Name")]
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}