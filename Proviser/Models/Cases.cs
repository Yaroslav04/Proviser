using SQLite;
using System;

namespace Proviser.Models
{
    public class Cases
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int Id { get; set; }

        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public string Case { get; set; }
        public string Judge { get; set; }
        public string Court { get; set; }
        public string Littigans { get; set; }
        public string Category { get; set; }
        public string Header { get; set; }
        public string Note { get; set; }
        public DateTime PrisonDate { get; set; }

    }
}
