// Using SQLite namespace from sqlite-net-pcl NuGet package to create the database fields
using SQLite;

namespace RealAppTest7.Classes
{
    // The Card class defines a template for Card objects that will be stored in the database
    class Card
    {
        // CardId is the primary key and will autoincrement
        [PrimaryKey, AutoIncrement] public int CardID { get; set; }
        // DeckID is a foreign key to the Deck table
        public int DeckID { get; set; }
        // The question that the user has to answer
        public string Question { get; set; }
        // The answer to the question
        public string Answer { get; set; }

        // Override the ToString method to control what is returned. In this case it will define what values to display in each listview item
        public override string ToString()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // Returns the formatted string containing the question, formatted using HTML
            return Android.Text.Html.FromHtml(string.Format($"{Question}")).ToString();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}