namespace AtonTest.Entities;

public class UserEntity : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool Admin { get; set; }
}