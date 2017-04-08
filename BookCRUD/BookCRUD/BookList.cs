using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using System.Net.Http;




namespace BookCRUD
{
    public class BookList
    {
        private const string fileName = "books.xml";
        private const long maxIsbn = 9999999999999;
        private List<Book> books;


        //le arquivo com codificacao EBCDIC (code page 500) e retorna lista de livros
        private List<Book> loadBooks()
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));
                TextReader reader = new StreamReader(fs, Encoding.GetEncoding(500));
                List<Book> books = (List<Book>)serializer.Deserialize(reader);
                return books;
            }

        }

        //escreve a lista de livros no arquivo com codificacao EBCDIC
        private void saveBooks()
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            XmlSerializer ser = new XmlSerializer(typeof(List<Book>));
            TextWriter writer = new StreamWriter(fs, Encoding.GetEncoding(500));
            ser.Serialize(writer, books);
            fs.Dispose();
        }

        //valida os dados de um livro
        private void validate (Book book)
        {
            //verificar se o objeto foi passado
            if (book == null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.ReasonPhrase = "Livro não enviado";
                response.Content = new StringContent("Nenhum livro foi enviado na requisição");
                throw new HttpResponseException(response);
            }

            //verificar se os campos estao preenchidos
            if (book.author == null || book.isbn == 0 || book.name == null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.ReasonPhrase = "Campo obrigatório não informado";
                response.Content = new StringContent("Os campos \"name\", \"isbn\" e \"author\" são obrigatórios");
                throw new HttpResponseException(response);
            }

            validate(book.isbn);
        }

        //valida um ISBN
        private void validate (long isbn)
        {
            if (isbn <= 0 || isbn > maxIsbn)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.ReasonPhrase = "ISBN inválido";
                response.Content = new StringContent("O ISBN deve ser um número positivo com no máximo 13 dígitos");
                throw new HttpResponseException(response);
            }
        }

        //valida se um livro existe na base
        private bool bookExists(long isbn)
        {
            Book book = books.Find(x => x.isbn == isbn);
            if (book == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }





        public BookList()
        {
            //ler livros do arquivo
            try
            {
                books = loadBooks();
            }
            catch (InvalidOperationException)
            {
                books = new List<Book>();
            }
        }


        //retorna todos os livros da base, ou o livro solicitado
        public List<Book> getBooks(long isbn = 0)
        {
            if (books.Count == 0)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.ReasonPhrase = "Base vazia";
                response.Content = new StringContent("Não há nenhum livro cadastrado na base");
                throw new HttpResponseException(response);
            }

            return books;
        }

        //retorna um livro especifico
        public Book getBook(long isbn)
        {
            validate(isbn);

            Book book = books.Find(x => x.isbn == isbn);
            if (book == null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.ReasonPhrase = "Livro não encontrado";
                response.Content = new StringContent(string.Format("O livro com ISBN {0} não existe na base", isbn));
                throw new HttpResponseException(response);
            }

            return book;        
        }

        //adiciona um novo livro a base
        public void add(Book book)
        {
            validate(book);

            if(bookExists(book.isbn))
            { 
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.ReasonPhrase = "Livro já existente";
                response.Content = new StringContent(string.Format("O livro com ISBN {0} já existe na base", book.isbn));
                throw new HttpResponseException(response);
            }

            books.Add(book);
            saveBooks();
        }

        //edita um livro ja existente na base
        public void update(long isbn, Book newBook)
        {
            Book existingBook = getBook(isbn);

            validate(newBook);
            if(newBook.isbn != isbn && bookExists(newBook.isbn))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.ReasonPhrase = "Livro já existente";
                response.Content = new StringContent(string.Format("Já existe outro livro com ISBN {0} na base", newBook.isbn));
                throw new HttpResponseException(response);
            }

            existingBook.name = newBook.name;
            existingBook.isbn = newBook.isbn;
            existingBook.author = newBook.author;

            saveBooks();           
        }

        //remove um livro da base
        public void delete(long isbn)
        {
            Book book = getBook(isbn);
            books.Remove(book);
            saveBooks();
        }
    }
}
