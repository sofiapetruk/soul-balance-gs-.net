using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Data;
using soulBalanceGs.Models;

namespace soulBalanceGs.Repository
{
    public class AtividadeRepository : IAtividadeRepository
    {
        private readonly AppDbContext _context;

        public AtividadeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Atividade> FindByUsuarioIdAndAtividadeIdAsync(long userId, long atividadeId)
        {
            return await _context.Atividades
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.FkIdUsuario == userId &&
                    a.AtividadeId == atividadeId
                );
        }
    }
}
