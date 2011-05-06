-- Drop
DROP TABLE "main"."PersonItem";
DROP TABLE "main"."Items";
DROP TABLE "main"."Person_Department";
DROP TABLE "main"."Departments";
DROP TABLE "main"."Persons";
DROP TABLE "main"."Categories";

-- Schema
CREATE TABLE "main"."Categories"
(
	"CategoryId" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Name" VARCHAR(50)  NOT NULL
);

CREATE TABLE "main"."Persons"
(
	"PersonId" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Name" VARCHAR(50) NOT NULL,
	"Email" VARCHAR(150),
	"Expires" DATETIME,
	"Active" BOOLEAN NOT NULL,
	"Photo" BLOB, 
	"Photo2" BLOB, 
	"Photo3" BLOB, 
	"MaritalStatus" INT(4),
	"ContractType" INT(4)
);

CREATE TABLE "main"."Items"
(
	"ItemId" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	"Description" VARCHAR(50) NOT NULL,
	"CategoryId" INTEGER NULL,
	FOREIGN KEY("CategoryId") REFERENCES Categories(CategoryId)
);

CREATE TABLE "main"."PersonItem" (
    "ItemId" INTEGER NOT NULL,
    "PersonId" INTEGER NOT NULL,
	FOREIGN KEY("ItemId") REFERENCES Items (ItemId),
	FOREIGN KEY("PersonId") REFERENCES Persons (PersonId),
    PRIMARY KEY ("ItemId", "PersonId")
);

CREATE TABLE "main"."Departments" 
(
  "DepartmentId" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Name" varchar(50) NOT NULL
); 

CREATE TABLE "main"."Person_Department" 
(
  "PersonId" INTEGER NOT NULL,
  "DepartmentId" INTEGER NOT NULL,
  PRIMARY KEY ("PersonId","DepartmentId"),
	FOREIGN KEY("PersonId") REFERENCES Persons (PersonId),
	FOREIGN KEY("DepartmentId") REFERENCES Departments (DepartmentId)
); 

-- Data
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



INSERT INTO "Categories" ("Name")
VALUES ('First category');
INSERT INTO "Categories" ("Name")
VALUES ('Second category');

INSERT INTO "Items" ("Description", "CategoryId")
VALUES ('First item', NULL);
INSERT INTO "Items" ("Description", "CategoryId")
VALUES ('Second item', 1);
INSERT INTO "Items" ("Description", "CategoryId")
VALUES ('Third item', 2);

INSERT INTO "Persons" ("Name", "Email", "Active")
VALUES ('John Doe', 'john@aol.com', 1);
INSERT INTO "Persons" ("Name", "Email", "Active")
VALUES ('Jane Doe', 'jane@aol.com', 1);

INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (1, 1);
INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (2, 2);
INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (3, 2);


INSERT INTO "Person_Department" ("DepartmentId", "PersonId")
VALUES (1, 1);
INSERT INTO "Person_Department" ("DepartmentId", "PersonId")
VALUES (2, 2);
INSERT INTO "Person_Department" ("DepartmentId", "PersonId")
VALUES (3, 2);
INSERT INTO "Person_Department" ("DepartmentId", "PersonId")
VALUES (4, 2);
INSERT INTO "Person_Department" ("DepartmentId", "PersonId")
VALUES (5, 2);
INSERT INTO "Person_Department" ("DepartmentId", "PersonId")
VALUES (5, 1);
