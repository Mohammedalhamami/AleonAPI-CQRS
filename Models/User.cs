using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AleonAPI.Models;

public class User : IdentityUser
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";
   
}