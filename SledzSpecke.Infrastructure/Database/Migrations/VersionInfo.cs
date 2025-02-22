using SQLite;
using System;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    [Table("VersionInfo")]
    public class VersionInfo
    {
        [PrimaryKey]
        public int Version { get; set; }
        public DateTime AppliedOn { get; set; }
        public string Description { get; set; }
    }
}
