using soulBalanceGs.Enuns;
using System.ComponentModel.DataAnnotations;

namespace soulBalanceGs.DTOs
{
    public class AtividadeRequestDto
    {
        public TipoAtividade TipoAtividade { get; set; }

        [Required(ErrorMessage = "A data e hora de início são obrigatórias.")]
        public DateTime Inicio { get; set; }

        [Required(ErrorMessage = "A data e hora de fim são obrigatórias.")]
        public DateTime Fim { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O e-mail do usuário é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }
    }
}
