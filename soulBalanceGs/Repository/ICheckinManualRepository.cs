using soulBalanceGs.Models;

namespace soulBalanceGs.Repository
{
    public interface ICheckinManualRepository
    {
        Task<List<CheckinManual>> FindByUsuarioIdAsync(int usuarioId);

        Task<int> DeleteByUsuarioIdAndChekinIdAsync(int usuarioId, int chekinId);
    }
}
