namespace Ecommerce.Service.src.DTO
{
    public class OrderProductReadDto
    {
        public Guid ProductId { get; set; }
        public required string ProductTitle { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductDiscountPercentage { get; set; }
        public int ProductQuantity { get; set; }
    }

    public class OrderProductCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    // to simplify the logic, only update status for an order
    // public class OrderProductUpdateDto
    // {
    //     public int Quantity { get; set; }
    // }
}