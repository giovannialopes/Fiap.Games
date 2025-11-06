using Games.Domain.DTO;
using Games.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Games.Domain.DTO.ErrorDto;

namespace Games.API.Controllers;

[Route("api/v1")]
[ApiController]
public class PromotionController(IPromocoesServices services, ILoggerServices logger) : ControllerBase
{
    [HttpPost]
    [Route("adicionar/promocoes")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> InserirPromocoes(
    [FromBody] PromocoesDto.PromocoesDtoRequest request) {
        await logger.LogInformation("Iniciou InserirPromocoes");
        var result = await services.InserePromocoes(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet]
    [Route("consultar/promocoes")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConsultarPromocoesAtivas() {
        await logger.LogInformation("Iniciou ConsultarPromocoesAtivas");
        var result = await services.ConsultaPromocoesAtivas();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete]
    [Route("deletar/promocoes")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletarPromocoes([FromHeader] long promocaoId) {
        await logger.LogInformation("Iniciou ConsultarPromocoes");
        var result = await services.DesativarPromocoes(promocaoId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
