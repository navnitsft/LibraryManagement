using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _libraryContext;

        public BooksController(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _libraryContext.Books.ToListAsync();
                return View(books);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching the book list.";
                return View("Error");
            }
            
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided.";
                return View("NotFound");
            }

            try
            {
                var book = await _libraryContext.Books.FirstOrDefaultAsync(b => b.BookId == id);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id}.";
                    return View("NotFound");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the book details.";
                return View("Error");
            }
            
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _libraryContext.Books.AddAsync(book);
                    await _libraryContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Successfully added the book: {book.Title}.";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the book.";
                    return View(book);
                }
            }
            
            return View(book);
        }
    }
}
