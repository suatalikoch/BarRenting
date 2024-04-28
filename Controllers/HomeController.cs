using BarRating.Data;
using BarRating.Data.Entities;
using BarRating.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BarRating.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Pass the search results to the view
            var model = new HomeViewModel
            {
                BarsCount = _context.Bars.Count(),
                ReviewsCount = _context.Reviews.Count(),
                UsersCount = _context.Users.Count(),
            };

            return View("Index", model);
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ModelState.AddModelError(string.Empty, "Search Query must not be null!");

                return View("Index");
            }

            ViewBag.SearchButtonClicked = Request.Query["searchButton"] == "true";

            // Search for bars based on the query
            var bars = _context.Bars.Where(a => a.Name.Contains(query)).ToList();

            // Pass the search results to the view
            var model = new HomeViewModel
            {
                BarsCount = _context.Bars.Count(),
                ReviewsCount = _context.Reviews.Count(),
                UsersCount = _context.Users.Count(),
                Bars = bars,
                Query = query
            };

            return View("Index", model);
        }

        public async Task<IActionResult> Review(int barId)
        {
            var bar = await _context.Bars.FindAsync(barId);

            if (bar == null)
            {
                return NotFound();
            }

            ReviewViewModel model = new()
            {
                BarId = barId,
                BarName = bar.Name,
                ReviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            return View("ReviewBar", model);
        }

        public async Task<IActionResult> ReviewConfirm(int barId, string reviewerId, int rating, string description)
        {
            Review review = new()
            {
                Rating = rating,
                Description = description,
                UserId = reviewerId,
                BarId = barId
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
