namespace WebApp.Net.Models;

public class MainViewModel
{
    public ImageViewModel? Images { get; set; }
    
    public LoginViewModel? Login { get; set; }

    public bool? IsValidUrl { get; set; }
}