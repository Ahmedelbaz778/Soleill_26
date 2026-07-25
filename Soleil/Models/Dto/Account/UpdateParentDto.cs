namespace Soleil.Models.DTOs.Account;

public class UpdateParentDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Relation { get; set; }
    public IFormFile? ProfileImage { get; set; }
}