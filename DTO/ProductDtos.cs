namespace QUANLIKHO.DTO
{
    public class ProductDtos
    {
        public class ProductCreateDto
        {
            public string ProductName { get; set; } = "";
            public decimal Price { get; set; }
            public int Categoryid { get; set; }
            public int InitialStock { get; set; } = 0;
        }
        public class ProductUpdateDto
        {
            public string ProductName { get; set; } = "";
            public decimal Price { get; set; }
            public int Categoryid { get; set; }
        }
        public class ProductResponseDto
        {
            public int Productid { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Categoryname { get; set; } = string.Empty;
            public int Quantity { get; set; }

        }
    }
}

