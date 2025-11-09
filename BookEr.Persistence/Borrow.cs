namespace BookEr.Persistence
{
    public class Borrow
    {

        public Int32 Id { get; set; }

        public virtual Volume Volume { get; set; } = null!;

        public Int32 VolumeId { get; set; }

        public virtual Visitor Visitor { get; set; } = null!;

        public Int32 VisitorId { get; set; }

        public DateTime StartDay { get; set; }

        public DateTime EndDay { get; set; } 

        public Boolean IsActive  { get; set; }
    }
}
