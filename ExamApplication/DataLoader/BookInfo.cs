using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace DataLoader
{
    class BookInfo
    {
        private static readonly string _connectionString = "Server=127.0.0.1;Port=3306;Database=exam;Uid=root;Pwd=;";
        private static IMongoDatabase _mongoDatabase;
        private static IMongoCollection<BsonDocument> _mongoCollection;
        private static readonly HashSet<string> BookSql = new HashSet<string>();
        private static readonly HashSet<string> BookMongoDb = new HashSet<string>();

        public static async Task Insert()
        {
            Console.WriteLine("Loading file locations.");
            //Change so that it leads to the location of the books on your PC
            string[] filePaths = Directory.GetFiles(@"D:\Desktop\kage\Downloads\rdf-files\cache\epub\", "*.rdf", SearchOption.AllDirectories);
            int count = 1;

            Console.WriteLine("Checking for existing books.");
            CheckBook();

            foreach (string path in filePaths)
            {
                string fileName = Path.GetFileName(path);
                fileName = fileName.Substring(2, fileName.Length - 6);

                bool existSql = BookSql.Contains(fileName);
                bool existMongoDb = BookMongoDb.Contains(fileName);

                if (!existSql || !existMongoDb)
                {
                    IGraph g = new Graph();
                    FileLoader.Load(g, path, new RdfXmlParser());

                    string author = "";
                    string title = "";

                    foreach (Triple triple in g.Triples)
                    {
                        if (triple.Predicate.ToString() == "http://www.gutenberg.org/2009/pgterms/name")
                        {
                            foreach (INode node in triple.Nodes)
                            {
                                if (node.NodeType == NodeType.Literal)
                                {
                                    author = node.ToString();
                                }
                            }
                        }

                        if (triple.Predicate.ToString() == "http://purl.org/dc/terms/title")
                        {
                            foreach (INode node in triple.Nodes)
                            {
                                if (node.NodeType == NodeType.Literal)
                                {
                                    title = node.ToString();
                                }
                            }
                        }
                    }

                    await InsertBook(fileName, title, author, existSql, existMongoDb);

                    Console.WriteLine("ID: " + fileName + ", left: " + (filePaths.Length - count) + ", in DB: " + count);
                }

                count++;
            }
        }

        static async Task InsertBook(string nameOrId, string title, string author, bool existSql, bool existMongoDb)
        {
            if (!existSql)
            {
                InsertBookSql(nameOrId, title, author);
            }
            if (!existMongoDb)
            {
                await InsertBookMongoDb(nameOrId, title, author);
            }
        }

        static void InsertBookSql(string nameOrId, string title, string author)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "insert into books (NameOrId, BookName, BookAuthor) VALUES (@NameOrId, @BookName, @BookAuthor);";
                    command.Parameters.AddWithValue("@NameOrId", nameOrId);
                    command.Parameters.AddWithValue("@BookName", title);
                    command.Parameters.AddWithValue("@BookAuthor", author);

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

        static async Task InsertBookMongoDb(string nameOrId, string title, string author)
        {
            string connectionString = "mongodb://10.0.75.2:27017";
            MongoClient mongoClient = new MongoClient(connectionString);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");

            await _mongoCollection.InsertOneAsync(new BsonDocument { { "NameOrId", nameOrId }, { "BookName", title }, { "Author", author} });
        }

        static void CheckBook()
        {
            BookSql.UnionWith(ChechBookSql());
            BookMongoDb.UnionWith(CheckBookMongoDb());
        }

        static List<string> ChechBookSql()
        {
            List<string> books = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand command = new MySqlCommand { Connection = connection };

                try
                {
                    command.CommandText = "select NameOrId FROM books;";

                    connection.Open();
                    var reader = command.ExecuteReader();

                    //transaction.Commit();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "NameOrId")
                            {
                                books.Add((string)reader.GetValue(i));
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

            return books;
        }

        static List<string> CheckBookMongoDb()
        {
            List<string> books = new List<string>();

            string connectionString = "mongodb://10.0.75.2:27017";
            MongoClient mongoClient = new MongoClient(connectionString);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");

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

            var pipeline = new[] { project };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                books.Add(res["NameOrId"].AsString);
            }

            return books;
        }
    }
}
