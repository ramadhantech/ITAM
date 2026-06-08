using ITAM.Models;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITAM.Controllers
{
    public class LocationController : Controller
    {
        private readonly LocationService _service;

        public LocationController(LocationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
           var data = await _service.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Location request)
        {
            if (!ModelState.IsValid) 
                return View(request);

            await _service.CreateAsync(request);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetById(id);

            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Location request)
        {
            await _service.UpdateAsync(id, request);
            return Redirect(nameof(Index));
        }
       
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
           await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
