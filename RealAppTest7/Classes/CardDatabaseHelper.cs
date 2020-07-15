// Using SQLite namespace from sqlite-net-pcl NuGet package to create the database fields
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RealAppTest7.Classes
{
    // The CardDatabaseHelper class handles all database logic for Cards
    class CardDatabaseHelper
    {
        // All non read methods return a boolean value that indicates if the operation was successful

        // Sets the database path as a read only field using the private method GetDBPath
        static readonly string dbPath = GetDBPath();

        // Inserts a card object into the database. The data parameter is the card object being passed in
        public static bool Insert(Card newCard)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
                // If the data is inserted successfully, return true
                if (conn.Insert(newCard) != 0)
                    return true;
            // Return false by default
            return false;
        }

        // Returns a list of all cards in the selected deck
        public static List<Card> ReadAll(int selectedDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Creates the table if it does not already exist
                conn.CreateTable<Card>();
                // List of cards in the selected deck by comparing the given deckID with the deckID of the card
                List<Card> cardsList = conn.Table<Card>().Where(p => p.DeckID == selectedDeckID).ToList();
                // Returns the list of cards
                return cardsList;
            }
        }

        // Returns the card object given its ID
        public static Card GetCard(int selectedCardID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
                // Queries the database for the card object that has the same ID as the parameter ID
                return conn.Table<Card>().Where(p => p.CardID == selectedCardID).SingleOrDefault();
        }

        // Updates the card's question and answer using the cardID
        public static bool Update(int selectedCardID, string newQuestion, string newAnswer)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Gets the card from the table using the selectedCardID parameter
                Card selectedCard = conn.Table<Card>().Where(p => p.CardID == selectedCardID).SingleOrDefault();
                // Change the question field
                selectedCard.Question = newQuestion;
                // Change the answer field
                selectedCard.Answer = newAnswer;
                // If the card is updated successfully, return true
                if (conn.Update(selectedCard) != 0)
                    return true;
            }
            // Return false by default
            return false;
        }

        // Another overload for Update. Moves the selected card to a different deck by changing the deckID, given the cardID
        public static bool Update(int selectedCardID, int newDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Gets the card from the table using the selectedCardID parameter
                Card selectedCard = conn.Table<Card>().Where(p => p.CardID == selectedCardID).SingleOrDefault();
                // Change the DeckID field
                selectedCard.DeckID = newDeckID;
                // Returns true if the cards is updated successfully
                if (conn.Update(selectedCard) != 0)
                    return true;
            }
            // Return false by default
            return false;
        }

        // Deletes a specific card using the cardID
        public static bool DeleteCard(int cardID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Delete the card from the table using the cardID
                if (conn.Delete<Card>(cardID) != 0)
                    // If the delete method does not return 0, then this method will return true
                    return true;
            }
            // Return false by default
            return false;
        }

        // Deletes all cards in the specified deck using the DeckID
        public static bool DeleteCardsInDeck(int selectedDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Query the table for a list of cards in the selected deck by comparing deckIDs
                var query = conn.Table<Card>().Where(p => p.DeckID == selectedDeckID);
                // Delete each item returned from the query (each card in the selected deck)
                foreach (Card item in query)
                    // Call the Delete method
                    conn.Delete(item);
            }
            // Return false by default
            return false;
        }

        // Checks if there is a card with the same question in the same deck given the deckID and question
        public static bool CardQuestionAlreadyExists(int selectedDeckID, string newCardQuestion)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Get a list of cards that are in the selected deck and have the same question as the newCardQuestion parameter
                List<Card> listOfCards = conn.Table<Card>().Where(p => p.Question == newCardQuestion && p.DeckID == selectedDeckID).ToList();
                // If the list is empty (ie no cards in the same deck with the same question), return true
                if (listOfCards.Count() != 0)
                    return true;
            }
            // Return false by default
            return false;
        }

        // Returns a random card from the specified deck
        public static Card GetRandomCard(int selectedDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Get the current number of cards in the selected deck
                int cardsListLength = NumberOfCardsInSelectedDeck(selectedDeckID);
                // Create an instance of the Random class to create a random number
                Random random = new Random();
                // Generate a random number between 0 and the number of cards in the deck that corresponds to an index of items in the database
                int skipInteger = random.Next(cardsListLength);
                // Query the database to return one card selected after skipping a random (from skipInteger variable) number of records
                Card randomCard = (from p in conn.Table<Card>() where p.DeckID == selectedDeckID select p).Skip(skipInteger).Take(1).ToList()
                    .First(p => p.DeckID == selectedDeckID);
                // Return the card
                return randomCard;
            }
        }

        // Returns a random card from the specified deck
        public static Card GetRandomCard(int selectedDeckID, int previousCardID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // List of cards in the selected deck
                List<Card> cardsInSelectedDeck = ReadAll(selectedDeckID);
                // Remove the perviously displayed card
                cardsInSelectedDeck.Remove(GetCard(previousCardID));
                // Get the current number of cards in the selected deck
                int cardsListLength = NumberOfCardsInSelectedDeck(selectedDeckID);
                // Create an instance of the Random class to create a random number
                Random random = new Random();
                // Generate a random number between 0 and the number of cards in the deck that corresponds to an index of items in the database
                int skipInteger = random.Next(cardsListLength);
                // Query the database to return one card selected after skipping a random (from skipInteger variable) number of records
                return cardsInSelectedDeck.Skip(skipInteger).Take(1).ToList().First();
            }
        }

        // Returns the number of cards in the specified deck
        public static int NumberOfCardsInSelectedDeck(int selectedDeckID)
        {
            // Estabish connection to the local database using the dbPath
            using (var conn = new SQLiteConnection(dbPath))
            {
                // Creates the table if it does not already exist
                conn.CreateTable<Card>();
                // Query the database for number cards in the selected deck
                int numberOfCards = conn.Table<Card>().Where(p => p.DeckID == selectedDeckID).Count();
                // Return number of cards in the deck
                return numberOfCards;
            }
        }

        // Returns the card database path
        private static string GetDBPath()
        {
            // Name of the database
            string dbName = "Card.sqlite";
            // The folder path, which is the user's personal folder
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            // Return the name and path combined
            return Path.Combine(folderPath, dbName);
        }
    }
}