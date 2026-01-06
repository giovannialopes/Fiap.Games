using Games.Domain.DTO;
using Games.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Games.Domain.DTO.ErrorDto;

namespace Games.API.Controllers;


[Route("api/v1")]
[ApiController]
public class GamesController(
    [FromServices] IJogosServices services, 
    ILoggerServices logger,
    [FromServices] IMetricsService metricsService) : ControllerBase
{

    /// <summary>
    /// Cadastra um novo jogo no sistema
    /// </summary>
    /// <param name="request">Dados do jogo a ser cadastrado</param>
    /// <returns>Dados do jogo cadastrado</returns>
    /// <response code="200">Jogo cadastrado com sucesso</response>
    /// <response code="400">Erro na validação ou jogo já existe</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="403">Acesso negado - requer permissão de Administrador</response>
    [HttpPost]
    [Route("cadastrar/jogos")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(JogosDto.JogosDtoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CadastrarJogos(
        [FromBody] JogosDto.JogosDtoRequest request) {
        
        if (request == null)
            return BadRequest(new ErrorResponse { Message = "Request não pode ser nulo", Code = "400" });
        
        await logger.LogInformation("Iniciou CadastrarJogos");
        var result = await services.CadastrarJogos(request);
        
        if (result.IsSuccess)
        {
            metricsService.IncrementGamesCreated();
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }

    [HttpPut]
    [Route("alterar/jogos")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AlterarJogos(
    [FromBody] JogosDto.JogosDtoRequest request,
    [FromQuery] string Nome) {
        await logger.LogInformation("Iniciou AlterarJogos");
        var result = await services.AlterarJogos(Nome, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete]
    [Route("deletar/jogos")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletarJogos(
    [FromHeader] string NomeDoJogo) {
        await logger.LogInformation("Iniciou DeletarJogos");
        var result = await services.DeletarJogos(NomeDoJogo);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Compra um jogo para o usuário autenticado
    /// </summary>
    /// <param name="request">Dados do jogo a ser comprado</param>
    /// <returns>Informações da compra iniciada</returns>
    /// <response code="200">Compra iniciada com sucesso</response>
    /// <response code="400">Erro na validação, jogo não encontrado, saldo insuficiente ou jogo já possuído</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost]
    [Route("comprar/jogos")]
    [Authorize]
    [ProducesResponseType(typeof(JogosDto.JogosDtoResponseCompra), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ComprarJogos(
        [FromBody] JogosDto.JogosDtoComprarJogos request) {
        
        if (request == null || string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new ErrorResponse { Message = "Nome do jogo é obrigatório", Code = "400" });
        
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdString))
            return Unauthorized(new ErrorResponse { Message = "Usuário não autenticado", Code = "401" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await logger.LogInformation("Iniciou ComprarJogos");
        
        var result = await services.ComprarJogos(request, userIdString);
        
        stopwatch.Stop();
        if (result.IsSuccess)
        {
            metricsService.IncrementGamesPurchased();
            metricsService.RecordPurchaseDuration(stopwatch.Elapsed.TotalSeconds);
            return Ok(result.Value);
        }
        
        return BadRequest(result.Error);
    }

    [HttpGet]
    [Route("consultar/unico/jogo")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConsultarUnicoJogo(
        [FromHeader] string NomeDoJogo) {
        await logger.LogInformation("Iniciou ConsultarUnicoJogo");
        var result = await services.ConsultarUnicoJogo(NomeDoJogo);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }


    /// <summary>
    /// Consulta todos os jogos disponíveis
    /// </summary>
    /// <returns>Lista de jogos disponíveis</returns>
    /// <response code="200">Lista de jogos retornada com sucesso</response>
    /// <response code="400">Nenhum jogo encontrado</response>
    [HttpGet]
    [Route("consultar/todos/jogos")]
    [ProducesResponseType(typeof(List<JogosDto.JogosDtoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarJogos() {
        await logger.LogInformation("Iniciou ConsultarJogos");
        metricsService.IncrementGamesQueried();
        var result = await services.ConsultarJogos();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

}