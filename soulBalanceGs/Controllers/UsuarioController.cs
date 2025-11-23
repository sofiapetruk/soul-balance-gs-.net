using Microsoft.AspNetCore.Mvc;
using soulBalanceGs.DTOs;
using soulBalanceGs.Exceptions;
using soulBalanceGs.Hateos;
using soulBalanceGs.Service;

namespace soulBalanceGs.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsuarioController : Controller
    {
        private readonly IService<UsuarioResponseDto, UsuarioRequestDto> _service;
        private readonly LinkGenerator _linkGenerator;

        public UsuarioController(
            IService<UsuarioResponseDto, UsuarioRequestDto> service,
            LinkGenerator linkGenerator)
        {
            _service = service;
            _linkGenerator = linkGenerator;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> Create([FromBody] UsuarioRequestDto dto)
        {
            try
            {
                var user = await _service.Save(dto);
                AddLinks(user);
                return CreatedAtAction(nameof(ReadById), new { id = user.IdUsuario }, user);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocorreu um erro interno inesperado ao processar a requisição." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> ReadAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var users = (await _service.GetAll(page, pageSize)).ToList();

            foreach (var u in users)
            {
                AddLinks(u);
            }

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioResponseDto>> ReadById(int id)
        {
            var user = await _service.GetById(id);
            AddLinks(user);
            return Ok(user);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UsuarioResponseDto>> Update(int id, [FromBody] UsuarioRequestDto dto)
        {
            try
            {
                var user = await _service.Update(id, dto);

                AddLinks(user);

                return Ok(user);
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
                    new { message = "Ocorreu um erro interno inesperado ao processar a requisição." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
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
                    new { message = "Ocorreu um erro interno inesperado ao processar a requisição." });
            }
        }

        private void AddLinks(UsuarioResponseDto user)
        {
            user.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(ReadById), "Usuario", new { id = user.IdUsuario }),
                "self",
                "GET"
            ));

            user.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(Update), "Usuario", new { id = user.IdUsuario }),
                "update",
                "PUT"
            ));

            user.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(Delete), "Usuario", new { id = user.IdUsuario }),
                "delete",
                "DELETE"
            ));

            user.Links.Add(new Link(
                _linkGenerator.GetUriByAction(HttpContext, nameof(ReadAll), "Usuario"),
                "collection",
                "GET"
            ));
        }
    }
}
