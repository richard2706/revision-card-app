// Using SQLite namespace from sqlite-net-pcl NuGet package to create the database fields
using SQLite;
using System;

namespace RealAppTest7.Classes
{
    // The Deck class defines a template for Deck objects that will be stored in the database
    class Deck
    {
        // DeckID is the primary key and will autoincrement
        [PrimaryKey, AutoIncrement] public int DeckID { get; set; }
        // Name of the deck
        public string DeckName { get; set; }
        // Date that the user last studied the selected deck
        public DateTime DateLastStudied { get; set; }
        // True if the deck was never studied
        public bool NeverStudied { get; set; }
        // True if the user has enabled notifications
        public bool NotificationsEnabled { get; set; }
        // Number of days that the user specified until reminder sent
        public int NumberOfDaysUntilReminder { get; set; }

        // Override the ToString method so a custom format can be used when displaying a record from this database. It returns the deck name and date
        // last studied in the specified format
        public override string ToString()
        {
            // If the NeverStudied property for the current Deck is true
            if (NeverStudied)
                // If the deck has never been studied, the last studied section will show "never"
                return string.Format($"{DeckName} | Last studied: never");
            else
                // If the deck has been studied before, the last studied section will show the corresponding date and time
                return string.Format($"{DeckName} | Last studied: {DateLastStudied}");
        }
    }
}