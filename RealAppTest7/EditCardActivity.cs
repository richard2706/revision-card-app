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
    [Activity(Label = "EditCardActivity")]
    public class EditCardActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add items from AddCard layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        EditText cardQuestionEditText, cardAnswerEditText;
        Button boldButton, italicButton, underlineButton;
        TextView questionFormattedTextView, answerFormattedTextView, cardQuestionErrorMessage, cardAnswerErrorMessage;
        CheckBox showPreviewCheckBox;
        Spinner colourPickerSpinner, fontPickerSpinner;

        // Fields for details about the selected deck and card
        int selectedDeckID;
        int selectedCardID;
        string selectedCardQuestion;
        string selectedCardAnswer;
        string newCardQuestion;
        string newCardAnswer;

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the AddCard.axml layout
            SetContentView(Resource.Layout.AddCard);

            // Get variables carried over by the Intent from previous screen using the GetStringExtra method (or corrosponding data type)
            selectedDeckID = Intent.GetIntExtra("selectedDeckID", 0);
            // Get the CardID carried over by the Intent from the previous screen using the GetStringExtra method
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
            SupportActionBar.Title = "Edit Card";

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

            // Define the question EditText from the Layout file using its auto-generated ID
            cardQuestionEditText = FindViewById<EditText>(Resource.Id.cardQuestionEditText);
            // Set the text to the current question
            cardQuestionEditText.Text = selectedCardQuestion;
            // Add the TextChanged event to the question EditText
            cardQuestionEditText.TextChanged += CardQuestionEditText_TextChanged;
            // Calls the method defined below to set the text to the question in the database with the HTML formatting
            FormatQuestionPreviewText();

            // Define the answer EditText from the Layout file using its auto-generated ID
            cardAnswerEditText = FindViewById<EditText>(Resource.Id.cardAnswerEditText);
            // Set the text to the current answer
            cardAnswerEditText.Text = selectedCardAnswer;
            // Add the TextChanged event to the question EditText
            cardAnswerEditText.TextChanged += CardAnswerEditText_TextChanged;
            // Calls the method defined below to set the text to the answer in the database with the HTML formatting
            FormatAnswerPreviewText();

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

            // Open the keyboard (so the user doesn't have to click on the EditText first)
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

        // Called when the text in the answer EditText changes
        private void CardAnswerEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            FormatAnswerPreviewText();
        }

        // Applies HTML formatting to the answer
        private void FormatAnswerPreviewText()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // Sets the answer preview textview to the text in the answer edittext, formatted using HTML
            answerFormattedTextView.TextFormatted = Android.Text.Html.FromHtml(cardAnswerEditText.Text);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // Called when the text in the answer EditText changes
        private void CardQuestionEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            FormatQuestionPreviewText();
        }

        // Applies HTML formatting to the question
        private void FormatQuestionPreviewText()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // Sets the question preview textview to the text in the question edittext, formatted using HTML
            questionFormattedTextView.TextFormatted = Android.Text.Html.FromHtml(cardQuestionEditText.Text);
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
            // Populates the menu in the toolbar using editCardMenu.xml
            MenuInflater.Inflate(Resource.Menu.editCardMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        // Override the OnOptionsItemSelected method so code can run when a button in the toolbar is clicked
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Set variables for contents of the EditTexts
            newCardQuestion = cardQuestionEditText.Text;
            newCardAnswer = cardAnswerEditText.Text;

            // Checks which item from the menu was selected by comparing the selected item ID (parameter "item") with the IDs of all items in the
            // toolbar
            switch (item.ItemId)
            {
                // If the save card item is selected
                case Resource.Id.saveCard:
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
                    // If both the question and answer are the same as before (ie the user has not changed them)
                    else if (newCardQuestion == selectedCardQuestion && newCardAnswer == selectedCardAnswer)
                    {
                        // Display a toast to tell the user that the card was not updated
                        Toast.MakeText(this, "Card not updated", ToastLength.Short).Show();
                        NavigateBack();
                    }
                    // If either the question and/or the answer has changed
                    else if (newCardQuestion != selectedCardQuestion || newCardAnswer != selectedCardAnswer)
                    {
                        // If the question has changed and it already exists in a different card in the same deck
                        if (newCardQuestion != selectedCardQuestion
                            && CardDatabaseHelper.CardQuestionAlreadyExists(selectedDeckID, newCardQuestion))
                        {
                            // Calls the method to show a dialog box with a specified title and message
                            RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Question already exists",
                                "Another card already has the same question", "OK").Show();
                            // Gets the question EditText in focus so the user can edit it without having to click on it
                            cardQuestionEditText.RequestFocus();
                        }
                        // Else, the answer has changed
                        else
                        {
                            // If the card updates successfully, the Update method returns true
                            if (CardDatabaseHelper.Update(selectedCardID, newCardQuestion, newCardAnswer))
                            {
                                // Displays a toast to tell the user that the card was updated
                                Toast.MakeText(this, "Card updated", ToastLength.Short).Show();
                                NavigateBack();
                            }
                        }
                    }
                    else
                        // Display a toast if the card could not be updated
                        Toast.MakeText(this, "Error: Card could not be updated", ToastLength.Short).Show();
                    break;
                // If the cancel item is selected
                case Resource.Id.cancel:
                    CancelEditCard();
                    break;
                // If the share card item is selected
                case Resource.Id.shareCard:
                    HideKeyboard();
                    // Create an intent object with the share action
                    Intent shareIntent = new Intent(Intent.ActionSend);
                    // Set the type of content to be shared to plain text
                    shareIntent.SetType("text/plain");
#pragma warning disable CS0618 // Type or member is obsolete
                    // A variable for the text to be shared, formatted using HTML
                    string shareContent = Android.Text.Html.FromHtml("Question: " + newCardQuestion + ". Answer: " + newCardAnswer + ".").ToString();
#pragma warning restore CS0618 // Type or member is obsolete
                    // Set the subject to "Question"
                    shareIntent.PutExtra(Intent.ExtraSubject, "Question");
                    // Set the text to the shareContent variable
                    shareIntent.PutExtra(Intent.ExtraText, shareContent);
                    // Set the title to "Question"
                    shareIntent.PutExtra(Intent.ExtraTitle, "Question");
                    // Start the share activity with the title at the top of the share reading "Share this card"
                    StartActivity(Intent.CreateChooser(shareIntent, "Share this card"));
                    break;
                // If the move card item is selected
                case Resource.Id.moveCard:
                    // Uses the GetNumberOfDecks method in the DeckDatabaseHelper class to check if there is more than 1 deck
                    if (DeckDatabaseHelper.GetNumberOfDecks() > 1)
                    {
                        HideKeyboard();
                        // Creates an intent object that navigates from this activity to MoveCardActivity
                        Intent intent = new Intent(this, typeof(MoveCardActivity));
                        intent.PutExtra("selectedDeckID", selectedDeckID);
                        intent.PutExtra("selectedCardID", selectedCardID);
                        // Start the activity using the intent object
                        StartActivity(intent);
                    }
                    // If the number of decks is 1 or less, the user will stay in the same activity
                    else
                        // Display a toast to tell the user that there are no other decks to move the card to
                        Toast.MakeText(this, "No other decks available", ToastLength.Short).Show();
                    break;
                // If the delete card item is selected
                case Resource.Id.deleteCard:
                    // Creates an alertDialog object from the result of the SetUpSingleActionAlertDialog method in RevisionCardsUtility
                    Android.App.AlertDialog alertDialog = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Delete Card",
                        "Are you sure you want to delete this card?", "Cancel");
                    // Set the first button to "Delete", run code when the user clicks it
                    alertDialog.SetButton("Delete", (a, b) =>
                    {
                        // If the card is deleted sucessfully, the DeleteCard method returns true
                        if (CardDatabaseHelper.DeleteCard(selectedCardID))
                        {
                            // Display a toast to tell the user the card was deleted
                            Toast.MakeText(this, "Card deleted", ToastLength.Short).Show();
                            NavigateBack();
                        }
                        else
                            // Display a toast to tell the user that the card could not be deleted
                            Toast.MakeText(this, "Error: card could not be deleted", ToastLength.Short).Show();
                    });
                    // Show the alert dialog
                    alertDialog.Show();
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
            CancelEditCard();
        }

        // Asks user for confirmation (if EditTexts are not empty) to cancel editing the card. This method is used instead of having the same code
        // in two places
        private void CancelEditCard()
        {
            // Set variables for the values in the EditTexts
            newCardQuestion = cardQuestionEditText.Text;
            newCardAnswer = cardAnswerEditText.Text;

            // If either the question or answer have changed
            if (newCardQuestion != selectedCardQuestion || newCardAnswer != selectedCardAnswer)
            {
                // Creates an alertDialog object from the result of the SetUpSingleActionAlertDialog method in RevisionCardsUtility
                Android.App.AlertDialog alertDialog1 = RevisionCardsUtility.SetUpSingleActionAlertDialog(this, "Discard Changes",
                    "Are you sure you want to discard changes to this card?", "No");
                // Add a button to the dialog box
                alertDialog1.SetButton("Yes", (a, b) =>
                {
                    // Display a toast to tell the user that the card was not updated
                    Toast.MakeText(this, "Card not updated", ToastLength.Short).Show();
                    NavigateBack();
                });
                // Show the alert dialog
                alertDialog1.Show();
            }
            else
            {
                // Display a toast to tell the user that the card was not updated
                Toast.MakeText(this, "Card not updated", ToastLength.Short).Show();
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
            inputMethodManager.HideSoftInputFromWindow(cardQuestionEditText.WindowToken, HideSoftInputFlags.None);
        }

        // If the user leaves the app by pressing the home or overview button
        protected override void OnUserLeaveHint()
        {
            // Call the method to close the keyboard
            HideKeyboard();
            base.OnUserLeaveHint();
        }

        // Navigates the user back to the Study activity
        private void NavigateBack()
        {
            // Creates an intent object that navigates from this activity to DeckDetailsActivity
            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
            intent.PutExtra("selectedDeckID", selectedDeckID);
            // Start the activity using the intent object
            StartActivity(intent);
        }
    }
}