using ITAM.Data;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class ServiceDepartment
    {
        private readonly ItamDbContext _context;

        public ServiceDepartment(ItamDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<Department> CreateAsync(Department model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var department = new Department
            {
                Name = model.Name.Trim()
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return department;
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<bool> UpdateAsync(int id, Department model)
        {
            var data = await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
                return false;

            data.Name = model.Name.Trim();

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteAsync(int id)
        {
            var data = await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
                return false;

            _context.Departments.Remove(data);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}