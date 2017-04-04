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
        string fileName = "books.xml";

        // GET api/values 
        public IEnumerable<Book> Get()
        {

            //abrir arquivo e verificar se existe
            FileStream fs;
            try
            {
                fs = new FileStream(fileName, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                HttpResponseMessage errorResponse = Request.CreateErrorResponse(HttpStatusCode.NotFound, "Arquivo de livros não existe");
                throw new HttpResponseException(errorResponse);
            }
            catch (Exception)
            {
                HttpResponseMessage errorResponse = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao abrir arquivo de livros");
                throw new HttpResponseException(errorResponse);
            }


            //ler arquivo com codificacao EBCDIC (code page 500)
            XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));
            TextReader reader = new StreamReader(fs, Encoding.GetEncoding(500));
            List<Book> books = (List<Book>)serializer.Deserialize(reader);


            fs.Dispose();

            return books;
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
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
