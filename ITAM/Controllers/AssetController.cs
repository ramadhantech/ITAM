using ITAM.Dto;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITAM.Controllers
{
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
            UserService userService
,
            VendorService vendorService)
        {
            _assetService = assetService;
            _categoryService = categoryService;
            _locationService = locationService;
            _userService = userService;
            _vendorService = vendorService;
        }

        /* =========================
           INDEX
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Index(
            string search,
            int? categoryId,
            int? locationId)
        {
            var data = await _assetService.GetAllAsync(search, categoryId, locationId);

            var kategori = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(kategori, "Id", "Name", categoryId);

            var lokasi = await _locationService.GetAllAsync();
            ViewBag.Locations = new SelectList(lokasi, "Id", "Name", locationId);

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentLocation = locationId;

            return View(data);
        }

        /* =========================
           DETAILS
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _assetService.GetDetailsAsync(id);

            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        /* =========================
           CREATE (GET)
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await AmbilDataDropdown();
            return View();
        }

        /* =========================
           CREATE (POST)
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken] // Rekomendasi keamanan tambahan
        public async Task<IActionResult> Create(CreateAssetDto dto)
        {
            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _assetService.CreateAsync(dto, currentUserId);
            return RedirectToAction(nameof(Index));
        }

        /* =========================
           EDIT (GET)
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _assetService.GetByIdAsync(id);

            if (asset == null)
            {
                return NotFound();
            }

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

            // isi category & location dulu
            await AmbilDataDropdown();

            // BARU isi user
            var users = await _userService.GetByLocationAsync(asset.LocationId);

            ViewBag.Users = new SelectList(
                users,
                "Id",
                "Name",
                asset.UserId
            );

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByLocation(int locationId)
        {
            var users = await _userService.GetByLocationAsync(locationId);

            return Json(
                users.Select(x => new
                {
                    x.Id,
                    x.Name
                }));
        }


        /* =========================
           EDIT (POST)
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken] // Rekomendasi keamanan tambahan
        public async Task<IActionResult> Edit(int id, CreateAssetDto request)
        {
            if (!ModelState.IsValid)
            {
                await AmbilDataDropdown();

                RouteData.Values["id"] = id;
                return View(request);
            }

            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sukses = await _assetService.UpdateAsync(
                id,
                request,
                currentUserId);

            if (!sukses)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           DELETE (POST)
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _assetService.DeleteAsync(id);
            
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        /* =========================
           HELPER DROPDOWN
        ========================= */
        private async Task AmbilDataDropdown()
        {
            var kategori = await _categoryService.GetAllAsync();
            var lokasi = await _locationService.GetAllAsync();
            var vendors = await _vendorService.GetAllAsync();

            ViewBag.Categories = new SelectList(kategori, "Id", "Name");
            ViewBag.Locations = new SelectList(lokasi, "Id", "Name");
            ViewBag.Vendors = new SelectList(vendors, "Id", "VendorName");

            // Kosong dulu, nanti diisi AJAX berdasarkan lokasi
            ViewBag.Users = new SelectList(
                new List<SelectListItem>(),
                "Value",
                "Text");
        }
    }
}
