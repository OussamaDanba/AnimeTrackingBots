using System;
using System.Data.SQLite;
using System.IO;

namespace CreateDatabase
{
    class CreateDatabase
    {
        static void Main(string[] args)
        {
            string FileName = "database.sqlite";
            if (!UserConfirmation(FileName))
                return;

            SQLiteConnection Database = GenerateDatabase(FileName);
            PopulateDatabase(Database);
        }

        static bool UserConfirmation(string fileName)
        {
            if (File.Exists(fileName))
            {
                Console.WriteLine("Existing database found.");
                Console.WriteLine("Overwrite existing database? [Y/N]");

                ConsoleKeyInfo Key;
                do
                {
                    Key = Console.ReadKey();

                    if (Key.Key == ConsoleKey.N)
                        return false;
                }
                while (Key.Key != ConsoleKey.Y);
            }

            File.Delete(fileName);
            return true;
        }

        static SQLiteConnection GenerateDatabase(string fileName)
        {
            SQLiteConnection.CreateFile(fileName);
            SQLiteConnection Database = new SQLiteConnection("Data source=" + fileName + ";Version=3;");
            Database.Open();

            // Create database structure
            new SQLiteCommand(@"
                CREATE TABLE ShowTypes (
                    ShowType        TEXT        PRIMARY KEY NOT NULL);

                CREATE TABLE Shows (
                    Id              INTEGER     PRIMARY KEY NOT NULL,
                    EpisodeCount    REAL,
                    ShowType        TEXT        NOT NULL DEFAULT ('Episode') REFERENCES ShowTypes (ShowType),
                    DisplayedTitle  TEXT        NOT NULL,
                    SourceExists    BOOLEAN     NOT NULL CHECK(SourceExists IN (0, 1)));

                CREATE TABLE Episodes (
                    Id              INTEGER     NOT NULL REFERENCES Shows (Id),
                    EpisodeNumber   REAL        NOT NULL,
                    PostURL         TEXT,
                    PRIMARY KEY (Id, EpisodeNumber));

                CREATE TABLE Websites (
                    Website         TEXT        PRIMARY KEY NOT NULL,
                    HTTPSSupport    BOOLEAN     NOT NULL DEFAULT (0) CHECK(HTTPSSupport IN (0, 1)));

                -- Some streaming sites have a slightly different way of doing things thus we introduce a Wildcard column
                CREATE TABLE Streaming (
                    Id              INTEGER     NOT NULL REFERENCES Shows (Id),
                    Website         TEXT        NOT NULL REFERENCES Websites (Website),
                    Source          TEXT        NOT NULL,
                    InternalTitle   TEXT        NOT NULL,
                    TitleURL        TEXT        NOT NULL,
                    Title           TEXT        NOT NULL,
                    InternalOffset  REAL        NOT NULL DEFAULT (0),
                    AKAOffset       REAL        NOT NULL DEFAULT (0),
                    Wildcard        TEXT,
                    PRIMARY KEY (Id, Website));

                CREATE TABLE Subreddits (
                    Id              INTEGER     NOT NULL REFERENCES Shows (Id),
                    Subreddit       TEXT        NOT NULL,
                    PRIMARY KEY (Id, Subreddit));

                CREATE TABLE Keywords (
                    Id              INTEGER     NOT NULL REFERENCES Shows (Id),
                    Keyword         TEXT        NOT NULL,
                    PRIMARY KEY (Id, Keyword));

                CREATE TABLE Information (
                    Id              INTEGER     NOT NULL REFERENCES Shows (Id),
                    Website         TEXT        NOT NULL REFERENCES Websites (Website),
                    Title           TEXT        NOT NULL,
                    TitleURL        TEXT        NOT NULL,
                    PRIMARY KEY (Id, Website, Title));

                -- I hate (reddit) OAuth
                CREATE TABLE User (
                    Username        TEXT        NOT NULL,
                    Password        TEXT        NOT NULL)
                ", Database).ExecuteNonQuery();

            Database.Close();
            return Database;
        }

        // Populate the database with commonly used entries
        static void PopulateDatabase(SQLiteConnection database)
        {
            database.Open();
            
            // Populate the ShowTypes table
            SQLiteCommand InsertShowType = new SQLiteCommand(@"
                INSERT INTO ShowTypes (ShowType) VALUES (@ShowType)
                ", database);

            string[] ShowTypes = { "Episode", "Movie", "Music", "ONA", "OVA", "Special" };
            foreach (string ShowType in ShowTypes)
            {
                InsertShowType.Parameters.AddWithValue("@ShowType", ShowType);
                InsertShowType.ExecuteNonQuery();
            }

            // Populate the Websites table
            SQLiteCommand InsertWebsites = new SQLiteCommand(@"
                INSERT INTO Websites (Website) VALUES (@Website)
                ", database);

            string[] Websites = { "AniDB", "AniList", "Anime News Network", "Anime-Planet", "AnimeLab", "Crunchyroll",
                "DAISUKI", "FUNimation", "Hulu", "Hummingbird", "MyAnimeList", "Netflix", "Viewster", "Wakanim", "Wikipedia"};
            foreach (string Website in Websites)
            {
                InsertWebsites.Parameters.AddWithValue("@Website", Website);
                InsertWebsites.ExecuteNonQuery();
            }

            database.Close();
        }
    }
}
