using BarRating.Data;
using BarRating.Data.Entities;
using BarRating.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarRating.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,UserName,Id")] User user, UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userManager.CreateAsync(user, model.Password);

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string? id, UserViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            ModelState.Clear();

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.UserName = user.UserName;

            return View(model);
        }

        // POST: Users/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FirstName,LastName,UserName,Id")] User user1, UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Find the existing user by id
                    var user = await _userManager.FindByIdAsync(id);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Update the user properties
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);

                    // Update the user
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        // Handle update errors
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    ModelState.AddModelError("", "An error occurred while updating the user.");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Users/Delete/id
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
