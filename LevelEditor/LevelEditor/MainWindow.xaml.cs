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
using Microsoft.Win32;
using LevelFramework;
using System.IO;

namespace LevelEditor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string filter = "Level File|*.lvl";
        public static Level level = new Level();

        private static LevelManager levelManager = new LevelManager();
        private static Helper h = new Helper();

        public MainWindow()
        {
            InitializeComponent();
        }


        #region Log
        /// <summary>
        /// Change text of Statusbar (label)
        /// </summary>
        /// <param name="_label">Label from which text shall be changed</param>
        /// <param name="_text">new Text of Label</param>
        /// <param name="_log">true if statusbar change shall be saved in log file</param>
        /// <param name="_logExtra">additional information for Log</param>
        public void SetStatus(Label _label, string _text, bool _log, params string[] _logExtra)
        {
            if (_log)
            {
                SaveToLog(_text, _logExtra);
            }

            _label.Content = _text;
        }

        /// <summary>
        /// Change text of Statusbar (label). NO LOG
        /// </summary>
        /// <param name="_label">Label from which text shall be changed</param>
        /// <param name="_text">new Text of Label</param>
        public void SetStatus(Label _label, string _text)
        {
            _label.Content = _text;
        }

        /// <summary>
        /// Save Text to log file
        /// </summary>
        /// <param name="_text">text to save</param>
        /// <param name="_logExtra">additional information for Log</param>
        public void SaveToLog(string _text, params string[] _logExtra)
        {

        }
        #endregion

        #region Header

        #region ButtonClickEvent
        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            // set status
            SetStatus(Label_StatusbarOne, "Select File...", false);

            // create and open file dialog
            OpenFileDialog file = h.OpenFile(out bool? c);

            // check result of file dialog
            if (c == true)
            {
                // check if file is an URL
                if (h.FileIsURL(file))
                {
                    SetStatus(Label_StatusbarOne, "URLs are not supported", true);
                    return;
                }

                // set level Path
                level.Path = file.FileName;
                level.directoryInfo = new DirectoryInfo(level.Path);
                SetStatus(Label_StatusbarOne, "Loading File...", true, level.Path);
            }
            // User canceled window
            else
            {
                SetStatus(Label_StatusbarOne, "Canceled", true);
                return;
            }

            // loading File
            //levelManager.LoadLevel(level.Path);

            // loading complete. set statusbar
            SetStatus(Label_StatusbarOne, "Loading Complete", true);

        }

        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion
        // --------------------------------- //



    }
}
