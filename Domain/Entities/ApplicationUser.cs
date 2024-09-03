using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string CityOrVillage { get; set; }
}