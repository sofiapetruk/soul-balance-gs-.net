using soulBalanceGs.Enuns;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace soulBalanceGs.Models
{
    public class CheckinManual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("chekin_id")]
        public int ChekinId { get; set; }

        [Column("humor")]
        public string Humor { get; set; }

        [Column("energia")]
        public string Energia { get; set; }

        [Column("foco")]
        public string Foco { get; set; }

        [Column("time")]
        public DateTime Time { get; set; }

        public Usuario Usuario { get; set; }

        [ForeignKey("Usuario")]
        [Column("fk_id_usuario")]
        public int FkIdUsuario { get; set; }
    }
}

