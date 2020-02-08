using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataClasses
{
    public class QueriesMongoDb
    {
        private static readonly string ConnectionStringMongoDb = "mongodb://10.0.75.2:27017";
        private static IMongoDatabase _mongoDatabase;
        private static IMongoCollection<BsonDocument> _mongoCollection;

        public List<Book> CityOld(string cityName)
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

        public List<Book> City(string cityName)
        {
            List<Book> books = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Mentionedcities");

            /*db.Mentionedcities.aggregate([
                    {$match: {CityName: "London"}},*/
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

                    /*{$lookup:
                         {
                           from: "Books",
                           let: {city_NameOrId: "$BookNameOrId"},
                           pipeline: 
                           [{
                               $match: 
                                    {
                                        $expr:
                                            {$eq: ["$NameOrId", "$$city_NameOrId"]}
                                    }
                                },
                                {$project: {_id:0, BookName:1, Author:1}}
                           ],
                           as: "books"
                         }},*/
            var lookup = new BsonDocument
            {
                {
                    "$lookup",
                    new BsonDocument
                    {
                        {"from", "Books"},
                        {
                            "let", 
                            new BsonDocument
                            {
                                {"city_NameOrId", "$BookNameOrId"}
                            }
                        },
                        {
                            "pipeline",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    {
                                        "$match",
                                        new BsonDocument
                                        {
                                            {
                                                "$expr",
                                                new BsonDocument
                                                {
                                                    {
                                                        "$eq",
                                                        new BsonArray
                                                        {
                                                            "$NameOrId", "$$city_NameOrId"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$project",
                                        new BsonDocument
                                        {
                                            {"_id", 0},
                                            { "BookName", 1},
                                            { "Author", 1}
                                        }
                                    }
                                }
                            }
                        },
                        {"as", "books"}
                    }
                }
            };

                     /*{$project: {_id:0, BookNameOrId:0, CityName:0}},*/
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", 0},
                        {"BookNameOrId", 0},
                        {"CityName", 0}
                    }
                }
            };

                     /*{$unwind: "$books"},*/
            var unwind = new BsonDocument
            {
                {"$unwind", "$books"}
            };

                     /*{$replaceRoot: {newRoot: "$books"}}
                ])*/
            var replaceRoot = new BsonDocument
            {
                {
                    "$replaceRoot",
                    new BsonDocument
                    {
                        {"newRoot", "$books"}
                    }
                }
            };

            var pipeline = new[] { match, lookup, project, unwind, replaceRoot };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                Book book = new Book
                {
                    Title = res["BookName"].AsString,
                    Author = res["Author"].AsString
                };
                books.Add(book);
            }

            return books;
        }

        public List<City> BookTitleOld(string bookTitle)
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

        public List<City> BookTitle(string bookTitle)
        {
            List<City> cities = new List<City>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");

            //{$match: {BookName: "Around the World in Eighty Days"}},
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

            /*{$lookup:
                    {
                        from: "Mentionedcities",
                        let: {book_NameOrId: "$NameOrId"},
                        pipeline:
                        [
                            {$match: {$expr: {$eq: ["$BookNameOrId", "$$book_NameOrId"]}}},
                            {$project: {_id:0, CityName:1}},
                            {$lookup:
                                {
                                    from: "geocities15000",
                                    let: {city_CityName: "$CityName"},
                                    pipeline:
                                    [
                                        {$match: {$expr: {$eq: ["$asciiname", "$$city_CityName"]}}},
                                        {$project: {_id:0, asciiname:1, latitude:1, longitude:1}}
                                    ],
                                    as: "geocities"
                                }}
                                    
                        ],
                        as: "cities"
                    }
                },*/
            var lookup = new BsonDocument
            {
                {
                    "$lookup",
                    new BsonDocument
                    {
                        {"from", "Mentionedcities"},
                        {
                            "let",
                            new BsonDocument
                            {
                                {"book_NameOrId", "$NameOrId"}
                            }
                        },
                        {
                            "pipeline",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    {
                                        "$match",
                                        new BsonDocument
                                        {
                                            {
                                                "$expr",
                                                new BsonDocument
                                                {
                                                    {
                                                        "$eq",
                                                        new BsonArray
                                                        {
                                                            "$BookNameOrId", "$$book_NameOrId"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$project",
                                        new BsonDocument
                                        {
                                            {"_id", 0},
                                            { "CityName", 1}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$lookup",
                                        new BsonDocument
                                        {
                                            {"from", "geocities15000"},
                                            {
                                                "let",
                                                new BsonDocument
                                                {
                                                    {"city_CityName", "$CityName"}
                                                }
                                            },
                                            {
                                                "pipeline",
                                                new BsonArray
                                                {
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$match",
                                                            new BsonDocument
                                                            {
                                                                {
                                                                    "$expr",
                                                                    new BsonDocument
                                                                    {
                                                                        {
                                                                            "$eq",
                                                                            new BsonArray
                                                                            {
                                                                                "$asciiname", "$$city_CityName"
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$project",
                                                            new BsonDocument
                                                            {
                                                                {"_id", 0},
                                                                { "asciiname", 1},
                                                                { "latitude", 1},
                                                                { "longitude", 1}
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            {"as", "geocities"}
                                        }
                                    }
                                }
                            }
                        },
                        {"as", "cities"}
                    }
                }
            };

            //{$project: {_id:0, NameOrId:0, BookName:0, Author:0}},
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", 0},
                        {"NameOrId", 0},
                        {"BookName", 0},
                        {"Author", 0}
                    }
                }
            };

            //{$unwind: "$cities"},
            var unwind = new BsonDocument
            {
                {"$unwind", "$cities"}
            };

            //{$replaceRoot: {newRoot: "$cities"}},
            var replaceRoot = new BsonDocument
            {
                {
                    "$replaceRoot",
                    new BsonDocument
                    {
                        {"newRoot", "$cities"}
                    }
                }
            };

            //{$project: {CityName:0}},
            var project2 = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"CityName", 0}
                    }
                }
            };

            //{$unwind: "$geocities"},
            var unwind2 = new BsonDocument
            {
                {"$unwind", "$geocities"}
            };

            //{$replaceRoot: {newRoot: "$geocities"}}
            var replaceRoot2 = new BsonDocument
            {
                {
                    "$replaceRoot",
                    new BsonDocument
                    {
                        {"newRoot", "$geocities"}
                    }
                }
            };

            var pipeline = new[] { match, lookup, project, unwind, replaceRoot, project2, unwind2, replaceRoot2 };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                City city = new City
                {
                    AsciiName = res["asciiname"].AsString,
                    Latitude = res["latitude"].AsDecimal,
                    Longitude = res["longitude"].AsDecimal
                };
                cities.Add(city);
            }

            return cities;
        }

        public (List<City>, List<Book>) BookAuthorOld(string bookAuthor)
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

        public (List<City>, List<Book>) BookAuthor(string bookAuthor)
        {
            List<City> cities = new List<City>();
            List<Book> books = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("Books");

            //{$match: {Author: "Shakespeare, William"}},
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

            /*{$lookup:
                    {
                        from: "Mentionedcities",
                        let: {book_NameOrId: "$NameOrId"},
                        pipeline:
                        [
                            {$match: {$expr: {$eq: ["$BookNameOrId", "$$book_NameOrId"]}}},
                            {$project: {_id:0, CityName:1}},
                            {$lookup:
                                {
                                    from: "geocities15000",
                                    let: {city_CityName: "$CityName"},
                                    pipeline:
                                    [
                                        {$match: {$expr: {$eq: ["$asciiname", "$$city_CityName"]}}},
                                        {$project: {_id:0, asciiname:1, latitude:1, longitude:1}}
                                    ],
                                    as: "geocities"
                                }
                            },
                            {$project: {CityName:0}},
                            {$unwind: "$geocities"},
                            {$replaceRoot: {newRoot: "$geocities"}}
                        ],
                        as: "cities"
                    }
                },*/
            var lookup = new BsonDocument
            {
                {
                    "$lookup",
                    new BsonDocument
                    {
                        {"from", "Mentionedcities"},
                        {
                            "let",
                            new BsonDocument
                            {
                                {"book_NameOrId", "$NameOrId"}
                            }
                        },
                        {
                            "pipeline",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    {
                                        "$match",
                                        new BsonDocument
                                        {
                                            {
                                                "$expr",
                                                new BsonDocument
                                                {
                                                    {
                                                        "$eq",
                                                        new BsonArray
                                                        {
                                                            "$BookNameOrId", "$$book_NameOrId"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$project",
                                        new BsonDocument
                                        {
                                            {"_id", 0},
                                            { "CityName", 1}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$lookup",
                                        new BsonDocument
                                        {
                                            {"from", "geocities15000"},
                                            {
                                                "let",
                                                new BsonDocument
                                                {
                                                    {"city_CityName", "$CityName"}
                                                }
                                            },
                                            {
                                                "pipeline",
                                                new BsonArray
                                                {
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$match",
                                                            new BsonDocument
                                                            {
                                                                {
                                                                    "$expr",
                                                                    new BsonDocument
                                                                    {
                                                                        {
                                                                            "$eq",
                                                                            new BsonArray
                                                                            {
                                                                                "$asciiname", "$$city_CityName"
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$project",
                                                            new BsonDocument
                                                            {
                                                                {"_id", 0},
                                                                { "asciiname", 1},
                                                                { "latitude", 1},
                                                                { "longitude", 1}
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            {"as", "geocities"}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$project",
                                        new BsonDocument
                                        {
                                            {"CityName", 0}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {"$unwind", "$geocities"}
                                },
                                new BsonDocument
                                {
                                    {
                                        "$replaceRoot",
                                        new BsonDocument
                                        {
                                            {"newRoot", "$geocities"}
                                        }
                                    }
                                }
                            }
                        },
                        {"as", "cities"}
                    }
                }
            };

            //{$project: {_id:0, NameOrId:0, Author:0}},
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", 0},
                        {"NameOrId", 0},
                        {"Author", 0}
                    }
                }
            };

            //{$replaceRoot: {newRoot: {$mergeObjects: [{$arrayElemAt: ["$cities", 0]}, "$$ROOT"]}}},
            var replaceRoot = new BsonDocument
            {
                {
                    "$replaceRoot",
                    new BsonDocument
                    {
                        {
                            "newRoot", 
                            new BsonDocument
                            {
                                {
                                    "$mergeObjects",
                                    new BsonArray
                                    {
                                        new BsonDocument
                                        {
                                            {
                                                "$arrayElemAt",
                                                new BsonArray
                                                {
                                                    "$cities", 0
                                                }
                                            }
                                        },
                                        "$$ROOT"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            //{$project: {cities: 0}}
            var project2 = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"cities", 0}
                    }
                }
            };

            var pipeline = new[] { match, lookup, project, replaceRoot, project2};
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                if (res.Contains("asciiname"))
                {
                    City city = new City
                    {
                        AsciiName = res["asciiname"].AsString,
                        Latitude = res["latitude"].AsDecimal,
                        Longitude = res["longitude"].AsDecimal
                    };
                    cities.Add(city);
                }

                Book book = new Book
                {
                    Title = res["BookName"].AsString
                };
                books.Add(book);
            }

            return (cities, books);
        }

        public List<Book> CityGeoOld(decimal cityLatitude, decimal cityLongitude)
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

        public List<Book> CityGeo(decimal cityLatitude, decimal cityLongitude)
        {
            List<Book> books = new List<Book>();

            MongoClient mongoClient = new MongoClient(ConnectionStringMongoDb);

            _mongoDatabase = mongoClient.GetDatabase("exam");
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("geocities15000");

            /*{$match:
                    {$and:
                        [
                            {latitude: {$gte: 37.77493 - 1}},
                            {latitude: {$lte: 37.77493 + 1}},
                            {longitude: {$gte: -122.41942 - 1}},
                            {longitude: {$lte: -122.41942 + 1}}
                        ]
                    }
                },*/
            var match = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "$and",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    {
                                        "latitude",
                                        new BsonDocument
                                        {
                                            {"$gte", 37.77493 - 1}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "latitude",
                                        new BsonDocument
                                        {
                                            {"$lte", 37.77493 + 1}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "longitude",
                                        new BsonDocument
                                        {
                                            {"$gte", -122.41942 - 1}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "longitude",
                                        new BsonDocument
                                        {
                                            {"$lte", -122.41942 + 1}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            /*{$lookup:
                    {
                        from: "Mentionedcities",
                        let: {geocities_asciiname: "$asciiname"},
                        pipeline:
                        [
                            {$match: {$expr: {$eq: ["$CityName", "$$geocities_asciiname"]}}},
                            {$project: {_id:0, BookNameOrId:1}},
                            {$lookup:
                                {
                                    from: "Books",
                                    let: {city_BookNameOrId: "$BookNameOrId"},
                                    pipeline:
                                    [
                                        {$match: {$expr: {$eq: ["$NameOrId", "$$city_BookNameOrId"]}}},
                                        {$project: {_id:0, BookName:1}}
                                    ],
                                    as: "books"
                                }
                            },
                            {$project: {BookNameOrId:0}}
                        ],
                        as: "cities"
                    }
                },*/
            var lookup = new BsonDocument
            {
                {
                    "$lookup",
                    new BsonDocument
                    {
                        {"from", "Mentionedcities"},
                        {
                            "let",
                            new BsonDocument
                            {
                                {"geocities_asciiname", "$asciiname"}
                            }
                        },
                        {
                            "pipeline",
                            new BsonArray
                            {
                                new BsonDocument
                                {
                                    {
                                        "$match",
                                        new BsonDocument
                                        {
                                            {
                                                "$expr",
                                                new BsonDocument
                                                {
                                                    {
                                                        "$eq",
                                                        new BsonArray
                                                        {
                                                            "$CityName", "$$geocities_asciiname"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$project",
                                        new BsonDocument
                                        {
                                            {"_id", 0},
                                            { "BookNameOrId", 1}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$lookup",
                                        new BsonDocument
                                        {
                                            {"from", "Books"},
                                            {
                                                "let",
                                                new BsonDocument
                                                {
                                                    {"city_BookNameOrId", "$BookNameOrId"}
                                                }
                                            },
                                            {
                                                "pipeline",
                                                new BsonArray
                                                {
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$match",
                                                            new BsonDocument
                                                            {
                                                                {
                                                                    "$expr",
                                                                    new BsonDocument
                                                                    {
                                                                        {
                                                                            "$eq",
                                                                            new BsonArray
                                                                            {
                                                                                "$NameOrId", "$$city_BookNameOrId"
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new BsonDocument
                                                    {
                                                        {
                                                            "$project",
                                                            new BsonDocument
                                                            {
                                                                {"_id", 0},
                                                                { "BookName", 1}
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            {"as", "books"}
                                        }
                                    }
                                },
                                new BsonDocument
                                {
                                    {
                                        "$project",
                                        new BsonDocument
                                        {
                                            { "BookNameOrId", 0}
                                        }
                                    }
                                }
                            }
                        },
                        {"as", "cities"}
                    }
                }
            };

            //{$project: {asciiname:0}},
            var project = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {"_id", 0},
                        {"asciiname", 0}
                    }
                }
            };

            //{$unwind: "$cities"},
            var unwind = new BsonDocument
            {
                {"$unwind", "$cities"}
            };

            //{$replaceRoot: {newRoot: "$cities"}},
            var replaceRoot = new BsonDocument
            {
                {
                    "$replaceRoot",
                    new BsonDocument
                    {
                        {"newRoot", "$cities"}
                    }
                }
            };

            //{$unwind: "$books"},
            var unwind2 = new BsonDocument
            {
                {"$unwind", "$books"}
            };

            //{$replaceRoot: {newRoot: "$books"}}
            var replaceRoot2 = new BsonDocument
            {
                {
                    "$replaceRoot",
                    new BsonDocument
                    {
                        {"newRoot", "$books"}
                    }
                }
            };

            var pipeline = new[] { match, lookup, project, unwind, replaceRoot, unwind2, replaceRoot2 };
            var result = _mongoCollection.Aggregate<BsonDocument>(pipeline);

            foreach (var res in result.ToListAsync().Result)
            {
                Book book = new Book
                {
                    Title = res["BookName"].AsString
                };
                books.Add(book);
            }

            return books;
        }
    }
}
