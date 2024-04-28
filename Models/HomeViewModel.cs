using BarRating.Data.Entities;

namespace BarRating.Models
{
    public class HomeViewModel
    {
        public string Query { get; set; }
        public List<Bar> Bars { get; set; }
        public int BarsCount { get; set; }
        public int ReviewsCount { get; set; }
        public int UsersCount { get; set; }
    }
}
