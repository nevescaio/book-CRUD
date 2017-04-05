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



        //le arquivo com codificacao EBCDIC (code page 500) e retorna lista de livros
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
            //ler livros do arquivo
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
            //verificar se o objeto foi passado
            if (book == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Livro não enviado"));
            }



            //ler arquivos do livro
            List<Book> books;
            try
            {
                books = readXML();
            }
            catch (InvalidOperationException)
            {
                books = new List<Book>();
            }

            //verificar se o livro ja existe na base antes de adicionar
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
        public void Put(int isbn, [FromBody]Book book)
        {
            //ler livros do arquivo
            List<Book> books;
            try
            {
                books = readXML();
            }
            catch (InvalidOperationException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Não há nenhum livro na base"));
            }

            //verificar se o objeto foi passado
            if (book == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Livro não enviado"));
            }

            //verificar se o livro existe na base
            Book existingBook = books.Find(x => x.isbn == isbn);
            if (existingBook == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Livro não encontrado"));
            }

            //verificar se o novo isbn ja existe na base
            if (book.isbn != isbn && books.Find(x => x.isbn == book.isbn) != null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Livro já existe na base"));
            }

            //atualizar o livro
            existingBook.name = book.name;
            existingBook.isbn = book.isbn;
            existingBook.author = book.author;

            //escrever no arquivo
            writeXML(books);

        }

        // DELETE api/values/5 
        public void Delete(int isbn)
        {
            //ler livros do arquivo
            List<Book> books;
            try
            {
                books = readXML();
            }
            catch (InvalidOperationException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Não há nenhum livro na base"));
            }

            //verificar se o livro existe na base
            Book existingBook = books.Find(x => x.isbn == isbn);
            if (existingBook == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Livro não encontrado"));
            }

            //remover o livro
            books.Remove(existingBook);

            //escrever no arquivo
            writeXML(books);

        }
    }
}
