-- Drop
DROP TABLE "TESTE"."PersonItem";
DROP TABLE "TESTE"."Items";
DROP TABLE "TESTE"."Persons";
DROP TABLE "TESTE"."Categories";

DROP SEQUENCE "TESTE"."CategoriesSequence";
DROP SEQUENCE "TESTE"."PersonsSequence";
DROP SEQUENCE "TESTE"."ItemsSequence";

-- Schema
CREATE TABLE "TESTE"."Categories"
(
	"CategoryId" NUMBER(8) NOT NULL PRIMARY KEY,
	"Name" NVARCHAR2(50)  NOT NULL
);

CREATE SEQUENCE "TESTE"."CategoriesSequence"
INCREMENT BY 1;

CREATE TABLE "TESTE"."Persons"
(
	"PersonId" NUMBER(8) NOT NULL PRIMARY KEY,
	"Name" NVARCHAR2(50) NOT NULL,
	"Email" NVARCHAR2(150),
	"Expires" TIMESTAMP(6),
	"Active" NUMBER(1) NOT NULL,
	"Photo" BLOB,
	"MaritalStatus" NUMBER(1)
);

CREATE SEQUENCE "TESTE"."PersonsSequence"
INCREMENT BY 1;

CREATE TABLE "TESTE"."Items"
(
	"ItemId" NUMBER(8) NOT NULL PRIMARY KEY,
	"Description" NVARCHAR2(50) NOT NULL,
	"CategoryId" NUMBER(8) NULL,
	FOREIGN KEY("CategoryId") REFERENCES "Categories" ("CategoryId")
);

CREATE SEQUENCE "TESTE"."ItemsSequence"
INCREMENT BY 1;

CREATE TABLE "TESTE"."PersonItem" (
    "ItemId" NUMBER(8) NOT NULL,
    "PersonId" NUMBER(8) NOT NULL,
	FOREIGN KEY("ItemId") REFERENCES "Items" ("ItemId"),
	FOREIGN KEY("PersonId") REFERENCES "Persons" ("PersonId"),
    PRIMARY KEY ("ItemId", "PersonId")
);


-- Data
INSERT INTO "Categories"
VALUES ("CategoriesSequence".NEXTVAL, 'First category');
INSERT INTO "Categories"
VALUES ("CategoriesSequence".NEXTVAL, 'Second category');

INSERT INTO "Items"
VALUES ("ItemsSequence".NEXTVAL, 'First item', NULL);
INSERT INTO "Items"
VALUES ("ItemsSequence".NEXTVAL, 'Second item', 1);
INSERT INTO "Items"
VALUES ("ItemsSequence".NEXTVAL, 'Third item', 2);

INSERT INTO "Persons" ("PersonId", "Name", "Email", "Active")
VALUES ("PersonsSequence".NEXTVAL, 'John Doe', 'john@aol.com', 1);
INSERT INTO "Persons" ("PersonId", "Name", "Email", "Active")
VALUES ("PersonsSequence".NEXTVAL, 'Jane Doe', 'jane@aol.com', 1);

INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (1, 1);
INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (2, 2);
INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (3, 2);