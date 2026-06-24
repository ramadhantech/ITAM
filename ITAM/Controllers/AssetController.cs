using ITAM.Dto;
using ITAM.Models;
using ITAM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ITAM.Controllers
{
    [Authorize]
    public class AssetController : Controller
    {
        private readonly AssetService _assetService;
        private readonly CategoryService _categoryService;
        private readonly LocationService _locationService;
        private readonly UserService _userService;
        private readonly VendorService _vendorService;

        public AssetController(
            AssetService assetService,
            CategoryService categoryService,
            LocationService locationService,
            UserService userService,
            VendorService vendorService)
        {
            _assetService = assetService;
            _categoryService = categoryService;
            _locationService = locationService;
            _userService = userService;
            _vendorService = vendorService;
        }

        /* =========================
           INDEX + PAGINATION FIX
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Index(
            string search,
            int? categoryId,
            int? locationId,
            int page = 1)
        {
            int pageSize = 3;

            var allData = await _assetService.GetAllAsync(search, categoryId, locationId);

            var totalItems = allData.Count;

            var items = allData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResultDto<Asset>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            var kategori = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(kategori, "Id", "Name", categoryId);

            var lokasi = await _locationService.GetAllAsync();
            ViewBag.Locations = new SelectList(lokasi, "Id", "Name", locationId);

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentLocation = locationId;

            return View(result);
        }

        /* =========================
           DETAILS
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _assetService.GetDetailsAsync(id);

            if (asset == null)
                return NotFound();

            return View(asset);
        }

        /* =========================
           CREATE
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await AmbilDataDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAssetDto dto)
        {
            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _assetService.CreateAsync(dto, currentUserId);

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           EDIT GET
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _assetService.GetByIdAsync(id);

            if (asset == null)
                return NotFound();

            var dto = new CreateAssetDto
            {
                AssetTag = asset.AssetTag,
                AssetName = asset.AssetName,
                CategoryId = asset.CategoryId,
                AssetSubtype = asset.AssetSubtype,
                ContractId = asset.ContractId,
                VendorId = asset.ContractName?.VendorId,
                SerialNumber = asset.SerialNumber,
                LocationId = asset.LocationId,
                UserId = asset.UserId,
                AssetType = asset.AssetType,
                Status = asset.Status,
                Condition = asset.Condition,
                Note = asset.Note
            };

            await AmbilDataDropdown();

            var users = await _userService.GetByLocationAsync(asset.LocationId);

            ViewBag.Users = new SelectList(users, "Id", "Name", asset.UserId);

            return View(dto);
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

        /* =========================
           EDIT POST
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateAssetDto request)
        {
            if (!ModelState.IsValid)
            {
                await AmbilDataDropdown();
                return View(request);
            }

            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sukses = await _assetService.UpdateAsync(id, request, currentUserId);

            if (!sukses)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           DELETE
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assetService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           DROPDOWN HELPER
        ========================= */
        private async Task AmbilDataDropdown()
        {
            var kategori = await _categoryService.GetAllAsync();
            var lokasi = await _locationService.GetAllAsync();
            var vendors = await _vendorService.GetAllAsync();

            ViewBag.Categories = new SelectList(kategori, "Id", "Name");
            ViewBag.Locations = new SelectList(lokasi, "Id", "Name");
            ViewBag.Vendors = new SelectList(vendors, "Id", "VendorName");

            ViewBag.Users = new SelectList(new List<SelectListItem>(), "Value", "Text");
        }
    }
}