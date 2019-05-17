using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamApplication
{
    public class City
    {
        private string name;
        private string location;
        public City(string newName, string newLocation)
        {
            name = newName;
            location = newLocation;
        }
    }
}