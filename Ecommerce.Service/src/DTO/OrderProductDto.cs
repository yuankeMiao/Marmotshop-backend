namespace Ecommerce.Service.src.DTO
{
    public class OrderProductReadDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public decimal ActualPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderProductCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}