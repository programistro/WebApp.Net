namespace WebApp.Net.Models;

public class ImageViewModel
{
    public IFormFileCollection Images { get; set; }

    public List<string> NewImages { get; set; }

    public List<string> ImagesNames { get; set; }
}