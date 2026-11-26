namespace QUANLIKHO.DTO
{
    public class CategoryDtos
    {
        public class CategoryCreateDto
        {
            public string Categoryname { get; set; } = string.Empty;
        }
        public class CategoryUpdateDto
        {
            public string Categoryname { get; set; } = string.Empty;
        }
        public class CategoryResponseDto
        {
            public int Categoryid { get; set; }
            public string Categoryname { get; set; } = string.Empty;
            public int TotalProducts { get; set; }
        }
    }
}
