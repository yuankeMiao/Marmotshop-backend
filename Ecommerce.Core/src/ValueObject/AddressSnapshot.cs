
namespace Ecommerce.Core.src.ValueObject
{
    // this snapshot is used to store immutable address info for an order
    // so the order info will not get changed when user changed address
    public class AddressSnapshot
    {
        public Guid Id { get; set; } // it will be the same with real address id
        public required string Recipient { get; set; }
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }

        public int PostalCode { get; set; }
        public required string City { get; set; }
    }
}