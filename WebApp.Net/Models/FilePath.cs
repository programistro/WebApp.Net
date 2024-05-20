using System.ComponentModel.DataAnnotations;

namespace WebApp.Net.Models;

public class FilePath
{
    [Key]
    public string Id { get; set; }
    
    public string IdImage { get; set; }
    
    public string Path { get; set; }
    
    public string Name { get; set; }
}