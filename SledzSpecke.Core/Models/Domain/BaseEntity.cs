using SQLite;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    public abstract class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public virtual int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}