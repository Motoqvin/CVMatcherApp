using Microsoft.AspNetCore.Identity;

namespace CVMatcherApp.Api.Models;
public class User : IdentityUser
{
    public override string Id { get => base.Id; set => base.Id = value; }
    public override string? Email { get => base.Email; set => base.Email = value; }

    public ICollection<CV>? CVs { get; set; }
}