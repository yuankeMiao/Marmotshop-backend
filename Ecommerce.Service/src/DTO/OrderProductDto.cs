namespace Ecommerce.Service.src.DTO
{
    public class OrderProductReadDto
    {
        public required string ProductTitle { get; set; }
        public required string ProductThumbnail { get; set; }
        public decimal ProductActualPrice { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductTotalPrice { get; set; }
    }

    public class OrderProductCreateDto
    {
        public Guid ProductId { get; set; }
        public int ProductQuantity { get; set; }
    }
}