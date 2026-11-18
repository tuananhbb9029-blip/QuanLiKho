namespace QUANLIKHO.DTO
{
    public class PurchasehistoryDtos
    {
        public class PurchasehistoryCreateDto
        {
            public int Customerid { get; set; }
            public int Orderid { get; set; }
            public int Productid { get; set; }
            public int Quantity { get; set; }
            public decimal Totalprice { get; set; }
        }
        public class PurchasehistoryUpdateDto
        {
            public int Customerid { get; set; }
            public int Orderid { get; set; }
            public int Productid { get; set; }
            public int Quantity { get; set; }
            public decimal Totalprice { get; set; }
        }
        public class ProductResponseDto {
            public int PurchasehistoryId { get; set; }
            public int Customerid { get; set; }
            public string Customername { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public int Orderid { get; set; }
            public int Productid { get; set; }
            public string Producname { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal Totalprice { get; set; }
            public DateTime Purchasedate { get; set; }

        }

    }
}
