using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;

namespace MyMvcApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.PublishedDate)
            .Take(4)
            .ToListAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("Index", posts);

        return View(posts);
    }

    public IActionResult WhyUs()
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("WhyUs");

        return View();
    }

    public IActionResult Team()
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("Team");

        return View();
    }

    public IActionResult Services()
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("Services");

        return View();
    }

    public IActionResult Pricing()
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("Pricing");

        return View();
    }

    public IActionResult DentalSolutions()
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("DentalSolutions");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}