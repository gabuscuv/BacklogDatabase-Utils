CREATE TABLE "Backlog" (
	"id"	INTEGER NOT NULL UNIQUE,
	"name"	TEXT NOT NULL,
	"plataform"	TEXT,
	"Score"	INTEGER,
	"releaseyear"	INTEGER,
	"nsfw"	INTEGER DEFAULT 0 COLLATE BINARY,
	"status"	TEXT DEFAULT 'Not Started',
	"priority"	INTEGER DEFAULT 5,
	"beaten"	INTEGER DEFAULT 0 COLLATE BINARY,
	"completed"	INTEGER DEFAULT 0 COLLATE BINARY,
	"completedyear"	INTEGER,
	"current_time"	TEXT,
	"min_time"	REAL,
	"max_time"	REAL,
	"gameSeriesID"	INTEGER,
	"playsite"	TEXT,
	"dependence"	INTEGER,
	"when_start"	TEXT,
	"notes"	TEXT,
	PRIMARY KEY("id" AUTOINCREMENT)
)

CREATE TABLE "EGameSeries" (
	"id"	INTEGER NOT NULL UNIQUE,
	"nameSeries"	INTEGER,
	"parentSeries"	INTEGER,
	"company"	INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT)
);

CREATE TABLE "EGameSystem" (
	"id"	INTEGER NOT NULL UNIQUE,
	"name"	TEXT,
	"isPortable"	INTEGER DEFAULT 0,
	"emulable"	INTEGER DEFAULT 0,
	"lowendEmulable"	INTEGER DEFAULT 0,
	"own"	INTEGER DEFAULT 0,
	"isRetro"	INTEGER DEFAULT 1,
	PRIMARY KEY("id" AUTOINCREMENT)
);

CREATE TABLE "GamesID" (
	"id"	INTEGER,
	"igdbID"	INTEGER,
	"SteamID"	INTEGER,
	"psnProfile"	INTEGER,
	"PSStoreID"	INTEGER,
	CONSTRAINT "Backlog" FOREIGN KEY("id") REFERENCES "GamesID",
	PRIMARY KEY("id")
);