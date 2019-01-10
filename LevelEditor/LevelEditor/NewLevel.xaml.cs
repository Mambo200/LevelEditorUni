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
using System.Windows.Shapes;

namespace LevelEditor
{
    /// <summary>
    /// Interaktionslogik für NewLevel.xaml
    /// </summary>
    public partial class NewLevel : Window
    {
        public string levelName = null;
        public int xSize = 0;
        public int ySize = 0;

        public NewLevel()
        {
            InitializeComponent();
            TextBox_NameUserInput.Focus();
        }

        private void Button_Ok_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Button bttn = (Button)sender;

            // OK button pressed
            if(bttn.Content.ToString() == "Ok")
            {
                // check level name
                if (TextBox_NameUserInput.Text == "")
                {
                    MessageBoxResult result = MessageBox.Show("Level name may not be empty", "Invalid Name", MessageBoxButton.OK);
                    TextBox_NameUserInput.Focus();
                    TextBox_NameUserInput.SelectAll();
                    return;
                }

                // check x size
                if (int.TryParse(TextBox_XSizeUserInput.Text.ToString(), out int resultX) == false)
                {
                    // invalid number at Y Size
                    MessageBoxResult result = MessageBox.Show("Invalid Number: " + TextBox_XSizeUserInput.Text.ToString(), "Invalid X size", MessageBoxButton.OK);
                    TextBox_XSizeUserInput.Focus();
                    TextBox_XSizeUserInput.SelectAll();
                    return;
                }
                // check y size
                if (int.TryParse(TextBox_YSizeUserInput.Text.ToString(), out int resultY) == false)
                {
                    // invalid number at X Size
                    MessageBoxResult result = MessageBox.Show("Invalid Number: " + TextBox_YSizeUserInput.Text.ToString(), "Invalid Y size", MessageBoxButton.OK);
                    TextBox_YSizeUserInput.Focus();
                    TextBox_YSizeUserInput.SelectAll();
                    return;
                }
                // set level name, X- and Y size
                levelName = TextBox_NameUserInput.Text;
                ySize = resultY;
                xSize = resultX;
            }

            //Cancel button pressed
            else if(bttn.Content.ToString() == "Cancel")
            {
                levelName = null;
            }

            CloseApplication();
        }

        private void CloseApplication()
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button bttn = new Button();
                bttn.Content = "Ok";
                Button_Ok_Cancel_Click(bttn, e);
            }
        }
    }
}
