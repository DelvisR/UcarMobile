using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Core.Entities;

/// <summary>
/// This entity inherits from <see cref="User"/> and is mapped using
/// EF Core Table-Per-Type (TPT) inheritance.
/// 
/// In the database:
/// – The base entity <see cref="User"/> is stored in the table "UserAccount".
/// – This derived entity is stored in its own table ("Client")
///   with the same primary key value as the related row in "UserAccount".
/// – EF Core creates a 1-to-1 relationship between the base and derived tables.
/// 
/// This means the derived table contains only its additional columns and
/// a primary key/foreign key referencing "UserAccount.Id".
/// </summary>

public class Client : User
{
    public string? Address { get; set; }

    public ICollection<Vehicle> Vehicles { get; set; } = [];
}
