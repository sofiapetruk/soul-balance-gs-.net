using Microsoft.AspNetCore.Mvc;
using soulBalanceGs.Service;
using soulBalanceGs.DTOs;
using soulBalanceGs.Hateos;

namespace soulBalanceGs.Controllers
{
    [ApiController]
    [Route("api/v1/checkin-manual")]
    public class CheckinManualController : ControllerBase
    {
        private readonly CheckinManualService _service;
        private readonly LinkGenerator _linkGenerator;

        public CheckinManualController(CheckinManualService service, LinkGenerator linkGenerator)
        {
            _service = service;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckinManualResponseDto>>> ReadAll(
                 [FromQuery] int page = 1,
                 [FromQuery] int pageSize = 10)
        {
            var lista = await _service.GetAllPagination(page, pageSize);

            foreach (var item in lista)
            {
                AddLinks(item);
            }

            return Ok(lista);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<CheckinManualResponseDto>> GetById(int id)
        {
            var dto = await _service.GetById(id);

            if (dto == null)
                return NotFound();

            AddLinks(dto);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<CheckinManualResponseDto>> Create([FromBody] CheckinManualRequestDto request)
        {
            var result = await _service.Save(request);

            AddLinks(result);

            return CreatedAtAction(nameof(GetById), new { id = result.ChekinId }, result);
        }

        [HttpDelete("users/{userId:long}/chekins/{chekinId:long}")]
        public async Task<ActionResult> DeleteChekins(int userId, int chekinId)
        {
            await _service.DeleteUserChekin(userId, chekinId);
            return NoContent();
        }

        private void AddLinks(CheckinManualResponseDto checkin)
        {
            checkin.Links.Add(new Link(
                _linkGenerator.GetUriByAction(
                    HttpContext,
                    nameof(GetById),
                    "CheckinManual",
                    new { id = checkin.ChekinId }
                ),
                "self",
                "GET"
            ));

            checkin.Links.Add(new Link(
                _linkGenerator.GetUriByAction(
                    HttpContext,
                    nameof(DeleteChekins),
                    "CheckinManual",
                    new { userId = checkin.Usuario, chekinId = checkin.ChekinId }
                ),
                "delete",
                "DELETE"
            ));

            checkin.Links.Add(new Link(
                _linkGenerator.GetUriByAction(
                    HttpContext,
                    nameof(ReadAll),
                    "CheckinManual",
                    null
                ),
                "collection",
                "GET"
            ));
        }
    }
}
