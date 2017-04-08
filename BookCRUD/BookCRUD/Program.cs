using System;
using Microsoft.Owin.Hosting;

namespace BookCRUD
{
    class Program
    {
        static void Main(string[] args)
        {
            WebApp.Start<Startup>(url: "http://localhost:9000/");

            Console.WriteLine("WEB API CRUD de Livros rodando\n");

            Console.ReadLine();
        }
    }
}
