namespace Games.Domain.Shared.Entities;

public class CarteiraEnt
{
    
    public long Id { get; set; }
    public Guid PerfilId { get; set; }
    public decimal Saldo { get; set; }


    public static CarteiraEnt Criar(Guid perfilId, decimal saldo) {
        return new CarteiraEnt {
            Saldo = saldo,
            PerfilId = perfilId

        };
    }

    public static CarteiraEnt Atualizar(long id, Guid perfilId, decimal saldo) {
        return new CarteiraEnt {
            Saldo = saldo,
            PerfilId = perfilId,
            Id = id

        };
    }
}
