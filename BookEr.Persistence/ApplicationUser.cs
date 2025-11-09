using Microsoft.AspNetCore.Identity;

namespace BookEr.Persistence
{
    public class ApplicationUser : IdentityUser<int>
    {
        public String Name { get; set; } = null!;
    }
}
