using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnquiryRequest3.Models
{
    public class UserCreateEditEnquiryViewModel
    {
        [Key]
        public int EnquiryId { get; set; }

        [Display(Name = "Enquiry Code")]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string InvoiceEmail { get; set; }
        
        [Required]
        [DataType(DataType.MultilineText)]
        public string SearchAreaWkt { get; set; }

        [Required]
        public int SearchTypeId { get; set; }
        public virtual SearchType SearchType { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Estimated Cost", Description = "This is estimated from the area and type of enquiry")]
        public decimal EstimatedCost { get; set; }

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

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comment", Prompt = "Enter Comments", Description = "Write here if there is anything else we should know about the request")]
        public string Comment { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        [DisplayName("Added To Rers Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime? AddedToRersDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        [DisplayName("Data Cleaned Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime? DataCleanedDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        [DisplayName("Report Complete Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime? ReportCompleteDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        [DisplayName("Documents Cleaned Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime? DocumentsCleanedDate { get; set; }

        //should be non createable, automatically timestamped on user click and show up in details
        [DisplayName("Enquiry Delivered Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
        public DateTime? EnquiryDeliveredDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string AdminComment { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }

        public int? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class EnquiryIndexData
    {
        public IEnumerable<Enquiry> Enquiries { get; set; }
        public IEnumerable<Quote> Quotes { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }
}