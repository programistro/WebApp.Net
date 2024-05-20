using System.ComponentModel.DataAnnotations;

namespace WebApp.Net.Models;

public class User
{
    [Key]
    public string Id { get; set; }
    
    public string IdImage { get; set; }
    
    public string Url { get; set; }
}