using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace BarRating.Data.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Reviews = [];
        }

        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
