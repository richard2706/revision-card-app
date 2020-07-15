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
    [Activity(Label = "AddCardActivity")]
    public class AddCardActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add the items from the AddCard layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        EditText cardQuestionEditText, cardAnswerEditText;
        Button boldButton, italicButton, underlineButton;
        TextView questionFormattedTextView, answerFormattedTextView, cardQuestionErrorMessage, cardAnswerErrorMessage;
        CheckBox showPreviewCheckBox;
        Spinner colourPickerSpinner, fontPickerSpinner;

        // Variables carried over by the Intent by the Intent by the Intent from previous activity
        int selectedDeckID;
        string selectedDeckName;

        // Create new variables for the new question and answers from the EditTexts
        string newCardQuestion, newCardAnswer;

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Dispay the AddCard.axml layout
            SetContentView(Resource.Layout.AddCard);

            // Get deck ID carried over by the Intent by the Intent from previous screen using the GetStringExtra method
            selectedDeckID = Intent.GetIntExtra("selectedDeckID", 0);
            // Get the name of the deck by accessing the DeckName property of the deck returned by GetDeck
            selectedDeckName = DeckDatabaseHelper.GetDeck(selectedDeckID).DeckName;

            // Add toolbar from the layout to a variable so it can be edited here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Set the action bar to the toolbar just created
            SetSupportActionBar(toolbar);
            // Set the title of the toolbar to "Add card to " and the deck name
            SupportActionBar.Title = "Add Card to " + selectedDeckName;

            // Define the question EditText from the Layout file using its auto-generated ID
            cardQuestionEditText = FindViewById<EditText>(Resource.Id.cardQuestionEditText);
            // Add the TextChanged event to the question EditText
            cardQuestionEditText.TextChanged += CardQuestionEditText_TextChanged;

            // Define the answer EditText from the Layout file using its auto-generated ID
            cardAnswerEditText = FindViewById<EditText>(Resource.Id.cardAnswerEditText);
            // Add the TextChanged event to the question EditText
            cardAnswerEditText.TextChanged += CardAnswerEditText_TextChanged;

            // Define the bold Button from the Layout file using its auto-generated ID
            boldButton = FindViewById<Button>(Resource.Id.boldButton);
            // Add the Click event handler to the bold button
            boldButton.Click += BoldButton_Click;

            // Define the italic Button from the Layout file using its auto-generated ID
            italicButton = FindViewById<Button>(Resource.Id.italicButton);
            // Add the Click event handler to the italic button
            italicButton.Click += ItalicButton_Click;

            // Define the underline Button from the Layout file using its auto-generated ID
            underlineButton = FindViewById<Button>(Resource.Id.underlineButton);
            // Add the Click event handler to the underline button
            underlineButton.Click += UnderlineButton_Click;

            // Define the answer TextView from the Layout file using its auto-generated ID
            answerFormattedTextView = FindViewById<TextView>(Resource.Id.answerFormattedTextView);
            // Define the question TextView from the Layout file using its auto-generated ID
            questionFormattedTextView = FindViewById<TextView>(Resource.Id.questionFormattedTextView);
            // Define the question error TextView from the layout file using its auto generated ID
            cardQuestionErrorMessage = FindViewById<TextView>(Resource.Id.cardQuestionErrorMessage);  
            // Hide the error message TextView
            cardQuestionErrorMessage.Visibility = ViewStates.Gone;
            // Define the answer error TextView from the layout file using its auto generated ID
            cardAnswerErrorMessage = FindViewById<TextView>(Resource.Id.cardAnswerErrorMessage);
            // Hide the error message TextView
            cardAnswerErrorMessage.Visibility = ViewStates.Gone;

            // Define the CheckBox from the Layout file using its auto-generated ID
            showPreviewCheckBox = FindViewById<CheckBox>(Resource.Id.showPreviewCheckBox);
            // Add the CheckChanged event to the CheckBox
            showPreviewCheckBox.CheckedChange += ShowPreviewCheckBox_CheckedChange;

            // Define the colour picker Spinner from the Layout file using its auto-generated ID
            colourPickerSpinner = FindViewById<Spinner>(Resource.Id.colourPickerSpinner);
            // Add the ItemSelected event handler
            colourPickerSpinner.ItemSelected += ColourPickerSpinner_ItemSelected;
            // Populate the Spinner using the method in RevisionCardsUtility
            RevisionCardsUtility.PopulateSpinner(this, colourPickerSpinner, Resource.Array.colours);

            // Define the font picker Spinner from the Layout file using its auto-generated ID
            fontPickerSpinner = FindViewById<Spinner>(Resource.Id.fontPickerSpinner);
            // Add the ItemSelected event handler
            fontPickerSpinner.ItemSelected += FontPickerSpinner_ItemSelected;
            // Populate the Spinner using the method in RevisionCardsUtility
            RevisionCardsUtility.PopulateSpinner(this, fontPickerSpinner, Resource.Array.fonts);

            // Open the keyboard usng the private method (so the user doesn't have to click on the EditText first)
            ShowKeyboard();
        }

        // Called when an item in the font picker Spinner is selected
        private void FontPickerSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            // Toggles the HTML tag specified by calling the method in RevisionCardsUtility
            RevisionCardsUtility.ToggleHtmlTag(GetFocussedEditText(), GetFocussedEditTextErrorTextView(), "font",
                "face=\"" + fontPickerSpinner.GetItemAtPosition(e.Position).ToString() + "\"");
        }

        // Called when an item in the colour picker Spinner is selected
        private void ColourPickerSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            // Toggles the HTML tag specified by calling the method in RevisionCardsUtility
            RevisionCardsUtility.ToggleHtmlTag(GetFocussedEditText(), GetFocussedEditTextErrorTextView(), "font",
                "color=\"" + colourPickerSpinner.GetItemAtPosition(e.Position).ToString() + "\"");
        }

        // Called when the checkbox is checked or unchecked
        private void ShowPreviewCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            // If the checkbox is checked, display the textviews that show the formatted previews
            if (showPreviewCheckBox.Checked)
            {
                questionFormattedTextView.Visibility = ViewStates.Visible;
                answerFormattedTextView.Visibility = ViewStates.Visible;
            }
            // Otherwise, hide the textviews
            else
            {
                questionFormattedTextView.Visibility = ViewStates.Gone;
                answerFormattedTextView.Visibility = ViewStates.Gone;
            }
        }

        // Called when the text in the question EditText changes
        private void CardQuestionEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // Sets the question preview textview to the text in the question edittext, formatted using HTML
            questionFormattedTextView.TextFormatted = Android.Text.Html.FromHtml(cardQuestionEditText.Text);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // Called when the text in the answer EditText changes
        private void CardAnswerEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // Sets the question preview textview to the text in the question edittext, formatted using HTML
            answerFormattedTextView.TextFormatted = Android.Text.Html.FromHtml(cardAnswerEditText.Text);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // Called when the underline button is clicked
        private void UnderlineButton_Click(object sender, EventArgs e)
        {
            // Calls the ToggleHtmlTag method defined in the RevisionCardsUtility class in the Classes folder to toggle the HTML tag
            RevisionCardsUtility.ToggleHtmlTag(GetFocussedEditText(), GetFocussedEditTextErrorTextView(), "span", "style=\"text-decoration: underline;\"");
        }

        // Called when the italic button is clicked
        private void ItalicButton_Click(object sender, EventArgs e)
        {
            // Calls the ToggleHtmlTag method defined in the RevisionCardsUtility class in the Classes folder to toggle the HTML tag
            RevisionCardsUtility.ToggleHtmlTag(GetFocussedEditText(), GetFocussedEditTextErrorTextView(), "i");
        }

        // Called when the bold button is clicked
        private void BoldButton_Click(object sender, EventArgs e)
        {
            // Calls the ToggleHtmlTag method defined in the RevisionCardsUtility class in the Classes folder to toggle the HTML tag
            RevisionCardsUtility.ToggleHtmlTag(GetFocussedEditText(), GetFocussedEditTextErrorTextView(), "b");
        }

        // Method that returns the EditText that currently has focus
        private EditText GetFocussedEditText()
        {
            if (cardQuestionEditText.HasFocus)
                return cardQuestionEditText;
            else if (cardAnswerEditText.HasFocus)
                return cardAnswerEditText;
            else
                return null;
        }

        // Method that returns the TextView used to display errors that corrosponds to the EditText that has the focus
        private TextView GetFocussedEditTextErrorTextView()
        {
            if (GetFocussedEditText() == cardQuestionEditText)
                return cardQuestionErrorMessage;
            else if (GetFocussedEditText() == cardAnswerEditText)
                return cardAnswerErrorMessage;
            return null;
        }

        // Override the OnRestart method so that the keyboard is shown
        protected override void OnRestart()
        {
            // Shows the keyboard
            ShowKeyboard();
            base.OnRestart();
        }

        // Override the OnCreateOptionsMenu method to allow the toolbar menu to be populated
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Populates the menu in the toolbar using addCardMenu.xml
            MenuInflater.Inflate(Resource.Menu.addCardMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        // Override the OnOptionsItemSelected method so code can run when a button in the toolbar is clicked
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Set variables for contents of the EditTexts
            newCardQuestion = cardQuestionEditText.Text;
            newCardAnswer = cardAnswerEditText.Text;

            // Checks which item from the menu was selected by comparing the selected item ID (parameter "item") with the IDs of all items in
            // the toolbar
            switch (item.ItemId)
            {
                // If the add card item is selected
                case Resource.Id.addCard:
                    // If both EditTexts are empty
                    if (newCardQuestion == "" && newCardAnswer == "")
                    {
                        // Calls the method to show a dialog box with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Empty Textbox", "Please fill out all textboxes", "OK").Show();
                        // Gets the question EditText in focus so the user can edit it without having to click on it
                        cardQuestionEditText.RequestFocus();
                    }
                    // If the question EditText is empty
                    else if (newCardQuestion == "")
                    {
                        // Calls the method to show a dialog box with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Empty Textbox", "Please fill question textbox", "OK").Show();
                        // Gets the question EditText in focus so the user can edit it without having to click on it
                        cardQuestionEditText.RequestFocus();
                    }
                    // If the answer EditText is empty
                    else if (newCardAnswer == "")
                    {
                        // Calls the method to show a dialog box with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Empty Textbox", "Please fill answer textbox", "OK").Show();
                        // Gets the answer EditText in focus so the user can edit it without having to click on it
                        cardAnswerEditText.RequestFocus();
                    }
                    // If the question already exists. This calls the CardQuestionAlreadyExists method from CardDatabaseHelper which returns a
                    // boolean
                    else if (CardDatabaseHelper.CardQuestionAlreadyExists(selectedDeckID, newCardQuestion))
                    {
                        // Calls the method to show a dialog box with a specified title and message
                        RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Question already exists",
                            "Another card already has the same question", "OK").Show();
                        // Gets the question EditText in focus so the user can edit it without having to click on it
                        cardQuestionEditText.RequestFocus();
                    }
                    // After all validation checks are successful, the card will be added to the database
                    else
                    {
                        // Create the card object to be inserted into the database, using the selectedDeckID and text from the question and
                        // answer EditTexts
                        Card newCard = new Card() { DeckID = selectedDeckID, Question = newCardQuestion, Answer = newCardAnswer };
                        // If the card is added to the database sucessfully (Insert method returns a boolean)
                        if (CardDatabaseHelper.Insert(newCard))
                        {
                            // Display a toast so the user knows their card was added
                            Toast.MakeText(this, "Card added to " + selectedDeckName, ToastLength.Short).Show();
                            // Calls the navigate back method defined below to navigate back to the previous screen
                            NavigateBack();
                        }
                        else
                            // Display a toast if the card could not be added (since the Insert method returned false)
                            Toast.MakeText(this, "Error: Could not add card to " + selectedDeckName, ToastLength.Short).Show();
                    }
                    break;
                // If the cancel item is selected
                case Resource.Id.cancel:
                    CancelAddCard();
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
            CancelAddCard();
        }

        // Asks user for confirmation (if EditTexts are not empty) to cancel adding the card. This method is used instead of having the same
        // code in two places
        private void CancelAddCard()
        {
            // Values in the EditTexts
            newCardQuestion = cardQuestionEditText.Text;
            newCardAnswer = cardAnswerEditText.Text;
            // If either EditText is not blank
            if (newCardQuestion != "" || newCardAnswer != "")
            {
                // Creates an alertDialog object from the result of the SetUpSingleActionAlertDialog method in RevisionCardsUtility
                Android.App.AlertDialog alertDialog = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Discard card",
                    "Are you sure you want to discard this card?", "No");
                // Add a button to the dialog box
                alertDialog.SetButton("Yes", (a, b) =>
                {
                    // If user clicks this button a toast will be displayed to tell the user that the card has been discarded
                    Toast.MakeText(this, "Card discarded", ToastLength.Short).Show();
                    NavigateBack();
                });
                // Show the alert dialog
                alertDialog.Show();
            }
            // If both EditTexts are blank
            else
                // Calls the navigate back method defined below to navigate back to the previous screen
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
            inputMethodManager.HideSoftInputFromWindow(cardQuestionEditText.WindowToken, HideSoftInputFlags.None);
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
            // Calls the HideKeyboard method 
            HideKeyboard();
            // Creates an intent object that navigates from this activity to deck details
            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
            // Carry forward the selected deck id and name variables
            intent.PutExtra("selectedDeckID", selectedDeckID);
            // Start the activity using the intent object
            StartActivity(intent);
        }
    }
}