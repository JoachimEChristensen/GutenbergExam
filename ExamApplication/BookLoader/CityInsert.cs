using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;

namespace BookLoader
{
    class CityInsert
    {
        static void Main(string[] args)
        {
            int geonameid, population, elevation, gtopo30;
            string name, asciiname, alternatenames, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, timezone;
            double latitude, longitude;
            DateTime moddate;
            
            using (var reader = new StreamReader(@"cities15000.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    
                    geonameid = Int32.Parse(values[0]);
                    name = values[1];
                    asciiname = values[2];
                    alternatenames = values[3];
                    latitude = Double.Parse(values[4]);
                    longitude = Double.Parse(values[5]);
                    fclass = values[6];
                    fcode = values[7];
                    country = values[8];
                    cc2 = values[9];
                    admin1 = values[10];
                    admin2 = values[11];
                    admin3 = values[12];
                    admin4 = values[13];
                    population = Int32.Parse(values[14]);
                    elevation = Int32.Parse(values[15]);
                    gtopo30 = Int32.Parse(values[16]);
                    timezone = values[17];
                    moddate = DateTime.Parse(values[18]);
                
                    InsertIntoDatabaseMethod(geonameid, name, asciiname, alternatenames, latitude, longitude, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, population, elevation, gtopo30, timezone, moddate);
                }
            }
        }

        private static void InsertIntoDatabaseMethod(int geonameid, string name, string asciiname, string alternatenames, double latitude, double longitude, string fclass, string fcode, string country, string cc2, string admin1, string admin2, string admin3, string admin4, int population, int elevation, int gtopo30, String timezone, DateTime moddate)
        {
            string dbConnectionString = "Server=127.0.0.1;Port=3306;Database=exam;Uid=root;Pwd=;";

            string query = "INSERT INTO geocities15000 (geonameid, name, asciiname, alternatenames, latitude, longitude, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, population, elevation, gtopo30, moddate) VALUES (@geonameid, @name, @asciiname, @alternatenames, @latitude, @longitude, @fclass, @fcode, @country, @cc2, @admin1, @admin2, @admin3, @admin4, @population, @elevation, @gtopo30, @moddate)";

            using (MySqlConnection cn = new MySqlConnection(dbConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, cn))
            {

                cmd.Parameters.Add("@geonameid", MySqlDbType.Int32, 11).Value = geonameid;
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 200).Value = name;
                cmd.Parameters.Add("@asciiname", MySqlDbType.VarChar, 200).Value = asciiname;
                cmd.Parameters.Add("@alternatenames", MySqlDbType.VarChar, 4000).Value = alternatenames;
                cmd.Parameters.Add(new MySqlParameter("@latitude", MySqlDbType.Decimal) {Precision = 10, Scale = 7 }).Value = latitude;
                cmd.Parameters.Add(new MySqlParameter("@longitude", MySqlDbType.Decimal) { Precision = 10, Scale = 7 }).Value = longitude;
                cmd.Parameters.Add("@fclass", MySqlDbType.VarChar, 1).Value = fclass;
                cmd.Parameters.Add("@fcode", MySqlDbType.VarChar, 10).Value = fcode;
                cmd.Parameters.Add("@country", MySqlDbType.VarChar, 2).Value = country;
                cmd.Parameters.Add("@cc2", MySqlDbType.VarChar, 60).Value = cc2;
                cmd.Parameters.Add("@admin1", MySqlDbType.VarChar, 20).Value = admin1;
                cmd.Parameters.Add("@admin2", MySqlDbType.VarChar, 80).Value = admin2;
                cmd.Parameters.Add("@admin3", MySqlDbType.VarChar, 20).Value = admin3;
                cmd.Parameters.Add("@admin4", MySqlDbType.VarChar, 20).Value = admin4;
                cmd.Parameters.Add("@population", MySqlDbType.Int32, 11).Value = population;
                cmd.Parameters.Add("@elevation", MySqlDbType.Int32, 11).Value = elevation;
                cmd.Parameters.Add("@gtopo30", MySqlDbType.Int32, 11).Value = gtopo30;
                cmd.Parameters.Add("@timezone", MySqlDbType.VarChar, 40).Value = timezone;
                cmd.Parameters.Add("@timezone", MySqlDbType.DateTime).Value = timezone;
                
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        /*
        private static IMongoDatabase _mongoDatabase;
        private static IMongoCollection<BsonDocument> _mongoCollection;
        static async Task InsertCityMongoDb(int geonameid, string name, string asciiname, string alternatenames, decimal latitude, decimal longitude, char fclass, string fcode, string country, string cc2, string admin1, string admin2, string admin3, string admin4, int population, int elevation, int gtopo30, string timezone, DateTime moddate)
        {
            string connectionString = "mongodb://10.0.75.2:27017";
            MongoClient mongoClient = new MongoClient(connectionString);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("geocities15000");

            await _mongoCollection.InsertOneAsync(new BsonDocument
            {
                { "geonameid", geonameid },
                { "name", name },
                { "asciiname", asciiname },
                { "alternatenames", alternatenames },
                { "latitude", latitude },
                { "longitude", longitude },
                { "fclass", fclass },
                { "fcode", fcode },
                { "country", country },
                { "cc2", cc2 },
                { "admin1", admin1 },
                { "admin2", admin2 },
                { "admin3", admin3 },
                { "admin4", admin4 },
                { "population", population },
                { "elevation", elevation },
                { "gtopo30", gtopo30 },
                { "timezone", timezone },
                { "moddate", moddate }
            });
        }*/
    }
}
