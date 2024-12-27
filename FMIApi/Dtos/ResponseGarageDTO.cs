using System.ComponentModel.DataAnnotations;

namespace FMIApi.Dtos
{
    public class ResponseGarageDTO
    {
        public ResponseGarageDTO(long id, string name, string location, string city, int capacity)
        {
            this.Id = id;
            this.Name = name;
            this.Location = location;
            this.City = city;
            this.Capacity = capacity;
        }

        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public int Capacity { get; set; }
    }
}
