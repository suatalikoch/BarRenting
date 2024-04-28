using BarRating.Data;
using BarRating.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarRating.Controllers
{
    [Authorize]
    public class MyReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MyReviews
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reviews.Include(r => r.Bar).Include(r => r.User).Where(r => r.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MyReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Bar)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: MyReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }
            ViewData["BarId"] = new SelectList(_context.Bars, "Id", "Name", review.BarId);

            return View(review);
        }

        // POST: MyReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rating,Description,UserId,BarId,Id")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Bar");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["BarId"] = new SelectList(_context.Bars, "Id", "Name", review.BarId);

            return View(review);
        }

        // GET: MyReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Bar)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: MyReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
