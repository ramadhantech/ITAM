using ITAM.Dto;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITAM.Controllers
{
    public class VendorController : Controller
    {
        private readonly VendorService _service;

        public VendorController(VendorService service)
        {
            _service = service;
        }

        // =========================
        // INDEX + PAGINATION
        // =========================
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;

            var allData = await _service.GetAllAsync();

            var totalItems = allData.Count;

            var items = allData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(items);
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VendorDto req)
        {
            if (!ModelState.IsValid)
                return View(req);

            await _service.CreateAsync(req);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VendorDto req)
        {
            if (!ModelState.IsValid)
                return View(req);

            var result = await _service.UpdateAsync(req);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}