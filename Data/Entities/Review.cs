using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BarRating.Data.Entities
{
    public class Review : BaseEntity
    {
        [Range(0, 5)]
        public int Rating { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
        [DisplayName("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Bar")]
        public int BarId { get; set; }
        public virtual Bar Bar { get; set; }
    }
}
