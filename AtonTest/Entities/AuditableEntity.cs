namespace AtonTest.Entities;

public class AuditableEntity
{
    public DateTime ModifiedOn { get; set; }
    public  string ModifiedBy { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public  string CreatedBy { get; set; }

    public void SetModified(string modifiedBy)
    {
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    
    public void SetCreated(string createdBy)
    {
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = createdBy;
        CreatedBy = createdBy;
    }

    public void SetRevoked(string revokedBy)
    {
        RevokedOn = DateTime.UtcNow;
        RevokedBy = revokedBy;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = revokedBy;
    }

    public void SetRestored(string restoredBy)
    {
        RevokedOn = null;
        RevokedBy = null;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = restoredBy;
    }
}