using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataClasses
{
    class QueriesMongoDb
    {
        private static readonly string ConnectionStringMongoDb = "mongodb://10.0.75.2:27017";
        private static IMongoDatabase _mongoDatabase;
        private static IMongoCollection<BsonDocument> _mongoCollection;

        public List<Book> City(string cityName)
        {
            List<Book> booksOut = new List<Book>();
            List<Book> books = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Mentionedcities");

            //db.Mentionedcities.aggregate([{$match: { CityName: "London" }},{ $project: { _id:0, BookNameOrId:1}}])
            var match = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {"CityName", cityName}
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
                        {"BookNameOrId", 1}
                    }
                }
            };

            var pipeline = new[] { match, project };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                Book book = new Book {NameOrId = res["BookNameOrId"].AsString};
                books.Add(book);
            }

            //db.Books.aggregate([{$match: { NameOrId: "11729" }},{ $project: { _id:0, BookName:1, Author:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");
            foreach (Book book in books)
            {
                var match2 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"NameOrId", book.NameOrId}
                        }
                    }
                };

                var project2 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"BookName", 1},
                            {"Author", 1}
                        }
                    }
                };

                var pipeline2 = new[] { match2, project2 };
                var result2 = _mongoCollection.Aggregate<BsonDocument>(pipeline2);

                foreach (var res in result2.ToListAsync().Result)
                {
                    Book book2 = new Book
                    {
                        Title = res["BookName"].AsString,
                        Author = res["Author"].AsString
                    };
                    booksOut.Add(book2);
                }
            }


            return booksOut;
        }

        public List<City> BookTitle(string bookTitle)
        {
            List<City> cities = new List<City>();
            List<City> citiesOut = new List<City>();
            List<Book> books = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");

            //db.Books.aggregate([{$match: { BookName: "Around the World in Eighty Days" }},{ $project: { _id:0, NameOrId:1}}])
            var match = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {"BookName", bookTitle}
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
                Book book = new Book { NameOrId = res["NameOrId"].AsString };
                books.Add(book);
            }

            //db.Mentionedcities.aggregate([{$match: { BookNameOrId: "103" }},{ $project: { _id:0, CityName:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Mentionedcities");
            foreach (Book book in books)
            {
                var match2 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"BookNameOrId", book.NameOrId}
                        }
                    }
                };

                var project2 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"CityName", 1}
                        }
                    }
                };

                var pipeline2 = new[] { match2, project2 };
                var result2 = _mongoCollection.Aggregate<BsonDocument>(pipeline2);

                foreach (var res in result2.ToListAsync().Result)
                {
                    City city = new City
                    {
                        AsciiName = res["CityName"].AsString
                    };
                    cities.Add(city);
                }
            }

            //db.geocities15000.aggregate([{$match: { asciiname: "San Francisco" }},{ $project: { _id:0, asciiname:1, latitude:1, longitude:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("geocities15000");
            foreach (City city in cities)
            {
                var match3 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"asciiname", city.AsciiName}
                        }
                    }
                };

                var project3 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"asciiname", 1},
                            {"latitude", 1},
                            {"longitude", 1}
                        }
                    }
                };

                var pipeline3 = new[] { match3, project3 };
                var result3 = _mongoCollection.Aggregate<BsonDocument>(pipeline3);

                foreach (var res in result3.ToListAsync().Result)
                {
                    City city2 = new City
                    {
                        AsciiName = res["asciiname"].AsString,
                        Latitude = res["latitude"].AsDecimal,
                        Longitude = res["longitude"].AsDecimal
                    };
                    citiesOut.Add(city2);
                }
            }

            return citiesOut;
        }

        public (List<City>, List<Book>) BookAuthor(string bookAuthor)
        {
            List<City> cities = new List<City>();
            List<City> citiesOut = new List<City>();
            List<Book> books = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");

            //db.Books.aggregate([{$match: { Author: "Shakespeare, William" }},{ $project: { _id:0, NameOrId:1, BookName:1}}])
            var match = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {"Author", bookAuthor}
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
                        {"NameOrId", 1},
                        {"BookName", 1}
                    }
                }
            };

            var pipeline = new[] { match, project };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                Book book = new Book
                {
                    NameOrId = res["NameOrId"].AsString,
                    Title = res["BookName"].AsString
                };
                books.Add(book);
            }

            //db.Mentionedcities.aggregate([{$match: { BookNameOrId: "103" }},{ $project: { _id:0, CityName:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Mentionedcities");
            foreach (Book book in books)
            {
                var match2 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"BookNameOrId", book.NameOrId}
                        }
                    }
                };

                var project2 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"CityName", 1}
                        }
                    }
                };

                var pipeline2 = new[] { match2, project2 };
                var result2 = _mongoCollection.Aggregate<BsonDocument>(pipeline2);

                foreach (var res in result2.ToListAsync().Result)
                {
                    City city = new City
                    {
                        AsciiName = res["CityName"].AsString
                    };
                    cities.Add(city);
                }
            }

            //db.geocities15000.aggregate([{$match: { asciiname: "San Francisco" }},{ $project: { _id:0, asciiname:1, latitude:1, longitude:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("geocities15000");
            foreach (City city in cities)
            {
                var match3 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"asciiname", city.AsciiName}
                        }
                    }
                };

                var project3 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"asciiname", 1},
                            {"latitude", 1},
                            {"longitude", 1}
                        }
                    }
                };

                var pipeline3 = new[] { match3, project3 };
                var result3 = _mongoCollection.Aggregate<BsonDocument>(pipeline3);

                foreach (var res in result3.ToListAsync().Result)
                {
                    City city2 = new City
                    {
                        AsciiName = res["asciiname"].AsString,
                        Latitude = res["latitude"].AsDecimal,
                        Longitude = res["longitude"].AsDecimal
                    };
                    citiesOut.Add(city2);
                }
            }

            return (citiesOut, books);
        }

        public List<Book> CityGeo(decimal cityLatitude, decimal cityLongitude)
        {
            List<City> cities = new List<City>();
            List<Book> books = new List<Book>();
            List<Book> booksOut = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("geocities15000");

            //db.geocities15000.aggregate([{$match: { latitude: {$gte: 37.77493 - 1} }},{$match: { latitude: {$lte: 37.77493 + 1} }},{$match: { longitude: {$gte: -122.41942 - 1} }},{$match: { longitude: {$lte: -122.41942 + 1} }},{ $project: { _id:0, asciiname:1}}])
            var match11 = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "latitude",
                            new BsonDocument
                            {
                                {
                                    "$gte", cityLatitude - 1
                                }
                            }
                            
                        }
                    }
                }
            };var match12 = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "latitude",
                            new BsonDocument
                            {
                                {
                                    "$lte", cityLatitude + 1
                                }
                            }

                        }
                    }
                }
            };var match13 = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "longitude",
                            new BsonDocument
                            {
                                {
                                    "$gte", cityLongitude - 1
                                }
                            }

                        }
                    }
                }
            };var match14 = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "longitude",
                            new BsonDocument
                            {
                                {
                                    "$lte", cityLongitude + 1
                                }
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
                        {"asciiname", 1}
                    }
                }
            };

            var pipeline = new[] { match11, match12, match13, match14, project };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                City city = new City
                {
                    AsciiName = res["asciiname"].AsString
                };
                cities.Add(city);
            }

            //db.Mentionedcities.aggregate([{$match: { CityName: "San Francisco" }},{ $project: { _id:0, BookNameOrId:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Mentionedcities");
            foreach (City city in cities)
            {
                var match2 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"CityName", city.AsciiName}
                        }
                    }
                };

                var project2 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"BookNameOrId", 1}
                        }
                    }
                };

                var pipeline2 = new[] { match2, project2 };
                var result2 = _mongoCollection.Aggregate<BsonDocument>(pipeline2);

                foreach (var res in result2.ToListAsync().Result)
                {
                    Book book = new Book
                    {
                        NameOrId = res["BookNameOrId"].AsString
                    };
                    books.Add(book);
                }
            }

            //db.Books.aggregate([{$match: { NameOrId: "11510" }},{ $project: { _id:0, BookName:1}}])
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");
            foreach (Book book in books)
            {
                var match3 = new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {"NameOrId", book.NameOrId}
                        }
                    }
                };

                var project3 = new BsonDocument
                {
                    {
                        "$project",
                        new BsonDocument
                        {
                            {"_id", 0},
                            {"BookName", 1}
                        }
                    }
                };

                var pipeline3 = new[] { match3, project3 };
                var result3 = _mongoCollection.Aggregate<BsonDocument>(pipeline3);

                foreach (var res in result3.ToListAsync().Result)
                {
                    Book book2 = new Book
                    {
                        Title = res["BookName"].AsString
                    };
                    booksOut.Add(book2);
                }
            }

            return booksOut;
        }
    }
}
