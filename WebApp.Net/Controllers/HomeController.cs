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

        var user = _context.Users.FirstOrDefault(x => x.Url == url);

        if (user != null)
        {
            if (user.UrlValid == false)
            {
                var items = _context.Files.Where(x => x.IdImage == url);

                List<string> images = new();

                foreach (var item in items)
                {
                    images.Add(item.Name);
                }

                MainViewModel viewModel = new()
                {
                    Images = new ImageViewModel()
                    {
                        ImagesNames = images
                    }
                };

                return View(viewModel);
            }
            else 
            if (user.UrlValid == true) 
            {
                var namesImg = user.SelectedImages.Remove(user.SelectedImages.Length - 1);

                namesImg = namesImg.Trim('[', ']').Replace("\\\"", "\"").TrimEnd(']');

                string[] fileNames = namesImg.Split(',');

                List<string> filteredFileNames = new List<string>(fileNames.Length);

                foreach (string fileName in fileNames)
                {
                    filteredFileNames.Add(fileName.Trim('"', '"'));
                }

                var newItems = _context.Files.Where(x => x.IdImage == url);

                List<string> newImages = new();

                foreach(var item in newItems)
                {
                    newImages.Add(item.Name);
                }

                foreach (string fileName in filteredFileNames)
                {
                    foreach (var item in newItems) 
                    {
                        if(item.Name == fileName)
                        {
                            newImages.Remove(item.Name);
                        }
                    }
                }

                MainViewModel viewModel = new()
                {
                    Images = new ImageViewModel()
                    {
                        ImagesNames = filteredFileNames,
                        NewImages = newImages
                    },
                    IsValidUrl = true
                };

                return View(viewModel);
            }
        }
        else
        {
            return View("Error");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadImages(MainViewModel files)
    {
        Guid guid = Guid.NewGuid();

        string urlHash = guid.ToString("N");
        
        User user = new()
        {
            Id = Guid.NewGuid().ToString("N"),
            Url = urlHash,
            UrlValid = false,
            IdImage = urlHash
        };
        _context.Users.Add(user);
        foreach (var formFile in files.Images.Images)
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
    
    [HttpPost]
    public IActionResult SelectImages(string[] selectedImages, string id_url)
    {
        if (selectedImages == null || !selectedImages.Any())
        {
            return RedirectToAction("Index");
        }

        string line = "";
        
        foreach (var imageName in selectedImages)
        {
            line += imageName + ";";
            Console.WriteLine($"Selected image: {imageName}");
        }

        var user = _context.Users.FirstOrDefault(x => x.Url == id_url);

        if (user != null)
        {
            user.SelectedImages = line;
            user.UrlValid = true;

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Index");   
        }
        else
        {
            return View("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}