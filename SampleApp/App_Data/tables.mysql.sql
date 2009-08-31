CREATE TABLE  `sampleapp`.`categories` 
(
  `CategoryId` int(10) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`CategoryId`)
) 

CREATE TABLE  `sampleapp`.`departments` 
(
  `DepartmentId` int(10) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`DepartmentId`)
) 

CREATE TABLE  `sampleapp`.`items` 
(
  `ItemId` int(10) NOT NULL AUTO_INCREMENT,
  `Description` varchar(50) CHARACTER SET utf8 NOT NULL,
  `CategoryId` int(10) DEFAULT NULL,
  PRIMARY KEY (`ItemId`),
  KEY `FK_Items_Categories` (`CategoryId`),
  CONSTRAINT `FK_Items_Categories` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`CategoryId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) 


CREATE TABLE  `sampleapp`.`persons` 
(
  `PersonId` int(10) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `Email` varchar(150) CHARACTER SET utf8 DEFAULT NULL,
  `Expires` datetime DEFAULT NULL,
  PRIMARY KEY (`PersonId`)
)


CREATE TABLE  `sampleapp`.`person_department` 
(
  `PersonId` int(10) NOT NULL,
  `DepartmentId` int(10) NOT NULL,
  PRIMARY KEY (`PersonId`,`DepartmentId`),
  KEY `FK_Person_Department_Departments` (`DepartmentId`),
  CONSTRAINT `FK_Person_Department_Persons` FOREIGN KEY (`PersonId`) REFERENCES `persons` (`PersonId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Person_Department_Departments` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`DepartmentId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) 


CREATE TABLE  `sampleapp`.`personitem` 
(
  `ItemId` int(10) NOT NULL,
  `PersonId` int(10) NOT NULL,
  PRIMARY KEY (`ItemId`,`PersonId`),
  KEY `FK_PersonItem_Persons` (`PersonId`),
  CONSTRAINT `FK_PersonItem_Items` FOREIGN KEY (`ItemId`) REFERENCES `items` (`ItemId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_PersonItem_Persons` FOREIGN KEY (`PersonId`) REFERENCES `persons` (`PersonId`) ON DELETE NO ACTION ON UPDATE NO ACTION
)


CREATE DEFINER = 'root'@'%' FUNCTION `CastToTiny`(
        value BIGINT
    )
    RETURNS tinyint(1)
    DETERMINISTIC
    NO SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
  RETURN value;
END;