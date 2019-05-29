using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace DataClasses
{
    public class QueriesSql
    {
        private static readonly string ConnectionStringSql = "Server=127.0.0.1;Port=3306;Database=exam;Uid=root;Pwd=;";

        public List<Book> City(string cityName)
        {
            List<Book> books = new List<Book>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select BookName, BookAuthor from books join mentionedcities on books.NameOrId = mentionedcities.BookNameOrId where mentionedcities.CityName = @CityName;";
                    command.Parameters.AddWithValue("@CityName", cityName);
                    command.CommandTimeout = 28800;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Book book = new Book();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "BookName")
                            {
                                book.Title = (string)reader.GetValue(i);
                            }
                            if (reader.GetName(i) == "BookAuthor")
                            {
                                book.Author = (string)reader.GetValue(i);
                            }
                        }

                        books.Add(book);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }

                return books;
            }
        }

        public List<City> BookTitle(string bookTitle)
        {
            List<City> cities = new List<City>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select asciiname, latitude, longitude from geocities15000 join mentionedcities on geocities15000.asciiname = mentionedcities.CityName join books on mentionedcities.BookNameOrId = books.NameOrId where BookName = @BookName;";
                    command.Parameters.AddWithValue("@BookName", bookTitle);
                    command.CommandTimeout = 28800;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        City city = new City();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "asciiname")
                            {
                                city.AsciiName = (string)reader.GetValue(i);
                            }
                            if (reader.GetName(i) == "latitude")
                            {
                                city.Latitude = (decimal)reader.GetValue(i);
                            }
                            if (reader.GetName(i) == "longitude")
                            {
                                city.Longitude = (decimal)reader.GetValue(i);
                            }
                        }

                        cities.Add(city);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }

                return cities;
            }
        }

        public (List<City>, List<Book>) BookAuthor(string bookAuthor)
        {
            List<City> cities = new List<City>();
            List<Book> books = new List<Book>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select asciiname, latitude, longitude, BookName from geocities15000 join mentionedcities on geocities15000.asciiname = mentionedcities.CityName join books on mentionedcities.BookNameOrId = books.NameOrId where BookAuthor = @BookAuthor;";
                    command.Parameters.AddWithValue("@BookAuthor", bookAuthor);
                    command.CommandTimeout = 28800;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        City city = new City();
                        Book book = new Book();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "asciiname")
                            {
                                city.AsciiName = (string)reader.GetValue(i);
                            }
                            if (reader.GetName(i) == "latitude")
                            {
                                city.Latitude = (decimal)reader.GetValue(i);
                            }
                            if (reader.GetName(i) == "longitude")
                            {
                                city.Longitude = (decimal)reader.GetValue(i);
                            }
                            if (reader.GetName(i) == "BookName")
                            {
                                book.Title = (string)reader.GetValue(i);
                            }
                        }

                        cities.Add(city);
                        books.Add(book);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }

                return (cities, books);
            }
        }

        public List<Book> CityGeo(decimal cityLatitude, decimal cityLongitude)
        {
            List<Book> books = new List<Book>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select BookName from books join mentionedcities on books.NameOrId = mentionedcities.BookNameOrId join geocities15000 on mentionedcities.CityName = geocities15000.asciiname where latitude between @latitude - 2 and @latitude + 2 and longitude between @longitude - 2 and @longitude + 2;";
                    command.Parameters.AddWithValue("@latitude", cityLatitude);
                    command.Parameters.AddWithValue("@longitude", cityLongitude);
                    command.CommandTimeout = 28800;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Book book = new Book();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "BookName")
                            {
                                book.Title = (string)reader.GetValue(i);
                            }
                        }

                        books.Add(book);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }

                return books;
            }
        }
    }
}
