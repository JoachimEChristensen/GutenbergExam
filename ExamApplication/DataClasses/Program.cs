using System;
using System.Linq;

namespace DataClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            QueriesSql queriesSql = new QueriesSql();
            //var test11s = queriesSql.City("London");
            //var test12s = queriesSql.City("Paris");
            //var test13s = queriesSql.City("Athens");
            //var test14s = queriesSql.City("Dubai");
            //var test15s = queriesSql.City("Tokyo");

            //var test21s = queriesSql.BookTitle("Around the World in Eighty Days");
            //var test22s = queriesSql.BookTitle("La Fiammetta");
            //var test23s = queriesSql.BookTitle("The Warriors");
            //var test24s = queriesSql.BookTitle("Apocolocyntosis");
            //var test25s = queriesSql.BookTitle("The Magna Carta");

            //var test31s = queriesSql.BookAuthor("Shakespeare, William");
            //var test32s = queriesSql.BookAuthor("Dante Alighieri");
            //var test33s = queriesSql.BookAuthor("Hodgson, William Hope");
            //var test34s = queriesSql.BookAuthor("Waddington, Mary King");
            //var test35s = queriesSql.BookAuthor("Tucker, George");

            var test41s = queriesSql.CityGeo((decimal)37.77493, (decimal)-122.41942);
            //var test42s = queriesSql.CityGeo((decimal)48.8534100, (decimal)2.3488000);
            //var test43s = queriesSql.CityGeo((decimal)37.9837600, (decimal)23.7278400);
            //var test44s = queriesSql.CityGeo((decimal)25.0657000, (decimal)55.1712800);
            //var test45s = queriesSql.CityGeo((decimal)35.6895000, (decimal)139.6917100);

            QueriesMongoDb queriesMongoDb = new QueriesMongoDb();
            //var test11m = queriesMongoDb.City("London");
            //var test12m = queriesMongoDb.City("Paris");
            //var test13m = queriesMongoDb.City("Athens");
            //var test14m = queriesMongoDb.City("Dubai");
            //var test15m = queriesMongoDb.City("Tokyo");

            //var test21m = queriesMongoDb.BookTitle("Around the World in Eighty Days");
            //var test22m = queriesMongoDb.BookTitle("La Fiammetta");
            //var test23m = queriesMongoDb.BookTitle("The Warriors");
            //var test24m = queriesMongoDb.BookTitle("Apocolocyntosis");
            //var test25m = queriesMongoDb.BookTitle("The Magna Carta");

            //var test31m = queriesMongoDb.BookAuthor("Shakespeare, William");
            //var test32m = queriesMongoDb.BookAuthor("Dante Alighieri");
            //var test33m = queriesMongoDb.BookAuthor("Hodgson, William Hope");
            //var test34m = queriesMongoDb.BookAuthor("Waddington, Mary King");
            //var test35m = queriesMongoDb.BookAuthor("Tucker, George");

            //var test41m = queriesMongoDb.CityGeo((decimal)37.77493, (decimal)-122.41942);
            //var test42m = queriesMongoDb.CityGeo((decimal)48.8534100, (decimal)2.3488000);
            //var test43m = queriesMongoDb.CityGeo((decimal)37.9837600, (decimal)23.7278400);
            //var test44m = queriesMongoDb.CityGeo((decimal)25.0657000, (decimal)55.1712800);
            //var test45m = queriesMongoDb.CityGeo((decimal)35.6895000, (decimal)139.6917100);
        }
    }
}
