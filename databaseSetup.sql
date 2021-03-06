DROP DATABASE IF EXISTS exam;
CREATE DATABASE exam DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;
USE exam;

DROP TABLE IF EXISTS books;
CREATE TABLE IF NOT EXISTS books (
ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
NameOrId VARCHAR(100),
BookName VARCHAR(100),
BookAuthor NVARCHAR(100),
FULLTEXT(NameOrId, BookName, BookAuthor)
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
BookNameOrId VARCHAR(100),
CityName VARCHAR(200)
);

ALTER TABLE mentionedcities ADD UNIQUE INDEX bookNameOrIdCityName (BookNameOrId, CityName);

SET GLOBAL max_allowed_packet=1024*1024*1024; 

DROP TABLE IF EXISTS `geocities15000`;

CREATE TABLE `geocities15000` (
    `geonameid` INT(11) NOT NULL,
    `name` VARCHAR(200) DEFAULT NULL,
    `asciiname` VARCHAR(200) DEFAULT NULL,
    `alternatenames` VARCHAR(4000) DEFAULT NULL,
    `latitude` DECIMAL(10 , 7 ) DEFAULT NULL,
    `longitude` DECIMAL(10 , 7 ) DEFAULT NULL,
    `fclass` VARCHAR(1) DEFAULT NULL,
    `fcode` VARCHAR(10) DEFAULT NULL,
    `country` VARCHAR(2) DEFAULT NULL,
    `cc2` VARCHAR(60) DEFAULT NULL,
    `admin1` VARCHAR(20) DEFAULT NULL,
    `admin2` VARCHAR(80) DEFAULT NULL,
    `admin3` VARCHAR(20) DEFAULT NULL,
    `admin4` VARCHAR(20) DEFAULT NULL,
    `population` INT(11) DEFAULT NULL,
    `elevation` INT(11) DEFAULT NULL,
    `gtopo30` INT(11) DEFAULT NULL,
    `timezone` VARCHAR(40) DEFAULT NULL,
    `moddate` DATE DEFAULT NULL,
    PRIMARY KEY (`geonameid`),
    INDEX (`asciiname`),
    index (`latitude`, `longitude`)
)  DEFAULT CHARSET=UTF8;

SET GLOBAL connect_timeout=28800;
SET GLOBAL wait_timeout=28800;
SET GLOBAL interactive_timeout=28800;
set GLOBAL net_write_timeout=28800; 
set GLOBAL net_read_timeout=28800;