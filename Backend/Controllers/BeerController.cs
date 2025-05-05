using Backend.DTOs;
using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private ICommonService<BeerDto, BeerInsertDto, BeerUpdateDto> _beerService;

        private IValidator<BeerInsertDto> _beerInsertValidator;
        private IValidator<BeerUpdateDto> _beerUpdateValidator;
        public BeerController(
            [FromKeyedServices("beerService")]ICommonService<BeerDto, BeerInsertDto, BeerUpdateDto> beerService,
            IValidator<BeerInsertDto> beerInsertValidator,
            IValidator<BeerUpdateDto> beerUpdateValidator)
        {
            _beerService = beerService;
            _beerInsertValidator = beerInsertValidator;
            _beerUpdateValidator = beerUpdateValidator;
        }

        [HttpGet]
        public async Task<IEnumerable<BeerDto>> Get() =>
            await _beerService.Get();

        [HttpGet("{id}")]
        public async Task<ActionResult<BeerDto>> GetById(int id)
        {
            var beerDto = await _beerService.GetById(id);
            return beerDto == null ? NotFound() : Ok(beerDto);
        }

        [HttpPost]
        public async Task<ActionResult<BeerDto>> Add(BeerInsertDto beerInsertDto) 
        {
            var validationResult = await _beerInsertValidator.ValidateAsync(beerInsertDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!_beerService.Validate(beerInsertDto))
            {
                return BadRequest(_beerService.Errors);
            }

            var beerDto = await _beerService.Add(beerInsertDto);
            return CreatedAtAction(nameof(GetById), new { id = beerDto.Id}, beerDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BeerDto>> Update(int id, BeerUpdateDto beerUpdateDto)
        {
            var validationResult = await _beerUpdateValidator.ValidateAsync(beerUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!_beerService.Validate(beerUpdateDto))
            {
                return BadRequest(_beerService.Errors);
            }

            var beerDto = await _beerService.Update(id, beerUpdateDto);
            return beerDto == null ? NotFound() : Ok(beerDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var beerDto = await _beerService.Delete(id);
            return beerDto == null ? NotFound() : Ok(beerDto); 
        }
    }
}