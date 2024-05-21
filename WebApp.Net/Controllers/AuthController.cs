using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.Net.Data;
using WebApp.Net.Models;
using WebApp.Net.Service;

namespace WebApp.Net.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;

    private readonly UserDbContext _context;

    private readonly AppService _appService;

    public AuthController(ILogger<AuthController> logger, UserDbContext context, AppService service)
    {
        _logger = logger;
        _context = context;
        _appService = service;
    }

    [HttpPost]
    public async Task<IActionResult> Register(MainViewModel viewModel, string type)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == viewModel.Login.Email);

        if (type == "reg" && user == null)
        {
            User newUser = new User()
            {
                Email = viewModel.Login.Email,
                PasswordHash = await _appService.CreatePasswordHash(viewModel.Login.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var claims = new List<Claim> { new (ClaimTypes.Name, newUser.Email) };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }
        else if (type == "login" && user != null)
        {
            if (user.PasswordHash == await _appService.CreatePasswordHash(viewModel.Login.Password))
            {
                var claims = new List<Claim> { new (ClaimTypes.Name, user.Email) };
                var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
            }
        }
        
        return RedirectToAction("Index", "Home");
    }
}