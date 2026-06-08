using Microsoft.EntityFrameworkCore;
using ITAM.Data;
using ITAM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ITAM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItamDbContext _context;

        public HomeController(ItamDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalAsset = await _context.Assets.CountAsync();

            ViewBag.ActiveAsset =
                await _context.Assets.CountAsync(x => x.Status == "Active");

            ViewBag.RepairAsset =
                await _context.Assets.CountAsync(x => x.Status == "Repair");

            ViewBag.RetiredAsset =
                await _context.Assets.CountAsync(x => x.Status == "Retired");

            ViewBag.GoodAsset =
                await _context.Assets.CountAsync(x => x.Condition == "Good");

            ViewBag.WarningAsset =
                await _context.Assets.CountAsync(x => x.Condition == "Warning");

            ViewBag.BadAsset =
                await _context.Assets.CountAsync(x => x.Condition == "Bad");

            return View();
        }
    }
}
