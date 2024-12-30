using FMIApi.Dtos;
using FMIApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMIApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MaintenanceController(AppDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseMaintenanceDTO>> Get(long id)
        {
            var result = await _context
                           .Maintenances
                           .Include(x => x.Car)
                           .Include(x => x.Garage)
                           .FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new ResponseMaintenanceDTO(result.Id,
                result.Car.Id,
                result.Car?.Make,
                result.ServiceType,
                result.ScheduledDate.ToString(),
                result.GarageId,
                result.Garage?.Name));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseMaintenanceDTO>> Update(long id, UpdateMaintenanceDTO dto)
        {
            var result = await _context
                            .Maintenances
                            .Include(x => x.Car)
                            .Include(x => x.Garage)
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            result.CarId = dto.CarId;
            result.ServiceType = dto.ServiceType ?? result.ServiceType;
            result.ScheduledDate = dto.ScheduledDate != null ? DateTime.Parse(dto.ScheduledDate) : result.ScheduledDate;
            result.GarageId = dto.GarageId;

            await _context.SaveChangesAsync();

            return await Get(id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var maintenanceToDelete = await _context.Maintenances.FirstOrDefaultAsync(x => x.Id == id);

            if (maintenanceToDelete == null)
            {
                return NotFound();
            }

            _context.Maintenances.Remove(maintenanceToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ResponseMaintenanceDTO>> Get(long? carId, long? garageId, DateTime? startDate, DateTime? endDate)
        {
            var result = await _context
                            .Maintenances
                            .Include(x => x.Garage)
                            .Include(x => x.Car)
                            .Where(x =>
                                (string.IsNullOrEmpty(startDate.ToString()) || x.ScheduledDate >= startDate) &&
                                (string.IsNullOrEmpty(endDate.ToString()) || x.ScheduledDate <= endDate) &&
                                (!carId.HasValue || x.CarId == carId) &&
                                (!garageId.HasValue || x.GarageId == garageId)
                            )
                            .ToListAsync();

            if (result == null)
            {
                return Ok(Enumerable.Empty<ResponseMaintenanceDTO>());
            }

            return Ok(result.Select(x => new ResponseMaintenanceDTO(
                x.Id,
                x.CarId,
                x.Car?.Model,
                x.ServiceType,
                x.ScheduledDate.ToString(),
                x.GarageId,
                x.Garage?.Name)));
        }

        [HttpPost]
        public async Task<ActionResult<ResponseMaintenanceDTO>> Create(CreateMaintenanceDTO dto)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == dto.CarId);
            var garage = await _context.Garages.FirstOrDefaultAsync(x => x.Id == dto.GarageId);

            var maintenanceToCreate = new Maintenance()
            {
                CarId = dto.CarId,
                Car = car,
                ScheduledDate = DateTime.Parse(dto.ScheduledDate),
                GarageId = dto.GarageId,
                ServiceType = dto.ServiceType,
            };

            var createdEntity = (await _context.Maintenances.AddAsync(maintenanceToCreate)).Entity;
            car.Garages.Add(garage);

            await _context.SaveChangesAsync();

            return await Get(createdEntity.Id);
        }

        [HttpGet("monthlyRequestsReport")]
        public async Task<ActionResult<ResponseMaintenanceDTO>> GetReport(long garageId, DateTime? startMonth, DateTime? endMonth)
        {
            var maintenanceRequests = await _context.Maintenances.Where(request => request.GarageId == garageId &&
                       request.ScheduledDate >= startMonth &&
                       request.ScheduledDate <= endMonth)
                .ToListAsync();

            var monthChunks = new List<MonthChunk>();

            var current = startMonth != null ? new(startMonth.Value.Year, startMonth.Value.Month, 1) : DateTime.Now;

            while (current <= endMonth)
            {
                monthChunks.Add(new MonthChunk
                {
                    Start = new DateTime(current.Year, current.Month, 1),
                    End = new DateTime(current.Year, current.Month, 1).AddMonths(1).AddDays(-1),
                    Month = current.ToString("MMMM").ToUpper(),
                    Year = current.Year,
                    MonthValue = current.Month - 1,
                    LeapYear = DateTime.IsLeapYear(current.Year)
                });

                current = current.AddMonths(1);
            }

            List<MonthlyRequestsReportDTO> results = [];

            foreach (MonthChunk chunk in monthChunks)
            {
                long chunkStartDateTimestamp = chunk.Start.Ticks;
                long chunkEndDateTimestamp = chunk.End.Ticks;

                IEnumerable<Maintenance> requests = maintenanceRequests
                    .Where(request => request.ScheduledDate.Ticks >= chunkStartDateTimestamp && request.ScheduledDate.Ticks <= chunkEndDateTimestamp)
                    .ToList();

                results.Add(new MonthlyRequestsReportDTO
                {
                    YearMonth = $"{chunk.Year}-{chunk.Month}",
                    Requests = requests.Count()
                });
            }

            return Ok(results);
        }

        private readonly AppDbContext _context = context;

        private class MonthChunk
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string? Month { get; set; }
            public int Year { get; set; }
            public int MonthValue { get; set; }
            public bool LeapYear { get; set; }
        }
    }
}
