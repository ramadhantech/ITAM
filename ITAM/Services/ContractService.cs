using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class ContractService
{
    private readonly ItamDbContext _context;
    private readonly IMapper _mapper;

    public ContractService(ItamDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Contract>> GetAllAsync()
    {
        return await _context.Contracts
            .AsNoTracking()
            .Include(x => x.Vendor)
            .OrderBy(x => x.ContractNumber)
            .ToListAsync();
    }

    public async Task<ContractDto?> GetByIdAsync(int id)
    {
        var data = await _context.Contracts.FindAsync(id);

        if (data == null) return null;

        return _mapper.Map<ContractDto>(data);
    }

    public async Task<List<ContractDto>> GetByVendorAsync(int vendorId)
    {
        return await _context.Contracts
            .AsNoTracking()
            .Where(x => x.VendorId == vendorId)
            .Select(x => new ContractDto
            {
                Id = x.Id,
                ContractNumber = x.ContractNumber
            })
            .ToListAsync();
    }

    public async Task CreateAsync(ContractDto dto)
    {
        var entity = _mapper.Map<Contract>(dto);

        entity.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
        entity.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);

        _context.Contracts.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(ContractDto dto)
    {
        var data = await _context.Contracts.FindAsync(dto.Id);

        if (data == null) return false;

        _mapper.Map(dto, data);

        data.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
        data.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var data = await _context.Contracts.FindAsync(id);

        if (data == null) return false;

        _context.Contracts.Remove(data);
        await _context.SaveChangesAsync();

        return true;
    }
}