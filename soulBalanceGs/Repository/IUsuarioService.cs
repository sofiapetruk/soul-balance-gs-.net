using soulBalanceGs.DTOs;

namespace soulBalanceGs.Repository
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto> FindByEmail(string email);
    }
}
