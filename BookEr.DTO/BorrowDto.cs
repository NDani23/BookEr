namespace BookEr.DTO
{
    public class BorrowDto
    {
        public Int32 Id { get; set; }

        public Int32 VolumeId { get; set; }

        public Int32 VisitorId { get; set; }

        public DateTime StartDay { get; set; }

        public DateTime EndDay { get; set; }

        public Boolean IsActive { get; set; }

        public String UserName { get; set; } = null!;
    }
}
