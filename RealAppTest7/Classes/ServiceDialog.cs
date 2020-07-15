using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace RealAppTest7
{
    // Suppress warning about DialogFragment being obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    // Create a new class called ServiceDialog that inherits the DialogFragment class which contains code used for the dialog box
    class ServiceDialog : DialogFragment
#pragma warning restore CS0618 // Type or member is obsolete
    {
        // Stores the context in which the dialog will be shown
        Context context;
        // Stores the deckID of the deck which notifications are being set for
        int deckID;
        // References to objects in the layout file in the dialog box
        CheckBox notificationsEnabledCheckBox;
        TextView descriptionTextView;
        NumberPicker numberOfDaysNumberPicker;

        // Constructor that sets the context and deckID
        public ServiceDialog(Context context, int deckID)
        {
            this.context = context;
            this.deckID = deckID;
        }

        // Override the OnCreateDialog method to set up the dialog box and add functionality when the buttons are clicked
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Run the base method
            base.OnCreateDialog(savedInstanceState);
            // LayoutInflater object used to get the layout file to be displayed in the dialog box
            LayoutInflater inflater = Activity.LayoutInflater;
            // View object set to the contents of the layout file for the dialog box
            View dialogView = inflater.Inflate(Resource.Layout.NotificationDialogBox, null);
            // Get references to each of the UI elements in the layout file
            descriptionTextView = dialogView.FindViewById<TextView>(Resource.Id.descriptionTextView);
            notificationsEnabledCheckBox = dialogView.FindViewById<CheckBox>(Resource.Id.notificationsEnabledCheckBox);
            numberOfDaysNumberPicker = dialogView.FindViewById<NumberPicker>(Resource.Id.numberOfDaysNumberPicker);

            // Add the CheckedChange event to the checkbox so that code can be executed when the checkbox is checked or unchecked
            notificationsEnabledCheckBox.CheckedChange += NotificationsEnabledCheckBox_CheckedChange;
            // Set checked state of the check box based on the boolean value stored in the database
            if (Classes.DeckDatabaseHelper.GetDeck(deckID).NotificationsEnabled)
                notificationsEnabledCheckBox.Checked = true;

            // Set the minimum and maximum values for the number picker
            numberOfDaysNumberPicker.MinValue = 1;
            numberOfDaysNumberPicker.MaxValue = 28;
            // Set the number picker value as the value stored in the database
            numberOfDaysNumberPicker.Value = Classes.DeckDatabaseHelper.GetDeck(deckID).NumberOfDaysUntilReminder;

            // If the checkbox is checked, then display the description textview and number picker. Could not be extracted into a private method
            // since there were NullPointerExceptions
            if (notificationsEnabledCheckBox.Checked)
            {
                descriptionTextView.Visibility = ViewStates.Visible;
                numberOfDaysNumberPicker.Visibility = ViewStates.Visible;
            }
            else
            {
                descriptionTextView.Visibility = ViewStates.Gone;
                numberOfDaysNumberPicker.Visibility = ViewStates.Gone;
            }

            // The builder object used to create the dialog box
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            // Sets the title of the dialog box
            builder.SetTitle("Notification Options");
            // Sets the layout file that defines what to display in the dialog box
            builder.SetView(dialogView);
            // Adds a confirmation button. 1st parameter is the button text and the second parameter is the method that runs when it is clicked
            builder.SetPositiveButton("OK", (a, b) =>
            {
                // If the database successfully updates (by calling the one of the Update method overloads in DeckDatabaseHelper) the Deck then
                // display a toast to let the user know
                if (Classes.DeckDatabaseHelper.Update(deckID, notificationsEnabledCheckBox.Checked, numberOfDaysNumberPicker.Value))
                    Toast.MakeText(Activity, "Notification options updated", ToastLength.Short).Show();
                else
                    // If the Update method returned false (ie the method did not update the Deck) display a toast to let the user know that their
                    // preferences were not updated
                    Toast.MakeText(Activity, "Database error: Notification options not updated", ToastLength.Short).Show();
            });
            // Adds a cancel button. 1st parameter is the button text and the second parameter is the method that runs when it is clicked
            builder.SetNegativeButton("Cancel", (a, b) => { });
            // Returns the builder for the dialog box
            return builder.Create();
        }

        // Called when the checkbox is checked or unchecked
        private void NotificationsEnabledCheckBox_CheckedChange(object sender, EventArgs e)
        {
            // If the checkbox is checked, then display the description textview and number picker
            if (notificationsEnabledCheckBox.Checked)
            {
                descriptionTextView.Visibility = ViewStates.Visible;
                numberOfDaysNumberPicker.Visibility = ViewStates.Visible;
            }
            else
            {
                descriptionTextView.Visibility = ViewStates.Gone;
                numberOfDaysNumberPicker.Visibility = ViewStates.Gone;
            }
        }
    }
}