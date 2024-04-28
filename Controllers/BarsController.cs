using BarRating.Data;
using BarRating.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BarRating.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BarsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Bars
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bars.ToListAsync());
        }

        // GET: Bars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bar = await _context.Bars
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bar == null)
            {
                return NotFound();
            }

            return View(bar);
        }

        // GET: Bars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImageLocation,ImageFile,Id")] Bar bar, IFormFile imageFile)
        {
            ModelState.Remove("ImageLocation");
            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Save the image file to the wwwroot/images directory
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    bar.ImageLocation = "/images/" + uniqueFileName; // Save the image location in the database
                }

                await _context.AddAsync(bar);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(bar);
        }

        // GET: Bars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bar = await _context.Bars.FindAsync(id);
            if (bar == null)
            {
                return NotFound();
            }
            return View(bar);
        }

        // POST: Bars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,ImageLocation,ImageFile,Id")] Bar bar, IFormFile imageFile)
        {
            if (id != bar.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Save the image file to the wwwroot/images directory
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Check if the file with the same content already exists
                        var existingImagePath = await FindExistingImageAsync(imageFile, uploadsFolder);

                        if (existingImagePath == null)
                        {
                            // If not, save the new image file
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            // Delete the existing image file if it exists
                            if (!string.IsNullOrEmpty(bar.ImageLocation))
                            {
                                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, bar.ImageLocation.TrimStart('/'));
                                if (System.IO.File.Exists(existingFilePath))
                                {
                                    System.IO.File.Delete(existingFilePath);
                                }
                            }

                            // Delete the existing image file if it exists
                            if (!string.IsNullOrEmpty(bar.ImageLocation))
                            {
                                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, bar.ImageLocation.TrimStart('/'));
                                if (System.IO.File.Exists(existingFilePath))
                                {
                                    System.IO.File.Delete(existingFilePath);
                                }
                            }

                            // Save the new image location in the database
                            bar.ImageLocation = "/images/" + uniqueFileName;
                        }
                        else
                        {
                            // Use the existing image location
                            bar.ImageLocation = existingImagePath;
                        }
                    }

                    _context.Update(bar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BarExists(bar.Id))
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

            return View(bar);
        }

        // GET: Bars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bar = await _context.Bars
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bar == null)
            {
                return NotFound();
            }

            return View(bar);
        }

        // POST: Bars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bar = await _context.Bars.FindAsync(id);

            if (bar == null)
            {
                return NotFound();
            }

            // Delete the bar image file from the wwwroot/images folder
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, bar.ImageLocation.TrimStart('/'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Bars.Remove(bar);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BarExists(int id)
        {
            return _context.Bars.Any(e => e.Id == id);
        }

        private static async Task<string> FindExistingImageAsync(IFormFile newImage, string directory)
        {
            // Calculate the hash of the new image file
            using (var ms = new MemoryStream())
            {
                await newImage.CopyToAsync(ms);
                ms.Position = 0;

                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(ms);
                    var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                    // Check if any file in the directory has the same hash
                    var files = Directory.GetFiles(directory);

                    foreach (var file in files)
                    {
                        using (var fileStream = new FileStream(file, FileMode.Open))
                        {
                            var existingHash = sha256.ComputeHash(fileStream);
                            var existingHashString = BitConverter.ToString(existingHash).Replace("-", "").ToLower();

                            if (existingHashString == hashString)
                            {
                                return "/images/" + Path.GetFileName(file);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
