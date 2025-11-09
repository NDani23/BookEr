namespace BookEr.DTO
{ 
    public class BookDto
    {
        public Int32 Id { get; set; }

        public String Title { get; set; } = null!;

        public String Author { get; set; } = null!;

        public Int32 Year { get; set; }

        public String ISBN { get; set; } = null!;

        public byte[]? Image { get; set; } = null!;
    }

}
