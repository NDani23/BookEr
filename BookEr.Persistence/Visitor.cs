using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookEr.Persistence
{
    public class Visitor
    {
        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public Visitor()
        {
            Borrows = new HashSet<Borrow>();
        }

        public String Address { get; set; } = null!;

        public virtual ICollection<Borrow> Borrows { get; set; }

    }
}
