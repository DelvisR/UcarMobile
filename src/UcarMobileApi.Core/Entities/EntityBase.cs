using System;

namespace UcarMobileApi.Core.Entities
{
    public abstract class EntityBase
    {
        public int Id { get; set; }  // PK autoincremental

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}