using System;

namespace UcarMobileApi.Core.Entities.Common;

public abstract class AuditableEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string LastModifiedBy { get; set; } = string.Empty;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
}


public abstract class EntityBase : AuditableEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
}
