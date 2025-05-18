namespace AtonTest.Dto;

public class UserResponseByLoginDto
{
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsActive { get; set; }
}