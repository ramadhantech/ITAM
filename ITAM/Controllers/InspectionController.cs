using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using ITAM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITAM.Controllers
{
    [Authorize]
    public class InspectionController : Controller
    {
        private readonly InspectionService _service;
        private readonly ItamDbContext _context;
        private readonly ServiceReport _reportService;
        private readonly UserService _userService;

        public InspectionController(
            InspectionService service,
            ItamDbContext context,
            ServiceReport reportService,
            UserService userService)
        {
            _service = service;
            _context = context;
            _reportService = reportService;
            _userService = userService;
        }

        /* ==========================================================
           INDEX
        ========================================================== */
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserId = int.Parse(userIdClaim);

            int pageSize = 10;

            var allData = await _service.GetAllAsync(currentUserId);

            var totalItems = allData.Count;

            var items = allData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        /* ==========================================================
           DETAIL
        ========================================================== */
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var report = await _service.GetByIdAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        /* ==========================================================
           PRINT
        ========================================================== */
        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            var report = await _service.GetByIdAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByLocation(int locationId)
        {
            var users = await _userService.GetByLocationAsync(locationId);

            return Json(users.Select(x => new
            {
                x.Id,
                x.Name
            }));
        }

        /* ==========================================================
           CREATE GET
        ========================================================== */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Locations =
                await _context.Locations
                 .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            // USERS / PIC APPROVAL
            ViewBag.Users = new SelectList(
                new List<SelectListItem>(),
                "Value",
                "Text"
            );

            // ASSETS
            ViewBag.Assets = await _context.Assets
                .AsNoTracking()
                .Where(a => !a.IsDeleted)
                .Select(a => new
                {
                    a.Id,
                    a.AssetName,
                    a.AssetTag,
                    a.CategoryId,
                    a.AssetSubtype
                })
                .ToListAsync();

            return View();
        }

        /* ==========================================================
           GET ASSETS BY LOCATION
        ========================================================== */
        [HttpGet]
        public async Task<IActionResult> GetAssetsByLocation(
            int locationId)
        {
            var assets =
                await _reportService
                    .GetAssetsByLocationIdAsync(locationId);

            if (assets == null)
            {
                return Json(new List<object>());
            }

            var result = assets.Select(a => new
            {
                id = a.Id,
                name = a.AssetName ?? "",
                tag = a.AssetTag ?? "",
                categoryname =
                    a.Category != null
                        ? a.Category.Name.ToUpper().Trim()
                        : "",
                subtype = a.AssetSubtype ?? ""
            }).ToList();

            return Json(result);
        }

        /* ==========================================================
           CREATE POST
        ========================================================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateInspectionReportDto dto)
        {
            if (!ModelState.IsValid)
            {
                // LOCATION
                ViewBag.Locations =
                    await _context.Locations
                        .AsNoTracking()
                        .OrderBy(x => x.Name)
                        .ToListAsync();

                // USERS
                ViewBag.Users = new SelectList(
                    await _context.Users
                        .AsNoTracking()
                        .OrderBy(x => x.Name)
                        .ToListAsync(),
                    "Id",
                    "Name"
                );

                // ASSETS
                ViewBag.Assets = await _context.Assets
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .Select(a => new
                    {
                        a.Id,
                        a.AssetName,
                        a.AssetTag,
                        a.CategoryId,
                        a.AssetSubtype
                    })
                    .ToListAsync();

                return View(dto);
            }

            // USER LOGIN
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int createdBy = int.Parse(userIdClaim);

            // SAVE
            await _service.CreateAsync(dto, createdBy);

            return RedirectToAction(nameof(Index));
        }

        /* ==========================================================
           DELETE
        ========================================================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}