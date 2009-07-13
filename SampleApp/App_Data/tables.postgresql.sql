-- Drop
DROP TABLE "PersonItem";
DROP TABLE "Items";
DROP TABLE "Persons";
DROP TABLE "Categories";

-- Schema
CREATE TABLE "Categories"
(
	"CategoryId" serial NOT NULL PRIMARY KEY,
	"Name" VARCHAR(50)  NOT NULL
);
ALTER TABLE "Categories" OWNER TO "teste";

CREATE TABLE "Persons"
(
	"PersonId" serial NOT NULL PRIMARY KEY,
	"Name" VARCHAR(50) NOT NULL,
	"Email" VARCHAR(150),
	"Expires" TIMESTAMP,
	"Active" BOOLEAN NOT NULL,
	"Photo" BYTEA,
	"MaritalStatus" SMALLINT
);
ALTER TABLE "Persons" OWNER TO "teste";

CREATE TABLE "Items"
(
	"ItemId" serial NOT NULL PRIMARY KEY,
	"Description" VARCHAR(50) NOT NULL,
	"CategoryId" INTEGER NULL,
	FOREIGN KEY("CategoryId") REFERENCES "Categories" ("CategoryId")
);
ALTER TABLE "Items" OWNER TO "teste";

CREATE TABLE "PersonItem" (
    "ItemId" INTEGER NOT NULL,
    "PersonId" INTEGER NOT NULL,
	FOREIGN KEY("ItemId") REFERENCES "Items" ("ItemId"),
	FOREIGN KEY("PersonId") REFERENCES "Persons" ("PersonId"),
    PRIMARY KEY ("ItemId", "PersonId")
);
ALTER TABLE "PersonItem" OWNER TO "teste";


-- Data
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
VALUES ('John Doe', 'john@aol.com', true);
INSERT INTO "Persons" ("Name", "Email", "Active")
VALUES ('Jane Doe', 'jane@aol.com', true);

INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (1, 1);
INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (2, 2);
INSERT INTO "PersonItem" ("ItemId", "PersonId")
VALUES (3, 2);