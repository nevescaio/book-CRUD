using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCRUD
{
    [Serializable]
    public class Book
    {
        public string name { get; set; }
        public long isbn { get; set; }
        public string author { get; set; }
    }
}
