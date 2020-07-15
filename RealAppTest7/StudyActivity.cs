using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
// Using the classes from the "Classes" folder in the solution
using RealAppTest7.Classes;
using System;

namespace RealAppTest7
{
    [Activity(Label = "StudyActivity")]
    public class StudyActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // Add items from Study layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        TextView questionTextView, answerTextView, numberOfCardsStudiedTextView;
        Button showAnswerButton, nextQuestionButton, finishButton;

        // Variables carried over by the Intent from previous activity
        int selectedDeckID;
        string selectedDeckName;
        int numberOfCardsStudied;

        // The card object used for the current card being displayed
        Card selectedCard = new Card();

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the Study.axml layout
            SetContentView(Resource.Layout.Study);

            // Get variables carried over by the Intent from previous screen using the GetStringExtra method (or corrosponding data type)
            selectedDeckID = Intent.GetIntExtra("selectedDeckID", 0);
            // Get the name of the deck by accessing the DeckName property of the deck returned by GetDeck
            selectedDeckName = DeckDatabaseHelper.GetDeck(selectedDeckID).DeckName;
            // Get a random card object from the database using the GetRandomCard method defined in CardDatabaseHelper. Not using private
            // method below since there is no previous card to compare with
            selectedCard = CardDatabaseHelper.GetRandomCard(selectedDeckID);

            // Add toolbar from the layout as a variable so it can be edited here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Set the action bar to the toolbar just created
            SetSupportActionBar(toolbar);
            // Set the title of the toolbar to "Studying " and the name of the deck the user is studying
            SupportActionBar.Title = "Studying " + selectedDeckName;

            // Add question textview from layout as a variable so it can be edited here
            questionTextView = FindViewById<TextView>(Resource.Id.questionTextView);
#pragma warning disable CS0618 // Type or member is obsolete
            // Set the text to the question of the currently selected card, formatted using HTML
            questionTextView.TextFormatted = Android.Text.Html.FromHtml(selectedCard.Question);
#pragma warning restore CS0618 // Type or member is obsolete

            // Add the answer textview from the layout as a variable so it can be edited here
            answerTextView = FindViewById<TextView>(Resource.Id.answerTextView);

            // Add the textview to show number of cards studied from the layout so it can be edited here
            numberOfCardsStudiedTextView = FindViewById<TextView>(Resource.Id.numberOfCardsStudiedTextView);
            // Set it's text to "Cards studied: " and the number of cards studied (using the variable created as a field converted to a string)
            numberOfCardsStudiedTextView.Text = string.Format("Cards studied: " + numberOfCardsStudied.ToString());

            // Add the button to show the answer as a variable so it can be edited here
            showAnswerButton = FindViewById<Button>(Resource.Id.showAnswerButton);
            // Add the click event to the button. When this button is clicked, the ShowAnswerButton_Click method will be called
            showAnswerButton.Click += ShowAnswerButton_Click;

            // Add the button to go to the next question from the layout so it can be edited here
            nextQuestionButton = FindViewById<Button>(Resource.Id.nextQuestionButton);
            // Add the click even to the button. When this button is clicked the NextQuestionButton_Click method is called
            nextQuestionButton.Click += NextQuestionButton_Click;
            // Make the button invisible
            nextQuestionButton.Visibility = ViewStates.Gone;

            // Add the finish button as a variable so it can be edited here
            finishButton = FindViewById<Button>(Resource.Id.finishButton);
            // Add the click even to the button. When this button is clicked the FinishButton_Click is called
            finishButton.Click += FinishButton_Click;
        }

        /*
        // Generates a random card that is not the same as the previous card
        private Card GetRandomCard()
        {
            Card newRandomCard = selectedCard;
            if (CardDatabaseHelper.NumberOfCardsInSelectedDeck(selectedDeckID) > 1)
                while (newRandomCard == selectedCard)
                    newRandomCard = CardDatabaseHelper.GetRandomCard(selectedDeckID);
            return newRandomCard;
        }
        */

        // Override the OnRestart method
        protected override void OnRestart()
        {
            // Shows the show answer button
            showAnswerButton.Visibility = ViewStates.Visible;
            // Hides the next question button
            nextQuestionButton.Visibility = ViewStates.Gone;
            base.OnRestart();
        }

        // Called when the show answer button is clicked
        private void ShowAnswerButton_Click(object sender, EventArgs e)
        {
            // Shows the next question button
            nextQuestionButton.Visibility = ViewStates.Visible;
            // Hides the show answer button
            showAnswerButton.Visibility = ViewStates.Gone;
#pragma warning disable CS0618 // Type or member is obsolete
            // Sets the text of the answer textview to the the answer of the current card, formatted using HTML
            answerTextView.TextFormatted = Android.Text.Html.FromHtml(selectedCard.Answer);
#pragma warning restore CS0618 // Type or member is obsolete
            // Adds 1 to the number of cards studied
            numberOfCardsStudied++;
            // Sets the text of the textview for the number of cards studied to the new number of cards studied
            numberOfCardsStudiedTextView.Text = string.Format("Cards studied: " + numberOfCardsStudied.ToString());
        }

        // Called when the next question button is clicked
        private void NextQuestionButton_Click(object sender, EventArgs e)
        {
            // Hides the next question button
            nextQuestionButton.Visibility = ViewStates.Gone;
            // Shows the show answer button
            showAnswerButton.Visibility = ViewStates.Visible;
            // sets the selectedCard variable to a new random card using the GetRandomCard method in CardDatabaseHelper
            selectedCard = CardDatabaseHelper.GetRandomCard(selectedDeckID, selectedCard.CardID);
#pragma warning disable CS0618 // Type or member is obsolete
            // Sets the text in the question textview to the new question, formatted using HTML
            questionTextView.TextFormatted = Android.Text.Html.FromHtml(selectedCard.Question);
#pragma warning restore CS0618 // Type or member is obsolete
            // Sets the text in the answer textview to an empty string
            answerTextView.Text = "";
        }

        // Called when the finish button is clicked
        private void FinishButton_Click(object sender, EventArgs e)
        {
            FinishStudy();
        }

        // Overrides the back button event
        public override void OnBackPressed()
        {
            FinishStudy();
        }

        // This method is called when the user clicks the back button or clicks finish
        private void FinishStudy()
        {
            // Creates an intent object that navigates from this activity to deck details
            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
            // Carry forward the selected deck id variables
            intent.PutExtra("selectedDeckID", selectedDeckID);

            // Get the current date and time in a variable
            DateTime date = DateTime.Now;

            if (numberOfCardsStudied != 0)
            {
                // Display a toast to tell the user how many cards they studied
                Toast.MakeText(this, "You studied " + numberOfCardsStudied + " cards from " + selectedDeckName, ToastLength.Short).Show();

                // Create the alarm intent object
                Intent alarmIntent = new Intent(this, typeof(AlarmReceiver));
                // Carry the deck id to the alarm reciever so that attrbutes of the deck can be accessed (eg so the name of the deck can be
                // displayed in the notification)
                alarmIntent.PutExtra("deckID", selectedDeckID);
                // Create the pending intent object from the alarm intent
                PendingIntent alarmPendingIntent = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
                // Create an alarm manager object used to schedule the alarm to fire after a set number of days
                AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);
                if (DeckDatabaseHelper.GetDeck(selectedDeckID).NotificationsEnabled)
                    // Send a notification if the deck hasn't been studied for the number of days specified in the database
                    alarmManager.Set(AlarmType.ElapsedRealtime,
                        SystemClock.ElapsedRealtime() + (DeckDatabaseHelper.GetDeck(selectedDeckID).NumberOfDaysUntilReminder * 24 * 60 * 60 * 1000),
                        alarmPendingIntent);
                // Update the neverStudied attribute to false
                DeckDatabaseHelper.Update(selectedDeckID, false);
                // Update the date last studied i\n database
                if (!(DeckDatabaseHelper.Update(selectedDeckID, date)))
                    // Display and error is the Update method returned false to tell the user that the deck was not updated successfully
                    Toast.MakeText(this, "Error: Could not update deck", ToastLength.Short).Show();
            }
            // Start the activity using the intent
            StartActivity(intent);
        }
    }
}