using soulBalanceGs.Enuns;
using soulBalanceGs.Hateos;
using soulBalanceGs.Models;

namespace soulBalanceGs.DTOs
{
    public class AtividadeResponseDto 
    {
        public int AtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }
        public string Descricao { get; set; }
        public long DuracaoMinutosAtividade { get; set; }
        public int Usuario { get; set; }

        public List<Link> Links { get; set; } = new List<Link>();

        public static AtividadeResponseDto FromEntity(Atividade atividade)
        {
            return new AtividadeResponseDto
            {
                AtividadeId = atividade.AtividadeId,
                TipoAtividade = atividade.TipoAtividade,
                Descricao = atividade.Descricao,
                DuracaoMinutosAtividade = atividade.DuracaoMinutosAtividade,
                Usuario = atividade.Usuario?.IdUsuario ?? atividade.FkIdUsuario,
                Links = new List<Link>()
            };
        }
    }
}
