using System.ComponentModel.DataAnnotations.Schema;

namespace BarRating.Data.Entities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
