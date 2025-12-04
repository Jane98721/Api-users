using Microsoft.AspNetCore.Mvc;
using MyApiUsers.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyApiUsers.Controllers

{  
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]

  public class BooksController : ControllerBase
  {
    
    private static int nextId = 1;
    private static List <Book> books = new();
    
    [HttpGet]
    public ActionResult GetBooks()
    
    {
      return Ok(books);
    }
    
    [HttpGet("{id}")]
    
    public ActionResult <Book> GetBookById(int id)
    {
      var book = books.FirstOrDefault(x => x.Id == id);
      if(book == null)
      return NotFound();
      
      return Ok(book);
      }
      
    [HttpPost]
    public ActionResult<Book> AddBook(Book book)
    {

     book.Id = nextId++;
     books.Add(book);
     return Ok(book);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateBook(int id, Book updatedBook)
    {
      var book = books.FirstOrDefault(x => x.Id == id);
      if(book == null)
      return NotFound();
      
       book.Title = updatedBook.Title; 
       book.Author = updatedBook.Author;
       book.PublishedYear = updatedBook.PublishedYear;
       
       return Ok(book);
       
       }
       
    [HttpDelete("{id}")]
    public IActionResult DeleteBook(int id)
    {
      
    books = books.Where(b => b.Id !=id).ToList();
    return Ok();
    }
  }
}

