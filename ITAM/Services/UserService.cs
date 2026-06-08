using BCrypt.Net;
using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class UserService
    {
        private readonly ItamDbContext _context;

        public UserService(ItamDbContext context)
        {
            _context = context;
        }

        /* ==========================================================
           VALIDASI LOGIN
        ========================================================== */
        public async Task<User?> ValidateLoginAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return null;
            }

            bool validPassword = BCrypt.Net.BCrypt.Verify(
                password,
                user.Password
            );

            if (!validPassword)
            {
                return null;
            }

            return user;
        }

        /* ==========================================================
           1. READ ALL
        ========================================================== */
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Location)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /* ==========================================================
           2. READ BY ID
        ========================================================== */
        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetByLocationAsync(int locationId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(x => x.LocationId == locationId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        /* ==========================================================
           3. CREATE
        ========================================================== */
        public async Task CreateAsync(CreateUserDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var data = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                DepartmentId = dto.DepartmentId,
                Role = dto.Role,
                LocationId = dto.LocationId,
                // HASH PASSWORD
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(data);

            await _context.SaveChangesAsync();
        }

        /* ==========================================================
           4. UPDATE
        ========================================================== */
        public async Task UpdateAsync(int id, UpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return;
            }

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.DepartmentId = dto.DepartmentId;
            user.Role = dto.Role;
            user.LocationId = dto.LocationId;
            // UPDATE PASSWORD JIKA DIISI
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
        }

        /* ==========================================================
           5. DELETE
        ========================================================== */
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return false;
            }

            var isUsed = await _context.Assets
                .AnyAsync(a => a.CreatedBy == id);

            if (isUsed)
            {
                throw new Exception("User masih digunakan oleh asset");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}