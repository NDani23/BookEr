namespace BookEr.DTO
{
    public class VolumeDto
    {
        public Int32 LibraryId { get; set; }

        public Int32 BookId { get; set; }

        public Boolean IsAvailable { get; set; }

        public String? CurrentUser { get; set; }

        public DateTime? CurrentDeadline { get; set; }

        public DateTime? NextBorrow { get; set; }
    }
}
