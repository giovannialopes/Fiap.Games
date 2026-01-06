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
    IWalletBusPublisher walletBusPublisher,
    IUserService userService) : IJogosServices
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
        
        // Validação de entrada
        if (string.IsNullOrWhiteSpace(request?.Nome))
        {
            await logger.LogError("Nome do jogo não informado na compra.");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Nome do jogo é obrigatório.", "400");
        }

        if (!Guid.TryParse(userId, out var perfilId))
        {
            await logger.LogError($"ID de usuário inválido: {userId}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("ID de usuário inválido.", "400");
        }

        // Verificar se o jogo existe e está ativo
        var jogo = await jogosRepository.ObterJogoPorNome(request.Nome);
        if (jogo == null) {
            await logger.LogError($"Jogo não encontrado: {request.Nome}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Jogo não encontrado.", "404");
        }

        if (!jogo.Ativo)
        {
            await logger.LogError($"Jogo inativo: {request.Nome}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Este jogo não está mais disponível para compra.", "400");
        }

        // Verificar se o usuário já possui o jogo
        var jaPossui = await jogosRepository.VerificaSeUsuarioPossuiJogo(jogo.Id.ToString(), userId);
        if (jaPossui != null) {
            await logger.LogWarning($"Usuário {userId} já possui o jogo {jogo.Nome}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Você já possui esse jogo, não é possível comprar novamente.", "400");
        }

        // Calcular preço final considerando promoções
        var promoResult = await promocoesServices.ConsultaPromocoesAtivas();
        var precoFinal = jogo.Preco;
        var descontoAplicado = 0m;

        if (promoResult.IsSuccess && promoResult.Value != null) {
            var promocao = promoResult.Value;

            if (promocao.IdJogos != null && promocao.IdJogos.Contains(jogo.Id)) {
                precoFinal = promocao.Valor;
                descontoAplicado = jogo.Preco - precoFinal;
                await logger.LogInformation($"Promoção aplicada ao jogo {jogo.Nome}: Desconto de {descontoAplicado:C}");
            }
        }

        // Validar preço
        if (precoFinal <= 0)
        {
            await logger.LogError($"Preço inválido para o jogo {jogo.Nome}: {precoFinal}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Preço do jogo inválido.", "400");
        }

        // Verificar saldo do usuário
        var carteira = userService.PegaSaldo(perfilId);
        if (carteira == null) {
            await logger.LogError($"Carteira não encontrada para o usuário {userId}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>("Carteira não encontrada. Por favor, adicione saldo primeiro.", "400");
        }

        if (carteira.Saldo < precoFinal) {
            await logger.LogWarning($"Saldo insuficiente para o usuário {userId}. Saldo: {carteira.Saldo:C}, Necessário: {precoFinal:C}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>(
                $"Saldo insuficiente. Você possui {carteira.Saldo:C} e precisa de {precoFinal:C}.", "400");
        }

        // Publicar evento de compra
        try
        {
            await walletBusPublisher.Publish(new PaymentCreatedEvent(
                JogoId: jogo.Id,
                PerfilId: perfilId,
                saldo: precoFinal
            ), CancellationToken.None);

            await logger.LogInformation(
                $"Compra iniciada: Usuário {userId} - Jogo {jogo.Nome} - Preço {precoFinal:C}");

            return Result.Success(new JogosDto.JogosDtoResponseCompra {
                PerfilId = perfilId,
                JogoId = jogo.Id     
            });
        }
        catch (Exception ex)
        {
            await logger.LogError($"Erro ao publicar evento de compra: {ex.Message}");
            return Result.Failure<JogosDto.JogosDtoResponseCompra>(
                "Erro ao processar a compra. Tente novamente mais tarde.", "500");
        }
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