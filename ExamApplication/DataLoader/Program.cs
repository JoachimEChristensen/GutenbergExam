using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;

namespace DataLoader
{
    class Program
    {
        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            //await City.Insert();// a list of cities and their location

            //await BookText.Insert();

            //await BookInfo.Insert();

            //await MentionedCities.Find();
        }
    }
}
