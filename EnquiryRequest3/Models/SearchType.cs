using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnquiryRequest.Models
{
    public class SearchType
    {
        [Key]
        public int SearchTypeId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual ICollection<Enquiry> Enquiries { get; set; }
    }
}