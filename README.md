# GutenbergExam
Group: CJX

Members: 

Joachim Ellingsgaard Christensen

Christian Philip Ege Østergaard Nielsen

Xinkai Huang

## Setup
To run the program completely you have to run it in a specific order. First, you need to set the SQL database up by running DatabaseSetupSQL. The MongoDB Database is set up by running DataLoader, although you need the zipfile with the various books to run it. You also need the cities15000.txt file to ensure that the cities are inserted properly. Make sure to change the paths in the various files to ensure that the program runs correctly.

Once the DataLoader is complete, you can run the DataClasses program to test out the queries, and you can run the actual front-end application to try out the queries for yourself.

## Which database
We have decided to use MongoDB (A Document Oriented Database) as well as MySQL (A SQL based database) as our two databases to compare. Originally we had planned to use Neo4J, but we decided to change to MongoDB because it was used in a wider context overalll in the real world, so it would make more sense in our minds to use it and MySQL.

## How is it modelled in the database
The database is modeled in such a fashion that we have two primary tables. The book table, which contains the Name, Author, ID and contents of the book. The city table contains every type of variable present in the Cities15000 table, as to allow for easy importing.

The reason why we've done this, is to make it easier to establish connections between the City table and the Book table, as reading through the entire book in a table column was far easier than reading through them programmatically and then inserting the connections into the database. The methods that SQL and MongoDB provide allow us to search through the book column easier, which makes faster to just do the searching in the database itself.

## How is it modelled in the Application
In our application, we retrieve the values that we query, and then insert them into object classes that match the two databases. However, for the city object, we're only interested in the ASCII name and the latitude/longitude values, so those are the only ones featured in the City object.

The data is imported in four separate segments. First, we load the cities. Then, we load the text of the books. Then, we load the books' titles and authors. Lastly, we establish the connections between the two databases. This last step took an extremely long time for both databases, so regardless of which database type you would pick, you would still end up with this issue.

## Behavior of the test set
Our general results for the two database query sets were done in sets of five. First, we measured the general time it took for the 20 queries. Then, we measured each set of five individually, allowing us the following results:

General SQL time: 2 min 4 sek

City SQL time: 16 sek

BookTitle SQL time: 1.869 sek

BookAuthor SQL time: 2.052 sek

CityGeo SQL time: 1 min 48 sek


General Mongo time: 2min 33sek

City Mongo time: 51 sek

BookTitle Mongo time: 2.063 sek

BookAuthor Mongo time: 10.871 sek

CityGeo Mongo time: 1min 46sek


As you can see, our SQL solution ended up quicker. This might be due to how we were forced to run three separate queries in MongoDB instead of one continuous query, but that's just a hypothesis. We've also done our best to apply an index on each of the colums that are commonly searched through by the queries, as well as a Unique only index to allow for no repeated values. This way, we reduce redundant entries gathered and optimize the queries in the best manner possible.

## Recommendation
We would recommend using MySQL, as we had many problems trying to establish connections between the City and Book tables in MongoDB. They are both great database types, but the issues we had with the connections in MongoDB were a result of missing RAM, and we do not like working with RAM intensive programs. Just out of personal preference.
