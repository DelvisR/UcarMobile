using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Core.Entities;

/// <summary>
/// This entity inherits from <see cref="User"/> and is mapped using
/// EF Core Table-Per-Type (TPT) inheritance.
/// 
/// In the database:
/// – The base entity <see cref="User"/> is stored in the table "UserAccount".
/// – This derived entity is stored in its own table ("Technician")
///   with the same primary key value as the related row in "UserAccount".
/// – EF Core creates a 1-to-1 relationship between the base and derived tables.
/// 
/// This means the derived table contains only its additional columns and
/// a primary key/foreign key referencing "UserAccount.Id".
/// </summary>

public class Technician : User
{
    public bool IsFreelance { get; set; } = false;
    //public decimal Rating { get; set; } = 5.0m;
    //public int TotalJobs { get; set; }
    //public DateTime? LastActive { get; set; }
    //public bool IsAvailable { get; set; } = true;
    //public string? ScheduleJson { get; set; } // or link to TechnicianSchedule table

}
