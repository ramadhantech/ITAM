using AutoMapper;
using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class VendorService
    {
        private readonly ItamDbContext _context;
        private readonly IMapper _mapper;
        public VendorService(ItamDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VendorDto>> GetAllAsync()
        {
            var data = await _context.Vendors
                .AsNoTracking()
                .OrderBy(x => x.VendorName)
                .ToListAsync();

            return _mapper.Map<List<VendorDto>>(data);

        }

        public async Task<VendorDto?> GetByIdAsync(int id)
        {
            var data = await _context.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null) 
                return null;

            return _mapper.Map<VendorDto>(data);
        }

        public async Task CreateAsync(VendorDto req)
        {
            var vendor = _mapper.Map<Vendor>(req);

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(VendorDto req)
        {
            var data = await _context.Vendors.FindAsync(req.Id);

            if (data == null)
                return false;

            // Memetakan nilai dari 'req' (VendorDto) langsung ke dalam objek 'data' (Vendor) yang ada di database
            _mapper.Map(req, data);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = await _context.Vendors
                .FindAsync (id);

            if (data == null) return false;

             _context.Vendors.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
