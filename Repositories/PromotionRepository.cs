
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── IBaseRepository<Promotion> ─────────────────────────────────────

        public async Task<Promotion?> GetByIdAsync(int id)
            => await _context.Promotions.FindAsync(id);

        public async Task<IEnumerable<Promotion>> GetAllAsync()
            => await _context.Promotions.ToListAsync();

        public async Task<Promotion> CreateAsync(Promotion entity)
        {
            _context.Promotions.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task<Promotion> UpdateAsync(Promotion entity)
        {
            _context.Promotions.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Promotions.FindAsync(id);
            if (entity is not null)
            {
                _context.Promotions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Promotions.AnyAsync(p => p.Id == id);

        // ── IPromotionRepository ───────────────────────────────────────────

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Promotions
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<DiscountCode?> GetDiscountCodeAsync(string code)
            => await _context.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code == code);

        public async Task IncrementCodeUsageAsync(int codeId)
        {
            var code = await _context.DiscountCodes.FindAsync(codeId);
            if (code is null) return;

            code.UsedCount++;
            await _context.SaveChangesAsync();
        }

        public async Task ExpirePromotionAsync(int promotionId)
        {
            var promotion = await _context.Promotions.FindAsync(promotionId);
            if (promotion is null) return;

            promotion.IsActive = false;
            promotion.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}