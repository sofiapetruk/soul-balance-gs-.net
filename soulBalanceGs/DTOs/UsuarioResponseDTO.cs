using soulBalanceGs.Hateos;


namespace soulBalanceGs.DTOs
{
    public class UsuarioResponseDto
    {
        public long IdUsuario { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public List<Link> Links { get; set; } = new List<Link>();
    }
}
