using Android.App;
using Android.Content;
using Android.OS;
// Using the imported package for the toolbar from NuGet
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
// Using the classes from the "Classes" folder in the solution
using RealAppTest7.Classes;
using System.Collections.Generic;

namespace RealAppTest7
{
    [Activity(Label = "MoveCardActivity")]
    public class MoveCardActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add items from Main layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        TextView addDeckPromptTextView;
        ListView deckListView;

        // Define the list of decks to be shown in the listview as a field so it can be accessed anywhere within this class
        List<Deck> decks;

        // Variables carried over by the Intent from previous activity
        int selectedDeckID;
        string selectedDeckName;
        int selectedCardID;
        string selectedCardQuestion;
        string selectedCardAnswer;

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the DeckDetails.axml layout
            SetContentView(Resource.Layout.Main);

            // Get deck ID carried over by the Intent by the Intent from previous screen using the GetStringExtra method
            selectedDeckID = Intent.GetIntExtra("selectedDeckID", 0);
            // Get the name of the deck by accessing the DeckName property of the deck returned by GetDeck
            selectedDeckName = DeckDatabaseHelper.GetDeck(selectedDeckID).DeckName;
            // Get card ID carried over by the Intent by the Intent from previous screen using the GetIntExtra method
            selectedCardID = Intent.GetIntExtra("selectedCardID", 0);
            // Get the question of the card by accessing the Question property of the card returned by GetCard
            selectedCardQuestion = CardDatabaseHelper.GetCard(selectedCardID).Question;
            // Get the answer of the card by accessing the Answer property of the card returned by GetCard
            selectedCardAnswer = CardDatabaseHelper.GetCard(selectedCardID).Answer;

            // Add toolbar from the layout to a variable so it can be edited here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Set the action bar to the toolbar just created
            SetSupportActionBar(toolbar);
            // Set the title of the toolbar
            SupportActionBar.Title = "Move card to...";

            // Add the textview from the layout
            addDeckPromptTextView = FindViewById<TextView>(Resource.Id.addDeckPromptTextView);
            // Hide the textview since it is only needed in MainActivity not MoveCardActivity
            addDeckPromptTextView.Visibility = ViewStates.Gone;

            // Add the listview from the layout to a variable so it can be edited here
            deckListView = FindViewById<ListView>(Resource.Id.deckListView);
            // Add the ItemClick event handler to the listview
            deckListView.ItemClick += DeckListView_ItemClick;
            // Populate the listview by calling the private method below
            PopulateListView();
        }

        // Called when an item in the listview is clicked
        private void DeckListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Create an alert dialog object using the builder
            Android.App.AlertDialog alertDialog = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Move card",
                "Are you sure you want to move this card to " + decks[e.Position].DeckName + "?", "Cancel");
            // Add a button to the dialog box
            alertDialog.SetButton("Move", (a, b) =>
            {
                // If the Update method from CardDatabaseHelper returns true
                if (CardDatabaseHelper.Update(selectedCardID, decks[e.Position].DeckID))
                {
                    // Display a toast to tell the user that the card was moved
                    Toast.MakeText(this, "Card moved to " + selectedDeckName, ToastLength.Short).Show();
                    // Creates an intent object that navigates from this activity to DeckDetailsActivity
                    Intent intent = new Intent(this, typeof(DeckDetailsActivity));
                    intent.PutExtra("selectedDeckID", selectedDeckID);
                    // Start the activity using the intent object
                    StartActivity(intent);
                }
                else
                    // Display a toast to tell the user that there was an error and the card could not be moved
                    Toast.MakeText(this, "Error: Card could not be moved", ToastLength.Short).Show();
            });
            // Show the alert dialog
            alertDialog.Show();
        }

        // Override the OnCreateOptionsMenu method to allow the toolbar menu to be populated
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Populates the menu in the toolbar using moveCardMenu.xml
            MenuInflater.Inflate(Resource.Menu.moveCardMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        // Override the OnOptionsItemSelected method so code can run when a button in the toolbar is clicked
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Checks which item from the menu was selected by comparing the selected item ID (parameter "item") with the IDs of all items in
            // the toolbar
            switch (item.ItemId)
            {
                // If the cancel item is selected
                case Resource.Id.cancel:
                    NavigateBack();
                    break;
                // If there was an error, and no IDs match, then this is the default case
                default:
                    // Shows the user a toast to say there was an error
                    Toast.MakeText(this, "Error: No option was selected", ToastLength.Short).Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        // Overrides the back button event
        public override void OnBackPressed()
        {
            NavigateBack();
            base.OnBackPressed();
        }

        // Navigates the user back to the EditCard activity
        private void NavigateBack()
        {
            // Creates an intent object that navigates from this activity to EditCardActivity
            Intent intent = new Intent(this, typeof(EditCardActivity));
            intent.PutExtra("selectedDeckID", selectedDeckID);
            intent.PutExtra("selectedCardID", selectedCardID);
            // Start the activity using the intent object
            StartActivity(intent);
        }

        // Populates the listview with records from the decks database, but without the deck the card is currently in
        private void PopulateListView()
        {
            // Set the list of decks to the records in the database using the read method from DatabaseHelper
            decks = DeckDatabaseHelper.ReadAll();
            // Loop through each item in the decks list and remove the selected deck (ie the deck that the selected card is already in)
            for (int i = 0; i < decks.Count; i++)
            {
                // If the ID of current deck object in the decks list is the same as the selected deck's ID (ie the deck that the card is already in)
                if (decks[i].DeckID == selectedDeckID)
                    // Remove that deck from the decks list so that it will not be displayed in the listview
                    decks.Remove(decks[i]);
            }
            // Set the array adapter to a new object using the SimpleListItem1 template
            ArrayAdapter arrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, decks);
            // Populate the listview
            deckListView.Adapter = arrayAdapter;
        }
    }
}