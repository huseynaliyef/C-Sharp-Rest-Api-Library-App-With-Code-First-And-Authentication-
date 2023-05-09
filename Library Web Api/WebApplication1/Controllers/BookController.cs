using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Entities;
using WebApplication1.Models.DTO.BookDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryEntityDbContext _database;
        private readonly UserManager<IdentityUser> _userManager;
        public BookController(LibraryEntityDbContext myDatabase, UserManager<IdentityUser> userManager)
        {
            _database = myDatabase;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            List<Book> Books = await _database.Books.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, Books);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddBook(BookAddUIDTO book)
        {
            try
            {
                Book b = new Book
                {
                    BookName = book.BookName,
                    Genre = book.Genre,
                    PageCount = book.PageCount,
                    AuthorName = book.AuthorName,
                    Price = book.Price,
                };
                await _database.Books.AddAsync(b);
                await _database.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, b.BookName);
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> EditBook(BookEditUIDTO book)
        {
            try
            {
                var b = await _database.Books.FindAsync(book.Id);
                if (b != null)
                {
                    b.BookName = book.BookName;
                    b.Genre = book.Genre;
                    b.PageCount = book.PageCount;
                    b.AuthorName = book.AuthorName;
                    b.Price = book.Price;
                    await _database.SaveChangesAsync();
                    return Ok(b.BookName);
                }
                else
                {
                    throw new Exception("Invalid Operation");
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> BookDelete(BookDeleteUIDTO book)
        {
            try
            {
                var b = await _database.Books.FindAsync(book.Id);
                if(b != null)
                {
                    _database.Books.Remove(b);
                    await _database.SaveChangesAsync();
                    return Ok(b.BookName);
                }
                else
                {
                    throw new Exception("Not Found");
                }
            }catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public async Task<IActionResult> BookBuy(BookBuyUIDTO b)
        {
            var user = await _userManager.FindByEmailAsync(b.Email);
            var book = await _database.Books.Where(x=>x.BookName == b.BookName).FirstOrDefaultAsync();
            if(user != null)
            {
                if(book != null)
                {
                    MyBookList myBookList = new MyBookList();
                    myBookList.Email = b.Email;
                    myBookList.BookName = b.BookName;
                    await _database.MyBookLists.AddAsync(myBookList);
                    await _database.SaveChangesAsync();
                    return Ok(myBookList);
                }
                else
                {
                    return NotFound(b.BookName);
                }
                
            }
            else
            {
                return NotFound(b.Email);
            }
            
        }
    }
}
