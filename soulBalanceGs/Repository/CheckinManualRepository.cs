using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.Models;

namespace soulBalanceGs.Repository
{
    public class CheckinManualRepository : ICheckinManualRepository
    {
        private readonly AppDbContext _context;

        public CheckinManualRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CheckinManual>> FindByUsuarioIdAsync(int userId)
        {
            return await _context.CheckinManuais
                .AsNoTracking()
                .Where(c => c.FkIdUsuario == userId)
                .ToListAsync();
        }

        public async Task<int> DeleteByUsuarioIdAndChekinIdAsync(int userId, int chekinId)
        {
            var entity = await _context.CheckinManuais
                .FirstOrDefaultAsync(c => c.FkIdUsuario == userId && c.ChekinId == chekinId);

            if (entity == null)
                return 0;

            _context.CheckinManuais.Remove(entity);
            return await _context.SaveChangesAsync();
        }
    }
}
