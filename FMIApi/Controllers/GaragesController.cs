using FMIApi.Dtos;
using FMIApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMIApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GaragesController : ControllerBase
    {
        public GaragesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseGarageDTO>> Get(long id)
        {
            var result = await _context.Garages.Include(x => x.Cars).FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new ResponseGarageDTO(result.Id, result.Name, result.Location, result.City, result.Capacity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseGarageDTO>> Update(long id, UpdateGarageDTO dto)
        {
            var result = await _context.Garages.Include(x => x.Cars).FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            result.Name = dto.Name;
            result.Location = dto.Location;
            result.Capacity = dto.Capacity;
            result.City = dto.City;

            await _context.SaveChangesAsync();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var garageToDelete = await _context.Garages.FirstOrDefaultAsync(x => x.Id == id);

            if (garageToDelete == null)
            {
                return NotFound();
            }

            _context.Garages.Remove(garageToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseGarageDTO>> Search([FromQuery] string? city)
        {
            var result = await _context
                          .Garages
                          .Where(x => string.IsNullOrWhiteSpace(city) || x.City.Contains(city))
                          .ToListAsync();

            if (result == null)
            {
                return Ok(Enumerable.Empty<ResponseGarageDTO>());
            }

            return Ok(result.Select(x => new ResponseGarageDTO(x.Id, x.Name, x.Location, x.City, x.Capacity)));
        }

        [HttpPost]
        public async Task<ActionResult<ResponseGarageDTO>> Create(CreateGarageDTO dto)
        {
            Garage garageToCreate = new()
            {
                Name = dto.Name,
                City = dto.City,
                Location = dto.Location,
                Capacity = dto.Capacity
            };

            Garage createdEntity = (await _context.Garages.AddAsync(garageToCreate)).Entity;
            await _context.SaveChangesAsync();

            return Ok(new ResponseGarageDTO(createdEntity.Id,
                createdEntity.Name,
                createdEntity.Location,
                createdEntity.City,
                createdEntity.Capacity));
        }

        [HttpGet("dailyAvailabilityReport")]
        public async Task<ActionResult<GarageDailyAvailabilityReportDTO>> Get(
        [FromQuery] long garageId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
        {
            var maintenances = await _context.Maintenances.Where(request => request.GarageId == garageId &&
                       request.ScheduledDate >= startDate &&
                       request.ScheduledDate <= endDate)
                .ToListAsync();

            List<DayChunk> dayChunks = [];

            DateTime current = startDate ?? DateTime.Now;

            while (current <= endDate)
            {
                dayChunks.Add(new DayChunk
                {
                    Start = current.Date,
                    End = current.Date.AddDays(1).AddMilliseconds(-1)
                });

                current = current.AddDays(1);
            }

            List<GarageDailyAvailabilityReportDTO> results = [];

            var result = await _context.Garages.Include(x => x.Cars).FirstOrDefaultAsync(x => x.Id == garageId);

            if (result == null)
            {
                return NotFound();
            }

            var garage = new ResponseGarageDTO(result.Id, result.Name, result.Location, result.City, result.Capacity);

            foreach (DayChunk chunk in dayChunks)
            {
                long chunkStartDateTimestamp = chunk.Start.Ticks;
                long chunkEndDateTimestamp = chunk.End.Ticks;

                IEnumerable<Maintenance> requests = maintenances.Where(request =>
                {
                    long requestDateTimestamp = request.ScheduledDate.Ticks;
                    return requestDateTimestamp >= chunkStartDateTimestamp && requestDateTimestamp <= chunkEndDateTimestamp;
                });

                results.Add(new GarageDailyAvailabilityReportDTO(chunk.Start.ToString("yyyy-MM-dd"), requests.Count(), garage.Capacity - requests.Count()));
            }

            return Ok(results);
        }
        private class DayChunk
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private readonly AppDbContext _context;
    }
}
