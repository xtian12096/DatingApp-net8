namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required string UserName { get; set; }  //.Net use Pascal Case while Angular uses camelCase
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

}
