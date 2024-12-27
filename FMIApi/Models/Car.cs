using System.ComponentModel.DataAnnotations;

namespace FMIApi.Models
{
    public class Car
    {
        public Car()
        {
            Garages = new HashSet<Garage>();
        }

        [Key]
        public long Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int ProductionYear { get; set; }

        public string LicensePlate { get; set; }

        public virtual ICollection<Garage> Garages { get; set; }
    }
}
