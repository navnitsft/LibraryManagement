using LibraryManagement.Models;
using LibraryManagement.ViewModels.BorrowViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BorrowController : Controller
    {
        private readonly LibraryContext _libraryContext;

        public BorrowController(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        [HttpGet]
        public IActionResult Create(int? bookId)
        {
            if (bookId == null || bookId == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided for borrowing.";
                return View("NotFound");
            }

            try
            {
                var book = _libraryContext.Books.FindAsync(bookId);

                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {bookId} to borrow.";
                    return View("NotFound");
                }

                var borrowViewModel = new BorrowViewModel
                {
                    BookId = book.Result.BookId,
                    BookTitle = book.Result.Title
                };

                return View(borrowViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the borrow form.";
                return View("Error");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var book = await _libraryContext.Books.FindAsync(model.BookId);

                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {model.BookId} to borrow.";
                    return View("NotFound");
                }
                if (!book.IsAvailable)
                {
                    TempData["ErrorMessage"] = $"The book '{book.Title}' is already borrowed.";
                    return View("NotAvailable");
                }

                var borrowRecord = new BorrowRecord
                {
                    BookId = book.BookId,
                    BorrowerName = model.BorrowerName,
                    BorrowerEmail = model.BorrowerEmail,
                    Phone = model.Phone,
                    BorrowDate = DateTime.UtcNow
                };

                book.IsAvailable = false;

                await _libraryContext.BorrowRecords.AddAsync(borrowRecord);
                await _libraryContext.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully borrowed the book: {book.Title}.";
                return RedirectToAction("Index", "Books");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the borrowing action.";
                return View("Error");
            }

            
        }

        [HttpGet]
        public async Task<IActionResult> Return(int? borrowRecordId)
        {
            if (borrowRecordId == null || borrowRecordId == 0)
            {
                TempData["ErrorMessage"] = "Borrow Record ID was not provided for returning.";
                return View("NotFound");
            }

            try
            {
                var borrowRecord = await _libraryContext.BorrowRecords
                    .Include(br => br.Book)
                    .FirstOrDefaultAsync(br => br.BorrowRecordId == borrowRecordId);
                if (borrowRecord == null)
                {
                    TempData["ErrorMessage"] = $"No borrow record found with ID {borrowRecordId} to return.";
                    return View("NotFound");
                }
                if (borrowRecord.ReturnDate != null)
                {
                    TempData["ErrorMessage"] = $"The borrow record for '{borrowRecord.Book.Title}' has already been returned.";
                    return View("AlreadyReturned");
                }

                var returnViewModel = new ReturnViewModel
                {
                    BorrowRecordId = borrowRecord.BorrowRecordId,
                    BookTitle = borrowRecord.Book.Title,
                    BorrowerName = borrowRecord.BorrowerName,
                    BorrowDate = borrowRecord.BorrowDate
                };

                return View(returnViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the return confirmation.";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(ReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var borrowRecord = await _libraryContext.BorrowRecords
                    .Include(br => br.Book)
                    .FirstOrDefaultAsync(br => br.BorrowRecordId == model.BorrowRecordId);
                if (borrowRecord == null)
                {
                    TempData["ErrorMessage"] = $"No borrow record found with ID {model.BorrowRecordId} to return.";
                    return View("NotFound");
                }
                if (borrowRecord.ReturnDate != null)
                {
                    TempData["ErrorMessage"] = $"The borrow record for '{borrowRecord.Book.Title}' has already been returned.";
                    return View("AlreadyReturned");
                }

                // Update the borrow record
                borrowRecord.ReturnDate = DateTime.UtcNow;
                // Update the book's availability
                borrowRecord.Book.IsAvailable = true;

                await _libraryContext.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully returned the book: {borrowRecord.Book.Title}.";

                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the return action.";
                return View("Error");
            }
        }

    }
}
