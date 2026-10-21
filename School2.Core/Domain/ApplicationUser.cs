using Microsoft.AspNetCore.Identity;

namespace School2.Core.Domain;

public class ApplicationUser : IdentityUser
{
    //public ClaimsIdentity UserCredential { get; set; } = null;
    public string Placeholder { get; set; }

    public string Name { get; set; }
    public RegisterStatus AccountStatus { get; set; }
}