namespace Restaurant.Core.Domain.Models;

public sealed class User
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string UserName { get; set; }
    public required string PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Address { get; set; }
    public required string Password { get; set; }
    public required string Salt { get; set; }
    public bool IsUndisciplined { get; set; }
}
