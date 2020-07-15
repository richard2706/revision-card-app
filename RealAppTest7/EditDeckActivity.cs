using Android.App;
using Android.Content;
using Android.OS;
// Using the imported package for the toolbar from NuGet
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
// Using the classes from the "Classes" folder in the solution
using RealAppTest7.Classes;

namespace RealAppTest7
{
    [Activity(Label = "EditDeckActivity")]
    public class EditDeckActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add items from AddDeck layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        EditText deckNameEditText;

        // Variables carried over by the Intent by the Intent from previous activity
        string selectedDeckName;
        int selectedDeckID;

        // String value used to store the new name of the deck
        string newDeckName;

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the AddDeck.axml layout
            SetContentView(Resource.Layout.AddDeck);

            // Add toolbar from the layout to a variable so it can be edited here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Set the action bar to the toolbar just created
            SetSupportActionBar(toolbar);
            // Set the title of the toolbar
            SupportActionBar.Title = "Edit Deck";

            // Get variables carried over by the Intent by the Intent from previous screen using the GetStringExtra method (or corrosponding data
            // type)
            selectedDeckID = Intent.GetIntExtra("selectedDeckID", 0);
            // Get the name of the deck by accessing the DeckName property of the deck returned by GetDeck
            selectedDeckName = DeckDatabaseHelper.GetDeck(selectedDeckID).DeckName;

            // Add the EditText from the layout as a variable
            deckNameEditText = FindViewById<EditText>(Resource.Id.deckNameEditText);
            // Set the text of the EditText to the value from the database
            deckNameEditText.Text = selectedDeckName;

            // Open the keyboard (so the user doesn't have to click on the EditText first)
            ShowKeyboard();
        }

        // Override the OnRestart method
        protected override void OnRestart()
        {
            // Shows the keyboard
            ShowKeyboard();
            base.OnRestart();
        }

        // Override the OnCreateOptionsMenu method to allow the toolbar menu to be populated
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Populates the menu in the toolbar using editDeckMenu.xml
            MenuInflater.Inflate(Resource.Menu.editDeckMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        // Override the OnOptionsItemSelected method so code can run when a button in the toolbar is clicked
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Set variable for contents of the EditText
            newDeckName = deckNameEditText.Text;

            // Checks which item from the menu was selected by comparing the selected item ID (parameter "item") with the IDs of all items in the
            // toolbar
            switch (item.ItemId)
            {
                // If the save deck item is selected
                case Resource.Id.saveDeck:
                    // If the deck name EditText is empty
                    if (newDeckName == "")
                        // Calls the method to show a dialog box with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Empty Textbox",
                            "Please fill out the deck name textbox", "OK").Show();
                    // If the deck name is the same as before
                    else if (newDeckName == selectedDeckName)
                    {
                        // Display a toast to tell the user that the deck was not updated
                        Toast.MakeText(this, "Deck not updated", ToastLength.Short).Show();
                        NavigateBack();
                    }
                    // If the deck with the same name already exists. Uses the DeckAlreadyExists method in the DeckDatabaseHelper class
                    else if (DeckDatabaseHelper.DeckAlreadyExists(newDeckName))
                        // Calls the method to show a dialog box with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Deck Already Exists",
                            "Another deck already has the name " + newDeckName, "OK").Show();
                    // After all validation checks are successful, the deck will be updated in the database
                    else
                    {
                        // If the deck is aded to the database sucessfully (Update method returns a boolean)
                        if (DeckDatabaseHelper.Update(selectedDeckID, newDeckName))
                        {
                            HideKeyboard();
                            // Display a toast to tell the user that the deck was updated
                            Toast.MakeText(this, "Deck updated", ToastLength.Short).Show();
                            // Creates an intent object that navigates from this activity to DeckDetailsActivity
                            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
                            // Carry forward the selected deck id and name variables
                            intent.PutExtra("selectedDeckID", selectedDeckID);
                            // Start the activity using the intent object
                            StartActivity(intent);
                        }
                        else
                            // Display a toast to tell the user that there was an error and the deck was not updated
                            Toast.MakeText(this, "Error: Could not update deck", ToastLength.Short).Show();
                    }
                    break;
                // If the cancel item is selected
                case Resource.Id.cancel:
                    CancelEditDeck();
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
            CancelEditDeck();
        }

        // Asks user for conformation (if EditTexts are not empty) to cancel editing the deck. This method is used instead of having the same
        // code in two places
        private void CancelEditDeck()
        {
            // Value in the EditText
            newDeckName = deckNameEditText.Text;
            // If the deck name is not the same as before
            if (newDeckName != selectedDeckName)
            {
                // Creates an alertDialog object from the result of the SetUpSingleActionAlertDialog method in RevisionCardsUtility
                Android.App.AlertDialog alertDialog = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Discard Changes",
                    "Are you sure you want to discard changes to this deck?", "No");
                // Set the first button to "Yes", run code when the user clicks it
                alertDialog.SetButton("Yes", (a, b) =>
                {
                    // Displays a toast to tell the user 
                    Toast.MakeText(this, "Deck not updated", ToastLength.Short).Show();
                    NavigateBack();
                });
                // Show the alert dialog
                alertDialog.Show();
            }
            else
            {
                // Display a toast to tell the user that the deck was not updated
                Toast.MakeText(this, "Deck not updated", ToastLength.Short).Show();
                NavigateBack();
            }
        }

        //Shows the keyboard
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
            inputMethodManager.HideSoftInputFromWindow(deckNameEditText.WindowToken, HideSoftInputFlags.None);
        }

        // If the user leaves the app by pressing the home or overview button
        protected override void OnUserLeaveHint()
        {
            // Call the method to close the keyboard
            HideKeyboard();
            base.OnUserLeaveHint();
        }

        // Navigates the user back to the DeckDetails activity
        private void NavigateBack()
        {
            HideKeyboard();
            // Creates an intent object that navigates from this activity to DeckDetailsActivity
            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
            // Carry forward the selected deck id and name variables
            intent.PutExtra("selectedDeckID", selectedDeckID);
            // Start the activity using the intent object
            StartActivity(intent);
        }
    }
}