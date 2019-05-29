using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;

namespace DataLoader
{
    class MentionedCities
    {
        private static readonly string ConnectionStringSql = "Server=127.0.0.1;Port=3306;Database=exam;Uid=root;Pwd=;";
        private static readonly string ConnectionStringMongoDb = "mongodb://10.0.75.2:27017";
        private static readonly List<string> CitiesListSql = new List<string>();
        private static IMongoDatabase _mongoDatabase;
        private static IMongoCollection<BsonDocument> _mongoCollection;
        private static readonly List<string> CitiesListMongoDb = new List<string>();
        private static readonly List<string> BookIdSql = new List<string>();
        private static readonly List<string> BookIdMongoDb = new List<string>();

        public static async Task Find()
        {
            LoadCities();
            await FindMentionedCities();
        }

        private static void LoadCities()
        {
            LoadCitiesSql();
            LoadCitiesMongoDb();
        }
        private static void LoadCitiesSql()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select asciiname from geocities15000;";

                    connection.Open();
                    var reader = command.ExecuteReader();

                    //transaction.Commit();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "asciiname")
                            {
                                CitiesListSql.Add((string)reader.GetValue(i));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
        }

        private static void LoadCitiesMongoDb()
        {
            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("geocities15000");

            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", 0},
                        {"asciiname", 1}
                    }
                }
            };

            var pipeline = new[] { project };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                CitiesListMongoDb.Add(res["asciiname"].AsString);
            }
        }

        private static async Task FindMentionedCities()
        {
            int countSql = 1;
            int countMongoDb = 1;
            foreach (string citySql in CitiesListSql)
            {
                Console.WriteLine("Finding: " + citySql + " in MySQL");
                FindMentionedCitiesSql(citySql);

                Console.WriteLine("Found: " + BookIdSql.Count + " in MySQL, cities left: " + (CitiesListSql.Count - countSql) + ", now inserting...");
                int countBookSql = BookIdSql.Count;
                foreach (string bookIdSql in BookIdSql)
                {
                    Console.Write("\rBooks left for this city: " + countBookSql + "                   ");
                    InsertFoundCitiesSql(citySql, bookIdSql);
                    countBookSql--;
                }
                if (BookIdSql.Count != 0)
                {
                    Console.Write("                                   \n");
                }

                BookIdSql.Clear();
                countSql++;
            }

            foreach (string cityMongoDb in CitiesListMongoDb)
            {
                Console.WriteLine("Finding: " + cityMongoDb + " in MongoDB");
                FindMentionedCitiesMongoDb(cityMongoDb);

                Console.WriteLine("Found: " + BookIdMongoDb.Count + " in MongoDB, cities left: " + (CitiesListMongoDb.Count - countMongoDb) + ", now inserting...");
                int countBookMongoDb = BookIdMongoDb.Count;
                foreach (string bookIdMongoDb in BookIdMongoDb)
                {
                    Console.Write("\rBooks left for this city: " + countBookMongoDb + "                   ");
                    await InsertFoundCitiesMongoDb(cityMongoDb, bookIdMongoDb);
                    countBookMongoDb--;
                }
                if (BookIdMongoDb.Count != 0)
                {
                    Console.Write("                                   \n");
                }

                BookIdMongoDb.Clear();
                countMongoDb++;
            }
        }

        private static void FindMentionedCitiesSql(string city)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select NameOrId from bookstext where match(NameOrId, `Text`) against(@city in natural language mode);";
                    command.Parameters.AddWithValue("@city", MySqlHelper.EscapeString("\"") + city + MySqlHelper.EscapeString("\""));
                    command.CommandTimeout = 28800;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "NameOrId")
                            {
                                BookIdSql.Add((string)reader.GetValue(i));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
        }

        private static void FindMentionedCitiesMongoDb(string city)
        {
            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("BooksText");

            var match = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "$text",
                            new BsonDocument
                            {
                                {"$search", "\"" + city + "\""}
                            }
                        }
                    }
                }
            };

            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", 0},
                        {"NameOrId", 1}
                    }
                }
            };

            var pipeline = new[] { match, project };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                BookIdMongoDb.Add(res["NameOrId"].AsString);
            }
        }

        private static void InsertFoundCitiesSql(string city, string bookNameOrId)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionStringSql))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "insert into mentionedcities (BookNameOrId, CityName) VALUES (@BookNameOrId, @CityName);";
                    command.Parameters.AddWithValue("@BookNameOrId", bookNameOrId);
                    command.Parameters.AddWithValue("@CityName", city);
                    

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
        }

        private static async Task InsertFoundCitiesMongoDb(string city, string bookNameOrId)
        {
            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Mentionedcities");

            await _mongoCollection.InsertOneAsync(new BsonDocument { { "BookNameOrId", bookNameOrId }, { "CityName", city } });
        }
    }
}
