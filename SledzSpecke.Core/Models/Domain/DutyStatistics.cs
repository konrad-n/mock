using SledzSpecke.Core.Models.Enums;
using SQLite;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("DutyStatistics")]
    public class DutyStatistics : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("TotalHours")]
        public decimal TotalHours { get; set; }

        [Column("MonthlyHours")]
        public decimal MonthlyHours { get; set; }

        [Column("TotalCount")]
        public int TotalCount { get; set; }

        [Column("SupervisedHours")]
        public decimal SupervisedHours { get; set; }

        [Column("EmergencyHours")]
        public decimal EmergencyHours { get; set; }

        [Column("WeekendHours")]
        public decimal WeekendHours { get; set; }

        [Column("RemainingHours")]
        public decimal RemainingHours { get; set; }

        private string _dutiesByTypeJson;
        [Column("DutiesByTypeJson")]
        public string DutiesByTypeJson
        {
            get => _dutiesByTypeJson;
            set => _dutiesByTypeJson = value;
        }

        private string _hoursByMonthJson;
        [Column("HoursByMonthJson")]
        public string HoursByMonthJson
        {
            get => _hoursByMonthJson;
            set => _hoursByMonthJson = value;
        }

        [Ignore]
        public Dictionary<DutyType, int> DutiesByType
        {
            get => string.IsNullOrEmpty(_dutiesByTypeJson)
                ? new Dictionary<DutyType, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<DutyType, int>>(_dutiesByTypeJson);
            set => _dutiesByTypeJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [Ignore]
        public Dictionary<string, decimal> HoursByMonth
        {
            get => string.IsNullOrEmpty(_hoursByMonthJson)
                ? new Dictionary<string, decimal>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, decimal>>(_hoursByMonthJson);
            set => _hoursByMonthJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}
