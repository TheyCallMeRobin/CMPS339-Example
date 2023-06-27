using CMPS339.Models;
using CMPS339.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMPS339.Controllers
{
    [ApiController]
    [Route("/api/amusement-parks")]
    public class AmusementParkController : ControllerBase
    {
        private readonly IAmusementParkService _amusementParkService;

        public AmusementParkController(IAmusementParkService amusementParkService)
        {
            _amusementParkService = amusementParkService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            List<Parks> parks = await _amusementParkService.GetAllAsync();

            return Ok(parks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            Parks? park = await _amusementParkService.GetByIdAsync(id);
            
            if (park != null)
            {
                return Ok(park);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ParksCreateDto dto)
        {
            if (ModelState.IsValid)
            {
             
                ParksGetDto? park = await _amusementParkService.InsertAsync(dto);

                if (park != null)
                {
                    return Ok(park);
                }

                return BadRequest("Unable to insert record.");
            }
            return BadRequest("The model is invalid.");
        }
    }
}
