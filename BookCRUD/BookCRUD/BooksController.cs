using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Serialization;

namespace BookCRUD
{
    public class BooksController : ApiController
    {
        private static BookList books = new BookList();

        // GET api/books 
        public IEnumerable<Book> Get()
        {
            return books.getBooks();
        }

        // GET api/books/5 
        public Book Get(int isbn)
        {
            return books.getBook(isbn);
        }

        // POST api/books 
        public void Post([FromBody]Book book)
        {
            books.add(book);
        }

        // PUT api/books/5 
        public void Put(long isbn, [FromBody]Book book)
        {
            books.update(isbn, book);
        }

        // DELETE api/books/5 
        public void Delete(int isbn)
        {
            books.delete(isbn);
        }
    }
}
