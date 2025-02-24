using ProfileService.Models;

namespace ProfileService.DTO
{
    public class DtoProfile
    {
        public string name { get; set; }
        public string lastname { get; set; }
        public string birthdate { get; set; }
        public string phone { get; set; }
        public string? imageId { get; set; }
        public string? tag_id { get; set; }
        public Address? address { get; set; }
    }
}
