namespace Restaurant.Core.Domain.DTOs
{
    public sealed record RegisterUserDTO
    {
        public required string UserName { get; set; }
        public required string PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public required string Password { get; set; }
    }
}
