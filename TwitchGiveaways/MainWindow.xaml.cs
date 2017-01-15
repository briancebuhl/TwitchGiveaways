using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwitchGiveaways
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> _names;

        public MainWindow()
        {
            InitializeComponent();

            DataObject.AddPastingHandler(chatChatBox, new DataObjectPastingEventHandler(OnPaste));
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!(e.SourceDataObject.GetDataPresent(DataFormats.Text, true)))
                return;

            string chatText = e.SourceDataObject.GetData(DataFormats.Text) as string;
            chatChatBox.Text = chatText;
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            // Bail out if we have nothing to process
            if (string.IsNullOrEmpty(chatChatBox.Text))
            {
                winnerNameBlock.Text = "No Names to Process";
                return;
            }

            GetNamesFromText();

            if (_names.Count == 0)
            {
                winnerNameBlock.Text = "No entries";
                return;
            }

            // Get a random number based on the count of names
            int winningIndex = new Random().Next(_names.Count);
            winnerNameBlock.Text = string.Format("{0} ({1} of {2})",
                _names[winningIndex], winningIndex + 1, _names.Count);
        }

        private void GetNamesFromText()
        {
            // Instantiate the names list
            _names = new List<string>();

            // Get the lines
            string[] lines = chatChatBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                if (!line.ToLower().Contains("me"))
                    continue;

                // Find the name
                var words = line.Split(' ');
                var name = "";
                foreach (var word in words)
                {
                    if (word.EndsWith(":"))
                    {
                        name = word;
                        break;
                    }
                }

                // Make sure the me is after the name
                var comment = line.Substring(line.IndexOf(name), line.Length - line.IndexOf(name));
                if (!comment.ToLower().Contains("me"))
                    continue;

                if (!_names.Contains(name))
                    _names.Add(name);
            }
        }
    }
}
