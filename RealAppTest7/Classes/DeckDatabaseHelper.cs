// Using SQLite namespace from sqlite-net-pcl NuGet package to create the database fields
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RealAppTest7.Classes
{
    // The DeckDatabaseHelper class handles all database logic for Decks
    class DeckDatabaseHelper
    {
        // All non read method will return a boolean to indicate if the operation was sucsessful

        // Sets the database path as a read only field using the private method GetDBPath
        static readonly string dbPath = GetDBPath();

        // This method will insert a record into the database, and return true if it was successfully added
        public static bool Insert(Deck newDeck)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
                // Return true if record is added successfully
                if (conn.Insert(newDeck) != 0)
                    return true;
            // Returns false by default
            return false;
        }

        // Returns a list of all the decks in the database
        public static List<Deck> ReadAll()
        {
            // Create the list of type database object
            List<Deck> decks = new List<Deck>();
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Creates the table if it does not already exist
                conn.CreateTable<Deck>();
                // Set the decks variable to a list of all the records from the deck table
                decks = conn.Table<Deck>().ToList();
            }
            // Return the list of decks
            return decks;
        }

        // Returns a deck object given its DeckID
        public static Deck GetDeck(int selectedDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Queries the Deck table to return the Deck object with an ID matching the ID parameter
                return conn.Table<Deck>().Where(p => p.DeckID == selectedDeckID).SingleOrDefault();
            }
        }

        // Returns the ID of the selected deck, using it's name
        public static int GetSelectedDeckID(string selectedDeckName)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
                // Queries the deck table for the record where the deck name matches the specified deck name,
                // then selects only the first item found, and then selects the deck ID of that object
                return conn.Table<Deck>().Where(p => p.DeckName == selectedDeckName).ToList().First().DeckID;
        }

        // Updates a deck's name using the deckID
        public static bool Update(int selectedDeckID, string newDeckName)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Queries the database for the deck with the selected deck ID (from the parameter selectedDeckID)
                Deck updatedDeck = conn.Table<Deck>().Where(p => p.DeckID == selectedDeckID).SingleOrDefault();
                // Changes the deck name to the new value from (from the parameter newDeckName)
                updatedDeck.DeckName = newDeckName;
                // If the deck is updated sucessfully (update returns an integer)
                if (conn.Update(updatedDeck) != 0)
                    return true;
            }
            // Returns false by default
            return false;
        }
        
        // Another overload for the Update method. Updates a deck's date last studied using the deckID
        public static bool Update(int selectedDeckID, DateTime newDateLastStudied)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Returns the deck object that the user selected from the database using the deckID field
                Deck updatedDeck = conn.Table<Deck>().Where(p => p.DeckID == selectedDeckID).SingleOrDefault();
                // Change the date last studied field
                updatedDeck.DateLastStudied = newDateLastStudied;
                // Returns true if the deck is updated successfully
                if (conn.Update(updatedDeck) != 0)
                    return true;
            }
            // Returns false by default
            return false;
        }

        // Another overload for the Update method. Updates a deck's date last studied using the deckID
        public static bool Update(int selectedDeckID, bool neverStudied)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Returns the deck object that the user selected from the database using the deckID field
                Deck updatedDeck = conn.Table<Deck>().Where(p => p.DeckID == selectedDeckID).SingleOrDefault();
                // Change the date last studied field
                updatedDeck.NeverStudied = neverStudied;
                // Returns true if the deck is updated successfully
                if (conn.Update(updatedDeck) != 0)
                    return true;
            }
            // Returns false by default
            return false;
        }

        // Another overload for the Update method. Updates a deck's notification settings
        public static bool Update(int selectedDeckID, bool notificationsEnabled, int numberOfDaysUntilReminder)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Returns the deck object that the user selected from the database using the deckID field
                Deck updatedDeck = conn.Table<Deck>().Where(p => p.DeckID == selectedDeckID).SingleOrDefault();
                // Change the date last studied field
                updatedDeck.NotificationsEnabled = notificationsEnabled;
                updatedDeck.NumberOfDaysUntilReminder = numberOfDaysUntilReminder;
                // Returns true if the deck is updated successfully
                if (conn.Update(updatedDeck) != 0)
                    return true;
            }
            // Returns false by default
            return false;
        }

        // Deletes a deck and all the cards in it using the deckID
        public static bool Delete(int selectedDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // If the deck is deleted successfully
                if (conn.Delete<Deck>(selectedDeckID) != 0)
                {
                    // Calls DeleteCardsInDeck from CardDatabaseHelper, and provides the selectedDeckID
                    CardDatabaseHelper.DeleteCardsInDeck(selectedDeckID);
                    // Returns true
                    return true;
                }
            }
            // Returns false by default
            return false;
        }

        // Returns the number of decks as an integer
        public static int GetNumberOfDecks()
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Counts the number of records in the table
                int numberofDecks = conn.Table<Deck>().Count();
                // Returns the number of decks
                return numberofDecks;
            }
        }

        // Returns true if a deck alreay exists with the given name
        public static bool DeckAlreadyExists(string newDeckName)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Returns a list of decks from the table that have the same name as the newDeckName
                List<Deck> listOfDecks = conn.Table<Deck>().Where(p => p.DeckName == newDeckName).ToList();
                // Return true if the list is not empty (ie there are no decks with the same name)
                if (listOfDecks.Count() != 0)
                    return true;
            }
            // Returns false by default
            return false;
        }

        // Returns the deck database path
        private static string GetDBPath()
        {
            // Name of the database
            string dbName = "Deck.sqlite";
            // The folder path, which is the user's personal folder
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            // Return the name and path combined
            return Path.Combine(folderPath, dbName);
        }
    }
}