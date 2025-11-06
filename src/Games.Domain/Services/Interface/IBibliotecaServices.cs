using Games.Domain.DTO;
using Games.Domain.Results;

namespace Games.Domain.Services.Interface;

public interface IBibliotecaServices
{
    Task<Result<List<JogosDto.JogosDtoResponse>>> ConsultarBibliotecaPorUsuario(string Usuario);
    Task<Result<LibraryDto.LibraryDtoResponse>> AdicionarJogoBiblioteca(LibraryDto.LibraryDtoRequest request);
}


