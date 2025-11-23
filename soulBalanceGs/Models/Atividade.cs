using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using soulBalanceGs.Enuns;

namespace soulBalanceGs.Models
{
    public class Atividade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AtividadeId { get; set; }

        [Required]
        public TipoAtividade TipoAtividade { get; set; }

        public string Descricao { get; set; }

        [Required]
        public DateTime Inicio { get; set; }

        [Required]
        public DateTime Fim { get; set; }

        [Required]
        public long DuracaoMinutosAtividade { get; set; }

        public int FkIdUsuario { get; set; }

        [ForeignKey(nameof(FkIdUsuario))]
        public Usuario Usuario { get; set; }
    }
}