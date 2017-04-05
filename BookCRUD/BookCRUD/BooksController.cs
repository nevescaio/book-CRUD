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
        private string fileName = "books.xml"; 



        // le arquivo com codificacao EBCDIC (code page 500) e retorna lista de livros
        private List<Book> readXML()
        {       
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            { 
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));
                TextReader reader = new StreamReader(fs, Encoding.GetEncoding(500));
                List<Book> books = (List<Book>)serializer.Deserialize(reader);
                return books;
            }
    
        }

        //escreve uma lista de livros no arquivo com codificacao EBCDIC
        private void writeXML(List<Book> books)
        {  
            FileStream fs = new FileStream(fileName, FileMode.Create);
            XmlSerializer ser = new XmlSerializer(typeof(List<Book>));
            TextWriter writer = new StreamWriter(fs, Encoding.GetEncoding(500));
            ser.Serialize(writer, books);
            fs.Dispose();
        }




        // GET api/values 
        public IEnumerable<Book> Get()
        {
            List<Book> books;
            try
            {
                books = readXML();
            }
            catch (InvalidOperationException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Não há nenhum livro na base"));
            }

            return books;
        }

        // GET api/values/5 
        public Book Get(int isbn)
        {
            List<Book> books;
            try
            {
                books = readXML();
            }
            catch (InvalidOperationException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Não há nenhum livro na base"));
            }

            //retornar o livro com isbn correspondente
            Book book = books.Find(x => x.isbn == isbn);
            if (book == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Livro não encontrado"));
            }
            else
            {
                return book;
            }
        }

        // POST api/values 
        public void Post([FromBody]Book book)
        {
            List<Book> books;
            try
            {
                books = readXML();
            }
            catch (InvalidOperationException)
            {
                books = new List<Book>();
            }


            if (books.Find(x => x.isbn == book.isbn) == null)
            {
                books.Add(book);
                writeXML(books);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Livro já existe na base"));
            }
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
