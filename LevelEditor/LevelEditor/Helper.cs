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
        /// Opens a file fialog
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
