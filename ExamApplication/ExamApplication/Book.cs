using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamApplication
{
    public class Book
    {
        private string name;
        private string author;
        public Book(string newName, string newAuthor)
        {
            name = newName;
            author = newAuthor;
        }
    }
}