using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ── IBaseRepository<User> ─────────────────────────────────────────────

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FindAsync(id);

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task<User> CreateAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Users.AnyAsync(u => u.Id == id);

        // ── IUserRepository ───────────────────────────────────────────────────

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<bool> EmailExistsAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<IEnumerable<User>> GetAllUsersAsync()
            => await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

        public async Task ToggleActiveStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRoleAsync(int userId, string role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
            }
        }
    }
}
