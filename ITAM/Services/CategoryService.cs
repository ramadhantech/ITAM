using ITAM.Data;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class CategoryService
    {
        private readonly ItamDbContext _context;

        public CategoryService(ItamDbContext context)
        {
            _context = context;
        }

        // 1. READ ALL (Dioptimasi agar muat menu Kategori secepat kilat)
        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking() // Perbaikan 1: Matikan pelacakan memori RAM server agar query super ringan
                .ToListAsync();
        }

        // 2. READ BY ID (Dioptimasi untuk memuat data lama di form Edit secara instan)
        public async Task<Category?> GetById(int id)
        {
            return await _context.Categories
                .AsNoTracking() // Perbaikan 2: Menghemat resource server untuk pembacaan data tunggal
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        // 3. CREATE (Tetap dipertahankan karena sudah benar)
        public async Task CreateAsync(Category request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var data = new Category
            {
                Name = request.Name,
            };

            _context.Categories.Add(data);
            await _context.SaveChangesAsync();
        }

        // 4. UPDATE (Optimasi: Hanya eksekusi SaveChanges jika data benar-benar ditemukan)
        public async Task UpdateAsync(int id, Category request)
        {
            var data = await _context.Categories.FindAsync(id);

            if (data != null)
            {
                data.Name = request.Name;
                await _context.SaveChangesAsync(); // Taruh di dalam blok IF agar tidak membuang proses database
            }
        }

        // 5. DELETE (Perbaikan Keamanan: Cek null sebelum menghapus dari database)
        public async Task DeleteAsync(int id)
        {
            var data = await _context.Categories.FindAsync(id);

            // Perbaikan 3: Pastikan data ada sebelum di-Remove untuk mencegah aplikasi crash/eror
            if (data != null)
            {
                _context.Categories.Remove(data);
                await _context.SaveChangesAsync();
            }
        }
    }
}
