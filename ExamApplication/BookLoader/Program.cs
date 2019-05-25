using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace BookLoader
{
    class Program
    {
        private static readonly string _connectionString = "Server=127.0.0.1;Port=3306;Database=exam;Uid=root;Pwd=;";

        static void Main(string[] args)
        {
            string[] filePaths = Directory.GetFiles(@"D:\Desktop\kage\Downloads\archive\root\zipfiles\", "*.txt", SearchOption.AllDirectories);
            int count = 1;

            foreach (string path in filePaths)
            {
                string readText = File.ReadAllText(path);
                string fileName = Path.GetFileName(path);
                fileName = fileName.Substring(0, fileName.Length - 4);

                InsertBook(fileName, readText);

                Console.WriteLine("The book: " + fileName + " have been added, there is now: " + (filePaths.Length - count) + " books left");
                count++;
            }
        }

        static void InsertBook(string nameOrId, string text)
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
                    command.CommandText = "insert into bookstext (NameOrId, `Text`) VALUES (@NameOrId, @Text);";
                    command.Parameters.AddWithValue("@NameOrId", nameOrId);
                    command.Parameters.AddWithValue("@Text", text);

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
    }
}
