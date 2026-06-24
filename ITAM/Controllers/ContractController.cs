using ITAM.Dto;
using ITAM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ContractController : Controller
{
    private readonly ContractService _contractService;
    private readonly VendorService _vendorService;

    public ContractController(
        ContractService contractService,
        VendorService vendorService)
    {
        _contractService = contractService;
        _vendorService = vendorService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        int pageSize = 10;

        var allData = await _contractService.GetAllAsync();

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

    [HttpGet]
    public async Task<IActionResult> GetByVendor(int vendorId)
    {
        var data = await _contractService.GetByVendorAsync(vendorId);
        return Json(data);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Vendors = await _vendorService.GetAllAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ContractDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Vendors = await _vendorService.GetAllAsync();
            return View(dto);
        }

        await _contractService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var data = await _contractService.GetByIdAsync(id);

        if (data == null) return NotFound();

        ViewBag.Vendors = await _vendorService.GetAllAsync();

        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ContractDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Vendors = await _vendorService.GetAllAsync();
            return View(dto);
        }

        var result = await _contractService.UpdateAsync(dto);

        if (!result) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _contractService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}