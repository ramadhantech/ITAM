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

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VendorDto req)
        {
            if (!ModelState.IsValid)
            {
                return View(req);
            }

            await _service.CreateAsync(req);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int Id)
        {
            var data = await _service.GetByIdAsync(Id);

            if (data == null)
                return NotFound();

            // Sederhana sekali, karena 'data' sudah otomatis bertipe VendorDto
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VendorDto req)
        {
            // Perbaikan Logika: Jika model TIDAK valid, kembalikan ke form beserta error-nya
            if (!ModelState.IsValid)
            {
                return View(req);
            }

            var result = await _service.UpdateAsync(req);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
