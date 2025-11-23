using System.ComponentModel;

namespace soulBalanceGs.Enuns
{
    public enum TipoDadoSensor
    {
        [Description("Horas de Sono")]
        SONO_HORAS,

        [Description("Média de Batimentos Cardíacos")]
        BATIMENTOS_MEDIOS,

        [Description("Passos Diários")]
        ATIVIDADE_PASSOS,

        [Description("Calorias Queimadas")]
        ATIVIDADE_CALORIAS
    }
}
