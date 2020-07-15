using Android.App;
using Android.Content;
using Android.Widget;

namespace RealAppTest7.Classes
{
    // This class contains code that is used in multiple places in the app. Having one method here instead of one private method for each file saves
    // code (ie reduces code duplication)
    class RevisionCardsUtility
    {
        // All methods in this class are declared public so that they can be accessed anywhere in this project and static so that they can be used
        // without creating instances of this class first

        // This overload of ToggleHtmlTag takes the edittext which is currently selected and a string for the Html tag to surround the selection with
        public static void ToggleHtmlTag(EditText focussedEditText, TextView errorTextView, string tag)
        {
            // Gets the text before the selection using the SubString method
            string textBeforeSelection = focussedEditText.Text.Substring(0, focussedEditText.SelectionStart);
            // Gets the selection text using the SubString method
            string selection = focussedEditText.Text.Substring(focussedEditText.SelectionStart,
                focussedEditText.SelectionEnd - focussedEditText.SelectionStart);
            // Gets the text after the selection using the SubString method
            string textAfterSelection = focussedEditText.Text.Substring(focussedEditText.SelectionEnd);
            // If the focussedEditText was not found (ie neither edittext was selected) OR there is no selected text
            if (focussedEditText == null || selection == "")
                // Exit out of the method
                return;
            // If the selected text already has the specified HTML tag on both sides of the selection
            if (textBeforeSelection.EndsWith("<" + tag + ">") && textAfterSelection.StartsWith("</" + tag + ">"))
                // Set the text in the selected edittext to the original but without the HTML tags
                focussedEditText.Text = focussedEditText.Text.Substring(0, focussedEditText.SelectionStart - ("<" + tag + ">").Length)
                    + selection
                    + focussedEditText.Text.Substring(focussedEditText.SelectionEnd + ("</" + tag + ">").Length);
            // If the selection starts and and with the HTML tags
            else if (selection.StartsWith("<" + tag + ">") && selection.EndsWith("</" + tag + ">"))
                // Set the text in the selected edittext to the original but without the HTML tags
                focussedEditText.Text = textBeforeSelection
                    + focussedEditText.Text.Substring(focussedEditText.SelectionStart + ("<" + tag + ">").Length,
                        selection.Length - ("<" + tag + ">").Length - ("</" + tag + ">").Length)
                    + textAfterSelection;
            // if the length of all the text in the textbox and the new tags is longer that the textbox character limit, then dont add the tags
            // and display an error message
            else if (focussedEditText.Text.Length + ("<" + tag + ">" + "</" + tag + ">").Length > 200)
            {
                errorTextView.Visibility = Android.Views.ViewStates.Visible;
                errorTextView.Text = "Maximum character limit reached";
            }
            // Otherwise, if the selection does not contain the specified HTML tags
            else
                // Add the tags around the selection
                focussedEditText.Text = textBeforeSelection + "<" + tag + ">" + selection + "</" + tag + ">" + textAfterSelection;
        }

        // This overload of ToggleHtmlTag takes the edittext which is currently selected, a string for the Html tag to surround the selection with and
        // another string containing the attributes for the tag
        public static void ToggleHtmlTag(EditText focussedEditText, TextView errorTextView, string tag, string attributes)
        {
            // Gets the text before the selection using the SubString method
            string textBeforeSelection = focussedEditText.Text.Substring(0, focussedEditText.SelectionStart);
            // Gets the selection text using the SubString method
            string selection = focussedEditText.Text.Substring(focussedEditText.SelectionStart,
                focussedEditText.SelectionEnd - focussedEditText.SelectionStart);
            // Gets the text after the selection using the SubString method
            string textAfterSelection = focussedEditText.Text.Substring(focussedEditText.SelectionEnd);
            // If the focussedEditText was not found (ie neither edittext was selected) OR there is no selected text
            if (focussedEditText == null || selection == "")
                // Exit out of the method
                return;
            // If the selected text already has the specified HTML tag on both sides of the selection
            if (textBeforeSelection.EndsWith("<" + tag + " " + attributes + ">") && textAfterSelection.StartsWith("</" + tag + ">"))
                // Set the text in the selected edittext to the original but without the HTML tags
                focussedEditText.Text = focussedEditText.Text.Substring(0, focussedEditText.SelectionStart - ("<" + tag + " " + attributes + ">").Length)
                    + selection
                    + focussedEditText.Text.Substring(focussedEditText.SelectionEnd + ("</" + tag + ">").Length);
            // If the selection starts and and with the HTML tags
            else if (selection.StartsWith("<" + tag + " " + attributes + ">") && selection.EndsWith("</" + tag + ">"))
                // Set the text in the selected edittext to the original but without the HTML tags
                focussedEditText.Text = textBeforeSelection
                    + focussedEditText.Text.Substring(focussedEditText.SelectionStart + ("<" + tag + " " + attributes + ">").Length,
                        selection.Length - ("<" + tag + " " + attributes + ">").Length - ("</" + tag + ">").Length)
                    + textAfterSelection;
            // if the length of all the text in the textbox and the new tags is longer that the textbox character limit, then dont add the tags
            // and display an error message
            else if (focussedEditText.Text.Length + ("<" + tag + " " + attributes + ">" + "</" + tag + ">").Length > 200)
            {
                errorTextView.Visibility = Android.Views.ViewStates.Visible;
                errorTextView.Text = "Maximum character limit reached";
            }
            // Otherwise if the selection does not contain the specified HTML tags
            else
                // Add the tags around the selection
                focussedEditText.Text = textBeforeSelection + "<" + tag + " " + attributes + ">" + selection + "</" + tag + ">" + textAfterSelection;
        }

        // Populates a given spinner using the array provided (must be defined in strings.xml, then accessed using its ID)
        public static void PopulateSpinner(Context context, Spinner spinner, int textArrayResourceId)
        {
            // Creates an array adapter object using the array defined in strings.xml used to populate the spinner
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(context, textArrayResourceId, Android.Resource.Layout.SimpleSpinnerItem);
            // Sets the format of each item in the spinner using the templete SimpleSpinnerDropDownItem
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            // Sets the apadper property of the spinner to the adapter defined above, populating the spinner with items from the array
            spinner.Adapter = adapter;
        }

        // Sets up a dialog box with an OK button, with a specified title and message
        public static AlertDialog SetUpSingleActionAlertDialog(Context context, string title, string message, string buttonText)
        {
            // Create a builder object
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            // Create an alert dialog object using the builder
            AlertDialog alertDialog = builder.Create();
            // Set the title to be the specified title from the parameter "title"
            alertDialog.SetTitle(title);
            // Set the message to be the specified message from the parameter "message"
            alertDialog.SetMessage(message);
            // Create a button that closes the alert dialog. This sets the second button to allow for some cases where a first button is needed
            alertDialog.SetButton2(buttonText, (a, b) => { });
            // returns the alert dialog object
            return alertDialog;
        }
    }
}