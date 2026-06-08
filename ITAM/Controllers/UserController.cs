using ITAM.Dto;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITAM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _service;
        private readonly ServiceDepartment _departmentService;
        private readonly LocationService _locationService;

        public UserController(
            UserService service,
            ServiceDepartment departmentService,
            LocationService locationService)
        {
            _service = service;
            _departmentService = departmentService;
            _locationService = locationService;
        }

        /* =========================
           INDEX GET
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _service.GetAllAsync();
            return View(user);
        }

        /* =========================
           CREATE GET
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new List<string> { "Admin", "User" };

            ViewBag.Departments = new SelectList(
                await _departmentService.GetAllAsync(),
                "Id",
                "Name"
            );

            ViewBag.Locations = new SelectList(
                await _locationService.GetAllAsync(),
                "Id",
                "Name"
            );

            return View();
        }

        /* =========================
           CREATE POST
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Admin", "User" };

                ViewBag.Departments = new SelectList(
                    await _departmentService.GetAllAsync(),
                    "Id",
                    "Name"
                );

                ViewBag.Locations = new SelectList(
                await _locationService.GetAllAsync(),
                "Id",
                 "Name",
                dto.LocationId
                );

                return View(dto);
            }

            await _service.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           EDIT GET
        ========================= */
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _service.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new List<string> { "Admin", "User" };

            ViewBag.Departments = new SelectList(
                await _departmentService.GetAllAsync(),
                "Id",
                "Name",
                user.DepartmentId
            );

            ViewBag.Locations = new SelectList(
            await _locationService.GetAllAsync(),
            "Id",
             "Name",
            user.LocationId
                );

            return View("Update", user);
        }

        /* =========================
           UPDATE POST
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Admin", "User" };

                ViewBag.Departments = new SelectList(
                    await _departmentService.GetAllAsync(),
                    "Id",
                    "Name",
                    dto.DepartmentId
                );

                ViewBag.Locations = new SelectList(
                 await _locationService.GetAllAsync(),
                 "Id",
                 "Name",
                 dto.LocationId
);
                return View(dto);
            }

            await _service.UpdateAsync(id, dto);

            return RedirectToAction(nameof(Index));
        }

        /* =========================
           DELETE POST
        ========================= */
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