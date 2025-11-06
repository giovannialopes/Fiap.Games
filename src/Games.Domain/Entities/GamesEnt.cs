namespace Games.Domain.Entities;

public class GamesEnt
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal Preco { get; set; }
    public string Tipo { get; set; } = null!;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public static GamesEnt Criar(string nome, string descricao, decimal preco, string tipo, bool ativo) {
        return new GamesEnt {
            Id = Guid.NewGuid(),
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            Tipo = tipo,
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow,
        };
    }
}
