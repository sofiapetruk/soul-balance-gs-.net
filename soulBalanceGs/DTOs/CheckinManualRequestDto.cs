using soulBalanceGs.Enuns;
using System.ComponentModel.DataAnnotations;

namespace soulBalanceGs.DTOs
{
    public class CheckinManualRequestDto
    {
        [Required(ErrorMessage = "O campo Humor é obrigatório.")]
        public string Humor { get; set; }

        [Required(ErrorMessage = "O campo Energia é obrigatório.")]
        public string Energia { get; set; }

        [Required(ErrorMessage = "O campo Foco é obrigatório.")]
        public string Foco { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }
    }
}
