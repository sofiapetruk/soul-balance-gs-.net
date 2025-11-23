using Microsoft.AspNetCore.Mvc;
using soulBalanceGs.DTOs;
using soulBalanceGs.Exceptions;
using soulBalanceGs.Hateos;
using soulBalanceGs.Service;

namespace soulBalanceGs.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/atividade")]
    public class AtividadeController : Controller
    {
        private readonly IService<AtividadeResponseDto, AtividadeRequestDto> _service;
        private readonly AtividadeService _atividadeService;
        private readonly IService<UsuarioResponseDto, UsuarioRequestDto> _usuarioService;
        private readonly LinkGenerator _linkGenerator;

        public AtividadeController(
            IService<AtividadeResponseDto, AtividadeRequestDto> service,
            AtividadeService atividadeService,
            IService<UsuarioResponseDto, UsuarioRequestDto> usuarioService,
            LinkGenerator linkGenerator)
        {
            _service = service;
            _atividadeService = atividadeService;
            _usuarioService = usuarioService;
            _linkGenerator = linkGenerator;
        }

        [HttpPost]
        public async Task<ActionResult<AtividadeResponseDto>> SaveAtividade([FromBody] AtividadeRequestDto dto)
        {
            try
            {
                var atividade = await _service.Save(dto);
                AddLinks(atividade);
                return CreatedAtAction(nameof(GetById), new { id = atividade.AtividadeId }, atividade);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro interno ao processar a requisição." });
            }
        }



        [HttpGet("{id:int}")]
        public async Task<ActionResult<AtividadeResponseDto>> GetById([FromRoute] int id)
        {
            try
            {
                var atividade = await _service.GetById(id);
                AddLinks(atividade);
                return Ok(atividade);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtividadeResponseDto>>> ReadAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var atividadeList = (await _service.GetAll(page, pageSize)).ToList();
            atividadeList.ForEach(AddLinks);

            return Ok(atividadeList);
        }


        [HttpGet("historico/{userId:int}/atividades/{atividadeId:int}")]
        public async Task<ActionResult<AtividadeResponseDto>> BuscarHistoricoPorPeriodo(
            [FromRoute] int userId,
            [FromRoute] int atividadeId)
        {
            try
            {
                var atividade = await _atividadeService.BuscarHistoricoPorPeriodo(userId, atividadeId);
                AddLinks(atividade);
                return Ok(atividade);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro interno ao processar a requisição." });
            }
        }

   

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteById([FromRoute] int id)
        {
            try
            {
                await _service.DeleteById(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro interno ao processar a requisição." });
            }
        }


        private void AddLinks(AtividadeResponseDto atividade)
        {
            atividade.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Atividade", new { id = atividade.AtividadeId }),
                "self",
                "GET"
            ));
            atividade.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(DeleteById), "Atividade", new { id = atividade.AtividadeId }),
                "delete",
                "DELETE"
            ));
            atividade.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(ReadAll), "Atividade"),
                "collection",
                "GET"
            ));
        }
    }
}
