using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace EnquiryRequest3.Models
{
    public class Enquiry
    {
        [Key]
        public int EnquiryId { get; set; }

        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Name { get; set; }

        [Required]
        public int ApplicationUserId { get; set; }
        public virtual  ApplicationUser ApplicationUser { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string InvoiceEmail { get; set; }

        [Required]
        public DbGeometry SearchArea { get; set; }

        [Required]
        public int SearchTypeId { get; set; }
        public virtual SearchType SearchType { get; set; }

        [Required]
        [Display(Name = "Number of Years", Prompt = "Enter the number of years", Description = "The number of years to search data for")]
        [Range(1, 20, ErrorMessage = "Please enter a whole number between 1-20 inclusive")]
        public int NoOfYears { get; set; }

        [Display(Name = "Job Number", Prompt = "Enter optional job number")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string JobNumber { get; set; }

        [Display(Name = "Agency", Prompt = "Enter agency name", Description = "If the enquiry is on behalf of an agency, please enter the agency here")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Agency { get; set; }

        [Display(Name = "Agency Contact", Prompt = "Enter agency contact name", Description = "If the enquiry is on behalf of an agency, please enter the agency contact here")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string AgencyContact { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Data Usage", Prompt = "Enter data usage information", Description = "What will the data be used for")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string DataUsedFor { get; set; }

        [Display(Name = "Site Citations", Prompt = "Require site citations", Description = "Select if you require site citations")]
        public Boolean Citations { get; set; }

        [Display(Name = "GIS & KML", Prompt = "Require GIS & KML", Description = "Select if you require additional GIS & KML format files")]
        public Boolean GisKml { get; set; }

        [Display(Name = "Express", Prompt = "Require express service", Description = "Select if you require express service")]
        public Boolean Express { get; set; }

        //should be non editable, automatically timestamped and show up in details
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime EnquiryDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        public DateTime? AddedToRersDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        public DateTime? DataCleanedDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        public DateTime? ReporCompleteDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        public DateTime? DocumentsCleanedDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        public DateTime? EnquiryDeliveredDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string AdminComment { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }

        public int? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
