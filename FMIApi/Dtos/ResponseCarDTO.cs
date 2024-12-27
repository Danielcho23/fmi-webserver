namespace FMIApi.Dtos
{
    public class ResponseCarDTO
    {
        public ResponseCarDTO(long id,
            string make,
            string model,
            int productionYear,
            string licensePlate,
            IEnumerable<ResponseGarageDTO> garages)
        {
            this.Id = id;
            this.Make = make;
            this.Model = model;
            this.ProductionYear = productionYear;
            this.LicensePlate = licensePlate;
            this.Garages = garages;
        }

        public long Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int ProductionYear { get; set; }

        public string LicensePlate { get; set; }

        public IEnumerable<ResponseGarageDTO> Garages { get; set; }
    }
}
