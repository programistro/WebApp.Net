using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Net.Models;

public class User : IdentityUser
{
    // [Key]
    // public string Id { get; set; }
    
    public string? IdImage { get; set; }
    
    public string? Url { get; set; }
    
    public string? SelectedImages { get; set; }
}