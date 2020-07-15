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
    // This activity is the first activity to launch since the MainLauncher attribute is set to true
    [Activity(Label = "Revision Cards", MainLauncher = true)]
    public class MainActivity : AppCompatActivity // AppCompatActivity enables use of components compatible with versions of Android before 5
    {
        // The ID of the notification channel used to display the notification stored as a constant string value
        private const string CHANNEL_ID = "reminder_notifications";

        // Add items from DeckDetails layout as fields so they can be used in any method in this class
        Android.Support.V7.Widget.Toolbar toolbar;
        TextView addDeckPromptTextView;
        ListView deckListView;

        // Define the list of decks to be shown in the listview as a field so it can be accessed anywhere within this class
        List<Deck> decks;

        // Runs when the activity is created
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Display the Main.axml layout
            SetContentView(Resource.Layout.Main);

            // Add toolbar from the layout to a variable so it can be edited here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Set the action bar to the toolbar just created
            SetSupportActionBar(toolbar);
            // Set the title of the toolbar
            SupportActionBar.Title = "My Decks";

            // Add the listview from the layout to a variable so it can be edited here
            deckListView = FindViewById<ListView>(Resource.Id.deckListView);
            // Calls the private method defined below to populate the listview
            PopulateListView();
            // Add the ItemClick event to the listview
            deckListView.ItemClick += DeckListView_ItemClick;

            // Add textview from layout
            addDeckPromptTextView = FindViewById<TextView>(Resource.Id.addDeckPromptTextView);
            // Using GetNumberOfDecks method from the DeckDatbaseHelper class, if the number of decks is 0, show the textview to prompt the user
            // to create a deck
            addDeckPromptTextView.Visibility = (DeckDatabaseHelper.GetNumberOfDecks() == 0) ? ViewStates.Visible : ViewStates.Gone;

            // Create notification channel by calling the method below, only if the API is version 26 or higher (ie Android 8.0 Oreo)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                // Create the NotificationChannel object using the id constant defined above
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, "Reminders", NotificationImportance.Default)
                { Description = "Study reminders are configurable for each deck" };
                // Create a notification manager object which is then used to create the notification channel
                NotificationManager notificationManager1 = (NotificationManager)GetSystemService(NotificationService);
                // Creates the notification channel by calling the 'CreateNotificationChannel' method on the notification manager object and
                // passing in the notification channel object defined above
                notificationManager1.CreateNotificationChannel(channel);
            }
        }

        // Runs when the user clicks an item in the listiew. Has parameter e which is the item that was clicked
        private void DeckListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Create intent object to naviate from this activity to DeckDetails
            Intent intent = new Intent(this, typeof(DeckDetailsActivity));
            // Carry selectedDeckID and selectedDeckName forward using e
            intent.PutExtra("selectedDeckID", decks[e.Position].DeckID);
            // Start the activity using the intent
            StartActivity(intent);
        }

        // Override the OnRestart method
        protected override void OnRestart()
        {
            // Refreshes the listview
            PopulateListView();
            base.OnRestart();
        }

        // Override the OnCreateOptionsMenu method to allow the toolbar menu to be populated
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Populate the toolbar menu using mainMenu.xml
            MenuInflater.Inflate(Resource.Menu.mainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        // Override the OnOptionsItemSelected method so code can run when a button in the toolbar is clicked
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Checks which item from the menu was selected by comparing the selected item ID (parameter "item") with the IDs of all items in the
            // toolbar
            switch (item.ItemId)
            {
                // If the add deck item selected
                case Resource.Id.addDeck:
                    // Creates an intent object that navigates from this activity to AddDeckActivity
                    Intent intent = new Intent(this, typeof(AddDeckActivity));
                    // Start the activity using the intent
                    StartActivity(intent);
                    break;
                // If there was an error, and no IDs match, then this is the default case
                default:
                    // Shows the user a toast to say there was an error
                    Toast.MakeText(this, "Error: No option was selected", ToastLength.Short).Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        // Populates the listview with records from the decks database
        private void PopulateListView()
        {
            // Set the list of decks to the records in the database using the read method from DatabaseHelper
            decks = DeckDatabaseHelper.ReadAll();
            // Set the array adapter to a new object using the SimpleListItem1 template
            ArrayAdapter arrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, decks);
            // Populate the listview
            deckListView.Adapter = arrayAdapter;
        }
    }
}