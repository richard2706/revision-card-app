using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using System;

namespace RealAppTest7.Classes
{
    // Create a new class called AlarmReciever that inherits the BroadcastReciever class. This class is used to send notifications when the alarm
    // fires
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        // The ID of the notification channel used to display the notification stored as a constant string value
        private const string CHANNEL_ID = "reminder_notifications";

        // Override the OnRecieve method to control what happens when the alarm fires, ie every time a notification needs to be sent
        public override void OnReceive(Context context, Intent intent)
        {
            // Gets the deckID from the Intent object parameter
            int deckID = intent.GetIntExtra("deckID", -1);
            // DeckIDs start at 0, so if it has defaulted to -1, then the notification will not be displayed
            if (deckID != -1)
            {
                // Create a TaskStackBuilder object using the context to set up what to do when the notification is clicked and allow the system
                // back button to work properly
                Android.Support.V4.App.TaskStackBuilder taskStackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);
                // Add intent object to the taskStackBuilder to naviate to MainActivity from the notification
                taskStackBuilder.AddNextIntent(new Intent(context, typeof(MainActivity)));
                // Creates a pending intent object from the taskStackBuilder to be added to the notification builder
                PendingIntent pendingIntent = taskStackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

                // Creates a Notification Builder used to create the notification using the context parameter and the notification channel id
                Notification.Builder builder = new Notification.Builder(context, CHANNEL_ID);
                // Sets the title of the notification to a string of text including the name of the deck retrieved from the database by calling the
                // GetDeck method and passing in the deckID then accessing the DeckName property from the resulting object returned by the method
                builder.SetContentTitle("Time to study " + DeckDatabaseHelper.GetDeck(deckID).DeckName);
                // Sets the content text of the notification to the time difference to the nearest day since the user last studied the specified deck.
                // The time difference is calculated by accessing the TotalDays property of the TimeSpan object (defined inline here). This value is a
                // double so it is then rounded to the nearest whole number using the Math.Round method, then the result of this function is cast to an
                // integer and dislayed as part of the notification
                builder.SetContentText("Its been " + (int)Math.Round((DateTime.Now - DeckDatabaseHelper.GetDeck(deckID).DateLastStudied).TotalDays)
                    + " days since you last studied it");
                // Makes the notification close when the user clicks it
                builder.SetAutoCancel(true);
                // Sets the PendingIntent object, allowing the user to be navigated to the MainActivity
                builder.SetContentIntent(pendingIntent);
                // Sets the small icon to be used on the notification. This is the question and answer icon used for the study option in the menu
                builder.SetSmallIcon(Resource.Drawable.question_answer);

                // Create a notification manager object used to display the notification
                NotificationManagerCompat notificationManager = NotificationManagerCompat.From(context);
                // Display the notification by building it from the builder object
                notificationManager.Notify(0, builder.Build());
            }
            else
                Toast.MakeText(context, "Notification error", ToastLength.Short).Show();
        }
    }
}