using Games.Domain.DTO;
using Games.Domain.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using static Games.Domain.DTO.ErrorDto;

namespace Games.API.Controllers;

[Route("api/v1")]
[ApiController]
public class LibraryController([FromServices] IBibliotecaServices services, ILoggerServices logger) : ControllerBase
{

    [HttpPost]
    [Route("adicionar/jogo/biblioteca")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdicionarJogoNaBiblioteca([FromBody] LibraryDto.LibraryDtoRequest request) {
        await logger.LogInformation("Iniciou AdicionarJogoNaBiblioteca");
        var result = await services.AdicionarJogoBiblioteca(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }


    [HttpGet]
    [Route("consultar/usuario/biblioteca")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConsultarBiblioteca([FromHeader] string perfilId) {
        await logger.LogInformation("Iniciou ConsultarBiblioteca");
        var result = await services.ConsultarBibliotecaPorUsuario(perfilId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
