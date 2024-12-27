using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMIApi.Models
{
    public class Maintenance
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Car))]
        public long CarId { get; set; }

        public Car? Car { get; set; }

        public string ServiceType { get; set; }

        public DateTime ScheduledDate { get; set; }

        [ForeignKey(nameof(Garage))]
        public long GarageId { get; set; }

        public Garage? Garage { get; set; }
    }
}
