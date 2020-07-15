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
using System;

namespace RealAppTest7
{
    [Activity(Label = "AddDeckActivity")]
    public class AddDeckActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add items from AddDeck layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        EditText deckNameEditText;
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
            SupportActionBar.Title = "Add Deck";

            // Add the EditText from the layout as a variable
            deckNameEditText = FindViewById<EditText>(Resource.Id.deckNameEditText);

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
            // Populates the menu in the toolbar using addDeckMenu.xml
            MenuInflater.Inflate(Resource.Menu.addDeckMenu, menu);
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
                // If the add deck item is selected
                case Resource.Id.addDeck:
                    // If the deck name EditText is empty
                    if (newDeckName == "")
                        // Calls the method defined below to show an alert dialog with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Empty Textbox",
                            "Please fill out the deck name textbox", "OK").Show();
                    // Calls the DeckAlreadyExists method in the DeckDatabaseHelper class to check if the deck name already exists
                    else if (DeckDatabaseHelper.DeckAlreadyExists(newDeckName))
                        // Calls the method defined below to show an alert dialog with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Deck already exists",
                            "Another deck already has the name " + newDeckName, "OK").Show();
                    // After all validation checks are successful, the deck will be created
                    else
                    {
                        // Set the current date to a variable
                        DateTime date = DateTime.Now;
                        // Create a new deck object to be inserted into the database using the values that the user entered and some default
                        // values for the notifications
                        Deck newDeck = new Deck()
                        {
                            DeckName = newDeckName,
                            DateLastStudied = date,
                            NeverStudied = true,
                            NotificationsEnabled = false,
                            NumberOfDaysUntilReminder = 1
                        };
                        // Insert the new deck object into the database using the Insert method from the DatabaseHelper class
                        if (DeckDatabaseHelper.Insert(newDeck))
                        {
                            HideKeyboard();
                            // Display a toast message if the deck was added successfully
                            Toast.MakeText(this, "Deck added", ToastLength.Short).Show();
                            // Creates an intent object that navigates from this activity to DeckDetails
                            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
                            // Calls the GetSelectedDeckID method from the DeckDatabaseHelper class to return the ID of the selected deck
                            int selectedDeckID = DeckDatabaseHelper.GetSelectedDeckID(newDeckName);
                            intent.PutExtra("selectedDeckID", selectedDeckID);
                            // Start the activity using the intent object
                            StartActivity(intent);
                        }
                        else
                            // Display a toast to tell the user if the deck was not added
                            Toast.MakeText(this, "Error: Could not add deck", ToastLength.Short).Show();
                    }
                    break;
                // If the cancel item is selected
                case Resource.Id.cancel:
                    CancelAddDeck();
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
            CancelAddDeck();
        }

        // Asks user for confirmation (if EditTexts are not empty) to cancel adding the deck. This method is used instead of having the same
        // code in two places
        private void CancelAddDeck()
        {
            // Value in the EditText
            newDeckName = deckNameEditText.Text;
            // If the deck name EditText is not empty
            if (newDeckName != "")
            {
                // Creates an alertDialog object from the result of the SetUpSingleActionAlertDialog method in RevisionCardsUtility
                Android.App.AlertDialog alertDialog1 = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Discard deck",
                    "Are you sure you want to discard this deck?", "No");
                // Add a button to the dialog box
                alertDialog1.SetButton("Yes", (a, b) =>
                {
                    // If user clicks this button a toast will be displayed to tell the user that the card has been discarded
                    Toast.MakeText(this, "Deck discarded", ToastLength.Short).Show();
                    NavigateBack();
                });
                // Show the alert dialog
                alertDialog1.Show();
            }
            else
                NavigateBack();
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
            inputMethodManager.HideSoftInputFromWindow(deckNameEditText.WindowToken, HideSoftInputFlags.None);
        }

        // If the user leaves the app by pressing the home or overview button
        protected override void OnUserLeaveHint()
        {
            // Call the method to close the keyboard
            HideKeyboard();
            base.OnUserLeaveHint();
        }

        // Navigates the user back to the Main activity
        private void NavigateBack()
        {
            // Hides the keyboard
            HideKeyboard();
            // Creates an intent object that navigates from this activity to MainActivity
            Intent intent = new Intent(this, typeof(MainActivity));
            // Start the activity using the intent object
            StartActivity(intent);
        }
    }
}