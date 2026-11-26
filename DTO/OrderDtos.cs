namespace QUANLIKHO.DTO
{
    public class OrderDtos
    {
        public class OrderCreateDto
        {
            public int Customerid { get; set; }
            public List<OrderItemDto> Items { get; set; } = new();
        }
        public class OrderItemDto
        {
            public int Productid { get; set; }
            public int Quantity { get; set; }
        }
        public class OrderResponseDto
        {
            public int Orderid { get; set; }
            public int Customerid { get; set; }
            public string Fullname { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount {  get; set; }
            public List<OrderDetailResponseDto> Details { get; set; } = new();   //Details để xem đơn hàng   
        }
        public class OrderDetailResponseDto
        {
            public int Productid { get; set; }
            public string Productname { get; set; } = string.Empty; 
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }
    }
}
