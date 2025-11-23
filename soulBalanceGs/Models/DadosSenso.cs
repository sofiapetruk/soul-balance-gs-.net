using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace soulBalanceGs.Models
{
    public class DadosSensor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DadosSensorId { get; set; }

        [Required]
        public string TipoDado { get; set; }

        [Required]
        public int Valor { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public int? FkIdUsuario { get; set; }

        [ForeignKey(nameof(FkIdUsuario))]
        public Usuario Usuario { get; set; }
    }
}
