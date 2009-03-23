DROP TABLE IF EXISTS `samplewebapp_dbo`.`categories`;
CREATE TABLE  `samplewebapp_dbo`.`categories` (
  `CategoryId` int(10) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `samplewebapp_dbo`.`departments`;
CREATE TABLE  `samplewebapp_dbo`.`departments` (
  `DepartmentId` int(10) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`DepartmentId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `samplewebapp_dbo`.`items`;
CREATE TABLE  `samplewebapp_dbo`.`items` (
  `ItemId` int(10) NOT NULL AUTO_INCREMENT,
  `Description` varchar(50) CHARACTER SET utf8 NOT NULL,
  `CategoryId` int(10) DEFAULT NULL,
  PRIMARY KEY (`ItemId`),
  KEY `FK_Items_Categories` (`CategoryId`),
  CONSTRAINT `FK_Items_Categories` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`CategoryId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `samplewebapp_dbo`.`person_department`;
CREATE TABLE  `samplewebapp_dbo`.`person_department` (
  `PersonId` int(10) NOT NULL,
  `DepartmentId` int(10) NOT NULL,
  PRIMARY KEY (`PersonId`,`DepartmentId`),
  KEY `FK_Person_Department_Departments` (`DepartmentId`),
  CONSTRAINT `FK_Person_Department_Persons` FOREIGN KEY (`PersonId`) REFERENCES `persons` (`PersonId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Person_Department_Departments` FOREIGN KEY (`DepartmentId`) REFERENCES `departments` (`DepartmentId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `samplewebapp_dbo`.`personitem`;
CREATE TABLE  `samplewebapp_dbo`.`personitem` (
  `ItemId` int(10) NOT NULL,
  `PersonId` int(10) NOT NULL,
  PRIMARY KEY (`ItemId`,`PersonId`),
  KEY `FK_PersonItem_Persons` (`PersonId`),
  CONSTRAINT `FK_PersonItem_Items` FOREIGN KEY (`ItemId`) REFERENCES `items` (`ItemId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_PersonItem_Persons` FOREIGN KEY (`PersonId`) REFERENCES `persons` (`PersonId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `samplewebapp_dbo`.`persons`;
CREATE TABLE  `samplewebapp_dbo`.`persons` (
  `PersonId` int(10) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `Email` varchar(150) CHARACTER SET utf8 DEFAULT NULL,
  `Expires` datetime DEFAULT NULL,
  PRIMARY KEY (`PersonId`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;