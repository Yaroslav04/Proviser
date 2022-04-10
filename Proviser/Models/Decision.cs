using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Proviser.Models
{
    public class Decision
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public string Id { get; set; }
        public string DecisionType { get; set; }
        public DateTime Date { get; set; }
        public string JudiciaryType { get; set; }
        public string Case { get; set; }
        public string Court { get; set; }
    }
}
