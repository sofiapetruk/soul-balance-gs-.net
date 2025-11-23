using soulBalanceGs.Models;

namespace soulBalanceGs.Repository
{
    public interface IAtividadeRepository 
    {
        Task<Atividade> FindByUsuarioIdAndAtividadeIdAsync(long userId, long atividadeId);
    }
}
