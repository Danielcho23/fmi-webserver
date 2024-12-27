using System.ComponentModel.DataAnnotations;

namespace FMIApi.Models
{
    public class Garage
    {
        public Garage()
        {
            Cars = new HashSet<Car>();
            Maintenances = new HashSet<Maintenance>();
        }

        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public int Capacity { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        public virtual ICollection<Maintenance> Maintenances { get; set; }

    }
}
