namespace Games.Domain.Entities;

public class PromotionEnt
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<Guid> IdJogos { get; set; } = new();


    public static PromotionEnt Criar(string nome, decimal valor, DateTime datainicio, DateTime datafim, List<Guid> idjogos) {
        return new PromotionEnt {
            Nome = nome,
            Valor = valor,
            DataInicio = datainicio,
            DataFim = datafim,
            IdJogos = idjogos
        };
    }

}
