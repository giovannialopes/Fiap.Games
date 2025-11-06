namespace Games.Domain.DTO;

public class LibraryDto
{
    public class LibraryDtoRequest
    {
        public Guid PerfilId { get; set; }
        public Guid JogoId { get; set; }
    }
    public class LibraryDtoResponse
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Tipo { get; set; } = string.Empty;

    }


}
