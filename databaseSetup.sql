DROP DATABASE IF EXISTS exam;
CREATE DATABASE exam DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;
USE exam;

DROP TABLE IF EXISTS books;
CREATE TABLE IF NOT EXISTS books (
ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
BookName VARCHAR(100),
BookAuthor NVARCHAR(100)
);

DROP TABLE IF EXISTS booksText;
CREATE TABLE IF NOT EXISTS booksText (
Id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
NameOrId VARCHAR(100),
`Text` text,
FULLTEXT(NameOrId,`Text`)
);

DROP TABLE IF EXISTS cities;
CREATE TABLE IF NOT EXISTS cities (
ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
CityName VARCHAR(100),
CityGeolocation VARCHAR(100)
);

DROP TABLE IF EXISTS mentionedCities;
CREATE TABLE IF NOT EXISTS mentionedCities (
BookID INT,
CityID INT
);

SET GLOBAL max_allowed_packet=1024*1024*1024; 