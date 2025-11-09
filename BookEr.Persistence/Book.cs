using System.ComponentModel.DataAnnotations;

namespace BookEr.Persistence
{
    public class Book
    {
        public Book()
        {
            Volumes=new HashSet<Volume>();
        }

        [Key]
        public Int32 Id { get; set; }

        [MaxLength(100)]
        public String Title { get; set; } = null!;

        [MaxLength(50)]
        public String Author { get; set; } = null!;

        public Int32 Year { get; set; }

        public String ISBN { get; set; } = null!;

        public byte[]? Image { get; set; }

        public virtual ICollection<Volume> Volumes { get; set; }
    }

}
