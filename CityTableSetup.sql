DROP DATABASE IF EXISTS `geonames`; 
CREATE DATABASE `geonames` DEFAULT CHARACTER SET utf8;
USE  `geonames`;

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
    PRIMARY KEY (`geonameid`)
)  DEFAULT CHARSET=UTF8;