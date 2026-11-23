using Microsoft.AspNetCore.Identity;

namespace QuizApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
