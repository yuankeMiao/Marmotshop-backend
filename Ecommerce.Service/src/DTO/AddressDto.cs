
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class AddressReadDto: BaseEntity
    {
        public required string Recipient { get; set; }
        public required string Phone { get; set; }
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required Guid UserId { get; set; }
    }

    public class AddressCreateDto
    {
        public required string Recipient { get; set; }
        public required string Phone { get; set; }
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? PostalCode { get; set; }
        public required string City { get; set; }
    }

    public class AddressUpdateDto
    {
        public Guid Id { get; set; }
        public string? Recipient { get; set; }
        public string? Phone { get; set; }
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
    }
}