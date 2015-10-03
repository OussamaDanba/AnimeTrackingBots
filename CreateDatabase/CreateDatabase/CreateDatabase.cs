using System;
using System.Data.SQLite;
using System.IO;

namespace CreateDatabase
{
    class CreateDatabase
    {
        static void Main(string[] args)
        {
            string fileName = "database.sqlite";
            if (!UserConfirmation(fileName))
                return;

            SQLiteConnection database = CreateDB(fileName);
            PopulateDB(database);
        }

        static bool UserConfirmation(string fileName)
        {
            if (File.Exists(fileName))
            {
                Console.WriteLine("Existing database found.");
                Console.WriteLine("Overwrite existing database? [Y/N]");

                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey();

                    if (key.Key == ConsoleKey.N)
                        return false;
                }
                while (key.Key != ConsoleKey.Y);
            }

            File.Delete(fileName);
            return true;
        }

        static SQLiteConnection CreateDB(string fileName)
        {
            SQLiteConnection.CreateFile(fileName);
            SQLiteConnection database = new SQLiteConnection("Data source=" + fileName);
            database.Open();

            // Create database structure
            new SQLiteCommand(@"
                CREATE TABLE ShowTypes (
                    ShowType        TEXT        PRIMARY KEY NOT NULL);

                CREATE TABLE Shows (
                    ID              INTEGER     PRIMARY KEY NOT NULL,
                    EpisodeCount    REAL,
                    ShowType        TEXT        NOT NULL DEFAULT ('Episode') REFERENCES ShowTypes (ShowType),
                    DisplayedTitle  TEXT        NOT NULL,
                    SourceExists    BOOLEAN     NOT NULL CHECK(SourceExists IN (0, 1)));

                CREATE TABLE Episodes (
                    ID              INTEGER     NOT NULL REFERENCES Shows (ID),
                    EpisodeNumber   REAL        NOT NULL,
                    PostURL         TEXT,
                    PRIMARY KEY (ID, EpisodeNumber));

                CREATE TABLE Websites (
                    Website         TEXT        PRIMARY KEY NOT NULL);

                CREATE TABLE Streaming (
                    ID              INTEGER     NOT NULL REFERENCES Shows (ID),
                    Website         TEXT        NOT NULL REFERENCES Websites (Website),
                    Source          TEXT        NOT NULL,
                    InternalTitle   TEXT        NOT NULL,
                    BaseURL         TEXT        NOT NULL,
                    Title           TEXT        NOT NULL,
                    InternalOffset  REAL        NOT NULL DEFAULT (0),
                    AKAOffset       REAL        NOT NULL DEFAULT (0),
                    PRIMARY KEY (ID, Website));

                CREATE TABLE Subreddits (
                    ID              INTEGER     NOT NULL REFERENCES Shows (ID),
                    Subreddit       TEXT        NOT NULL,
                    PRIMARY KEY (ID, Subreddit));

                CREATE TABLE Keywords (
                    ID              INTEGER     NOT NULL REFERENCES Shows (ID),
                    Keyword         TEXT        NOT NULL,
                    PRIMARY KEY (ID, Keyword));

                CREATE TABLE Information (
                    ID              INTEGER     NOT NULL REFERENCES Shows (ID),
                    Website         TEXT        NOT NULL REFERENCES Websites (Website),
                    Title           TEXT        NOT NULL,
                    BaseURL         TEXT        NOT NULL,
                    PRIMARY KEY (ID, Website, Title));

                -- I hate (reddit) OAuth
                CREATE TABLE User (
                    ClientID        TEXT        NOT NULL,
                    ClientSecret    TEXT        NOT NULL,
                    RedirectURI     TEXT        NOT NULL,
                    Username        TEXT        NOT NULL,
                    Password        TEXT        NOT NULL)
                ", database).ExecuteNonQuery();

            database.Close();
            return database;
        }

        // Populate the database with commonly used entries
        static void PopulateDB(SQLiteConnection database)
        {
            database.Open();
            
            // Populate the ShowTypes table
            SQLiteCommand insertShowType = new SQLiteCommand(@"
                INSERT INTO ShowTypes (ShowType) VALUES (@ShowType)
                ", database);

            string[] ShowTypes = { "Episode", "Movie", "Music", "ONA", "OVA", "Special" };
            foreach (string ShowType in ShowTypes)
            {
                insertShowType.Parameters.AddWithValue("@ShowType", ShowType);
                insertShowType.ExecuteNonQuery();
            }

            // Populate the Websites table
            SQLiteCommand insertWebsites = new SQLiteCommand(@"
                INSERT INTO Websites (Website) VALUES (@Website)
                ", database);

            string[] Websites = { "AniDB", "AniList", "Anime News Network", "Anime-Planet", "AnimeLab", "Crunchyroll",
                "DAISUKI", "FUNimation", "Hulu", "Hummingbird", "MyAnimeList", "Netflix", "Viewster", "Wakanim"};
            foreach (string Website in Websites)
            {
                insertWebsites.Parameters.AddWithValue("@Website", Website);
                insertWebsites.ExecuteNonQuery();
            }

            database.Close();
        }
    }
}
