namespace LoginApi.Models;

public class User(string name, string email, string hashedPassword)
{
    public Guid Id { get; set; }

    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string HashedPassword { get; set; } = hashedPassword;
}
