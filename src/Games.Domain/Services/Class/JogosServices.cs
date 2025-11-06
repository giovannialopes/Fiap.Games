using Games.Domain.DTO;
using Games.Domain.Entities;
using Games.Domain.Queue;
using Games.Domain.Queue.Event;
using Games.Domain.Repositories;
using Games.Domain.Results;
using Games.Domain.Services.Interface;
using Games.Domain.Shared.Http;

namespace Games.Domain.Services.Class;

/// <summary>
/// Serviço responsável pelas operações de gerenciamento de jogos, incluindo cadastro, alteração, compra, consulta e exclusão.
/// </summary>
public class JogosServices(
    IJogosRepository jogosRepository,
    IPromocoesServices promocoesServices,
    ILoggerServices logger,
    IWalletBusPublisher walletBusPublisher) : IJogosServices
{
    public async Task<Result<JogosDto.JogosDtoResponse>> AlterarJogos(string Nome, JogosDto.JogosDtoRequest request) {
        var jogo = await jogosRepository.ObterJogoPorNome(Nome);
        if (jogo == null) {
            await logger.LogError($"Jogo não encontrado.");
            return Result.Failure<JogosDto.JogosDtoResponse>("Jogo não encontrado.", "500");
        }

        jogo.Nome = request.Nome;
        jogo.Descricao = request.Descricao;
        jogo.Preco = request.Preco;
        jogo.Tipo = request.Tipo;
        await jogosRepository.AtualizarJogos(jogo);

        await logger.LogInformation("Finalizou AlterarJogos");
        return Result.Success(new JogosDto.JogosDtoResponse {
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            Preco = jogo.Preco,
            Tipo = jogo.Tipo,
        });
    }


    public async Task<Result<JogosDto.JogosDtoResponse>> CadastrarJogos(JogosDto.JogosDtoRequest request) {

        var jogo = await jogosRepository.ObterJogoPorNome(request.Nome);
        if (jogo != null) {
            await logger.LogError($"Jogo já foi cadastrado.");
            return Result.Failure<JogosDto.JogosDtoResponse>("Esse jogo já foi cadastrado.", "500");
        }

        jogo = GamesEnt.Criar(request.Nome,
           request.Descricao,
           request.Preco,
           request.Tipo,
           true);

        await jogosRepository.AdicionarJogos(jogo);

        await logger.LogInformation("Finalizou CadastrarJogos");
        return Result.Success(new JogosDto.JogosDtoResponse {
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            Preco = jogo.Preco,
            Tipo = jogo.Tipo,
        });

    }


    public async Task<Result<JogosDto.JogosDtoResponseCompra>> ComprarJogos(JogosDto.JogosDtoComprarJogos request, string userId) {

        var jogo = await jogosRepository.ObterJogoPorNome(request.Nome);
        if (jogo == null) {
            await logger.LogError($"Jogo não encontrado.");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Jogo não encontrado.", "500");
        }

        var jaPossui = await jogosRepository.VerificaSeUsuarioPossuiJogo(jogo.Id.ToString(), userId);
        if (jaPossui != null) {
            await logger.LogError($"Você ja possui esse jogo, não é possivel comprar novamente.");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Você ja possui esse jogo, não é possivel comprar novamente.", "500");
        }

        var promoResult = await promocoesServices.ConsultaPromocoesAtivas();
        var precoFinal = jogo.Preco;

        if (promoResult.IsSuccess) {
            var promocao = promoResult.Value;

            if (promocao.IdJogos.Contains(jogo.Id)) {
                precoFinal = promocao.Valor;
            }
        }

        var carteira = User.PegaSaldo(Guid.Parse(userId));

        if (carteira == null || carteira.Saldo < precoFinal) {
            await logger.LogError($"Saldo insuficiente.");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Saldo insuficiente.", "500");
        }

        await walletBusPublisher.Publish(new PaymentCreatedEvent(
            JogoId : jogo.Id,
            PerfilId: Guid.Parse(userId),
            saldo: precoFinal
        ), CancellationToken.None);


        await logger.LogInformation("Finalizou ComprarJogos");
        return Result.Success(new JogosDto.JogosDtoResponseCompra {
            PerfilId = Guid.Parse(userId),
            JogoId = jogo.Id     
        });

    }

    public async Task<Result<List<JogosDto.JogosDtoResponse>>> ConsultarJogos() {
        var jogos = await jogosRepository.ObterJogos();

        if (jogos == null || !jogos.Any()) {
            await logger.LogError($"Essa plataforma não possui jogos disponíveis.");
            return Result.Failure<List<JogosDto.JogosDtoResponse>>("Essa plataforma não possui jogos disponíveis.", "500");
        }

        await logger.LogInformation("Finalizou ConsultarJogos");
        var response = jogos.Select(x => new JogosDto.JogosDtoResponse {
            Nome = x.Nome,
            Descricao = x.Descricao,
            Preco = x.Preco,
            Tipo = x.Tipo,
        }).ToList();

        return Result.Success(response);
    }

    public async Task<Result<JogosDto.JogosDtoResponse>> ConsultarUnicoJogo(string Nome) {
        var jogo = await jogosRepository.ObterJogoPorNome(Nome);
        if (jogo == null) {
            await logger.LogError($"Jogo não encontrado.");
            return Result.Failure<JogosDto.JogosDtoResponse>("Jogo não encontrado.", "500");
        }

        await logger.LogInformation("Finalizou ConsultarUnicoJogo");
        return Result.Success(new JogosDto.JogosDtoResponse {
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            Preco = jogo.Preco,
            Tipo = jogo.Tipo,
        });
    }

    public async Task<Result<JogosDto.JogosDtoResponse>> DeletarJogos(string Nome) {
        var jogo = await jogosRepository.ObterJogoPorNome(Nome);
        if (jogo == null) {
            await logger.LogError($"Jogo não encontrado.");
            return Result.Failure<JogosDto.JogosDtoResponse>("Jogo não encontrado.", "500");
        }

        jogo.Ativo = false;
        await jogosRepository.AtualizarJogos(jogo);

        await logger.LogInformation("Finalizou DeletarJogos");
        return Result.Success(new JogosDto.JogosDtoResponse {
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            Preco = jogo.Preco,
            Tipo = jogo.Tipo,
        });
    }
}