using LevelFramework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class Helper
    {
        /// <summary>
        /// Opens a open file dialog
        /// </summary>
        /// <param name="c">return which button was pressed (Ok, Cancel, Null)</param>
        /// <returns>File Dialog information</returns>
        public OpenFileDialog OpenFile(out bool? c)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = MainWindow.filter,
                Multiselect = false
            };
            c = openFileDialog.ShowDialog();
            return openFileDialog;
        }

        ///<summary>
        /// Opens a save file dialog and save current level
        /// </summary>
        /// <param name="successful">file was saved (true) or not (false)</param>
        public void SaveFile(out bool? successful)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = MainWindow.filter
            };
            successful = saveFileDialog.ShowDialog();

            if (successful == true)
            {
                if (saveFileDialog.FilterIndex == 1)
                {
                    MainWindow.LvlManager.SaveLevel(saveFileDialog.FileName, MainWindow.level);
                }
                else if (saveFileDialog.FilterIndex == 2)
                {
                    MainWindow.LvlManager.SaveLevelXML(saveFileDialog.FileName, MainWindow.level);
                }

                MainWindow.path = saveFileDialog.FileName;
                MainWindow.changed = false;
            }

        }
        ///<summary>
        /// Opens a save file dialog and save level
        /// </summary>
        /// <param name="_level">level to save</param>
        /// <param name="successful">file was saved (true) or not (false)</param>
        public void SaveFile(Level _level, out bool? successful)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = MainWindow.filter
            };
            successful = saveFileDialog.ShowDialog();

            if (successful == true)
            {
                // check Filterindex
                if (saveFileDialog.FilterIndex == 0)
                {
                    MainWindow.LvlManager.SaveLevel(saveFileDialog.FileName, _level);
                }
                else if (saveFileDialog.FilterIndex == 1)
                {
                    MainWindow.LvlManager.SaveLevelXML(saveFileDialog.FileName, _level);
                }
            }
        }

        /// <summary>
        /// check if chosen file was an URL (Does only work with internet connection).
        /// </summary>
        /// <param name="_dialog">File Dialog Information</param>
        /// <returns>true: file was an URL</returns>
        public bool FileIsURL(OpenFileDialog _dialog)
        {
            // stop when chosen file is an URL
            string temporaryInternetFilesDir = Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache);
            if (!string.IsNullOrEmpty(temporaryInternetFilesDir) &&
            _dialog.FileName.StartsWith(temporaryInternetFilesDir, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }

        /// <summary>
        /// check if chosen file was an URL (Does only work with internet connection).
        /// </summary>
        /// <param name="_fileName">Path of File</param>
        /// <returns>true: file was an URL</returns>
        public bool FileIsURL(string _fileName)
        {
            // stop when chosen file is an URL
            string temporaryInternetFilesDir = Environment.GetFolderPath(System.Environment.SpecialFolder.InternetCache);
            if (!string.IsNullOrEmpty(temporaryInternetFilesDir) &&
            _fileName.StartsWith(temporaryInternetFilesDir, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }

    }
}
