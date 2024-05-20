using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Net.Data;
using WebApp.Net.Models;
using WebApp.Net.Service;

namespace WebApp.Net.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly AppDbContext _context;

    private readonly AppService _service;

    public HomeController(ILogger<HomeController> logger, AppDbContext context, AppService service)
    {
        _logger = logger;
        _context = context;
        _service = service;
    }

    [HttpGet]
    public IActionResult Index(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return View();
        }

        var items = _context.Files.Where(x => x.IdImage == url);

        List<string> images = new();

        foreach (var item in items)
        {
            images.Add(item.Name);
        }
        
        ImageViewModel viewModel = new()
        {
            ImagesNames = images
        };
        
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadImages(ImageViewModel files)
    {
        Guid guid = Guid.NewGuid();

        string urlHash = guid.ToString("N");
        
        User user = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            Url = urlHash,
            IdImage = urlHash
        };
        _context.Users.Add(user);
        foreach (var formFile in files.Images)
        {
            if (formFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image");

                Directory.CreateDirectory(folderPath);

                var fileName = Path.GetFileName(formFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    formFile.CopyTo(stream);
                }
        
                ViewData["Url"] = urlHash;

                FilePath file = new()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Path = filePath,
                    IdImage = user.IdImage,
                    Name = fileName
                };
                _context.Files.Add(file);

                _context.SaveChanges();
            }
        }
        
        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}