namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SahibindenCars
    {
        public int ID { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; }

        [StringLength(1000)]
        public string Title { get; set; }

        public int? Year { get; set; }

        public int? Km { get; set; }

        [StringLength(20)]
        public string Color { get; set; }

        [StringLength(50)]
        public string Price { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(50)]
        public string Place { get; set; }
    }
}
