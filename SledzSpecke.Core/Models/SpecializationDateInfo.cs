using System;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.Core.Models
{
    public class SpecializationDateInfo
    {
        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateType Type { get; set; }

        public bool IsPast { get; set; }

        public int DaysRemaining { get; set; }

        public int? RelatedItemId { get; set; }
    }
}
