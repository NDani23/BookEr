using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookEr.Persistence
{
    public class Librarian
    {
        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }
        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
