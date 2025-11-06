using Games.Domain.DTO;
using Games.Domain.Results;

namespace Games.Domain.Services.Interface;

public interface IJogosServices
{
    Task<Result<JogosDto.JogosDtoResponse>> CadastrarJogos(JogosDto.JogosDtoRequest request);
    Task<Result<JogosDto.JogosDtoResponseCompra>> ComprarJogos(JogosDto.JogosDtoComprarJogos request, string UserId);
    Task<Result<JogosDto.JogosDtoResponse>> ConsultarUnicoJogo(string Nome);
    Task<Result<List<JogosDto.JogosDtoResponse>>> ConsultarJogos();
    Task<Result<JogosDto.JogosDtoResponse>> DeletarJogos(string Nome);
    Task<Result<JogosDto.JogosDtoResponse>> AlterarJogos(string Nome, JogosDto.JogosDtoRequest request);
}
