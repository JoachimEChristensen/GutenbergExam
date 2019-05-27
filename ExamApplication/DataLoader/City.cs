using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;

namespace DataLoader
{
    static class City
    {
        private static IMongoDatabase _mongoDatabase;
        private static IMongoCollection<BsonDocument> _mongoCollection;

        public static async Task Insert()
        {
            int geonameid, population, elevation, gtopo30;
            string name, asciiname, alternatenames, fcode, country, cc2, admin1, admin2, admin3, admin4, timezone;
            char fclass;
            decimal latitude, longitude;
            DateTime moddate;
            
            using (var reader = new StreamReader(@"..\..\..\cities15000.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');
                    
                    Int32.TryParse(values[0], out geonameid);
                    name = values[1];
                    asciiname = values[2];
                    alternatenames = values[3];
                    decimal.TryParse(values[4], out latitude);
                    decimal.TryParse(values[5], out longitude);
                    char.TryParse(values[6], out fclass);
                    fcode = values[7];
                    country = values[8];
                    cc2 = values[9];
                    admin1 = values[10];
                    admin2 = values[11];
                    admin3 = values[12];
                    admin4 = values[13];
                    Int32.TryParse(values[14], out population);
                    Int32.TryParse(values[15], out elevation);
                    Int32.TryParse(values[16], out gtopo30);
                    timezone = values[17];
                    DateTime.TryParse(values[18], out moddate);

                    await Insert(geonameid, name, asciiname, alternatenames, latitude, longitude, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, population, elevation, gtopo30, timezone, moddate);
                }
            }
        }

        private static async Task Insert(int geonameid, string name, string asciiname, string alternatenames, decimal latitude, decimal longitude, char fclass, string fcode, string country, string cc2, string admin1, string admin2, string admin3, string admin4, int population, int elevation, int gtopo30, String timezone, DateTime moddate)
        {
            InsertIntoDatabaseMethod(geonameid, name, asciiname, alternatenames, latitude, longitude, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, population, elevation, gtopo30, timezone, moddate);
            await InsertCityMongoDb(geonameid, name, asciiname, alternatenames, latitude, longitude, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, population, elevation, gtopo30, timezone, moddate);
        }

        private static void InsertIntoDatabaseMethod(int geonameid, string name, string asciiname, string alternatenames, decimal latitude, decimal longitude, char fclass, string fcode, string country, string cc2, string admin1, string admin2, string admin3, string admin4, int population, int elevation, int gtopo30, String timezone, DateTime moddate)
        {
            string dbConnectionString = "Server=127.0.0.1;Port=3306;Database=exam;Uid=root;Pwd=;";

            string query = "INSERT INTO geocities15000 (geonameid, name, asciiname, alternatenames, latitude, longitude, fclass, fcode, country, cc2, admin1, admin2, admin3, admin4, population, elevation, gtopo30, timezone, moddate) VALUES (@geonameid, @name, @asciiname, @alternatenames, @latitude, @longitude, @fclass, @fcode, @country, @cc2, @admin1, @admin2, @admin3, @admin4, @population, @elevation, @gtopo30, @timezone, @moddate)";

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
                cmd.Parameters.Add("@moddate", MySqlDbType.DateTime).Value = moddate;
                
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        
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
        }
    }
}
