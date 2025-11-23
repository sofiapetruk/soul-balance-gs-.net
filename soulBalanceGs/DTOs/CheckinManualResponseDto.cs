using soulBalanceGs.Hateos;
using soulBalanceGs.Models;
using System.Text.Json.Serialization;

namespace soulBalanceGs.DTOs
{
    public class CheckinManualResponseDto
    {
        public long ChekinId { get; set; }
        public string Humor { get; set; }

        public string Energia { get; set; }

        public string Foco { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        public int Usuario { get; set; }

        public List<Link> Links { get; set; } = new List<Link>();

        public static CheckinManualResponseDto FromEntity(CheckinManual checkinManual)
        {
            return new CheckinManualResponseDto
            {
                ChekinId = checkinManual.ChekinId,
                Humor = checkinManual.Humor,
                Energia = checkinManual.Energia,
                Foco = checkinManual.Foco,
                Time = checkinManual.Time,
                Usuario = checkinManual.Usuario?.IdUsuario ?? checkinManual.FkIdUsuario,
                Links = new List<Link>()

            };
        }
    }
}
