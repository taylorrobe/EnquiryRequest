using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace EnquiryRequest3.Models
{
    public class Boundary
    {
        [Key]
        public int BoundaryId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        public string Name { get; set; }

        [Required]
        public DbGeometry Area { get; set; }

        [Required]
        public bool displayOnMap { get; set; }

        [Required]
        public bool isCoverageArea { get; set; }

        public Boundary()
        {
            displayOnMap = true;
            isCoverageArea = false;
        }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}