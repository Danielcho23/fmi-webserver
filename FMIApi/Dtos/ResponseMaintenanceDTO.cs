using System.ComponentModel.DataAnnotations;

namespace FMIApi.Dtos
{
    public class ResponseMaintenanceDTO
    {
        public ResponseMaintenanceDTO(long id,
        long carId,
        string carName,
        string serviceType,
        string scheduledDate,
        long garageId,
        string garageName)
        {
            this.Id = id;
            this.CarId = carId;
            this.CarName = carName;
            this.ServiceType = serviceType;
            this.ScheduledDate = scheduledDate;
            this.GarageId = garageId;
            this.GarageName = garageName;
        }

        public long Id { get; set; }

        public long CarId { get; set; }

        public string CarName { get; set; }

        public string ServiceType { get; set; }

        [DataType(DataType.Date)]
        public string ScheduledDate { get; set; } 

        public long GarageId { get; set; }

        public string GarageName { get; set; }
    }
}
