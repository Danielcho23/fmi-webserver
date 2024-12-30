using FMIApi.Dtos;
using FMIApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMIApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController(AppDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseCarDTO>> Get(long id)
        {
            var result = await _context.Cars.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            var garages = result.Garages.Select(x => new ResponseGarageDTO(x.Id, x.Name, x.Location, x.City, x.Capacity));

            return Ok(new ResponseCarDTO(result.Id, result.Make, result.Model, result.ProductionYear, result.LicensePlate, garages));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseCarDTO>> Update(long id, UpdateCarDTO dto)
        {
            var result = await _context.Cars.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            result.Make = dto.Make ?? result.Make;
            result.Model = dto.Model ?? result.Model;
            result.ProductionYear = dto.ProductionYear;
            result.LicensePlate = dto.LicensePlate ?? result.LicensePlate;

            await _context.SaveChangesAsync();

            var garagesDtos =
                result.Garages.Select(x => new ResponseGarageDTO(x.Id, x.Name, x.Location, x.City, x.Capacity));

            return Ok(new ResponseCarDTO(result.Id,
                result.Make,
                result.Model,
                result.ProductionYear,
                result.LicensePlate,
                garagesDtos));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var carToDelete = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);

            if (carToDelete == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(carToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseCarDTO>>> Get(string? carMake, long? garageId, int? startYear, int? endYear)
        {
            var cars = await _context.Cars
                .Include(x => x.Garages)
                .Where(x =>
                    (carMake == null || x.Make.Contains(carMake)) &&
                    (!garageId.HasValue || x.Garages.Any(g => g.Id == garageId.Value)) &&
                    (!startYear.HasValue || x.ProductionYear >= startYear) &&
                    (!endYear.HasValue || x.ProductionYear <= endYear))
                .Select(x => new ResponseCarDTO(
                    x.Id,
                    x.Make,
                    x.Model,
                    x.ProductionYear,
                    x.LicensePlate,
                    x.Garages.Select(y => new ResponseGarageDTO(y.Id, y.Name, y.Location, y.City, y.Capacity))))
                .ToListAsync();

            return Ok(cars);
        }


        [HttpPost]
        public async Task<ActionResult<ResponseCarDTO>> Create(CreateCarDTO dto)
        {
            var carToCreate = new Car()
            {
                Make = dto.Make,
                Model = dto.Model,
                ProductionYear = dto.ProductionYear,
                LicensePlate = dto.LicensePlate
            };

            var entity = (await _context.Cars.AddAsync(carToCreate)).Entity;
            await _context.SaveChangesAsync();

            IEnumerable<ResponseGarageDTO> garagesDtos =
                entity.Garages.Select(x => new ResponseGarageDTO(x.Id, x.Name, x.Location, x.City, x.Capacity));

            return Ok(new ResponseCarDTO(entity.Id, entity.Make, entity.Model, entity.ProductionYear, entity.LicensePlate, garagesDtos));
        }

        private readonly AppDbContext _context = context;
    }
}
