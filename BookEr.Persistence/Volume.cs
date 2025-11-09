using System.ComponentModel.DataAnnotations;

namespace BookEr.Persistence
{
    public class Volume
    {
        public Volume()
        {
            Borrows = new HashSet<Borrow>();
            IsAvailable = true;
        }

        [Key]
        public Int32 LibraryId { get; set; }

        [Required]
        public virtual Book Book { get; set; } = null!;

        public Int32 BookId { get; set; }

        public Boolean IsAvailable { get; set; }

        public virtual ICollection<Borrow> Borrows { get; set; }
    }
}
