using Android.App;
using Android.Content;
using Android.OS;
using Android.Print;
// Using the imported package for the toolbar from NuGet
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
// Using the classes from the "Classes" folder in the solution
using RealAppTest7.Classes;
using System.Collections.Generic;

namespace RealAppTest7
{
    [Activity(Label = "DeckDetailsActivity")]
    public class DeckDetailsActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add items from DeckDetails layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        TextView addCardPromptTextView, studyPrompt;
        EditText searchEditText;
        ListView cardListView;

        // Define the list of cards to be shown in the listview as a field so it can be accessed anywhere within this class
        List<Card> cards;
        ArrayAdapter arrayAdapter;

        // Variables carried over by the Intent by the Intent from previous activity
        int selectedDeckID;
        string selectedDeckName;
        int numberofCardsInSelectedDeck;

        // Field used to switch between search and non search mode
        bool searchMode = false;

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the DeckDetails.axml layout
            SetContentView(Resource.Layout.DeckDetails);

            // Get deck ID carried over by the Intent by the Intent from previous screen using the GetStringExtra method
            selectedDeckID = Intent.GetIntExtra("selectedDeckID", 0);
            // Get the name of the deck by accessing the DeckName property of the deck returned by GetDeck
            selectedDeckName = DeckDatabaseHelper.GetDeck(selectedDeckID).DeckName;
            // Calls the NumberOfCardsInSelectedDeck method from the CardDatebaseHelper class to return an integer of the number of cards in the
            // selected deck
            numberofCardsInSelectedDeck = CardDatabaseHelper.NumberOfCardsInSelectedDeck(selectedDeckID);

            // Add toolbar from the layout to a variable so it can be edited here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Set the action bar to the toolbar just created
            SetSupportActionBar(toolbar);

            // Add the textviews from the layout
            addCardPromptTextView = FindViewById<TextView>(Resource.Id.addCardPromptTextView);
            studyPrompt = FindViewById<TextView>(Resource.Id.studyPrompt);
            // Add the search edittext from the layout
            searchEditText = FindViewById<EditText>(Resource.Id.searchEditText);
            // Add the textchanged event to this edittext
            searchEditText.TextChanged += SearchEditText_TextChanged;

            // Calls the private method defined below to show or hide the prompt to create a new card
            ShowOrHideItemsIf0OrNon0CardsInDeck();

            // Add the listview from the layout
            cardListView = FindViewById<ListView>(Resource.Id.cardListView);
            // Add the ItemClick event to the listview
            cardListView.ItemClick += CardListView_ItemClick;
            // Set the field cards to a list of cards in the selected deck using the ReadAll method from CardDatabaseHelper
            cards = CardDatabaseHelper.ReadAll(selectedDeckID);
            // Set the array adapter to a new object using the SimpleListItem1 template
            arrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, cards);
            // Populate the listview
            cardListView.Adapter = arrayAdapter;
        }

        // Called every time the text in SearchEditText changes
        private void SearchEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // Filters the ListView based on the text in the search edittext
            arrayAdapter.Filter.InvokeFilter(searchEditText.Text);
        }

        // Called when an item in the listview is clicked
        private void CardListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Creates an intent object that navigates from this activity to EditCardActivity
            Intent intent = new Intent(this, typeof(EditCardActivity));
            intent.PutExtra("selectedDeckID", selectedDeckID);
            intent.PutExtra("selectedCardID", cards[e.Position].CardID);
            // Start the activity using the intent
            StartActivity(intent);
        }

        // Override the OnRestart method
        protected override void OnRestart()
        {
            base.OnRestart();
            // Populate the listview
            cardListView.Adapter = arrayAdapter;
            // Hides the search edittext
            searchEditText.Visibility = ViewStates.Gone;
            // Calls the private method defined below to show or hide the prompt to create a new card
            ShowOrHideItemsIf0OrNon0CardsInDeck();
        }

        // Override the OnCreateOptionsMenu method to allow the toolbar menu to be populated
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Populate the toolbar menu using deckDetailsMenuIf0Cards.xml or deckDetailsMenuIfNot0Cards.xml depending on the number of cards in
            // the deck
            if (numberofCardsInSelectedDeck == 0)
            {
                // Populates the menu in the toolbar using deckDetailsMenuIf0Cards.xml
                MenuInflater.Inflate(Resource.Menu.deckDetailsMenuIf0Cards, menu);
                // Changes the title in the toolbar to the selected deck name
                SupportActionBar.Title = selectedDeckName;
            }
            else
            {
                if (searchMode)
                {
                    // Populates the menu in the toolbar using deckDetailsSearchMenu.xml
                    MenuInflater.Inflate(Resource.Menu.deckDetailsSearchMenu, menu);
                    // Changes the title in the toolbar to "Search " and the selected deck name
                    SupportActionBar.Title = "Search " + selectedDeckName;
                }
                else
                {
                    // Populates the menu in the toolbar using deckDetailsMenuIfNot0Cards.xml
                    MenuInflater.Inflate(Resource.Menu.deckDetailsMenuIfNot0Cards, menu);
                    // Changes the title in the toolbar to the selected deck name
                    SupportActionBar.Title = selectedDeckName;
                }
            }
            return base.OnCreateOptionsMenu(menu);
        }

        // Override the OnOptionsItemSelected method so code can run when a button in the toolbar is clicked
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Checks which item from the menu was selected by comparing the selected item ID (parameter "item") with the IDs of all items in the
            // toolbar
            switch (item.ItemId)
            {
                // If the study deck item is selected
                case Resource.Id.studyDeck:
                    // Creates an intent object that navigates from this activity to StudyActivity
                    Intent intent1 = new Intent(this, typeof(StudyActivity));
                    intent1.PutExtra("selectedDeckID", selectedDeckID);
                    // Start the activity using the intent
                    StartActivity(intent1);
                    break;
                // If the add card item is selected
                case Resource.Id.addCard:
                    // Creates an intent object that navigates from this activity to AddCardActivity
                    Intent intent2 = new Intent(this, typeof(AddCardActivity));
                    intent2.PutExtra("selectedDeckID", selectedDeckID);
                    // Start the activity using the intent
                    StartActivity(intent2);
                    break;
                // If the search item is selected
                case Resource.Id.search:
                    // Change search mode variable to true so the toolbar menu will change, and so that pressing back will exit search mode
                    searchMode = true;
                    // Make the EditText for searching visible
                    searchEditText.Visibility = ViewStates.Visible;
                    // Show the keyboard
                    ShowKeyboard();
                    // Get focus on the EditText to allow the user to start typing in it without having to click on it
                    searchEditText.RequestFocus();
                    // Invalidate the options menu to force it to run OnCreateOptionsMenu again which will display a different menu
                    InvalidateOptionsMenu();
                    break;
                // If the edit deck name item is selected
                case Resource.Id.editDeckName:
                    // Creates an intent object that navigates from this activity to EditDeckActivity
                    Intent intent3 = new Intent(this, typeof(EditDeckActivity));
                    intent3.PutExtra("selectedDeckID", selectedDeckID);
                    // Start the activity using the intent 
                    StartActivity(intent3);
                    break;
                // If the print item is selected
                case Resource.Id.printDeck:
                    // Create a webview object to hold the HTML file to be printed
                    Android.Webkit.WebView webView = new Android.Webkit.WebView(this);
                    // Create the table that the questions and answers will be in in HTML as a string variable
                    string cardsTable = "<table>";
                    // Loop through each item in the list of cards
                    foreach (var card in cards)
                    {
                        // Add a new table row to the HTML string variable (cardsTable) with two columns for the question and answer
                        cardsTable += "<tr><td>" + card.Question + "</td><td>" + card.Answer + "</td></tr>";
                    }
                    // Close the table tag in HTML
                    cardsTable += "</table>";
                    // Add the rest of the HTML necessary to make the page functional, including some styling for the table to make the border 1
                    // pixel thick, add 5 pixels of padding on the inside of the cells and add spacing between rows
                    string htmlDocument = "<html><style>table td { border: 1px solid black; padding: 5px; } " +
                        "table { border-collapse: separate; border-spacing: 0px 10px; }" +
                        "</style><body>" + cardsTable + "</body></html>";
                    // Load the HTML document into the webview
                    webView.LoadDataWithBaseURL(null, htmlDocument, "text/HTML", "UTF-8", null);
                    // Create a PrintManager object
                    PrintManager printManager = (PrintManager)GetSystemService(PrintService);
#pragma warning disable CS0618 // Type or member is obsolete
                    // Create a PrintDocumentAdapter object using the webview. This version of the CreatePrintDocumentAdapter method is deprecated
                    // but allows compatibility with Android versions below 5
                    PrintDocumentAdapter printDocumentAdapter = webView.CreatePrintDocumentAdapter();
#pragma warning restore CS0618 // Type or member is obsolete
                    // Print the document. The first argument is the name of the print job
                    printManager.Print(selectedDeckName, printDocumentAdapter, null);
                    break;
                // If the notification options item is selected
                case Resource.Id.notificationOptions:
                    // Suppress the warning about the FragmentManager being obselete
#pragma warning disable CS0618 // Type or member is obsolete
                    ServiceDialog serviceDialog = new ServiceDialog(this, selectedDeckID);
                    // Call the show method on the service dialog object to show the dialog box
                    serviceDialog.Show(base.FragmentManager, "dialog");
#pragma warning restore CS0618 // Type or member is obsolete
                    break;
                // If the delete deck item is selected
                case Resource.Id.deleteDeck:
                    // Creates an alertDialog object from the result of the SetUpSingleActionAlertDialog method in RevisionCardsUtility
                    Android.App.AlertDialog alertDialog = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Delete Deck",
                        "Are you sure you want to delete " + selectedDeckName + "? This will also delete all it's cards", "Cancel");
                    // Set the first button to "Delete", run code when the user clicks it
                    alertDialog.SetButton("Delete", (a, b) =>
                    {
                        // If the deck is deleted sucessfully, the Delete method returns true
                        if (DeckDatabaseHelper.Delete(selectedDeckID))
                        {
                            // Display a toast to tell the user that the deck was deleted
                            Toast.MakeText(this, selectedDeckName + " deleted", ToastLength.Short).Show();
                            // Creates an intent object that navigates from this activity to MainActivity
                            Intent intent4 = new Intent(this, typeof(MainActivity));
                            // Start the activity using the intent 
                            StartActivity(intent4);
                        }
                        else
                            // Display a toast to tell the user there was an error and the deck could not be deleted
                            Toast.MakeText(this, "Error: " + selectedDeckName + " could not be deleted", ToastLength.Short).Show();
                    });
                    // Show the alert dialog
                    alertDialog.Show();
                    break;
                // If the cancel item is selected
                case Resource.Id.cancel:
                    ExitSearch();
                    break;
                // If there was an error, and no IDs match, then this is the default case
                default:
                    // Shows the user a toast to say there was an error
                    Toast.MakeText(this, "Error: No option was selected", ToastLength.Short).Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        // Overrides the back button event to control what happens when the system back button is clicked
        public override void OnBackPressed()
        {
            // If the user is in search mode, ie they clicked the search option from the menu
            if (searchMode)
                // Call the ExitSearch method defined below
                ExitSearch();
            else
            {
                // Creates an intent object that navigates from this activity to MainActivity
                Intent intent = new Intent(this, typeof(MainActivity));
                // Start the activity using the intent
                StartActivity(intent);
            }
        }

        // If the user leaves the app by pressing the home or overview button
        protected override void OnUserLeaveHint()
        {
            // Call the method to close the keyboard
            HideKeyboard();
            base.OnUserLeaveHint();
        }

        // Shows or hides the text that prompts the user to create a card based on whether there are any cards in the deck
        private void ShowOrHideItemsIf0OrNon0CardsInDeck()
        {
            // If the number of cards in the selected deck is not 0
            if (CardDatabaseHelper.NumberOfCardsInSelectedDeck(selectedDeckID) == 0)
            {
                addCardPromptTextView.Visibility = ViewStates.Visible;
                studyPrompt.Visibility = ViewStates.Gone;
            }
            else
            {
                addCardPromptTextView.Visibility = ViewStates.Gone;
                studyPrompt.Visibility = ViewStates.Visible;
            }
        }

        // Exists search mode by hiding the EditText and changing the options in the toolbar
        private void ExitSearch()
        {
            // Change search mode variable to false so the toolbar menu will change, and so that pressing back will go back
            searchMode = false;
            // Hide the search EditText
            searchEditText.Visibility = ViewStates.Gone;
            // Hide the keyboard
            HideKeyboard();
            // Invalidate the options menu to force it to run OnCreateOptionsMenu again which will display a different menu
            InvalidateOptionsMenu();
        }

        // Shows the keyboard
        private void ShowKeyboard()
        {
            // Creates an InputMethodManager object
            InputMethodManager inputMethodManager = Application.GetSystemService(InputMethodService) as InputMethodManager;
            // Show the keyboard
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        // Hides the keyboard
        private void HideKeyboard()
        {
            // Creates an InputMethodManager object
            InputMethodManager inputMethodManager = Application.GetSystemService(InputMethodService) as InputMethodManager;
            // Hide the keyboard
            inputMethodManager.HideSoftInputFromWindow(searchEditText.WindowToken, HideSoftInputFlags.None);
        }
    }
}