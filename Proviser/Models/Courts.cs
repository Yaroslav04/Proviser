using SQLite;
using System;

namespace Proviser.Models
{
    public class Courts
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int Id { get; set; }
        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public DateTime Date { get; set; }
        public string Judge { get; set; }

        [Indexed(Name = "ListingID", Order = 2, Unique = true)]
        public string Case { get; set; }
        public string Court { get; set; }
        public string Littigans { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string SoketHeader { get; set; }
        public string SoketNote { get; set; }
        public string SoketPrisonDate { get; set; }
        
    }
}
