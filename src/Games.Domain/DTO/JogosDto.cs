using System.ComponentModel.DataAnnotations;

namespace Games.Domain.DTO;

public class JogosDto
{
    public class JogosDtoRequest
    {
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public decimal Preco { get; set; }
        [Required]
        public string Descricao { get; set; } = string.Empty;
        [Required]
        public string Tipo { get; set; } = string.Empty;
    }

    public class JogosDtoResponse
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class JogosDtoComprarJogos
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class JogosDtoResponseCompra
    {
        public Guid PerfilId { get; set; }
        public Guid JogoId { get; set; }
        
    }

}
