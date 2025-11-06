using Games.Domain.DTO;
using Games.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Games.Domain.DTO.ErrorDto;

namespace Games.API.Controllers;


[Route("api/v1")]
[ApiController]
public class GamesController([FromServices] IJogosServices services, ILoggerServices logger) : ControllerBase
{

    [HttpPost]
    [Route("cadastrar/jogos")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CadastrarJogos(
        [FromBody] JogosDto.JogosDtoRequest request) {
        await logger.LogInformation("Iniciou CadastrarJogos");
        var result = await services.CadastrarJogos(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
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

    [HttpPost]
    [Route("comprar/jogos")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ComprarJogos(
        [FromBody] JogosDto.JogosDtoComprarJogos request) {
        await logger.LogInformation("Iniciou ComprarJogos");
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await services.ComprarJogos(request, userIdString!);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
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


    [HttpGet]
    [Route("consultar/todos/jogos")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConsultarJogos() {
        await logger.LogInformation("Iniciou ConsultarJogos");
        var result = await services.ConsultarJogos();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

}