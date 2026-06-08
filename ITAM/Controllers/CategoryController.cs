using ITAM.Models;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITAM.Controllers
{
    public class CategoryController : Controller
    {
       private readonly CategoryService _service;

        public CategoryController(CategoryService service)
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
        public async Task<IActionResult> Create(Category request)
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
                return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category req)
        {
            await _service.UpdateAsync(id, req);
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
