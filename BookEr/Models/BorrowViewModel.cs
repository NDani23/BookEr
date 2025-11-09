using System.ComponentModel.DataAnnotations;
using BookEr.Persistence;

namespace BookEr.Web.Models
{
    public class BorrowViewModel : VisitorViewModel
    {
        public Volume? Volume { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime BorrowStartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime BorrowEndDate { get; set; }
    }
}
