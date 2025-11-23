using System.ComponentModel;

namespace soulBalanceGs.Enuns
{
    public enum TipoAtividade
    {
        [Description("Trabalho de Alto Foco")]
        TRABALHO_FOCO = 1,

        [Description("Trabalho Criativo/Soft Skill")]
        TRABALHO_CRIATIVO = 2,

        [Description("Estudo e Aprendizado Formal")]
        ESTUDO_APRENDIZADO = 3,

        [Description("Pausa Ativa (ex: alongamento)")]
        PAUSA_ATIVA = 4,

        [Description("Descanso Passivo (ex: cochilo, ócio)")]
        DESCANSO_PASSIVO = 5,

        [Description("Lazer e Interação Social")]
        LAZER_SOCIAL = 6,

        [Description("Meditação e Mindfulness")]
        MEDITACAO_MINDFULNESS = 7,

        [Description("Exercício Físico Moderado/Intenso")]
        EXERCICIO_FISICO = 8
    }
}
