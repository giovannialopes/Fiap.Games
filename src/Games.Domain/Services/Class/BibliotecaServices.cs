using Games.Domain.DTO;
using Games.Domain.Entities;
using Games.Domain.Repositories;
using Games.Domain.Results;
using Games.Domain.Services.Interface;
using Games.Domain.Shared.Http;

namespace Games.Domain.Services.Class;

/// <summary>
/// Serviço responsável pelas operações de consulta da biblioteca de jogos de um usuário.
/// </summary>
public class BibliotecaServices(
    IJogosRepository jogosRepository,
    IBibliotecaRepository bibliotecaRepository,
    ILoggerServices logger) : IBibliotecaServices
{
    public async Task<Result<LibraryDto.LibraryDtoResponse>> AdicionarJogoBiblioteca(LibraryDto.LibraryDtoRequest request) {

        var jaPossui = await bibliotecaRepository.ValidaSeJaPossuiJogo(request.JogoId, request.PerfilId);
        if (jaPossui != null) {
            // Idempotente: se já possui, retorna sucesso com os dados do jogo
            var jogoExistente = await jogosRepository.ObterJogoPorId(request.JogoId);
            await logger.LogInformation($"Usuário {request.PerfilId} já possuía o jogo {request.JogoId}. Operação idempotente.");
            var responseExistente = new LibraryDto.LibraryDtoResponse {
                Nome = jogoExistente.Nome,
                Descricao = jogoExistente.Descricao,
                Preco = jogoExistente.Preco,
                Tipo = jogoExistente.Tipo,
            };
            return Result.Success(responseExistente);
        }

        var bibliotecaEnt = LibraryEnt.Criar(request.JogoId, request.PerfilId);
        await bibliotecaRepository.AdicionaNaBiblioteca(bibliotecaEnt);

        var Jogo = await jogosRepository.ObterJogoPorId(request.JogoId);

        await logger.LogInformation("AdicionarJogoBiblioteca");

        var response = new LibraryDto.LibraryDtoResponse {
            Nome = Jogo.Nome,
            Descricao = Jogo.Descricao,
            Preco = Jogo.Preco,
            Tipo = Jogo.Tipo,
        };

        return Result.Success(response);
    }

    public async Task<Result<List<JogosDto.JogosDtoResponse>>> ConsultarBibliotecaPorUsuario(string Usuario) {
        var jogos = await jogosRepository.ObterJogosPorUsuario(Usuario);

        if (!jogos.Any()) {
            await logger.LogError($"O Usuário : {Usuario} não possui jogos na biblioteca.");
            return Result.Failure<List<JogosDto.JogosDtoResponse>>("O Usuário não possui jogos na biblioteca.", "404");
        }

        var response = jogos.Select(x => new JogosDto.JogosDtoResponse {
            Nome = x.Nome,
            Descricao = x.Descricao,
            Preco = x.Preco,
            Tipo = x.Tipo,
        }).ToList();

        await logger.LogInformation("Finalizou ConsultarBiblioteca");
        return Result.Success(response);
    }
}