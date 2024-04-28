using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarRating.Data.Entities
{
    public class Bar : BaseEntity
    {
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        [DisplayName("Image Location")]
        public string ImageLocation { get; set; }
        //- [Add Max Image Size Here later]
        [DisplayName("Image")]
        //-[DataType(DataType.Upload)]
        [NotMapped]
        [MaxLength(2 * 1024 * 1024, ErrorMessage = "The file size should not exceed 2 MB.")]
        public IFormFile ImageFile { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
