﻿-- Drop
IF OBJECT_ID('dbo.[PersonItem]','U') IS NOT NULL
DROP TABLE PersonItem;

IF OBJECT_ID('dbo.[Items]','U') IS NOT NULL
DROP TABLE Items;

IF OBJECT_ID('dbo.[Categories]','U') IS NOT NULL
DROP TABLE Categories;

IF OBJECT_ID('dbo.[Person_Department]','U') IS NOT NULL
DROP TABLE Person_Department;

IF OBJECT_ID('dbo.[Departments]','U') IS NOT NULL
DROP TABLE Departments;

IF OBJECT_ID('dbo.[Persons]','U') IS NOT NULL
DROP TABLE Persons;

GO

-- Schema
CREATE TABLE Categories
(
	CategoryId INT NOT NULL PRIMARY KEY IDENTITY,
	Name NVARCHAR(50)  NOT NULL
);

CREATE TABLE Persons
(
	PersonId INT NOT NULL PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL,
	Email NVARCHAR(150),
	Expires DATETIME,
	Active BIT NOT NULL,
	Photo VARBINARY(MAX),
	MaritalStatus TINYINT,
	ContractType TINYINT
);

CREATE TABLE Items
(
	ItemId INT NOT NULL PRIMARY KEY IDENTITY,
	Description NVARCHAR(50) NOT NULL,
	CategoryId INT NULL,
	FOREIGN KEY(CategoryId) REFERENCES Categories (CategoryId)
);

CREATE TABLE PersonItem (
    ItemId INT NOT NULL,
    PersonId INT NOT NULL,
	FOREIGN KEY(ItemId) REFERENCES Items (ItemId),
	FOREIGN KEY(PersonId) REFERENCES Persons (PersonId),
    PRIMARY KEY (ItemId, PersonId)
);

CREATE TABLE  Departments (
  DepartmentId int NOT NULL PRIMARY KEY IDENTITY,
  Name varchar(50) NOT NULL
) 

CREATE TABLE Person_Department (
  PersonId int NOT NULL,
  DepartmentId int NOT NULL,
  PRIMARY KEY (PersonId,DepartmentId),
	FOREIGN KEY(PersonId) REFERENCES Persons (PersonId),
	FOREIGN KEY(DepartmentId) REFERENCES Departments (DepartmentId),
) 

GO

-- Data
INSERT INTO Categories (Name)
VALUES ('First category');
INSERT INTO Categories (Name)
VALUES ('Second category');

INSERT INTO Items (Description, CategoryId)
VALUES ('First item', NULL);
INSERT INTO Items (Description, CategoryId)
VALUES ('Second item', 1);
INSERT INTO Items (Description, CategoryId)
VALUES ('Third item', 2);

INSERT INTO "Departments" ("Name")
VALUES ('Department 1');
INSERT INTO "Departments" ("Name")
VALUES ('Department 2');
INSERT INTO "Departments" ("Name")
VALUES ('Department 3');
INSERT INTO "Departments" ("Name")
VALUES ('Department 4');
INSERT INTO "Departments" ("Name")
VALUES ('Department 5');

INSERT INTO Persons (Name, Email, Active)
VALUES ('John Doe', 'john@aol.com', 1);
INSERT INTO Persons (Name, Email, Active)
VALUES ('Jane Doe', 'jane@aol.com', 1);

INSERT INTO PersonItem (ItemId, PersonId)
VALUES (1, 1);
INSERT INTO PersonItem (ItemId, PersonId)
VALUES (2, 2);
INSERT INTO PersonItem (ItemId, PersonId)
VALUES (3, 2);

INSERT INTO Person_Department (DepartmentId, PersonId)
VALUES (1, 1);
INSERT INTO Person_Department (DepartmentId, PersonId)
VALUES (2, 2);
INSERT INTO Person_Department (DepartmentId, PersonId)
VALUES (3, 2);
INSERT INTO Person_Department (DepartmentId, PersonId)
VALUES (4, 2);
INSERT INTO Person_Department (DepartmentId, PersonId)
VALUES (5, 2);
INSERT INTO Person_Department (DepartmentId, PersonId)
VALUES (5, 1);
