using SledzSpecke.Core.Models.Domain;
using SQLite;
using System;

[Table("Duty")]
public class Duty : BaseEntity
{
    [PrimaryKey, AutoIncrement]
    public override int Id { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    [Column("StartTime")]
    public DateTime StartTime { get; set; }

    [Column("EndTime")]
    public DateTime EndTime { get; set; }

    [Column("Location")]
    public string Location { get; set; }

    [Column("SupervisorId")]
    public int? SupervisorId { get; set; }

    [Column("Notes")]
    public string Notes { get; set; }

    [Column("IsConfirmed")]
    public bool IsConfirmed { get; set; }

    [Column("DurationInHours")]
    public decimal DurationInHours { get; set; }

    [Ignore]
    public User User { get; set; }

    [Ignore]
    public User Supervisor { get; set; }
}