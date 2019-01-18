using LevelFramework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Resources;
using System.IO;

namespace LevelEditor
{
    public class Helper
    {
        public static string RecentFilesPath { get { return Environment.CurrentDirectory + "\\RecentFiles.txt"; } }
        public static int RecentFilesAmount { get { return 10; } }

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

        /// <summary>
        /// Add path to text file. If no file exists create one
        /// </summary>
        /// <param name="_pathOfLevel">Path of level file</param>
        public void AddPathToTextFile(string _pathOfLevel)
        {
            // check if given Path exist
            if (File.Exists(_pathOfLevel) == false)
            {
                System.Windows.MessageBox.Show(
                    "File could not be found. Path will not be added to recent files",
                    "File not found",
                    System.Windows.MessageBoxButton.OK
                    );
                return;
            }

            // Create file if file not exists
            if (File.Exists(RecentFilesPath) == false)
            {
                CreateTextFile();
            }

            // open file
            StreamReader reader = new StreamReader(RecentFilesPath);

            string[] content = new string[RecentFilesAmount];

            for (int i = 0; i < RecentFilesAmount; i++)
            {
                string s = "";
                s += reader.ReadLine();

                content[i] = s;
            }
            reader.Close();

            // write to File
            System.IO.StreamWriter writer = new StreamWriter(RecentFilesPath);
            writer.WriteLine(_pathOfLevel);
            for (int arrayPos = 0; arrayPos < content.Length - 1; arrayPos++)
            {
                // if no content in string break out
                if (content[arrayPos] == null)
                {
                    break;
                }
                writer.WriteLine(content[arrayPos]);
            }
            writer.Close();
        }

        /// <summary>
        /// Read text file of level paths
        /// </summary>
        /// <returns>all saved level paths</returns>
        public string[] ReadTextFile()
        {
            // Create file if file not exists
            if (File.Exists(RecentFilesPath) == false)
            {
                CreateTextFile();
            }

            string[] content = new string[RecentFilesAmount];
            StreamReader reader = new StreamReader(RecentFilesPath);

            // Read Lines
            for (int i = 0; i < content.Length; i++)
            {
                string s = "";
                s += reader.ReadLine();

                // if string is empty break out
                if (s == "")
                {
                    break;
                }

                content[i] = s;
            }

            return content;
        }

        /// <summary>
        /// Create text file
        /// </summary>
        public void CreateTextFile()
        {
            StreamWriter s = File.CreateText(RecentFilesPath);
            s.Close();
        }

        #region Export Files if folder "Sprites" does not exist
        public void ExportFiles()
        {
            // check if Sprites folder exist
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites");
            // if folder exist return
            if (info.Exists)
            {
                return;
            }

            DirectoryInfo infoA1 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_A1\\Frames");
            DirectoryInfo infoA2 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_A2\\Frames");
            DirectoryInfo infoA3 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_A3\\Frames");
            DirectoryInfo infoA4 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_A4\\Frames");
            DirectoryInfo infoA5 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_A5\\Frames");
            DirectoryInfo infoB1 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_B\\Frames");
            DirectoryInfo infoC1 = new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_C\\Frames");

            // create folder
            info.Create();
            Properties.Resources.empty.Save(info.FullName + "\\empty.png");
            infoA1.Create();
            infoA2.Create();
            infoA3.Create();
            infoA4.Create();
            infoA5.Create();
            infoB1.Create();
            infoC1.Create();

            // copy files
            CopyA1(infoA1);
            CopyA2(infoA2);
            CopyA3(infoA3);
            CopyA4(infoA4);
            CopyA5(infoA5);
            CopyB(infoB1);
            CopyC(infoC1);

        }

        private void CopyA1(DirectoryInfo _infoA1)
        {
            Properties.Resources.A1_tile000.Save(_infoA1.FullName + "\\A1_tile000.png");
            Properties.Resources.A1_tile001.Save(_infoA1.FullName + "\\A1_tile001.png");
            Properties.Resources.A1_tile002.Save(_infoA1.FullName + "\\A1_tile002.png");
            Properties.Resources.A1_tile003.Save(_infoA1.FullName + "\\A1_tile003.png");
            Properties.Resources.A1_tile004.Save(_infoA1.FullName + "\\A1_tile004.png");
            Properties.Resources.A1_tile005.Save(_infoA1.FullName + "\\A1_tile005.png");
            Properties.Resources.A1_tile006.Save(_infoA1.FullName + "\\A1_tile006.png");
            Properties.Resources.A1_tile007.Save(_infoA1.FullName + "\\A1_tile007.png");
            Properties.Resources.A1_tile008.Save(_infoA1.FullName + "\\A1_tile008.png");
            Properties.Resources.A1_tile009.Save(_infoA1.FullName + "\\A1_tile009.png");
            Properties.Resources.A1_tile010.Save(_infoA1.FullName + "\\A1_tile010.png");
            Properties.Resources.A1_tile011.Save(_infoA1.FullName + "\\A1_tile011.png");
            Properties.Resources.A1_tile012.Save(_infoA1.FullName + "\\A1_tile012.png");
            Properties.Resources.A1_tile013.Save(_infoA1.FullName + "\\A1_tile013.png");
            Properties.Resources.A1_tile014.Save(_infoA1.FullName + "\\A1_tile014.png");
            Properties.Resources.A1_tile015.Save(_infoA1.FullName + "\\A1_tile015.png");
            Properties.Resources.A1_tile016.Save(_infoA1.FullName + "\\A1_tile016.png");
            Properties.Resources.A1_tile017.Save(_infoA1.FullName + "\\A1_tile017.png");
            Properties.Resources.A1_tile018.Save(_infoA1.FullName + "\\A1_tile018.png");
            Properties.Resources.A1_tile019.Save(_infoA1.FullName + "\\A1_tile019.png");
            Properties.Resources.A1_tile020.Save(_infoA1.FullName + "\\A1_tile020.png");
            Properties.Resources.A1_tile021.Save(_infoA1.FullName + "\\A1_tile021.png");
            Properties.Resources.A1_tile022.Save(_infoA1.FullName + "\\A1_tile022.png");
            Properties.Resources.A1_tile023.Save(_infoA1.FullName + "\\A1_tile023.png");
            Properties.Resources.A1_tile024.Save(_infoA1.FullName + "\\A1_tile024.png");
            Properties.Resources.A1_tile025.Save(_infoA1.FullName + "\\A1_tile025.png");
            Properties.Resources.A1_tile026.Save(_infoA1.FullName + "\\A1_tile026.png");
            Properties.Resources.A1_tile027.Save(_infoA1.FullName + "\\A1_tile027.png");
            Properties.Resources.A1_tile028.Save(_infoA1.FullName + "\\A1_tile028.png");
            Properties.Resources.A1_tile029.Save(_infoA1.FullName + "\\A1_tile029.png");
            Properties.Resources.A1_tile030.Save(_infoA1.FullName + "\\A1_tile030.png");
            Properties.Resources.A1_tile031.Save(_infoA1.FullName + "\\A1_tile031.png");
            Properties.Resources.A1_tile032.Save(_infoA1.FullName + "\\A1_tile032.png");
            Properties.Resources.A1_tile033.Save(_infoA1.FullName + "\\A1_tile033.png");
            Properties.Resources.A1_tile034.Save(_infoA1.FullName + "\\A1_tile034.png");
            Properties.Resources.A1_tile035.Save(_infoA1.FullName + "\\A1_tile035.png");
            Properties.Resources.A1_tile036.Save(_infoA1.FullName + "\\A1_tile036.png");
            Properties.Resources.A1_tile037.Save(_infoA1.FullName + "\\A1_tile037.png");
            Properties.Resources.A1_tile038.Save(_infoA1.FullName + "\\A1_tile038.png");
            Properties.Resources.A1_tile039.Save(_infoA1.FullName + "\\A1_tile039.png");
            Properties.Resources.A1_tile040.Save(_infoA1.FullName + "\\A1_tile040.png");
            Properties.Resources.A1_tile041.Save(_infoA1.FullName + "\\A1_tile041.png");
            Properties.Resources.A1_tile042.Save(_infoA1.FullName + "\\A1_tile042.png");
            Properties.Resources.A1_tile043.Save(_infoA1.FullName + "\\A1_tile043.png");
            Properties.Resources.A1_tile044.Save(_infoA1.FullName + "\\A1_tile044.png");
            Properties.Resources.A1_tile045.Save(_infoA1.FullName + "\\A1_tile045.png");
            Properties.Resources.A1_tile046.Save(_infoA1.FullName + "\\A1_tile046.png");
            Properties.Resources.A1_tile047.Save(_infoA1.FullName + "\\A1_tile047.png");
            Properties.Resources.A1_tile048.Save(_infoA1.FullName + "\\A1_tile048.png");
            Properties.Resources.A1_tile049.Save(_infoA1.FullName + "\\A1_tile049.png");
            Properties.Resources.A1_tile050.Save(_infoA1.FullName + "\\A1_tile050.png");  
            Properties.Resources.A1_tile051.Save(_infoA1.FullName + "\\A1_tile051.png");
            Properties.Resources.A1_tile052.Save(_infoA1.FullName + "\\A1_tile052.png");
            Properties.Resources.A1_tile053.Save(_infoA1.FullName + "\\A1_tile053.png");
            Properties.Resources.A1_tile054.Save(_infoA1.FullName + "\\A1_tile054.png");
            Properties.Resources.A1_tile055.Save(_infoA1.FullName + "\\A1_tile055.png");
            Properties.Resources.A1_tile056.Save(_infoA1.FullName + "\\A1_tile056.png");
            Properties.Resources.A1_tile057.Save(_infoA1.FullName + "\\A1_tile057.png");
            Properties.Resources.A1_tile058.Save(_infoA1.FullName + "\\A1_tile058.png");
            Properties.Resources.A1_tile059.Save(_infoA1.FullName + "\\A1_tile059.png");
            Properties.Resources.A1_tile060.Save(_infoA1.FullName + "\\A1_tile060.png");
            Properties.Resources.A1_tile061.Save(_infoA1.FullName + "\\A1_tile061.png");
            Properties.Resources.A1_tile062.Save(_infoA1.FullName + "\\A1_tile062.png");
            Properties.Resources.A1_tile063.Save(_infoA1.FullName + "\\A1_tile063.png");
            Properties.Resources.A1_tile064.Save(_infoA1.FullName + "\\A1_tile064.png");
            Properties.Resources.A1_tile065.Save(_infoA1.FullName + "\\A1_tile065.png");
            Properties.Resources.A1_tile066.Save(_infoA1.FullName + "\\A1_tile066.png");
            Properties.Resources.A1_tile067.Save(_infoA1.FullName + "\\A1_tile067.png");
            Properties.Resources.A1_tile068.Save(_infoA1.FullName + "\\A1_tile068.png");
            Properties.Resources.A1_tile069.Save(_infoA1.FullName + "\\A1_tile069.png");
            Properties.Resources.A1_tile070.Save(_infoA1.FullName + "\\A1_tile070.png");
            Properties.Resources.A1_tile071.Save(_infoA1.FullName + "\\A1_tile071.png");
            Properties.Resources.A1_tile072.Save(_infoA1.FullName + "\\A1_tile072.png");
            Properties.Resources.A1_tile073.Save(_infoA1.FullName + "\\A1_tile073.png");
            Properties.Resources.A1_tile074.Save(_infoA1.FullName + "\\A1_tile074.png");
            Properties.Resources.A1_tile075.Save(_infoA1.FullName + "\\A1_tile075.png");
            Properties.Resources.A1_tile076.Save(_infoA1.FullName + "\\A1_tile076.png");
            Properties.Resources.A1_tile077.Save(_infoA1.FullName + "\\A1_tile077.png");
            Properties.Resources.A1_tile078.Save(_infoA1.FullName + "\\A1_tile078.png");
            Properties.Resources.A1_tile079.Save(_infoA1.FullName + "\\A1_tile079.png");
            Properties.Resources.A1_tile080.Save(_infoA1.FullName + "\\A1_tile080.png");
            Properties.Resources.A1_tile081.Save(_infoA1.FullName + "\\A1_tile081.png");
            Properties.Resources.A1_tile082.Save(_infoA1.FullName + "\\A1_tile082.png");
            Properties.Resources.A1_tile083.Save(_infoA1.FullName + "\\A1_tile083.png");
            Properties.Resources.A1_tile084.Save(_infoA1.FullName + "\\A1_tile084.png");
            Properties.Resources.A1_tile085.Save(_infoA1.FullName + "\\A1_tile085.png");
            Properties.Resources.A1_tile086.Save(_infoA1.FullName + "\\A1_tile086.png");
            Properties.Resources.A1_tile087.Save(_infoA1.FullName + "\\A1_tile087.png");
            Properties.Resources.A1_tile088.Save(_infoA1.FullName + "\\A1_tile088.png");
            Properties.Resources.A1_tile089.Save(_infoA1.FullName + "\\A1_tile089.png");
            Properties.Resources.A1_tile090.Save(_infoA1.FullName + "\\A1_tile090.png");
            Properties.Resources.A1_tile091.Save(_infoA1.FullName + "\\A1_tile091.png");
            Properties.Resources.A1_tile092.Save(_infoA1.FullName + "\\A1_tile092.png");
            Properties.Resources.A1_tile093.Save(_infoA1.FullName + "\\A1_tile093.png");
            Properties.Resources.A1_tile094.Save(_infoA1.FullName + "\\A1_tile094.png");
            Properties.Resources.A1_tile095.Save(_infoA1.FullName + "\\A1_tile095.png");
            Properties.Resources.A1_tile096.Save(_infoA1.FullName + "\\A1_tile096.png");
            Properties.Resources.A1_tile097.Save(_infoA1.FullName + "\\A1_tile097.png");
            Properties.Resources.A1_tile098.Save(_infoA1.FullName + "\\A1_tile098.png");
            Properties.Resources.A1_tile099.Save(_infoA1.FullName + "\\A1_tile099.png");
            Properties.Resources.A1_tile100.Save(_infoA1.FullName + "\\A1_tile100.png");
            Properties.Resources.A1_tile101.Save(_infoA1.FullName + "\\A1_tile101.png");
            Properties.Resources.A1_tile102.Save(_infoA1.FullName + "\\A1_tile102.png");
            Properties.Resources.A1_tile103.Save(_infoA1.FullName + "\\A1_tile103.png");
            Properties.Resources.A1_tile104.Save(_infoA1.FullName + "\\A1_tile104.png");
            Properties.Resources.A1_tile105.Save(_infoA1.FullName + "\\A1_tile105.png");
            Properties.Resources.A1_tile106.Save(_infoA1.FullName + "\\A1_tile106.png");
            Properties.Resources.A1_tile107.Save(_infoA1.FullName + "\\A1_tile107.png");
            Properties.Resources.A1_tile108.Save(_infoA1.FullName + "\\A1_tile108.png");
            Properties.Resources.A1_tile109.Save(_infoA1.FullName + "\\A1_tile109.png");
            Properties.Resources.A1_tile110.Save(_infoA1.FullName + "\\A1_tile110.png");
            Properties.Resources.A1_tile111.Save(_infoA1.FullName + "\\A1_tile111.png");
            Properties.Resources.A1_tile112.Save(_infoA1.FullName + "\\A1_tile112.png");
            Properties.Resources.A1_tile113.Save(_infoA1.FullName + "\\A1_tile113.png");
            Properties.Resources.A1_tile114.Save(_infoA1.FullName + "\\A1_tile114.png");
            Properties.Resources.A1_tile115.Save(_infoA1.FullName + "\\A1_tile115.png");
            Properties.Resources.A1_tile116.Save(_infoA1.FullName + "\\A1_tile116.png");
            Properties.Resources.A1_tile117.Save(_infoA1.FullName + "\\A1_tile117.png");
            Properties.Resources.A1_tile118.Save(_infoA1.FullName + "\\A1_tile118.png");
            Properties.Resources.A1_tile119.Save(_infoA1.FullName + "\\A1_tile119.png");
            Properties.Resources.A1_tile120.Save(_infoA1.FullName + "\\A1_tile120.png");
            Properties.Resources.A1_tile121.Save(_infoA1.FullName + "\\A1_tile121.png");
            Properties.Resources.A1_tile122.Save(_infoA1.FullName + "\\A1_tile122.png");
            Properties.Resources.A1_tile123.Save(_infoA1.FullName + "\\A1_tile123.png");
            Properties.Resources.A1_tile124.Save(_infoA1.FullName + "\\A1_tile124.png");
            Properties.Resources.A1_tile125.Save(_infoA1.FullName + "\\A1_tile125.png");
            Properties.Resources.A1_tile126.Save(_infoA1.FullName + "\\A1_tile126.png");
            Properties.Resources.A1_tile127.Save(_infoA1.FullName + "\\A1_tile127.png");
            Properties.Resources.A1_tile128.Save(_infoA1.FullName + "\\A1_tile128.png");
            Properties.Resources.A1_tile129.Save(_infoA1.FullName + "\\A1_tile129.png");
            Properties.Resources.A1_tile130.Save(_infoA1.FullName + "\\A1_tile130.png");
            Properties.Resources.A1_tile131.Save(_infoA1.FullName + "\\A1_tile131.png");
            Properties.Resources.A1_tile132.Save(_infoA1.FullName + "\\A1_tile132.png");
            Properties.Resources.A1_tile133.Save(_infoA1.FullName + "\\A1_tile133.png");
            Properties.Resources.A1_tile134.Save(_infoA1.FullName + "\\A1_tile134.png");
            Properties.Resources.A1_tile135.Save(_infoA1.FullName + "\\A1_tile135.png");
            Properties.Resources.A1_tile136.Save(_infoA1.FullName + "\\A1_tile136.png");
            Properties.Resources.A1_tile137.Save(_infoA1.FullName + "\\A1_tile137.png");
            Properties.Resources.A1_tile138.Save(_infoA1.FullName + "\\A1_tile138.png");
            Properties.Resources.A1_tile139.Save(_infoA1.FullName + "\\A1_tile139.png");
            Properties.Resources.A1_tile140.Save(_infoA1.FullName + "\\A1_tile140.png");
            Properties.Resources.A1_tile141.Save(_infoA1.FullName + "\\A1_tile141.png");
            Properties.Resources.A1_tile142.Save(_infoA1.FullName + "\\A1_tile142.png");
            Properties.Resources.A1_tile143.Save(_infoA1.FullName + "\\A1_tile143.png");
            Properties.Resources.A1_tile144.Save(_infoA1.FullName + "\\A1_tile144.png");
            Properties.Resources.A1_tile145.Save(_infoA1.FullName + "\\A1_tile145.png");
            Properties.Resources.A1_tile146.Save(_infoA1.FullName + "\\A1_tile146.png");
            Properties.Resources.A1_tile147.Save(_infoA1.FullName + "\\A1_tile147.png");
            Properties.Resources.A1_tile148.Save(_infoA1.FullName + "\\A1_tile148.png");
            Properties.Resources.A1_tile149.Save(_infoA1.FullName + "\\A1_tile149.png");
            Properties.Resources.A1_tile150.Save(_infoA1.FullName + "\\A1_tile150.png");
            Properties.Resources.A1_tile151.Save(_infoA1.FullName + "\\A1_tile151.png");
            Properties.Resources.A1_tile152.Save(_infoA1.FullName + "\\A1_tile152.png");
            Properties.Resources.A1_tile153.Save(_infoA1.FullName + "\\A1_tile153.png");
            Properties.Resources.A1_tile154.Save(_infoA1.FullName + "\\A1_tile154.png");
            Properties.Resources.A1_tile155.Save(_infoA1.FullName + "\\A1_tile155.png");
            Properties.Resources.A1_tile156.Save(_infoA1.FullName + "\\A1_tile156.png");
            Properties.Resources.A1_tile157.Save(_infoA1.FullName + "\\A1_tile157.png");
            Properties.Resources.A1_tile158.Save(_infoA1.FullName + "\\A1_tile158.png");
            Properties.Resources.A1_tile159.Save(_infoA1.FullName + "\\A1_tile159.png");
            Properties.Resources.A1_tile160.Save(_infoA1.FullName + "\\A1_tile160.png");
            Properties.Resources.A1_tile161.Save(_infoA1.FullName + "\\A1_tile161.png");
            Properties.Resources.A1_tile162.Save(_infoA1.FullName + "\\A1_tile162.png");
            Properties.Resources.A1_tile163.Save(_infoA1.FullName + "\\A1_tile163.png");
            Properties.Resources.A1_tile164.Save(_infoA1.FullName + "\\A1_tile164.png");
            Properties.Resources.A1_tile165.Save(_infoA1.FullName + "\\A1_tile165.png");
            Properties.Resources.A1_tile166.Save(_infoA1.FullName + "\\A1_tile166.png");
            Properties.Resources.A1_tile167.Save(_infoA1.FullName + "\\A1_tile167.png");
            Properties.Resources.A1_tile168.Save(_infoA1.FullName + "\\A1_tile168.png");
            Properties.Resources.A1_tile169.Save(_infoA1.FullName + "\\A1_tile169.png");
            Properties.Resources.A1_tile170.Save(_infoA1.FullName + "\\A1_tile170.png");
            Properties.Resources.A1_tile171.Save(_infoA1.FullName + "\\A1_tile171.png");
            Properties.Resources.A1_tile172.Save(_infoA1.FullName + "\\A1_tile172.png");
            Properties.Resources.A1_tile173.Save(_infoA1.FullName + "\\A1_tile173.png");
            Properties.Resources.A1_tile174.Save(_infoA1.FullName + "\\A1_tile174.png");
            Properties.Resources.A1_tile175.Save(_infoA1.FullName + "\\A1_tile175.png");
            Properties.Resources.A1_tile176.Save(_infoA1.FullName + "\\A1_tile176.png");
            Properties.Resources.A1_tile177.Save(_infoA1.FullName + "\\A1_tile177.png");
            Properties.Resources.A1_tile178.Save(_infoA1.FullName + "\\A1_tile178.png");
            Properties.Resources.A1_tile179.Save(_infoA1.FullName + "\\A1_tile179.png");
            Properties.Resources.A1_tile180.Save(_infoA1.FullName + "\\A1_tile180.png");
            Properties.Resources.A1_tile181.Save(_infoA1.FullName + "\\A1_tile181.png");
            Properties.Resources.A1_tile182.Save(_infoA1.FullName + "\\A1_tile182.png");
            Properties.Resources.A1_tile183.Save(_infoA1.FullName + "\\A1_tile183.png");
            Properties.Resources.A1_tile184.Save(_infoA1.FullName + "\\A1_tile184.png");
            Properties.Resources.A1_tile185.Save(_infoA1.FullName + "\\A1_tile185.png");
            Properties.Resources.A1_tile186.Save(_infoA1.FullName + "\\A1_tile186.png");
            Properties.Resources.A1_tile187.Save(_infoA1.FullName + "\\A1_tile187.png");
            Properties.Resources.A1_tile188.Save(_infoA1.FullName + "\\A1_tile188.png");
            Properties.Resources.A1_tile189.Save(_infoA1.FullName + "\\A1_tile189.png");
            Properties.Resources.A1_tile190.Save(_infoA1.FullName + "\\A1_tile190.png");
            Properties.Resources.A1_tile191.Save(_infoA1.FullName + "\\A1_tile191.png");

        }

        private void CopyA2(DirectoryInfo _infoA2)
        {
            Properties.Resources.A2_tile000.Save(_infoA2.FullName + "\\A2_tile000.png");
            Properties.Resources.A2_tile001.Save(_infoA2.FullName + "\\A2_tile001.png");
            Properties.Resources.A2_tile002.Save(_infoA2.FullName + "\\A2_tile002.png");
            Properties.Resources.A2_tile003.Save(_infoA2.FullName + "\\A2_tile003.png");
            Properties.Resources.A2_tile004.Save(_infoA2.FullName + "\\A2_tile004.png");
            Properties.Resources.A2_tile005.Save(_infoA2.FullName + "\\A2_tile005.png");
            Properties.Resources.A2_tile006.Save(_infoA2.FullName + "\\A2_tile006.png");
            Properties.Resources.A2_tile007.Save(_infoA2.FullName + "\\A2_tile007.png");
            Properties.Resources.A2_tile008.Save(_infoA2.FullName + "\\A2_tile008.png");
            Properties.Resources.A2_tile009.Save(_infoA2.FullName + "\\A2_tile009.png");
            Properties.Resources.A2_tile010.Save(_infoA2.FullName + "\\A2_tile010.png");
            Properties.Resources.A2_tile011.Save(_infoA2.FullName + "\\A2_tile011.png");
            Properties.Resources.A2_tile012.Save(_infoA2.FullName + "\\A2_tile012.png");
            Properties.Resources.A2_tile013.Save(_infoA2.FullName + "\\A2_tile013.png");
            Properties.Resources.A2_tile014.Save(_infoA2.FullName + "\\A2_tile014.png");
            Properties.Resources.A2_tile015.Save(_infoA2.FullName + "\\A2_tile015.png");
            Properties.Resources.A2_tile016.Save(_infoA2.FullName + "\\A2_tile016.png");
            Properties.Resources.A2_tile017.Save(_infoA2.FullName + "\\A2_tile017.png");
            Properties.Resources.A2_tile018.Save(_infoA2.FullName + "\\A2_tile018.png");
            Properties.Resources.A2_tile019.Save(_infoA2.FullName + "\\A2_tile019.png");
            Properties.Resources.A2_tile020.Save(_infoA2.FullName + "\\A2_tile020.png");
            Properties.Resources.A2_tile021.Save(_infoA2.FullName + "\\A2_tile021.png");
            Properties.Resources.A2_tile022.Save(_infoA2.FullName + "\\A2_tile022.png");
            Properties.Resources.A2_tile023.Save(_infoA2.FullName + "\\A2_tile023.png");
            Properties.Resources.A2_tile024.Save(_infoA2.FullName + "\\A2_tile024.png");
            Properties.Resources.A2_tile025.Save(_infoA2.FullName + "\\A2_tile025.png");
            Properties.Resources.A2_tile026.Save(_infoA2.FullName + "\\A2_tile026.png");
            Properties.Resources.A2_tile027.Save(_infoA2.FullName + "\\A2_tile027.png");
            Properties.Resources.A2_tile028.Save(_infoA2.FullName + "\\A2_tile028.png");
            Properties.Resources.A2_tile029.Save(_infoA2.FullName + "\\A2_tile029.png");
            Properties.Resources.A2_tile030.Save(_infoA2.FullName + "\\A2_tile030.png");
            Properties.Resources.A2_tile031.Save(_infoA2.FullName + "\\A2_tile031.png");
            Properties.Resources.A2_tile032.Save(_infoA2.FullName + "\\A2_tile032.png");
            Properties.Resources.A2_tile033.Save(_infoA2.FullName + "\\A2_tile033.png");
            Properties.Resources.A2_tile034.Save(_infoA2.FullName + "\\A2_tile034.png");
            Properties.Resources.A2_tile035.Save(_infoA2.FullName + "\\A2_tile035.png");
            Properties.Resources.A2_tile036.Save(_infoA2.FullName + "\\A2_tile036.png");
            Properties.Resources.A2_tile037.Save(_infoA2.FullName + "\\A2_tile037.png");
            Properties.Resources.A2_tile038.Save(_infoA2.FullName + "\\A2_tile038.png");
            Properties.Resources.A2_tile039.Save(_infoA2.FullName + "\\A2_tile039.png");
            Properties.Resources.A2_tile040.Save(_infoA2.FullName + "\\A2_tile040.png");
            Properties.Resources.A2_tile041.Save(_infoA2.FullName + "\\A2_tile041.png");
            Properties.Resources.A2_tile042.Save(_infoA2.FullName + "\\A2_tile042.png");
            Properties.Resources.A2_tile043.Save(_infoA2.FullName + "\\A2_tile043.png");
            Properties.Resources.A2_tile044.Save(_infoA2.FullName + "\\A2_tile044.png");
            Properties.Resources.A2_tile045.Save(_infoA2.FullName + "\\A2_tile045.png");
            Properties.Resources.A2_tile046.Save(_infoA2.FullName + "\\A2_tile046.png");
            Properties.Resources.A2_tile047.Save(_infoA2.FullName + "\\A2_tile047.png");
            Properties.Resources.A2_tile048.Save(_infoA2.FullName + "\\A2_tile048.png");
            Properties.Resources.A2_tile049.Save(_infoA2.FullName + "\\A2_tile049.png");
            Properties.Resources.A2_tile050.Save(_infoA2.FullName + "\\A2_tile050.png");
            Properties.Resources.A2_tile051.Save(_infoA2.FullName + "\\A2_tile051.png");
            Properties.Resources.A2_tile052.Save(_infoA2.FullName + "\\A2_tile052.png");
            Properties.Resources.A2_tile053.Save(_infoA2.FullName + "\\A2_tile053.png");
            Properties.Resources.A2_tile054.Save(_infoA2.FullName + "\\A2_tile054.png");
            Properties.Resources.A2_tile055.Save(_infoA2.FullName + "\\A2_tile055.png");
            Properties.Resources.A2_tile056.Save(_infoA2.FullName + "\\A2_tile056.png");
            Properties.Resources.A2_tile057.Save(_infoA2.FullName + "\\A2_tile057.png");
            Properties.Resources.A2_tile058.Save(_infoA2.FullName + "\\A2_tile058.png");
            Properties.Resources.A2_tile059.Save(_infoA2.FullName + "\\A2_tile059.png");
            Properties.Resources.A2_tile060.Save(_infoA2.FullName + "\\A2_tile060.png");
            Properties.Resources.A2_tile061.Save(_infoA2.FullName + "\\A2_tile061.png");
            Properties.Resources.A2_tile062.Save(_infoA2.FullName + "\\A2_tile062.png");
            Properties.Resources.A2_tile063.Save(_infoA2.FullName + "\\A2_tile063.png");
            Properties.Resources.A2_tile064.Save(_infoA2.FullName + "\\A2_tile064.png");
            Properties.Resources.A2_tile065.Save(_infoA2.FullName + "\\A2_tile065.png");
            Properties.Resources.A2_tile066.Save(_infoA2.FullName + "\\A2_tile066.png");
            Properties.Resources.A2_tile067.Save(_infoA2.FullName + "\\A2_tile067.png");
            Properties.Resources.A2_tile068.Save(_infoA2.FullName + "\\A2_tile068.png");
            Properties.Resources.A2_tile069.Save(_infoA2.FullName + "\\A2_tile069.png");
            Properties.Resources.A2_tile070.Save(_infoA2.FullName + "\\A2_tile070.png");
            Properties.Resources.A2_tile071.Save(_infoA2.FullName + "\\A2_tile071.png");
            Properties.Resources.A2_tile072.Save(_infoA2.FullName + "\\A2_tile072.png");
            Properties.Resources.A2_tile073.Save(_infoA2.FullName + "\\A2_tile073.png");
            Properties.Resources.A2_tile074.Save(_infoA2.FullName + "\\A2_tile074.png");
            Properties.Resources.A2_tile075.Save(_infoA2.FullName + "\\A2_tile075.png");
            Properties.Resources.A2_tile076.Save(_infoA2.FullName + "\\A2_tile076.png");
            Properties.Resources.A2_tile077.Save(_infoA2.FullName + "\\A2_tile077.png");
            Properties.Resources.A2_tile078.Save(_infoA2.FullName + "\\A2_tile078.png");
            Properties.Resources.A2_tile079.Save(_infoA2.FullName + "\\A2_tile079.png");
            Properties.Resources.A2_tile080.Save(_infoA2.FullName + "\\A2_tile080.png");
            Properties.Resources.A2_tile081.Save(_infoA2.FullName + "\\A2_tile081.png");
            Properties.Resources.A2_tile082.Save(_infoA2.FullName + "\\A2_tile082.png");
            Properties.Resources.A2_tile083.Save(_infoA2.FullName + "\\A2_tile083.png");
            Properties.Resources.A2_tile084.Save(_infoA2.FullName + "\\A2_tile084.png");
            Properties.Resources.A2_tile085.Save(_infoA2.FullName + "\\A2_tile085.png");
            Properties.Resources.A2_tile086.Save(_infoA2.FullName + "\\A2_tile086.png");
            Properties.Resources.A2_tile087.Save(_infoA2.FullName + "\\A2_tile087.png");
            Properties.Resources.A2_tile088.Save(_infoA2.FullName + "\\A2_tile088.png");
            Properties.Resources.A2_tile089.Save(_infoA2.FullName + "\\A2_tile089.png");
            Properties.Resources.A2_tile090.Save(_infoA2.FullName + "\\A2_tile090.png");
            Properties.Resources.A2_tile091.Save(_infoA2.FullName + "\\A2_tile091.png");
            Properties.Resources.A2_tile092.Save(_infoA2.FullName + "\\A2_tile092.png");
            Properties.Resources.A2_tile093.Save(_infoA2.FullName + "\\A2_tile093.png");
            Properties.Resources.A2_tile094.Save(_infoA2.FullName + "\\A2_tile094.png");
            Properties.Resources.A2_tile095.Save(_infoA2.FullName + "\\A2_tile095.png");
            Properties.Resources.A2_tile096.Save(_infoA2.FullName + "\\A2_tile096.png");
            Properties.Resources.A2_tile097.Save(_infoA2.FullName + "\\A2_tile097.png");
            Properties.Resources.A2_tile098.Save(_infoA2.FullName + "\\A2_tile098.png");
            Properties.Resources.A2_tile099.Save(_infoA2.FullName + "\\A2_tile099.png");
            Properties.Resources.A2_tile100.Save(_infoA2.FullName + "\\A2_tile100.png");
            Properties.Resources.A2_tile101.Save(_infoA2.FullName + "\\A2_tile101.png");
            Properties.Resources.A2_tile102.Save(_infoA2.FullName + "\\A2_tile102.png");
            Properties.Resources.A2_tile103.Save(_infoA2.FullName + "\\A2_tile103.png");
            Properties.Resources.A2_tile104.Save(_infoA2.FullName + "\\A2_tile104.png");
            Properties.Resources.A2_tile105.Save(_infoA2.FullName + "\\A2_tile105.png");
            Properties.Resources.A2_tile106.Save(_infoA2.FullName + "\\A2_tile106.png");
            Properties.Resources.A2_tile107.Save(_infoA2.FullName + "\\A2_tile107.png");
            Properties.Resources.A2_tile108.Save(_infoA2.FullName + "\\A2_tile108.png");
            Properties.Resources.A2_tile109.Save(_infoA2.FullName + "\\A2_tile109.png");
            Properties.Resources.A2_tile110.Save(_infoA2.FullName + "\\A2_tile110.png");
            Properties.Resources.A2_tile111.Save(_infoA2.FullName + "\\A2_tile111.png");
            Properties.Resources.A2_tile112.Save(_infoA2.FullName + "\\A2_tile112.png");
            Properties.Resources.A2_tile113.Save(_infoA2.FullName + "\\A2_tile113.png");
            Properties.Resources.A2_tile114.Save(_infoA2.FullName + "\\A2_tile114.png");
            Properties.Resources.A2_tile115.Save(_infoA2.FullName + "\\A2_tile115.png");
            Properties.Resources.A2_tile116.Save(_infoA2.FullName + "\\A2_tile116.png");
            Properties.Resources.A2_tile117.Save(_infoA2.FullName + "\\A2_tile117.png");
            Properties.Resources.A2_tile118.Save(_infoA2.FullName + "\\A2_tile118.png");
            Properties.Resources.A2_tile119.Save(_infoA2.FullName + "\\A2_tile119.png");
            Properties.Resources.A2_tile120.Save(_infoA2.FullName + "\\A2_tile120.png");
            Properties.Resources.A2_tile121.Save(_infoA2.FullName + "\\A2_tile121.png");
            Properties.Resources.A2_tile122.Save(_infoA2.FullName + "\\A2_tile122.png");
            Properties.Resources.A2_tile123.Save(_infoA2.FullName + "\\A2_tile123.png");
            Properties.Resources.A2_tile124.Save(_infoA2.FullName + "\\A2_tile124.png");
            Properties.Resources.A2_tile125.Save(_infoA2.FullName + "\\A2_tile125.png");
            Properties.Resources.A2_tile126.Save(_infoA2.FullName + "\\A2_tile126.png");
            Properties.Resources.A2_tile127.Save(_infoA2.FullName + "\\A2_tile127.png");
            Properties.Resources.A2_tile128.Save(_infoA2.FullName + "\\A2_tile128.png");
            Properties.Resources.A2_tile129.Save(_infoA2.FullName + "\\A2_tile129.png");
            Properties.Resources.A2_tile130.Save(_infoA2.FullName + "\\A2_tile130.png");
            Properties.Resources.A2_tile131.Save(_infoA2.FullName + "\\A2_tile131.png");
            Properties.Resources.A2_tile132.Save(_infoA2.FullName + "\\A2_tile132.png");
            Properties.Resources.A2_tile133.Save(_infoA2.FullName + "\\A2_tile133.png");
            Properties.Resources.A2_tile134.Save(_infoA2.FullName + "\\A2_tile134.png");
            Properties.Resources.A2_tile135.Save(_infoA2.FullName + "\\A2_tile135.png");
            Properties.Resources.A2_tile136.Save(_infoA2.FullName + "\\A2_tile136.png");
            Properties.Resources.A2_tile137.Save(_infoA2.FullName + "\\A2_tile137.png");
            Properties.Resources.A2_tile138.Save(_infoA2.FullName + "\\A2_tile138.png");
            Properties.Resources.A2_tile139.Save(_infoA2.FullName + "\\A2_tile139.png");
            Properties.Resources.A2_tile140.Save(_infoA2.FullName + "\\A2_tile140.png");
            Properties.Resources.A2_tile141.Save(_infoA2.FullName + "\\A2_tile141.png");
            Properties.Resources.A2_tile142.Save(_infoA2.FullName + "\\A2_tile142.png");
            Properties.Resources.A2_tile143.Save(_infoA2.FullName + "\\A2_tile143.png");
            Properties.Resources.A2_tile144.Save(_infoA2.FullName + "\\A2_tile144.png");
            Properties.Resources.A2_tile145.Save(_infoA2.FullName + "\\A2_tile145.png");
            Properties.Resources.A2_tile146.Save(_infoA2.FullName + "\\A2_tile146.png");
            Properties.Resources.A2_tile147.Save(_infoA2.FullName + "\\A2_tile147.png");
            Properties.Resources.A2_tile148.Save(_infoA2.FullName + "\\A2_tile148.png");
            Properties.Resources.A2_tile149.Save(_infoA2.FullName + "\\A2_tile149.png");
            Properties.Resources.A2_tile150.Save(_infoA2.FullName + "\\A2_tile150.png");
            Properties.Resources.A2_tile151.Save(_infoA2.FullName + "\\A2_tile151.png");
            Properties.Resources.A2_tile152.Save(_infoA2.FullName + "\\A2_tile152.png");
            Properties.Resources.A2_tile153.Save(_infoA2.FullName + "\\A2_tile153.png");
            Properties.Resources.A2_tile154.Save(_infoA2.FullName + "\\A2_tile154.png");
            Properties.Resources.A2_tile155.Save(_infoA2.FullName + "\\A2_tile155.png");
            Properties.Resources.A2_tile156.Save(_infoA2.FullName + "\\A2_tile156.png");
            Properties.Resources.A2_tile157.Save(_infoA2.FullName + "\\A2_tile157.png");
            Properties.Resources.A2_tile158.Save(_infoA2.FullName + "\\A2_tile158.png");
            Properties.Resources.A2_tile159.Save(_infoA2.FullName + "\\A2_tile159.png");
            Properties.Resources.A2_tile160.Save(_infoA2.FullName + "\\A2_tile160.png");
            Properties.Resources.A2_tile161.Save(_infoA2.FullName + "\\A2_tile161.png");
            Properties.Resources.A2_tile162.Save(_infoA2.FullName + "\\A2_tile162.png");
            Properties.Resources.A2_tile163.Save(_infoA2.FullName + "\\A2_tile163.png");
            Properties.Resources.A2_tile164.Save(_infoA2.FullName + "\\A2_tile164.png");
            Properties.Resources.A2_tile165.Save(_infoA2.FullName + "\\A2_tile165.png");
            Properties.Resources.A2_tile166.Save(_infoA2.FullName + "\\A2_tile166.png");
            Properties.Resources.A2_tile167.Save(_infoA2.FullName + "\\A2_tile167.png");
            Properties.Resources.A2_tile168.Save(_infoA2.FullName + "\\A2_tile168.png");
            Properties.Resources.A2_tile169.Save(_infoA2.FullName + "\\A2_tile169.png");
            Properties.Resources.A2_tile170.Save(_infoA2.FullName + "\\A2_tile170.png");
            Properties.Resources.A2_tile171.Save(_infoA2.FullName + "\\A2_tile171.png");
            Properties.Resources.A2_tile172.Save(_infoA2.FullName + "\\A2_tile172.png");
            Properties.Resources.A2_tile173.Save(_infoA2.FullName + "\\A2_tile173.png");
            Properties.Resources.A2_tile174.Save(_infoA2.FullName + "\\A2_tile174.png");
            Properties.Resources.A2_tile175.Save(_infoA2.FullName + "\\A2_tile175.png");
            Properties.Resources.A2_tile176.Save(_infoA2.FullName + "\\A2_tile176.png");
            Properties.Resources.A2_tile177.Save(_infoA2.FullName + "\\A2_tile177.png");
            Properties.Resources.A2_tile178.Save(_infoA2.FullName + "\\A2_tile178.png");
            Properties.Resources.A2_tile179.Save(_infoA2.FullName + "\\A2_tile179.png");
            Properties.Resources.A2_tile180.Save(_infoA2.FullName + "\\A2_tile180.png");
            Properties.Resources.A2_tile181.Save(_infoA2.FullName + "\\A2_tile181.png");
            Properties.Resources.A2_tile182.Save(_infoA2.FullName + "\\A2_tile182.png");
            Properties.Resources.A2_tile183.Save(_infoA2.FullName + "\\A2_tile183.png");
            Properties.Resources.A2_tile184.Save(_infoA2.FullName + "\\A2_tile184.png");
            Properties.Resources.A2_tile185.Save(_infoA2.FullName + "\\A2_tile185.png");
            Properties.Resources.A2_tile186.Save(_infoA2.FullName + "\\A2_tile186.png");
            Properties.Resources.A2_tile187.Save(_infoA2.FullName + "\\A2_tile187.png");
            Properties.Resources.A2_tile188.Save(_infoA2.FullName + "\\A2_tile188.png");
            Properties.Resources.A2_tile189.Save(_infoA2.FullName + "\\A2_tile189.png");
            Properties.Resources.A2_tile190.Save(_infoA2.FullName + "\\A2_tile190.png");
            Properties.Resources.A2_tile191.Save(_infoA2.FullName + "\\A2_tile191.png");

        }

        private void CopyA3(DirectoryInfo _infoA3)
        {
            Properties.Resources.A3_tile000.Save(_infoA3.FullName + "\\A3_tile000.png");
            Properties.Resources.A3_tile001.Save(_infoA3.FullName + "\\A3_tile001.png");
            Properties.Resources.A3_tile002.Save(_infoA3.FullName + "\\A3_tile002.png");
            Properties.Resources.A3_tile003.Save(_infoA3.FullName + "\\A3_tile003.png");
            Properties.Resources.A3_tile004.Save(_infoA3.FullName + "\\A3_tile004.png");
            Properties.Resources.A3_tile005.Save(_infoA3.FullName + "\\A3_tile005.png");
            Properties.Resources.A3_tile006.Save(_infoA3.FullName + "\\A3_tile006.png");
            Properties.Resources.A3_tile007.Save(_infoA3.FullName + "\\A3_tile007.png");
            Properties.Resources.A3_tile008.Save(_infoA3.FullName + "\\A3_tile008.png");
            Properties.Resources.A3_tile009.Save(_infoA3.FullName + "\\A3_tile009.png");
            Properties.Resources.A3_tile010.Save(_infoA3.FullName + "\\A3_tile010.png");
            Properties.Resources.A3_tile011.Save(_infoA3.FullName + "\\A3_tile011.png");
            Properties.Resources.A3_tile012.Save(_infoA3.FullName + "\\A3_tile012.png");
            Properties.Resources.A3_tile013.Save(_infoA3.FullName + "\\A3_tile013.png");
            Properties.Resources.A3_tile014.Save(_infoA3.FullName + "\\A3_tile014.png");
            Properties.Resources.A3_tile015.Save(_infoA3.FullName + "\\A3_tile015.png");
            Properties.Resources.A3_tile016.Save(_infoA3.FullName + "\\A3_tile016.png");
            Properties.Resources.A3_tile017.Save(_infoA3.FullName + "\\A3_tile017.png");
            Properties.Resources.A3_tile018.Save(_infoA3.FullName + "\\A3_tile018.png");
            Properties.Resources.A3_tile019.Save(_infoA3.FullName + "\\A3_tile019.png");
            Properties.Resources.A3_tile020.Save(_infoA3.FullName + "\\A3_tile020.png");
            Properties.Resources.A3_tile021.Save(_infoA3.FullName + "\\A3_tile021.png");
            Properties.Resources.A3_tile022.Save(_infoA3.FullName + "\\A3_tile022.png");
            Properties.Resources.A3_tile023.Save(_infoA3.FullName + "\\A3_tile023.png");
            Properties.Resources.A3_tile024.Save(_infoA3.FullName + "\\A3_tile024.png");
            Properties.Resources.A3_tile025.Save(_infoA3.FullName + "\\A3_tile025.png");
            Properties.Resources.A3_tile026.Save(_infoA3.FullName + "\\A3_tile026.png");
            Properties.Resources.A3_tile027.Save(_infoA3.FullName + "\\A3_tile027.png");
            Properties.Resources.A3_tile028.Save(_infoA3.FullName + "\\A3_tile028.png");
            Properties.Resources.A3_tile029.Save(_infoA3.FullName + "\\A3_tile029.png");
            Properties.Resources.A3_tile030.Save(_infoA3.FullName + "\\A3_tile030.png");
            Properties.Resources.A3_tile031.Save(_infoA3.FullName + "\\A3_tile031.png");
            Properties.Resources.A3_tile032.Save(_infoA3.FullName + "\\A3_tile032.png");
            Properties.Resources.A3_tile033.Save(_infoA3.FullName + "\\A3_tile033.png");
            Properties.Resources.A3_tile034.Save(_infoA3.FullName + "\\A3_tile034.png");
            Properties.Resources.A3_tile035.Save(_infoA3.FullName + "\\A3_tile035.png");
            Properties.Resources.A3_tile036.Save(_infoA3.FullName + "\\A3_tile036.png");
            Properties.Resources.A3_tile037.Save(_infoA3.FullName + "\\A3_tile037.png");
            Properties.Resources.A3_tile038.Save(_infoA3.FullName + "\\A3_tile038.png");
            Properties.Resources.A3_tile039.Save(_infoA3.FullName + "\\A3_tile039.png");
            Properties.Resources.A3_tile040.Save(_infoA3.FullName + "\\A3_tile040.png");
            Properties.Resources.A3_tile041.Save(_infoA3.FullName + "\\A3_tile041.png");
            Properties.Resources.A3_tile042.Save(_infoA3.FullName + "\\A3_tile042.png");
            Properties.Resources.A3_tile043.Save(_infoA3.FullName + "\\A3_tile043.png");
            Properties.Resources.A3_tile044.Save(_infoA3.FullName + "\\A3_tile044.png");
            Properties.Resources.A3_tile045.Save(_infoA3.FullName + "\\A3_tile045.png");
            Properties.Resources.A3_tile046.Save(_infoA3.FullName + "\\A3_tile046.png");
            Properties.Resources.A3_tile047.Save(_infoA3.FullName + "\\A3_tile047.png");
            Properties.Resources.A3_tile048.Save(_infoA3.FullName + "\\A3_tile048.png");
            Properties.Resources.A3_tile049.Save(_infoA3.FullName + "\\A3_tile049.png");
            Properties.Resources.A3_tile050.Save(_infoA3.FullName + "\\A3_tile050.png");
            Properties.Resources.A3_tile051.Save(_infoA3.FullName + "\\A3_tile051.png");
            Properties.Resources.A3_tile052.Save(_infoA3.FullName + "\\A3_tile052.png");
            Properties.Resources.A3_tile053.Save(_infoA3.FullName + "\\A3_tile053.png");
            Properties.Resources.A3_tile054.Save(_infoA3.FullName + "\\A3_tile054.png");
            Properties.Resources.A3_tile055.Save(_infoA3.FullName + "\\A3_tile055.png");
            Properties.Resources.A3_tile056.Save(_infoA3.FullName + "\\A3_tile056.png");
            Properties.Resources.A3_tile057.Save(_infoA3.FullName + "\\A3_tile057.png");
            Properties.Resources.A3_tile058.Save(_infoA3.FullName + "\\A3_tile058.png");
            Properties.Resources.A3_tile059.Save(_infoA3.FullName + "\\A3_tile059.png");
            Properties.Resources.A3_tile060.Save(_infoA3.FullName + "\\A3_tile060.png");
            Properties.Resources.A3_tile061.Save(_infoA3.FullName + "\\A3_tile061.png");
            Properties.Resources.A3_tile062.Save(_infoA3.FullName + "\\A3_tile062.png");
            Properties.Resources.A3_tile063.Save(_infoA3.FullName + "\\A3_tile063.png");
            Properties.Resources.A3_tile064.Save(_infoA3.FullName + "\\A3_tile064.png");
            Properties.Resources.A3_tile065.Save(_infoA3.FullName + "\\A3_tile065.png");
            Properties.Resources.A3_tile066.Save(_infoA3.FullName + "\\A3_tile066.png");
            Properties.Resources.A3_tile067.Save(_infoA3.FullName + "\\A3_tile067.png");
            Properties.Resources.A3_tile068.Save(_infoA3.FullName + "\\A3_tile068.png");
            Properties.Resources.A3_tile069.Save(_infoA3.FullName + "\\A3_tile069.png");
            Properties.Resources.A3_tile070.Save(_infoA3.FullName + "\\A3_tile070.png");
            Properties.Resources.A3_tile071.Save(_infoA3.FullName + "\\A3_tile071.png");
            Properties.Resources.A3_tile072.Save(_infoA3.FullName + "\\A3_tile072.png");
            Properties.Resources.A3_tile073.Save(_infoA3.FullName + "\\A3_tile073.png");
            Properties.Resources.A3_tile074.Save(_infoA3.FullName + "\\A3_tile074.png");
            Properties.Resources.A3_tile075.Save(_infoA3.FullName + "\\A3_tile075.png");
            Properties.Resources.A3_tile076.Save(_infoA3.FullName + "\\A3_tile076.png");
            Properties.Resources.A3_tile077.Save(_infoA3.FullName + "\\A3_tile077.png");
            Properties.Resources.A3_tile078.Save(_infoA3.FullName + "\\A3_tile078.png");
            Properties.Resources.A3_tile079.Save(_infoA3.FullName + "\\A3_tile079.png");
            Properties.Resources.A3_tile080.Save(_infoA3.FullName + "\\A3_tile080.png");
            Properties.Resources.A3_tile081.Save(_infoA3.FullName + "\\A3_tile081.png");
            Properties.Resources.A3_tile082.Save(_infoA3.FullName + "\\A3_tile082.png");
            Properties.Resources.A3_tile083.Save(_infoA3.FullName + "\\A3_tile083.png");
            Properties.Resources.A3_tile084.Save(_infoA3.FullName + "\\A3_tile084.png");
            Properties.Resources.A3_tile085.Save(_infoA3.FullName + "\\A3_tile085.png");
            Properties.Resources.A3_tile086.Save(_infoA3.FullName + "\\A3_tile086.png");
            Properties.Resources.A3_tile087.Save(_infoA3.FullName + "\\A3_tile087.png");
            Properties.Resources.A3_tile088.Save(_infoA3.FullName + "\\A3_tile088.png");
            Properties.Resources.A3_tile089.Save(_infoA3.FullName + "\\A3_tile089.png");
            Properties.Resources.A3_tile090.Save(_infoA3.FullName + "\\A3_tile090.png");
            Properties.Resources.A3_tile091.Save(_infoA3.FullName + "\\A3_tile091.png");
            Properties.Resources.A3_tile092.Save(_infoA3.FullName + "\\A3_tile092.png");
            Properties.Resources.A3_tile093.Save(_infoA3.FullName + "\\A3_tile093.png");
            Properties.Resources.A3_tile094.Save(_infoA3.FullName + "\\A3_tile094.png");
            Properties.Resources.A3_tile095.Save(_infoA3.FullName + "\\A3_tile095.png");
            Properties.Resources.A3_tile096.Save(_infoA3.FullName + "\\A3_tile096.png");
            Properties.Resources.A3_tile097.Save(_infoA3.FullName + "\\A3_tile097.png");
            Properties.Resources.A3_tile098.Save(_infoA3.FullName + "\\A3_tile098.png");
            Properties.Resources.A3_tile099.Save(_infoA3.FullName + "\\A3_tile099.png");
            Properties.Resources.A3_tile100.Save(_infoA3.FullName + "\\A3_tile100.png");
            Properties.Resources.A3_tile101.Save(_infoA3.FullName + "\\A3_tile101.png");
            Properties.Resources.A3_tile102.Save(_infoA3.FullName + "\\A3_tile102.png");
            Properties.Resources.A3_tile103.Save(_infoA3.FullName + "\\A3_tile103.png");
            Properties.Resources.A3_tile104.Save(_infoA3.FullName + "\\A3_tile104.png");
            Properties.Resources.A3_tile105.Save(_infoA3.FullName + "\\A3_tile105.png");
            Properties.Resources.A3_tile106.Save(_infoA3.FullName + "\\A3_tile106.png");
            Properties.Resources.A3_tile107.Save(_infoA3.FullName + "\\A3_tile107.png");
            Properties.Resources.A3_tile108.Save(_infoA3.FullName + "\\A3_tile108.png");
            Properties.Resources.A3_tile109.Save(_infoA3.FullName + "\\A3_tile109.png");
            Properties.Resources.A3_tile110.Save(_infoA3.FullName + "\\A3_tile110.png");
            Properties.Resources.A3_tile111.Save(_infoA3.FullName + "\\A3_tile111.png");
            Properties.Resources.A3_tile112.Save(_infoA3.FullName + "\\A3_tile112.png");
            Properties.Resources.A3_tile113.Save(_infoA3.FullName + "\\A3_tile113.png");
            Properties.Resources.A3_tile114.Save(_infoA3.FullName + "\\A3_tile114.png");
            Properties.Resources.A3_tile115.Save(_infoA3.FullName + "\\A3_tile115.png");
            Properties.Resources.A3_tile116.Save(_infoA3.FullName + "\\A3_tile116.png");
            Properties.Resources.A3_tile117.Save(_infoA3.FullName + "\\A3_tile117.png");
            Properties.Resources.A3_tile118.Save(_infoA3.FullName + "\\A3_tile118.png");
            Properties.Resources.A3_tile119.Save(_infoA3.FullName + "\\A3_tile119.png");
            Properties.Resources.A3_tile120.Save(_infoA3.FullName + "\\A3_tile120.png");
            Properties.Resources.A3_tile121.Save(_infoA3.FullName + "\\A3_tile121.png");
            Properties.Resources.A3_tile122.Save(_infoA3.FullName + "\\A3_tile122.png");
            Properties.Resources.A3_tile123.Save(_infoA3.FullName + "\\A3_tile123.png");
            Properties.Resources.A3_tile124.Save(_infoA3.FullName + "\\A3_tile124.png");
            Properties.Resources.A3_tile125.Save(_infoA3.FullName + "\\A3_tile125.png");
            Properties.Resources.A3_tile126.Save(_infoA3.FullName + "\\A3_tile126.png");
            Properties.Resources.A3_tile127.Save(_infoA3.FullName + "\\A3_tile127.png");
            Properties.Resources.A3_tile128.Save(_infoA3.FullName + "\\A3_tile128.png");
            Properties.Resources.A3_tile129.Save(_infoA3.FullName + "\\A3_tile129.png");
            Properties.Resources.A3_tile130.Save(_infoA3.FullName + "\\A3_tile130.png");
            Properties.Resources.A3_tile131.Save(_infoA3.FullName + "\\A3_tile131.png");
            Properties.Resources.A3_tile132.Save(_infoA3.FullName + "\\A3_tile132.png");
            Properties.Resources.A3_tile133.Save(_infoA3.FullName + "\\A3_tile133.png");
            Properties.Resources.A3_tile134.Save(_infoA3.FullName + "\\A3_tile134.png");
            Properties.Resources.A3_tile135.Save(_infoA3.FullName + "\\A3_tile135.png");
            Properties.Resources.A3_tile136.Save(_infoA3.FullName + "\\A3_tile136.png");
            Properties.Resources.A3_tile137.Save(_infoA3.FullName + "\\A3_tile137.png");
            Properties.Resources.A3_tile138.Save(_infoA3.FullName + "\\A3_tile138.png");
            Properties.Resources.A3_tile139.Save(_infoA3.FullName + "\\A3_tile139.png");
            Properties.Resources.A3_tile140.Save(_infoA3.FullName + "\\A3_tile140.png");
            Properties.Resources.A3_tile141.Save(_infoA3.FullName + "\\A3_tile141.png");
            Properties.Resources.A3_tile142.Save(_infoA3.FullName + "\\A3_tile142.png");
            Properties.Resources.A3_tile143.Save(_infoA3.FullName + "\\A3_tile143.png");
            Properties.Resources.A3_tile144.Save(_infoA3.FullName + "\\A3_tile144.png");
            Properties.Resources.A3_tile145.Save(_infoA3.FullName + "\\A3_tile145.png");
            Properties.Resources.A3_tile146.Save(_infoA3.FullName + "\\A3_tile146.png");
            Properties.Resources.A3_tile147.Save(_infoA3.FullName + "\\A3_tile147.png");
            Properties.Resources.A3_tile148.Save(_infoA3.FullName + "\\A3_tile148.png");
            Properties.Resources.A3_tile149.Save(_infoA3.FullName + "\\A3_tile149.png");
            Properties.Resources.A3_tile150.Save(_infoA3.FullName + "\\A3_tile150.png");
            Properties.Resources.A3_tile151.Save(_infoA3.FullName + "\\A3_tile151.png");
            Properties.Resources.A3_tile152.Save(_infoA3.FullName + "\\A3_tile152.png");
            Properties.Resources.A3_tile153.Save(_infoA3.FullName + "\\A3_tile153.png");
            Properties.Resources.A3_tile154.Save(_infoA3.FullName + "\\A3_tile154.png");
            Properties.Resources.A3_tile155.Save(_infoA3.FullName + "\\A3_tile155.png");
            Properties.Resources.A3_tile156.Save(_infoA3.FullName + "\\A3_tile156.png");
            Properties.Resources.A3_tile157.Save(_infoA3.FullName + "\\A3_tile157.png");
            Properties.Resources.A3_tile158.Save(_infoA3.FullName + "\\A3_tile158.png");
            Properties.Resources.A3_tile159.Save(_infoA3.FullName + "\\A3_tile159.png");
            Properties.Resources.A3_tile160.Save(_infoA3.FullName + "\\A3_tile160.png");
            Properties.Resources.A3_tile161.Save(_infoA3.FullName + "\\A3_tile161.png");
            Properties.Resources.A3_tile162.Save(_infoA3.FullName + "\\A3_tile162.png");
            Properties.Resources.A3_tile163.Save(_infoA3.FullName + "\\A3_tile163.png");
            Properties.Resources.A3_tile164.Save(_infoA3.FullName + "\\A3_tile164.png");
            Properties.Resources.A3_tile165.Save(_infoA3.FullName + "\\A3_tile165.png");
            Properties.Resources.A3_tile166.Save(_infoA3.FullName + "\\A3_tile166.png");
            Properties.Resources.A3_tile167.Save(_infoA3.FullName + "\\A3_tile167.png");
            Properties.Resources.A3_tile168.Save(_infoA3.FullName + "\\A3_tile168.png");
            Properties.Resources.A3_tile169.Save(_infoA3.FullName + "\\A3_tile169.png");
            Properties.Resources.A3_tile170.Save(_infoA3.FullName + "\\A3_tile170.png");
            Properties.Resources.A3_tile171.Save(_infoA3.FullName + "\\A3_tile171.png");
            Properties.Resources.A3_tile172.Save(_infoA3.FullName + "\\A3_tile172.png");
            Properties.Resources.A3_tile173.Save(_infoA3.FullName + "\\A3_tile173.png");
            Properties.Resources.A3_tile174.Save(_infoA3.FullName + "\\A3_tile174.png");
            Properties.Resources.A3_tile175.Save(_infoA3.FullName + "\\A3_tile175.png");
            Properties.Resources.A3_tile176.Save(_infoA3.FullName + "\\A3_tile176.png");
            Properties.Resources.A3_tile177.Save(_infoA3.FullName + "\\A3_tile177.png");
            Properties.Resources.A3_tile178.Save(_infoA3.FullName + "\\A3_tile178.png");
            Properties.Resources.A3_tile179.Save(_infoA3.FullName + "\\A3_tile179.png");
            Properties.Resources.A3_tile180.Save(_infoA3.FullName + "\\A3_tile180.png");
            Properties.Resources.A3_tile181.Save(_infoA3.FullName + "\\A3_tile181.png");
            Properties.Resources.A3_tile182.Save(_infoA3.FullName + "\\A3_tile182.png");
            Properties.Resources.A3_tile183.Save(_infoA3.FullName + "\\A3_tile183.png");
            Properties.Resources.A3_tile184.Save(_infoA3.FullName + "\\A3_tile184.png");
            Properties.Resources.A3_tile185.Save(_infoA3.FullName + "\\A3_tile185.png");
            Properties.Resources.A3_tile186.Save(_infoA3.FullName + "\\A3_tile186.png");
            Properties.Resources.A3_tile187.Save(_infoA3.FullName + "\\A3_tile187.png");
            Properties.Resources.A3_tile188.Save(_infoA3.FullName + "\\A3_tile188.png");
            Properties.Resources.A3_tile189.Save(_infoA3.FullName + "\\A3_tile189.png");
            Properties.Resources.A3_tile190.Save(_infoA3.FullName + "\\A3_tile190.png");
            Properties.Resources.A3_tile191.Save(_infoA3.FullName + "\\A3_tile191.png");

        }

        private void CopyA4(DirectoryInfo _infoA4)
        {
            Properties.Resources.A4_tile000.Save(_infoA4.FullName + "\\A4_tile000.png");
            Properties.Resources.A4_tile001.Save(_infoA4.FullName + "\\A4_tile001.png");
            Properties.Resources.A4_tile002.Save(_infoA4.FullName + "\\A4_tile002.png");
            Properties.Resources.A4_tile003.Save(_infoA4.FullName + "\\A4_tile003.png");
            Properties.Resources.A4_tile004.Save(_infoA4.FullName + "\\A4_tile004.png");
            Properties.Resources.A4_tile005.Save(_infoA4.FullName + "\\A4_tile005.png");
            Properties.Resources.A4_tile006.Save(_infoA4.FullName + "\\A4_tile006.png");
            Properties.Resources.A4_tile007.Save(_infoA4.FullName + "\\A4_tile007.png");
            Properties.Resources.A4_tile008.Save(_infoA4.FullName + "\\A4_tile008.png");
            Properties.Resources.A4_tile009.Save(_infoA4.FullName + "\\A4_tile009.png");
            Properties.Resources.A4_tile010.Save(_infoA4.FullName + "\\A4_tile010.png");
            Properties.Resources.A4_tile011.Save(_infoA4.FullName + "\\A4_tile011.png");
            Properties.Resources.A4_tile012.Save(_infoA4.FullName + "\\A4_tile012.png");
            Properties.Resources.A4_tile013.Save(_infoA4.FullName + "\\A4_tile013.png");
            Properties.Resources.A4_tile014.Save(_infoA4.FullName + "\\A4_tile014.png");
            Properties.Resources.A4_tile015.Save(_infoA4.FullName + "\\A4_tile015.png");
            Properties.Resources.A4_tile016.Save(_infoA4.FullName + "\\A4_tile016.png");
            Properties.Resources.A4_tile017.Save(_infoA4.FullName + "\\A4_tile017.png");
            Properties.Resources.A4_tile018.Save(_infoA4.FullName + "\\A4_tile018.png");
            Properties.Resources.A4_tile019.Save(_infoA4.FullName + "\\A4_tile019.png");
            Properties.Resources.A4_tile020.Save(_infoA4.FullName + "\\A4_tile020.png");
            Properties.Resources.A4_tile021.Save(_infoA4.FullName + "\\A4_tile021.png");
            Properties.Resources.A4_tile022.Save(_infoA4.FullName + "\\A4_tile022.png");
            Properties.Resources.A4_tile023.Save(_infoA4.FullName + "\\A4_tile023.png");
            Properties.Resources.A4_tile024.Save(_infoA4.FullName + "\\A4_tile024.png");
            Properties.Resources.A4_tile025.Save(_infoA4.FullName + "\\A4_tile025.png");
            Properties.Resources.A4_tile026.Save(_infoA4.FullName + "\\A4_tile026.png");
            Properties.Resources.A4_tile027.Save(_infoA4.FullName + "\\A4_tile027.png");
            Properties.Resources.A4_tile028.Save(_infoA4.FullName + "\\A4_tile028.png");
            Properties.Resources.A4_tile029.Save(_infoA4.FullName + "\\A4_tile029.png");
            Properties.Resources.A4_tile030.Save(_infoA4.FullName + "\\A4_tile030.png");
            Properties.Resources.A4_tile031.Save(_infoA4.FullName + "\\A4_tile031.png");
            Properties.Resources.A4_tile032.Save(_infoA4.FullName + "\\A4_tile032.png");
            Properties.Resources.A4_tile033.Save(_infoA4.FullName + "\\A4_tile033.png");
            Properties.Resources.A4_tile034.Save(_infoA4.FullName + "\\A4_tile034.png");
            Properties.Resources.A4_tile035.Save(_infoA4.FullName + "\\A4_tile035.png");
            Properties.Resources.A4_tile036.Save(_infoA4.FullName + "\\A4_tile036.png");
            Properties.Resources.A4_tile037.Save(_infoA4.FullName + "\\A4_tile037.png");
            Properties.Resources.A4_tile038.Save(_infoA4.FullName + "\\A4_tile038.png");
            Properties.Resources.A4_tile039.Save(_infoA4.FullName + "\\A4_tile039.png");
            Properties.Resources.A4_tile040.Save(_infoA4.FullName + "\\A4_tile040.png");
            Properties.Resources.A4_tile041.Save(_infoA4.FullName + "\\A4_tile041.png");
            Properties.Resources.A4_tile042.Save(_infoA4.FullName + "\\A4_tile042.png");
            Properties.Resources.A4_tile043.Save(_infoA4.FullName + "\\A4_tile043.png");
            Properties.Resources.A4_tile044.Save(_infoA4.FullName + "\\A4_tile044.png");
            Properties.Resources.A4_tile045.Save(_infoA4.FullName + "\\A4_tile045.png");
            Properties.Resources.A4_tile046.Save(_infoA4.FullName + "\\A4_tile046.png");
            Properties.Resources.A4_tile047.Save(_infoA4.FullName + "\\A4_tile047.png");
            Properties.Resources.A4_tile048.Save(_infoA4.FullName + "\\A4_tile048.png");
            Properties.Resources.A4_tile049.Save(_infoA4.FullName + "\\A4_tile049.png");
            Properties.Resources.A4_tile050.Save(_infoA4.FullName + "\\A4_tile050.png");
            Properties.Resources.A4_tile051.Save(_infoA4.FullName + "\\A4_tile051.png");
            Properties.Resources.A4_tile052.Save(_infoA4.FullName + "\\A4_tile052.png");
            Properties.Resources.A4_tile053.Save(_infoA4.FullName + "\\A4_tile053.png");
            Properties.Resources.A4_tile054.Save(_infoA4.FullName + "\\A4_tile054.png");
            Properties.Resources.A4_tile055.Save(_infoA4.FullName + "\\A4_tile055.png");
            Properties.Resources.A4_tile056.Save(_infoA4.FullName + "\\A4_tile056.png");
            Properties.Resources.A4_tile057.Save(_infoA4.FullName + "\\A4_tile057.png");
            Properties.Resources.A4_tile058.Save(_infoA4.FullName + "\\A4_tile058.png");
            Properties.Resources.A4_tile059.Save(_infoA4.FullName + "\\A4_tile059.png");
            Properties.Resources.A4_tile060.Save(_infoA4.FullName + "\\A4_tile060.png");
            Properties.Resources.A4_tile061.Save(_infoA4.FullName + "\\A4_tile061.png");
            Properties.Resources.A4_tile062.Save(_infoA4.FullName + "\\A4_tile062.png");
            Properties.Resources.A4_tile063.Save(_infoA4.FullName + "\\A4_tile063.png");
            Properties.Resources.A4_tile064.Save(_infoA4.FullName + "\\A4_tile064.png");
            Properties.Resources.A4_tile065.Save(_infoA4.FullName + "\\A4_tile065.png");
            Properties.Resources.A4_tile066.Save(_infoA4.FullName + "\\A4_tile066.png");
            Properties.Resources.A4_tile067.Save(_infoA4.FullName + "\\A4_tile067.png");
            Properties.Resources.A4_tile068.Save(_infoA4.FullName + "\\A4_tile068.png");
            Properties.Resources.A4_tile069.Save(_infoA4.FullName + "\\A4_tile069.png");
            Properties.Resources.A4_tile070.Save(_infoA4.FullName + "\\A4_tile070.png");
            Properties.Resources.A4_tile071.Save(_infoA4.FullName + "\\A4_tile071.png");
            Properties.Resources.A4_tile072.Save(_infoA4.FullName + "\\A4_tile072.png");
            Properties.Resources.A4_tile073.Save(_infoA4.FullName + "\\A4_tile073.png");
            Properties.Resources.A4_tile074.Save(_infoA4.FullName + "\\A4_tile074.png");
            Properties.Resources.A4_tile075.Save(_infoA4.FullName + "\\A4_tile075.png");
            Properties.Resources.A4_tile076.Save(_infoA4.FullName + "\\A4_tile076.png");
            Properties.Resources.A4_tile077.Save(_infoA4.FullName + "\\A4_tile077.png");
            Properties.Resources.A4_tile078.Save(_infoA4.FullName + "\\A4_tile078.png");
            Properties.Resources.A4_tile079.Save(_infoA4.FullName + "\\A4_tile079.png");
            Properties.Resources.A4_tile080.Save(_infoA4.FullName + "\\A4_tile080.png");
            Properties.Resources.A4_tile081.Save(_infoA4.FullName + "\\A4_tile081.png");
            Properties.Resources.A4_tile082.Save(_infoA4.FullName + "\\A4_tile082.png");
            Properties.Resources.A4_tile083.Save(_infoA4.FullName + "\\A4_tile083.png");
            Properties.Resources.A4_tile084.Save(_infoA4.FullName + "\\A4_tile084.png");
            Properties.Resources.A4_tile085.Save(_infoA4.FullName + "\\A4_tile085.png");
            Properties.Resources.A4_tile086.Save(_infoA4.FullName + "\\A4_tile086.png");
            Properties.Resources.A4_tile087.Save(_infoA4.FullName + "\\A4_tile087.png");
            Properties.Resources.A4_tile088.Save(_infoA4.FullName + "\\A4_tile088.png");
            Properties.Resources.A4_tile089.Save(_infoA4.FullName + "\\A4_tile089.png");
            Properties.Resources.A4_tile090.Save(_infoA4.FullName + "\\A4_tile090.png");
            Properties.Resources.A4_tile091.Save(_infoA4.FullName + "\\A4_tile091.png");
            Properties.Resources.A4_tile092.Save(_infoA4.FullName + "\\A4_tile092.png");
            Properties.Resources.A4_tile093.Save(_infoA4.FullName + "\\A4_tile093.png");
            Properties.Resources.A4_tile094.Save(_infoA4.FullName + "\\A4_tile094.png");
            Properties.Resources.A4_tile095.Save(_infoA4.FullName + "\\A4_tile095.png");
            Properties.Resources.A4_tile096.Save(_infoA4.FullName + "\\A4_tile096.png");
            Properties.Resources.A4_tile097.Save(_infoA4.FullName + "\\A4_tile097.png");
            Properties.Resources.A4_tile098.Save(_infoA4.FullName + "\\A4_tile098.png");
            Properties.Resources.A4_tile099.Save(_infoA4.FullName + "\\A4_tile099.png");
            Properties.Resources.A4_tile100.Save(_infoA4.FullName + "\\A4_tile100.png");
            Properties.Resources.A4_tile101.Save(_infoA4.FullName + "\\A4_tile101.png");
            Properties.Resources.A4_tile102.Save(_infoA4.FullName + "\\A4_tile102.png");
            Properties.Resources.A4_tile103.Save(_infoA4.FullName + "\\A4_tile103.png");
            Properties.Resources.A4_tile104.Save(_infoA4.FullName + "\\A4_tile104.png");
            Properties.Resources.A4_tile105.Save(_infoA4.FullName + "\\A4_tile105.png");
            Properties.Resources.A4_tile106.Save(_infoA4.FullName + "\\A4_tile106.png");
            Properties.Resources.A4_tile107.Save(_infoA4.FullName + "\\A4_tile107.png");
            Properties.Resources.A4_tile108.Save(_infoA4.FullName + "\\A4_tile108.png");
            Properties.Resources.A4_tile109.Save(_infoA4.FullName + "\\A4_tile109.png");
            Properties.Resources.A4_tile110.Save(_infoA4.FullName + "\\A4_tile110.png");
            Properties.Resources.A4_tile111.Save(_infoA4.FullName + "\\A4_tile111.png");
            Properties.Resources.A4_tile112.Save(_infoA4.FullName + "\\A4_tile112.png");
            Properties.Resources.A4_tile113.Save(_infoA4.FullName + "\\A4_tile113.png");
            Properties.Resources.A4_tile114.Save(_infoA4.FullName + "\\A4_tile114.png");
            Properties.Resources.A4_tile115.Save(_infoA4.FullName + "\\A4_tile115.png");
            Properties.Resources.A4_tile116.Save(_infoA4.FullName + "\\A4_tile116.png");
            Properties.Resources.A4_tile117.Save(_infoA4.FullName + "\\A4_tile117.png");
            Properties.Resources.A4_tile118.Save(_infoA4.FullName + "\\A4_tile118.png");
            Properties.Resources.A4_tile119.Save(_infoA4.FullName + "\\A4_tile119.png");
            Properties.Resources.A4_tile120.Save(_infoA4.FullName + "\\A4_tile120.png");
            Properties.Resources.A4_tile121.Save(_infoA4.FullName + "\\A4_tile121.png");
            Properties.Resources.A4_tile122.Save(_infoA4.FullName + "\\A4_tile122.png");
            Properties.Resources.A4_tile123.Save(_infoA4.FullName + "\\A4_tile123.png");
            Properties.Resources.A4_tile124.Save(_infoA4.FullName + "\\A4_tile124.png");
            Properties.Resources.A4_tile125.Save(_infoA4.FullName + "\\A4_tile125.png");
            Properties.Resources.A4_tile126.Save(_infoA4.FullName + "\\A4_tile126.png");
            Properties.Resources.A4_tile127.Save(_infoA4.FullName + "\\A4_tile127.png");
            Properties.Resources.A4_tile128.Save(_infoA4.FullName + "\\A4_tile128.png");
            Properties.Resources.A4_tile129.Save(_infoA4.FullName + "\\A4_tile129.png");
            Properties.Resources.A4_tile130.Save(_infoA4.FullName + "\\A4_tile130.png");
            Properties.Resources.A4_tile131.Save(_infoA4.FullName + "\\A4_tile131.png");
            Properties.Resources.A4_tile132.Save(_infoA4.FullName + "\\A4_tile132.png");
            Properties.Resources.A4_tile133.Save(_infoA4.FullName + "\\A4_tile133.png");
            Properties.Resources.A4_tile134.Save(_infoA4.FullName + "\\A4_tile134.png");
            Properties.Resources.A4_tile135.Save(_infoA4.FullName + "\\A4_tile135.png");
            Properties.Resources.A4_tile136.Save(_infoA4.FullName + "\\A4_tile136.png");
            Properties.Resources.A4_tile137.Save(_infoA4.FullName + "\\A4_tile137.png");
            Properties.Resources.A4_tile138.Save(_infoA4.FullName + "\\A4_tile138.png");
            Properties.Resources.A4_tile139.Save(_infoA4.FullName + "\\A4_tile139.png");
            Properties.Resources.A4_tile140.Save(_infoA4.FullName + "\\A4_tile140.png");
            Properties.Resources.A4_tile141.Save(_infoA4.FullName + "\\A4_tile141.png");
            Properties.Resources.A4_tile142.Save(_infoA4.FullName + "\\A4_tile142.png");
            Properties.Resources.A4_tile143.Save(_infoA4.FullName + "\\A4_tile143.png");
            Properties.Resources.A4_tile144.Save(_infoA4.FullName + "\\A4_tile144.png");
            Properties.Resources.A4_tile145.Save(_infoA4.FullName + "\\A4_tile145.png");
            Properties.Resources.A4_tile146.Save(_infoA4.FullName + "\\A4_tile146.png");
            Properties.Resources.A4_tile147.Save(_infoA4.FullName + "\\A4_tile147.png");
            Properties.Resources.A4_tile148.Save(_infoA4.FullName + "\\A4_tile148.png");
            Properties.Resources.A4_tile149.Save(_infoA4.FullName + "\\A4_tile149.png");
            Properties.Resources.A4_tile150.Save(_infoA4.FullName + "\\A4_tile150.png");
            Properties.Resources.A4_tile151.Save(_infoA4.FullName + "\\A4_tile151.png");
            Properties.Resources.A4_tile152.Save(_infoA4.FullName + "\\A4_tile152.png");
            Properties.Resources.A4_tile153.Save(_infoA4.FullName + "\\A4_tile153.png");
            Properties.Resources.A4_tile154.Save(_infoA4.FullName + "\\A4_tile154.png");
            Properties.Resources.A4_tile155.Save(_infoA4.FullName + "\\A4_tile155.png");
            Properties.Resources.A4_tile156.Save(_infoA4.FullName + "\\A4_tile156.png");
            Properties.Resources.A4_tile157.Save(_infoA4.FullName + "\\A4_tile157.png");
            Properties.Resources.A4_tile158.Save(_infoA4.FullName + "\\A4_tile158.png");
            Properties.Resources.A4_tile159.Save(_infoA4.FullName + "\\A4_tile159.png");
            Properties.Resources.A4_tile160.Save(_infoA4.FullName + "\\A4_tile160.png");
            Properties.Resources.A4_tile161.Save(_infoA4.FullName + "\\A4_tile161.png");
            Properties.Resources.A4_tile162.Save(_infoA4.FullName + "\\A4_tile162.png");
            Properties.Resources.A4_tile163.Save(_infoA4.FullName + "\\A4_tile163.png");
            Properties.Resources.A4_tile164.Save(_infoA4.FullName + "\\A4_tile164.png");
            Properties.Resources.A4_tile165.Save(_infoA4.FullName + "\\A4_tile165.png");
            Properties.Resources.A4_tile166.Save(_infoA4.FullName + "\\A4_tile166.png");
            Properties.Resources.A4_tile167.Save(_infoA4.FullName + "\\A4_tile167.png");
            Properties.Resources.A4_tile168.Save(_infoA4.FullName + "\\A4_tile168.png");
            Properties.Resources.A4_tile169.Save(_infoA4.FullName + "\\A4_tile169.png");
            Properties.Resources.A4_tile170.Save(_infoA4.FullName + "\\A4_tile170.png");
            Properties.Resources.A4_tile171.Save(_infoA4.FullName + "\\A4_tile171.png");
            Properties.Resources.A4_tile172.Save(_infoA4.FullName + "\\A4_tile172.png");
            Properties.Resources.A4_tile173.Save(_infoA4.FullName + "\\A4_tile173.png");
            Properties.Resources.A4_tile174.Save(_infoA4.FullName + "\\A4_tile174.png");
            Properties.Resources.A4_tile175.Save(_infoA4.FullName + "\\A4_tile175.png");
            Properties.Resources.A4_tile176.Save(_infoA4.FullName + "\\A4_tile176.png");
            Properties.Resources.A4_tile177.Save(_infoA4.FullName + "\\A4_tile177.png");
            Properties.Resources.A4_tile178.Save(_infoA4.FullName + "\\A4_tile178.png");
            Properties.Resources.A4_tile179.Save(_infoA4.FullName + "\\A4_tile179.png");
            Properties.Resources.A4_tile180.Save(_infoA4.FullName + "\\A4_tile180.png");
            Properties.Resources.A4_tile181.Save(_infoA4.FullName + "\\A4_tile181.png");
            Properties.Resources.A4_tile182.Save(_infoA4.FullName + "\\A4_tile182.png");
            Properties.Resources.A4_tile183.Save(_infoA4.FullName + "\\A4_tile183.png");
            Properties.Resources.A4_tile184.Save(_infoA4.FullName + "\\A4_tile184.png");
            Properties.Resources.A4_tile185.Save(_infoA4.FullName + "\\A4_tile185.png");
            Properties.Resources.A4_tile186.Save(_infoA4.FullName + "\\A4_tile186.png");
            Properties.Resources.A4_tile187.Save(_infoA4.FullName + "\\A4_tile187.png");
            Properties.Resources.A4_tile188.Save(_infoA4.FullName + "\\A4_tile188.png");
            Properties.Resources.A4_tile189.Save(_infoA4.FullName + "\\A4_tile189.png");
            Properties.Resources.A4_tile190.Save(_infoA4.FullName + "\\A4_tile190.png");
            Properties.Resources.A4_tile191.Save(_infoA4.FullName + "\\A4_tile191.png");
            Properties.Resources.A4_tile192.Save(_infoA4.FullName + "\\A4_tile192.png");
            Properties.Resources.A4_tile193.Save(_infoA4.FullName + "\\A4_tile193.png");
            Properties.Resources.A4_tile194.Save(_infoA4.FullName + "\\A4_tile194.png");
            Properties.Resources.A4_tile195.Save(_infoA4.FullName + "\\A4_tile195.png");
            Properties.Resources.A4_tile196.Save(_infoA4.FullName + "\\A4_tile196.png");
            Properties.Resources.A4_tile197.Save(_infoA4.FullName + "\\A4_tile197.png");
            Properties.Resources.A4_tile198.Save(_infoA4.FullName + "\\A4_tile198.png");
            Properties.Resources.A4_tile199.Save(_infoA4.FullName + "\\A4_tile199.png");
            Properties.Resources.A4_tile200.Save(_infoA4.FullName + "\\A4_tile200.png");
            Properties.Resources.A4_tile201.Save(_infoA4.FullName + "\\A4_tile201.png");
            Properties.Resources.A4_tile202.Save(_infoA4.FullName + "\\A4_tile202.png");
            Properties.Resources.A4_tile203.Save(_infoA4.FullName + "\\A4_tile203.png");
            Properties.Resources.A4_tile204.Save(_infoA4.FullName + "\\A4_tile204.png");
            Properties.Resources.A4_tile205.Save(_infoA4.FullName + "\\A4_tile205.png");
            Properties.Resources.A4_tile206.Save(_infoA4.FullName + "\\A4_tile206.png");
            Properties.Resources.A4_tile207.Save(_infoA4.FullName + "\\A4_tile207.png");
            Properties.Resources.A4_tile208.Save(_infoA4.FullName + "\\A4_tile208.png");
            Properties.Resources.A4_tile209.Save(_infoA4.FullName + "\\A4_tile209.png");
            Properties.Resources.A4_tile210.Save(_infoA4.FullName + "\\A4_tile210.png");
            Properties.Resources.A4_tile211.Save(_infoA4.FullName + "\\A4_tile211.png");
            Properties.Resources.A4_tile212.Save(_infoA4.FullName + "\\A4_tile212.png");
            Properties.Resources.A4_tile213.Save(_infoA4.FullName + "\\A4_tile213.png");
            Properties.Resources.A4_tile214.Save(_infoA4.FullName + "\\A4_tile214.png");
            Properties.Resources.A4_tile215.Save(_infoA4.FullName + "\\A4_tile215.png");
            Properties.Resources.A4_tile216.Save(_infoA4.FullName + "\\A4_tile216.png");
            Properties.Resources.A4_tile217.Save(_infoA4.FullName + "\\A4_tile217.png");
            Properties.Resources.A4_tile218.Save(_infoA4.FullName + "\\A4_tile218.png");
            Properties.Resources.A4_tile219.Save(_infoA4.FullName + "\\A4_tile219.png");
            Properties.Resources.A4_tile220.Save(_infoA4.FullName + "\\A4_tile220.png");
            Properties.Resources.A4_tile221.Save(_infoA4.FullName + "\\A4_tile221.png");
            Properties.Resources.A4_tile222.Save(_infoA4.FullName + "\\A4_tile222.png");
            Properties.Resources.A4_tile223.Save(_infoA4.FullName + "\\A4_tile223.png");
            Properties.Resources.A4_tile224.Save(_infoA4.FullName + "\\A4_tile224.png");
            Properties.Resources.A4_tile225.Save(_infoA4.FullName + "\\A4_tile225.png");
            Properties.Resources.A4_tile226.Save(_infoA4.FullName + "\\A4_tile226.png");
            Properties.Resources.A4_tile227.Save(_infoA4.FullName + "\\A4_tile227.png");
            Properties.Resources.A4_tile228.Save(_infoA4.FullName + "\\A4_tile228.png");
            Properties.Resources.A4_tile229.Save(_infoA4.FullName + "\\A4_tile229.png");
            Properties.Resources.A4_tile230.Save(_infoA4.FullName + "\\A4_tile230.png");
            Properties.Resources.A4_tile231.Save(_infoA4.FullName + "\\A4_tile231.png");
            Properties.Resources.A4_tile232.Save(_infoA4.FullName + "\\A4_tile232.png");
            Properties.Resources.A4_tile233.Save(_infoA4.FullName + "\\A4_tile233.png");
            Properties.Resources.A4_tile234.Save(_infoA4.FullName + "\\A4_tile234.png");
            Properties.Resources.A4_tile235.Save(_infoA4.FullName + "\\A4_tile235.png");
            Properties.Resources.A4_tile236.Save(_infoA4.FullName + "\\A4_tile236.png");
            Properties.Resources.A4_tile237.Save(_infoA4.FullName + "\\A4_tile237.png");
            Properties.Resources.A4_tile238.Save(_infoA4.FullName + "\\A4_tile238.png");
            Properties.Resources.A4_tile239.Save(_infoA4.FullName + "\\A4_tile239.png");

        }

        private void CopyA5(DirectoryInfo _infoA5)
        {
            Properties.Resources.A5_tile000.Save(_infoA5.FullName + "\\A5_tile000.png");
            Properties.Resources.A5_tile001.Save(_infoA5.FullName + "\\A5_tile001.png");
            Properties.Resources.A5_tile002.Save(_infoA5.FullName + "\\A5_tile002.png");
            Properties.Resources.A5_tile003.Save(_infoA5.FullName + "\\A5_tile003.png");
            Properties.Resources.A5_tile004.Save(_infoA5.FullName + "\\A5_tile004.png");
            Properties.Resources.A5_tile005.Save(_infoA5.FullName + "\\A5_tile005.png");
            Properties.Resources.A5_tile006.Save(_infoA5.FullName + "\\A5_tile006.png");
            Properties.Resources.A5_tile007.Save(_infoA5.FullName + "\\A5_tile007.png");
            Properties.Resources.A5_tile008.Save(_infoA5.FullName + "\\A5_tile008.png");
            Properties.Resources.A5_tile009.Save(_infoA5.FullName + "\\A5_tile009.png");
            Properties.Resources.A5_tile010.Save(_infoA5.FullName + "\\A5_tile010.png");
            Properties.Resources.A5_tile011.Save(_infoA5.FullName + "\\A5_tile011.png");
            Properties.Resources.A5_tile012.Save(_infoA5.FullName + "\\A5_tile012.png");
            Properties.Resources.A5_tile013.Save(_infoA5.FullName + "\\A5_tile013.png");
            Properties.Resources.A5_tile014.Save(_infoA5.FullName + "\\A5_tile014.png");
            Properties.Resources.A5_tile015.Save(_infoA5.FullName + "\\A5_tile015.png");
            Properties.Resources.A5_tile016.Save(_infoA5.FullName + "\\A5_tile016.png");
            Properties.Resources.A5_tile017.Save(_infoA5.FullName + "\\A5_tile017.png");
            Properties.Resources.A5_tile018.Save(_infoA5.FullName + "\\A5_tile018.png");
            Properties.Resources.A5_tile019.Save(_infoA5.FullName + "\\A5_tile019.png");
            Properties.Resources.A5_tile020.Save(_infoA5.FullName + "\\A5_tile020.png");
            Properties.Resources.A5_tile021.Save(_infoA5.FullName + "\\A5_tile021.png");
            Properties.Resources.A5_tile022.Save(_infoA5.FullName + "\\A5_tile022.png");
            Properties.Resources.A5_tile023.Save(_infoA5.FullName + "\\A5_tile023.png");
            Properties.Resources.A5_tile024.Save(_infoA5.FullName + "\\A5_tile024.png");
            Properties.Resources.A5_tile025.Save(_infoA5.FullName + "\\A5_tile025.png");
            Properties.Resources.A5_tile026.Save(_infoA5.FullName + "\\A5_tile026.png");
            Properties.Resources.A5_tile027.Save(_infoA5.FullName + "\\A5_tile027.png");
            Properties.Resources.A5_tile028.Save(_infoA5.FullName + "\\A5_tile028.png");
            Properties.Resources.A5_tile029.Save(_infoA5.FullName + "\\A5_tile029.png");
            Properties.Resources.A5_tile030.Save(_infoA5.FullName + "\\A5_tile030.png");
            Properties.Resources.A5_tile031.Save(_infoA5.FullName + "\\A5_tile031.png");
            Properties.Resources.A5_tile032.Save(_infoA5.FullName + "\\A5_tile032.png");
            Properties.Resources.A5_tile033.Save(_infoA5.FullName + "\\A5_tile033.png");
            Properties.Resources.A5_tile034.Save(_infoA5.FullName + "\\A5_tile034.png");
            Properties.Resources.A5_tile035.Save(_infoA5.FullName + "\\A5_tile035.png");
            Properties.Resources.A5_tile036.Save(_infoA5.FullName + "\\A5_tile036.png");
            Properties.Resources.A5_tile037.Save(_infoA5.FullName + "\\A5_tile037.png");
            Properties.Resources.A5_tile038.Save(_infoA5.FullName + "\\A5_tile038.png");
            Properties.Resources.A5_tile039.Save(_infoA5.FullName + "\\A5_tile039.png");
            Properties.Resources.A5_tile040.Save(_infoA5.FullName + "\\A5_tile040.png");
            Properties.Resources.A5_tile041.Save(_infoA5.FullName + "\\A5_tile041.png");
            Properties.Resources.A5_tile042.Save(_infoA5.FullName + "\\A5_tile042.png");
            Properties.Resources.A5_tile043.Save(_infoA5.FullName + "\\A5_tile043.png");
            Properties.Resources.A5_tile044.Save(_infoA5.FullName + "\\A5_tile044.png");
            Properties.Resources.A5_tile045.Save(_infoA5.FullName + "\\A5_tile045.png");
            Properties.Resources.A5_tile046.Save(_infoA5.FullName + "\\A5_tile046.png");
            Properties.Resources.A5_tile047.Save(_infoA5.FullName + "\\A5_tile047.png");
            Properties.Resources.A5_tile048.Save(_infoA5.FullName + "\\A5_tile048.png");
            Properties.Resources.A5_tile049.Save(_infoA5.FullName + "\\A5_tile049.png");
            Properties.Resources.A5_tile050.Save(_infoA5.FullName + "\\A5_tile050.png");
            Properties.Resources.A5_tile051.Save(_infoA5.FullName + "\\A5_tile051.png");
            Properties.Resources.A5_tile052.Save(_infoA5.FullName + "\\A5_tile052.png");
            Properties.Resources.A5_tile053.Save(_infoA5.FullName + "\\A5_tile053.png");
            Properties.Resources.A5_tile054.Save(_infoA5.FullName + "\\A5_tile054.png");
            Properties.Resources.A5_tile055.Save(_infoA5.FullName + "\\A5_tile055.png");
            Properties.Resources.A5_tile056.Save(_infoA5.FullName + "\\A5_tile056.png");
            Properties.Resources.A5_tile057.Save(_infoA5.FullName + "\\A5_tile057.png");
            Properties.Resources.A5_tile058.Save(_infoA5.FullName + "\\A5_tile058.png");
            Properties.Resources.A5_tile059.Save(_infoA5.FullName + "\\A5_tile059.png");
            Properties.Resources.A5_tile060.Save(_infoA5.FullName + "\\A5_tile060.png");
            Properties.Resources.A5_tile061.Save(_infoA5.FullName + "\\A5_tile061.png");
            Properties.Resources.A5_tile062.Save(_infoA5.FullName + "\\A5_tile062.png");
            Properties.Resources.A5_tile063.Save(_infoA5.FullName + "\\A5_tile063.png");
            Properties.Resources.A5_tile064.Save(_infoA5.FullName + "\\A5_tile064.png");
            Properties.Resources.A5_tile065.Save(_infoA5.FullName + "\\A5_tile065.png");
            Properties.Resources.A5_tile066.Save(_infoA5.FullName + "\\A5_tile066.png");
            Properties.Resources.A5_tile067.Save(_infoA5.FullName + "\\A5_tile067.png");
            Properties.Resources.A5_tile068.Save(_infoA5.FullName + "\\A5_tile068.png");
            Properties.Resources.A5_tile069.Save(_infoA5.FullName + "\\A5_tile069.png");
            Properties.Resources.A5_tile070.Save(_infoA5.FullName + "\\A5_tile070.png");
            Properties.Resources.A5_tile071.Save(_infoA5.FullName + "\\A5_tile071.png");
            Properties.Resources.A5_tile072.Save(_infoA5.FullName + "\\A5_tile072.png");
            Properties.Resources.A5_tile073.Save(_infoA5.FullName + "\\A5_tile073.png");
            Properties.Resources.A5_tile074.Save(_infoA5.FullName + "\\A5_tile074.png");
            Properties.Resources.A5_tile075.Save(_infoA5.FullName + "\\A5_tile075.png");
            Properties.Resources.A5_tile076.Save(_infoA5.FullName + "\\A5_tile076.png");
            Properties.Resources.A5_tile077.Save(_infoA5.FullName + "\\A5_tile077.png");
            Properties.Resources.A5_tile078.Save(_infoA5.FullName + "\\A5_tile078.png");
            Properties.Resources.A5_tile079.Save(_infoA5.FullName + "\\A5_tile079.png");
            Properties.Resources.A5_tile080.Save(_infoA5.FullName + "\\A5_tile080.png");
            Properties.Resources.A5_tile081.Save(_infoA5.FullName + "\\A5_tile081.png");
            Properties.Resources.A5_tile082.Save(_infoA5.FullName + "\\A5_tile082.png");
            Properties.Resources.A5_tile083.Save(_infoA5.FullName + "\\A5_tile083.png");
            Properties.Resources.A5_tile084.Save(_infoA5.FullName + "\\A5_tile084.png");
            Properties.Resources.A5_tile085.Save(_infoA5.FullName + "\\A5_tile085.png");
            Properties.Resources.A5_tile086.Save(_infoA5.FullName + "\\A5_tile086.png");
            Properties.Resources.A5_tile087.Save(_infoA5.FullName + "\\A5_tile087.png");
            Properties.Resources.A5_tile088.Save(_infoA5.FullName + "\\A5_tile088.png");
            Properties.Resources.A5_tile089.Save(_infoA5.FullName + "\\A5_tile089.png");
            Properties.Resources.A5_tile090.Save(_infoA5.FullName + "\\A5_tile090.png");
            Properties.Resources.A5_tile091.Save(_infoA5.FullName + "\\A5_tile091.png");
            Properties.Resources.A5_tile092.Save(_infoA5.FullName + "\\A5_tile092.png");
            Properties.Resources.A5_tile093.Save(_infoA5.FullName + "\\A5_tile093.png");
            Properties.Resources.A5_tile094.Save(_infoA5.FullName + "\\A5_tile094.png");
            Properties.Resources.A5_tile095.Save(_infoA5.FullName + "\\A5_tile095.png");
            Properties.Resources.A5_tile096.Save(_infoA5.FullName + "\\A5_tile096.png");
            Properties.Resources.A5_tile097.Save(_infoA5.FullName + "\\A5_tile097.png");
            Properties.Resources.A5_tile098.Save(_infoA5.FullName + "\\A5_tile098.png");
            Properties.Resources.A5_tile099.Save(_infoA5.FullName + "\\A5_tile099.png");
            Properties.Resources.A5_tile100.Save(_infoA5.FullName + "\\A5_tile100.png");
            Properties.Resources.A5_tile101.Save(_infoA5.FullName + "\\A5_tile101.png");
            Properties.Resources.A5_tile102.Save(_infoA5.FullName + "\\A5_tile102.png");
            Properties.Resources.A5_tile103.Save(_infoA5.FullName + "\\A5_tile103.png");
            Properties.Resources.A5_tile104.Save(_infoA5.FullName + "\\A5_tile104.png");
            Properties.Resources.A5_tile105.Save(_infoA5.FullName + "\\A5_tile105.png");
            Properties.Resources.A5_tile106.Save(_infoA5.FullName + "\\A5_tile106.png");
            Properties.Resources.A5_tile107.Save(_infoA5.FullName + "\\A5_tile107.png");
            Properties.Resources.A5_tile108.Save(_infoA5.FullName + "\\A5_tile108.png");
            Properties.Resources.A5_tile109.Save(_infoA5.FullName + "\\A5_tile109.png");
            Properties.Resources.A5_tile110.Save(_infoA5.FullName + "\\A5_tile110.png");
            Properties.Resources.A5_tile111.Save(_infoA5.FullName + "\\A5_tile111.png");
            Properties.Resources.A5_tile112.Save(_infoA5.FullName + "\\A5_tile112.png");
            Properties.Resources.A5_tile113.Save(_infoA5.FullName + "\\A5_tile113.png");
            Properties.Resources.A5_tile114.Save(_infoA5.FullName + "\\A5_tile114.png");
            Properties.Resources.A5_tile115.Save(_infoA5.FullName + "\\A5_tile115.png");
            Properties.Resources.A5_tile116.Save(_infoA5.FullName + "\\A5_tile116.png");
            Properties.Resources.A5_tile117.Save(_infoA5.FullName + "\\A5_tile117.png");
            Properties.Resources.A5_tile118.Save(_infoA5.FullName + "\\A5_tile118.png");
            Properties.Resources.A5_tile119.Save(_infoA5.FullName + "\\A5_tile119.png");
            Properties.Resources.A5_tile120.Save(_infoA5.FullName + "\\A5_tile120.png");
            Properties.Resources.A5_tile121.Save(_infoA5.FullName + "\\A5_tile121.png");
            Properties.Resources.A5_tile122.Save(_infoA5.FullName + "\\A5_tile122.png");
            Properties.Resources.A5_tile123.Save(_infoA5.FullName + "\\A5_tile123.png");
            Properties.Resources.A5_tile124.Save(_infoA5.FullName + "\\A5_tile124.png");
            Properties.Resources.A5_tile125.Save(_infoA5.FullName + "\\A5_tile125.png");
            Properties.Resources.A5_tile126.Save(_infoA5.FullName + "\\A5_tile126.png");
            Properties.Resources.A5_tile127.Save(_infoA5.FullName + "\\A5_tile127.png");
        }

        private void CopyB(DirectoryInfo _infoB)
        {
            Properties.Resources.B1_tile000.Save(_infoB.FullName + "\\B1_tile000.png");
            Properties.Resources.B1_tile001.Save(_infoB.FullName + "\\B1_tile001.png");
            Properties.Resources.B1_tile002.Save(_infoB.FullName + "\\B1_tile002.png");
            Properties.Resources.B1_tile003.Save(_infoB.FullName + "\\B1_tile003.png");
            Properties.Resources.B1_tile004.Save(_infoB.FullName + "\\B1_tile004.png");
            Properties.Resources.B1_tile005.Save(_infoB.FullName + "\\B1_tile005.png");
            Properties.Resources.B1_tile006.Save(_infoB.FullName + "\\B1_tile006.png");
            Properties.Resources.B1_tile007.Save(_infoB.FullName + "\\B1_tile007.png");
            Properties.Resources.B1_tile008.Save(_infoB.FullName + "\\B1_tile008.png");
            Properties.Resources.B1_tile009.Save(_infoB.FullName + "\\B1_tile009.png");
            Properties.Resources.B1_tile010.Save(_infoB.FullName + "\\B1_tile010.png");
            Properties.Resources.B1_tile011.Save(_infoB.FullName + "\\B1_tile011.png");
            Properties.Resources.B1_tile012.Save(_infoB.FullName + "\\B1_tile012.png");
            Properties.Resources.B1_tile013.Save(_infoB.FullName + "\\B1_tile013.png");
            Properties.Resources.B1_tile014.Save(_infoB.FullName + "\\B1_tile014.png");
            Properties.Resources.B1_tile015.Save(_infoB.FullName + "\\B1_tile015.png");
            Properties.Resources.B1_tile016.Save(_infoB.FullName + "\\B1_tile016.png");
            Properties.Resources.B1_tile017.Save(_infoB.FullName + "\\B1_tile017.png");
            Properties.Resources.B1_tile018.Save(_infoB.FullName + "\\B1_tile018.png");
            Properties.Resources.B1_tile019.Save(_infoB.FullName + "\\B1_tile019.png");
            Properties.Resources.B1_tile020.Save(_infoB.FullName + "\\B1_tile020.png");
            Properties.Resources.B1_tile021.Save(_infoB.FullName + "\\B1_tile021.png");
            Properties.Resources.B1_tile022.Save(_infoB.FullName + "\\B1_tile022.png");
            Properties.Resources.B1_tile023.Save(_infoB.FullName + "\\B1_tile023.png");
            Properties.Resources.B1_tile024.Save(_infoB.FullName + "\\B1_tile024.png");
            Properties.Resources.B1_tile025.Save(_infoB.FullName + "\\B1_tile025.png");
            Properties.Resources.B1_tile026.Save(_infoB.FullName + "\\B1_tile026.png");
            Properties.Resources.B1_tile027.Save(_infoB.FullName + "\\B1_tile027.png");
            Properties.Resources.B1_tile028.Save(_infoB.FullName + "\\B1_tile028.png");
            Properties.Resources.B1_tile029.Save(_infoB.FullName + "\\B1_tile029.png");
            Properties.Resources.B1_tile030.Save(_infoB.FullName + "\\B1_tile030.png");
            Properties.Resources.B1_tile031.Save(_infoB.FullName + "\\B1_tile031.png");
            Properties.Resources.B1_tile032.Save(_infoB.FullName + "\\B1_tile032.png");
            Properties.Resources.B1_tile033.Save(_infoB.FullName + "\\B1_tile033.png");
            Properties.Resources.B1_tile034.Save(_infoB.FullName + "\\B1_tile034.png");
            Properties.Resources.B1_tile035.Save(_infoB.FullName + "\\B1_tile035.png");
            Properties.Resources.B1_tile036.Save(_infoB.FullName + "\\B1_tile036.png");
            Properties.Resources.B1_tile037.Save(_infoB.FullName + "\\B1_tile037.png");
            Properties.Resources.B1_tile038.Save(_infoB.FullName + "\\B1_tile038.png");
            Properties.Resources.B1_tile039.Save(_infoB.FullName + "\\B1_tile039.png");
            Properties.Resources.B1_tile040.Save(_infoB.FullName + "\\B1_tile040.png");
            Properties.Resources.B1_tile041.Save(_infoB.FullName + "\\B1_tile041.png");
            Properties.Resources.B1_tile042.Save(_infoB.FullName + "\\B1_tile042.png");
            Properties.Resources.B1_tile043.Save(_infoB.FullName + "\\B1_tile043.png");
            Properties.Resources.B1_tile044.Save(_infoB.FullName + "\\B1_tile044.png");
            Properties.Resources.B1_tile045.Save(_infoB.FullName + "\\B1_tile045.png");
            Properties.Resources.B1_tile046.Save(_infoB.FullName + "\\B1_tile046.png");
            Properties.Resources.B1_tile047.Save(_infoB.FullName + "\\B1_tile047.png");
            Properties.Resources.B1_tile048.Save(_infoB.FullName + "\\B1_tile048.png");
            Properties.Resources.B1_tile049.Save(_infoB.FullName + "\\B1_tile049.png");
            Properties.Resources.B1_tile050.Save(_infoB.FullName + "\\B1_tile050.png");
            Properties.Resources.B1_tile051.Save(_infoB.FullName + "\\B1_tile051.png");
            Properties.Resources.B1_tile052.Save(_infoB.FullName + "\\B1_tile052.png");
            Properties.Resources.B1_tile053.Save(_infoB.FullName + "\\B1_tile053.png");
            Properties.Resources.B1_tile054.Save(_infoB.FullName + "\\B1_tile054.png");
            Properties.Resources.B1_tile055.Save(_infoB.FullName + "\\B1_tile055.png");
            Properties.Resources.B1_tile056.Save(_infoB.FullName + "\\B1_tile056.png");
            Properties.Resources.B1_tile057.Save(_infoB.FullName + "\\B1_tile057.png");
            Properties.Resources.B1_tile058.Save(_infoB.FullName + "\\B1_tile058.png");
            Properties.Resources.B1_tile059.Save(_infoB.FullName + "\\B1_tile059.png");
            Properties.Resources.B1_tile060.Save(_infoB.FullName + "\\B1_tile060.png");
            Properties.Resources.B1_tile061.Save(_infoB.FullName + "\\B1_tile061.png");
            Properties.Resources.B1_tile062.Save(_infoB.FullName + "\\B1_tile062.png");
            Properties.Resources.B1_tile063.Save(_infoB.FullName + "\\B1_tile063.png");
            Properties.Resources.B1_tile064.Save(_infoB.FullName + "\\B1_tile064.png");
            Properties.Resources.B1_tile065.Save(_infoB.FullName + "\\B1_tile065.png");
            Properties.Resources.B1_tile066.Save(_infoB.FullName + "\\B1_tile066.png");
            Properties.Resources.B1_tile067.Save(_infoB.FullName + "\\B1_tile067.png");
            Properties.Resources.B1_tile068.Save(_infoB.FullName + "\\B1_tile068.png");
            Properties.Resources.B1_tile069.Save(_infoB.FullName + "\\B1_tile069.png");
            Properties.Resources.B1_tile070.Save(_infoB.FullName + "\\B1_tile070.png");
            Properties.Resources.B1_tile071.Save(_infoB.FullName + "\\B1_tile071.png");
            Properties.Resources.B1_tile072.Save(_infoB.FullName + "\\B1_tile072.png");
            Properties.Resources.B1_tile073.Save(_infoB.FullName + "\\B1_tile073.png");
            Properties.Resources.B1_tile074.Save(_infoB.FullName + "\\B1_tile074.png");
            Properties.Resources.B1_tile075.Save(_infoB.FullName + "\\B1_tile075.png");
            Properties.Resources.B1_tile076.Save(_infoB.FullName + "\\B1_tile076.png");
            Properties.Resources.B1_tile077.Save(_infoB.FullName + "\\B1_tile077.png");
            Properties.Resources.B1_tile078.Save(_infoB.FullName + "\\B1_tile078.png");
            Properties.Resources.B1_tile079.Save(_infoB.FullName + "\\B1_tile079.png");
            Properties.Resources.B1_tile080.Save(_infoB.FullName + "\\B1_tile080.png");
            Properties.Resources.B1_tile081.Save(_infoB.FullName + "\\B1_tile081.png");
            Properties.Resources.B1_tile082.Save(_infoB.FullName + "\\B1_tile082.png");
            Properties.Resources.B1_tile083.Save(_infoB.FullName + "\\B1_tile083.png");
            Properties.Resources.B1_tile084.Save(_infoB.FullName + "\\B1_tile084.png");
            Properties.Resources.B1_tile085.Save(_infoB.FullName + "\\B1_tile085.png");
            Properties.Resources.B1_tile086.Save(_infoB.FullName + "\\B1_tile086.png");
            Properties.Resources.B1_tile087.Save(_infoB.FullName + "\\B1_tile087.png");
            Properties.Resources.B1_tile088.Save(_infoB.FullName + "\\B1_tile088.png");
            Properties.Resources.B1_tile089.Save(_infoB.FullName + "\\B1_tile089.png");
            Properties.Resources.B1_tile090.Save(_infoB.FullName + "\\B1_tile090.png");
            Properties.Resources.B1_tile091.Save(_infoB.FullName + "\\B1_tile091.png");
            Properties.Resources.B1_tile092.Save(_infoB.FullName + "\\B1_tile092.png");
            Properties.Resources.B1_tile093.Save(_infoB.FullName + "\\B1_tile093.png");
            Properties.Resources.B1_tile094.Save(_infoB.FullName + "\\B1_tile094.png");
            Properties.Resources.B1_tile095.Save(_infoB.FullName + "\\B1_tile095.png");
            Properties.Resources.B1_tile096.Save(_infoB.FullName + "\\B1_tile096.png");
            Properties.Resources.B1_tile097.Save(_infoB.FullName + "\\B1_tile097.png");
            Properties.Resources.B1_tile098.Save(_infoB.FullName + "\\B1_tile098.png");
            Properties.Resources.B1_tile099.Save(_infoB.FullName + "\\B1_tile099.png");
            Properties.Resources.B1_tile100.Save(_infoB.FullName + "\\B1_tile100.png");
            Properties.Resources.B1_tile101.Save(_infoB.FullName + "\\B1_tile101.png");
            Properties.Resources.B1_tile102.Save(_infoB.FullName + "\\B1_tile102.png");
            Properties.Resources.B1_tile103.Save(_infoB.FullName + "\\B1_tile103.png");
            Properties.Resources.B1_tile104.Save(_infoB.FullName + "\\B1_tile104.png");
            Properties.Resources.B1_tile105.Save(_infoB.FullName + "\\B1_tile105.png");
            Properties.Resources.B1_tile106.Save(_infoB.FullName + "\\B1_tile106.png");
            Properties.Resources.B1_tile107.Save(_infoB.FullName + "\\B1_tile107.png");
            Properties.Resources.B1_tile108.Save(_infoB.FullName + "\\B1_tile108.png");
            Properties.Resources.B1_tile109.Save(_infoB.FullName + "\\B1_tile109.png");
            Properties.Resources.B1_tile110.Save(_infoB.FullName + "\\B1_tile110.png");
            Properties.Resources.B1_tile111.Save(_infoB.FullName + "\\B1_tile111.png");
            Properties.Resources.B1_tile112.Save(_infoB.FullName + "\\B1_tile112.png");
            Properties.Resources.B1_tile113.Save(_infoB.FullName + "\\B1_tile113.png");
            Properties.Resources.B1_tile114.Save(_infoB.FullName + "\\B1_tile114.png");
            Properties.Resources.B1_tile115.Save(_infoB.FullName + "\\B1_tile115.png");
            Properties.Resources.B1_tile116.Save(_infoB.FullName + "\\B1_tile116.png");
            Properties.Resources.B1_tile117.Save(_infoB.FullName + "\\B1_tile117.png");
            Properties.Resources.B1_tile118.Save(_infoB.FullName + "\\B1_tile118.png");
            Properties.Resources.B1_tile119.Save(_infoB.FullName + "\\B1_tile119.png");
            Properties.Resources.B1_tile120.Save(_infoB.FullName + "\\B1_tile120.png");
            Properties.Resources.B1_tile121.Save(_infoB.FullName + "\\B1_tile121.png");
            Properties.Resources.B1_tile122.Save(_infoB.FullName + "\\B1_tile122.png");
            Properties.Resources.B1_tile123.Save(_infoB.FullName + "\\B1_tile123.png");
            Properties.Resources.B1_tile124.Save(_infoB.FullName + "\\B1_tile124.png");
            Properties.Resources.B1_tile125.Save(_infoB.FullName + "\\B1_tile125.png");
            Properties.Resources.B1_tile126.Save(_infoB.FullName + "\\B1_tile126.png");
            Properties.Resources.B1_tile127.Save(_infoB.FullName + "\\B1_tile127.png");
            Properties.Resources.B1_tile128.Save(_infoB.FullName + "\\B1_tile128.png");
            Properties.Resources.B1_tile129.Save(_infoB.FullName + "\\B1_tile129.png");
            Properties.Resources.B1_tile130.Save(_infoB.FullName + "\\B1_tile130.png");
            Properties.Resources.B1_tile131.Save(_infoB.FullName + "\\B1_tile131.png");
            Properties.Resources.B1_tile132.Save(_infoB.FullName + "\\B1_tile132.png");
            Properties.Resources.B1_tile133.Save(_infoB.FullName + "\\B1_tile133.png");
            Properties.Resources.B1_tile134.Save(_infoB.FullName + "\\B1_tile134.png");
            Properties.Resources.B1_tile135.Save(_infoB.FullName + "\\B1_tile135.png");
            Properties.Resources.B1_tile136.Save(_infoB.FullName + "\\B1_tile136.png");
            Properties.Resources.B1_tile137.Save(_infoB.FullName + "\\B1_tile137.png");
            Properties.Resources.B1_tile138.Save(_infoB.FullName + "\\B1_tile138.png");
            Properties.Resources.B1_tile139.Save(_infoB.FullName + "\\B1_tile139.png");
            Properties.Resources.B1_tile140.Save(_infoB.FullName + "\\B1_tile140.png");
            Properties.Resources.B1_tile141.Save(_infoB.FullName + "\\B1_tile141.png");
            Properties.Resources.B1_tile142.Save(_infoB.FullName + "\\B1_tile142.png");
            Properties.Resources.B1_tile143.Save(_infoB.FullName + "\\B1_tile143.png");
            Properties.Resources.B1_tile144.Save(_infoB.FullName + "\\B1_tile144.png");
            Properties.Resources.B1_tile145.Save(_infoB.FullName + "\\B1_tile145.png");
            Properties.Resources.B1_tile146.Save(_infoB.FullName + "\\B1_tile146.png");
            Properties.Resources.B1_tile147.Save(_infoB.FullName + "\\B1_tile147.png");
            Properties.Resources.B1_tile148.Save(_infoB.FullName + "\\B1_tile148.png");
            Properties.Resources.B1_tile149.Save(_infoB.FullName + "\\B1_tile149.png");
            Properties.Resources.B1_tile150.Save(_infoB.FullName + "\\B1_tile150.png");
            Properties.Resources.B1_tile151.Save(_infoB.FullName + "\\B1_tile151.png");
            Properties.Resources.B1_tile152.Save(_infoB.FullName + "\\B1_tile152.png");
            Properties.Resources.B1_tile153.Save(_infoB.FullName + "\\B1_tile153.png");
            Properties.Resources.B1_tile154.Save(_infoB.FullName + "\\B1_tile154.png");
            Properties.Resources.B1_tile155.Save(_infoB.FullName + "\\B1_tile155.png");
            Properties.Resources.B1_tile156.Save(_infoB.FullName + "\\B1_tile156.png");
            Properties.Resources.B1_tile157.Save(_infoB.FullName + "\\B1_tile157.png");
            Properties.Resources.B1_tile158.Save(_infoB.FullName + "\\B1_tile158.png");
            Properties.Resources.B1_tile159.Save(_infoB.FullName + "\\B1_tile159.png");
            Properties.Resources.B1_tile160.Save(_infoB.FullName + "\\B1_tile160.png");
            Properties.Resources.B1_tile161.Save(_infoB.FullName + "\\B1_tile161.png");
            Properties.Resources.B1_tile162.Save(_infoB.FullName + "\\B1_tile162.png");
            Properties.Resources.B1_tile163.Save(_infoB.FullName + "\\B1_tile163.png");
            Properties.Resources.B1_tile164.Save(_infoB.FullName + "\\B1_tile164.png");
            Properties.Resources.B1_tile165.Save(_infoB.FullName + "\\B1_tile165.png");
            Properties.Resources.B1_tile166.Save(_infoB.FullName + "\\B1_tile166.png");
            Properties.Resources.B1_tile167.Save(_infoB.FullName + "\\B1_tile167.png");
            Properties.Resources.B1_tile168.Save(_infoB.FullName + "\\B1_tile168.png");
            Properties.Resources.B1_tile169.Save(_infoB.FullName + "\\B1_tile169.png");
            Properties.Resources.B1_tile170.Save(_infoB.FullName + "\\B1_tile170.png");
            Properties.Resources.B1_tile171.Save(_infoB.FullName + "\\B1_tile171.png");
            Properties.Resources.B1_tile172.Save(_infoB.FullName + "\\B1_tile172.png");
            Properties.Resources.B1_tile173.Save(_infoB.FullName + "\\B1_tile173.png");
            Properties.Resources.B1_tile174.Save(_infoB.FullName + "\\B1_tile174.png");
            Properties.Resources.B1_tile175.Save(_infoB.FullName + "\\B1_tile175.png");
            Properties.Resources.B1_tile176.Save(_infoB.FullName + "\\B1_tile176.png");
            Properties.Resources.B1_tile177.Save(_infoB.FullName + "\\B1_tile177.png");
            Properties.Resources.B1_tile178.Save(_infoB.FullName + "\\B1_tile178.png");
            Properties.Resources.B1_tile179.Save(_infoB.FullName + "\\B1_tile179.png");
            Properties.Resources.B1_tile180.Save(_infoB.FullName + "\\B1_tile180.png");
            Properties.Resources.B1_tile181.Save(_infoB.FullName + "\\B1_tile181.png");
            Properties.Resources.B1_tile182.Save(_infoB.FullName + "\\B1_tile182.png");
            Properties.Resources.B1_tile183.Save(_infoB.FullName + "\\B1_tile183.png");
            Properties.Resources.B1_tile184.Save(_infoB.FullName + "\\B1_tile184.png");
            Properties.Resources.B1_tile185.Save(_infoB.FullName + "\\B1_tile185.png");
            Properties.Resources.B1_tile186.Save(_infoB.FullName + "\\B1_tile186.png");
            Properties.Resources.B1_tile187.Save(_infoB.FullName + "\\B1_tile187.png");
            Properties.Resources.B1_tile188.Save(_infoB.FullName + "\\B1_tile188.png");
            Properties.Resources.B1_tile189.Save(_infoB.FullName + "\\B1_tile189.png");
            Properties.Resources.B1_tile190.Save(_infoB.FullName + "\\B1_tile190.png");
            Properties.Resources.B1_tile191.Save(_infoB.FullName + "\\B1_tile191.png");
            Properties.Resources.B1_tile192.Save(_infoB.FullName + "\\B1_tile192.png");
            Properties.Resources.B1_tile193.Save(_infoB.FullName + "\\B1_tile193.png");
            Properties.Resources.B1_tile194.Save(_infoB.FullName + "\\B1_tile194.png");
            Properties.Resources.B1_tile195.Save(_infoB.FullName + "\\B1_tile195.png");
            Properties.Resources.B1_tile196.Save(_infoB.FullName + "\\B1_tile196.png");
            Properties.Resources.B1_tile197.Save(_infoB.FullName + "\\B1_tile197.png");
            Properties.Resources.B1_tile198.Save(_infoB.FullName + "\\B1_tile198.png");
            Properties.Resources.B1_tile199.Save(_infoB.FullName + "\\B1_tile199.png");
            Properties.Resources.B1_tile200.Save(_infoB.FullName + "\\B1_tile200.png");
            Properties.Resources.B1_tile201.Save(_infoB.FullName + "\\B1_tile201.png");
            Properties.Resources.B1_tile202.Save(_infoB.FullName + "\\B1_tile202.png");
            Properties.Resources.B1_tile203.Save(_infoB.FullName + "\\B1_tile203.png");
            Properties.Resources.B1_tile204.Save(_infoB.FullName + "\\B1_tile204.png");
            Properties.Resources.B1_tile205.Save(_infoB.FullName + "\\B1_tile205.png");
            Properties.Resources.B1_tile206.Save(_infoB.FullName + "\\B1_tile206.png");
            Properties.Resources.B1_tile207.Save(_infoB.FullName + "\\B1_tile207.png");
            Properties.Resources.B1_tile208.Save(_infoB.FullName + "\\B1_tile208.png");
            Properties.Resources.B1_tile209.Save(_infoB.FullName + "\\B1_tile209.png");
            Properties.Resources.B1_tile210.Save(_infoB.FullName + "\\B1_tile210.png");
            Properties.Resources.B1_tile211.Save(_infoB.FullName + "\\B1_tile211.png");
            Properties.Resources.B1_tile212.Save(_infoB.FullName + "\\B1_tile212.png");
            Properties.Resources.B1_tile213.Save(_infoB.FullName + "\\B1_tile213.png");
            Properties.Resources.B1_tile214.Save(_infoB.FullName + "\\B1_tile214.png");
            Properties.Resources.B1_tile215.Save(_infoB.FullName + "\\B1_tile215.png");
            Properties.Resources.B1_tile216.Save(_infoB.FullName + "\\B1_tile216.png");
            Properties.Resources.B1_tile217.Save(_infoB.FullName + "\\B1_tile217.png");
            Properties.Resources.B1_tile218.Save(_infoB.FullName + "\\B1_tile218.png");
            Properties.Resources.B1_tile219.Save(_infoB.FullName + "\\B1_tile219.png");
            Properties.Resources.B1_tile220.Save(_infoB.FullName + "\\B1_tile220.png");
            Properties.Resources.B1_tile221.Save(_infoB.FullName + "\\B1_tile221.png");
            Properties.Resources.B1_tile222.Save(_infoB.FullName + "\\B1_tile222.png");
            Properties.Resources.B1_tile223.Save(_infoB.FullName + "\\B1_tile223.png");
            Properties.Resources.B1_tile224.Save(_infoB.FullName + "\\B1_tile224.png");
            Properties.Resources.B1_tile225.Save(_infoB.FullName + "\\B1_tile225.png");
            Properties.Resources.B1_tile226.Save(_infoB.FullName + "\\B1_tile226.png");
            Properties.Resources.B1_tile227.Save(_infoB.FullName + "\\B1_tile227.png");
            Properties.Resources.B1_tile228.Save(_infoB.FullName + "\\B1_tile228.png");
            Properties.Resources.B1_tile229.Save(_infoB.FullName + "\\B1_tile229.png");
            Properties.Resources.B1_tile230.Save(_infoB.FullName + "\\B1_tile230.png");
            Properties.Resources.B1_tile231.Save(_infoB.FullName + "\\B1_tile231.png");
            Properties.Resources.B1_tile232.Save(_infoB.FullName + "\\B1_tile232.png");
            Properties.Resources.B1_tile233.Save(_infoB.FullName + "\\B1_tile233.png");
            Properties.Resources.B1_tile234.Save(_infoB.FullName + "\\B1_tile234.png");
            Properties.Resources.B1_tile235.Save(_infoB.FullName + "\\B1_tile235.png");
            Properties.Resources.B1_tile236.Save(_infoB.FullName + "\\B1_tile236.png");
            Properties.Resources.B1_tile237.Save(_infoB.FullName + "\\B1_tile237.png");
            Properties.Resources.B1_tile238.Save(_infoB.FullName + "\\B1_tile238.png");
            Properties.Resources.B1_tile239.Save(_infoB.FullName + "\\B1_tile239.png");
            Properties.Resources.B1_tile240.Save(_infoB.FullName + "\\B1_tile240.png");
            Properties.Resources.B1_tile241.Save(_infoB.FullName + "\\B1_tile241.png");
            Properties.Resources.B1_tile242.Save(_infoB.FullName + "\\B1_tile242.png");
            Properties.Resources.B1_tile243.Save(_infoB.FullName + "\\B1_tile243.png");
            Properties.Resources.B1_tile244.Save(_infoB.FullName + "\\B1_tile244.png");
            Properties.Resources.B1_tile245.Save(_infoB.FullName + "\\B1_tile245.png");
            Properties.Resources.B1_tile246.Save(_infoB.FullName + "\\B1_tile246.png");
            Properties.Resources.B1_tile247.Save(_infoB.FullName + "\\B1_tile247.png");
            Properties.Resources.B1_tile248.Save(_infoB.FullName + "\\B1_tile248.png");
            Properties.Resources.B1_tile249.Save(_infoB.FullName + "\\B1_tile249.png");
            Properties.Resources.B1_tile250.Save(_infoB.FullName + "\\B1_tile250.png");
            Properties.Resources.B1_tile251.Save(_infoB.FullName + "\\B1_tile251.png");
            Properties.Resources.B1_tile252.Save(_infoB.FullName + "\\B1_tile252.png");
            Properties.Resources.B1_tile253.Save(_infoB.FullName + "\\B1_tile253.png");
            Properties.Resources.B1_tile254.Save(_infoB.FullName + "\\B1_tile254.png");
            Properties.Resources.B1_tile255.Save(_infoB.FullName + "\\B1_tile255.png");

        }

        private void CopyC(DirectoryInfo _infoC)
        {
            Properties.Resources.C1_tile000.Save(_infoC.FullName + "\\C1_tile000.png");
            Properties.Resources.C1_tile001.Save(_infoC.FullName + "\\C1_tile001.png");
            Properties.Resources.C1_tile002.Save(_infoC.FullName + "\\C1_tile002.png");
            Properties.Resources.C1_tile003.Save(_infoC.FullName + "\\C1_tile003.png");
            Properties.Resources.C1_tile004.Save(_infoC.FullName + "\\C1_tile004.png");
            Properties.Resources.C1_tile005.Save(_infoC.FullName + "\\C1_tile005.png");
            Properties.Resources.C1_tile006.Save(_infoC.FullName + "\\C1_tile006.png");
            Properties.Resources.C1_tile007.Save(_infoC.FullName + "\\C1_tile007.png");
            Properties.Resources.C1_tile008.Save(_infoC.FullName + "\\C1_tile008.png");
            Properties.Resources.C1_tile009.Save(_infoC.FullName + "\\C1_tile009.png");
            Properties.Resources.C1_tile010.Save(_infoC.FullName + "\\C1_tile010.png");
            Properties.Resources.C1_tile011.Save(_infoC.FullName + "\\C1_tile011.png");
            Properties.Resources.C1_tile012.Save(_infoC.FullName + "\\C1_tile012.png");
            Properties.Resources.C1_tile013.Save(_infoC.FullName + "\\C1_tile013.png");
            Properties.Resources.C1_tile014.Save(_infoC.FullName + "\\C1_tile014.png");
            Properties.Resources.C1_tile015.Save(_infoC.FullName + "\\C1_tile015.png");
            Properties.Resources.C1_tile016.Save(_infoC.FullName + "\\C1_tile016.png");
            Properties.Resources.C1_tile017.Save(_infoC.FullName + "\\C1_tile017.png");
            Properties.Resources.C1_tile018.Save(_infoC.FullName + "\\C1_tile018.png");
            Properties.Resources.C1_tile019.Save(_infoC.FullName + "\\C1_tile019.png");
            Properties.Resources.C1_tile020.Save(_infoC.FullName + "\\C1_tile020.png");
            Properties.Resources.C1_tile021.Save(_infoC.FullName + "\\C1_tile021.png");
            Properties.Resources.C1_tile022.Save(_infoC.FullName + "\\C1_tile022.png");
            Properties.Resources.C1_tile023.Save(_infoC.FullName + "\\C1_tile023.png");
            Properties.Resources.C1_tile024.Save(_infoC.FullName + "\\C1_tile024.png");
            Properties.Resources.C1_tile025.Save(_infoC.FullName + "\\C1_tile025.png");
            Properties.Resources.C1_tile026.Save(_infoC.FullName + "\\C1_tile026.png");
            Properties.Resources.C1_tile027.Save(_infoC.FullName + "\\C1_tile027.png");
            Properties.Resources.C1_tile028.Save(_infoC.FullName + "\\C1_tile028.png");
            Properties.Resources.C1_tile029.Save(_infoC.FullName + "\\C1_tile029.png");
            Properties.Resources.C1_tile030.Save(_infoC.FullName + "\\C1_tile030.png");
            Properties.Resources.C1_tile031.Save(_infoC.FullName + "\\C1_tile031.png");
            Properties.Resources.C1_tile032.Save(_infoC.FullName + "\\C1_tile032.png");
            Properties.Resources.C1_tile033.Save(_infoC.FullName + "\\C1_tile033.png");
            Properties.Resources.C1_tile034.Save(_infoC.FullName + "\\C1_tile034.png");
            Properties.Resources.C1_tile035.Save(_infoC.FullName + "\\C1_tile035.png");
            Properties.Resources.C1_tile036.Save(_infoC.FullName + "\\C1_tile036.png");
            Properties.Resources.C1_tile037.Save(_infoC.FullName + "\\C1_tile037.png");
            Properties.Resources.C1_tile038.Save(_infoC.FullName + "\\C1_tile038.png");
            Properties.Resources.C1_tile039.Save(_infoC.FullName + "\\C1_tile039.png");
            Properties.Resources.C1_tile040.Save(_infoC.FullName + "\\C1_tile040.png");
            Properties.Resources.C1_tile041.Save(_infoC.FullName + "\\C1_tile041.png");
            Properties.Resources.C1_tile042.Save(_infoC.FullName + "\\C1_tile042.png");
            Properties.Resources.C1_tile043.Save(_infoC.FullName + "\\C1_tile043.png");
            Properties.Resources.C1_tile044.Save(_infoC.FullName + "\\C1_tile044.png");
            Properties.Resources.C1_tile045.Save(_infoC.FullName + "\\C1_tile045.png");
            Properties.Resources.C1_tile046.Save(_infoC.FullName + "\\C1_tile046.png");
            Properties.Resources.C1_tile047.Save(_infoC.FullName + "\\C1_tile047.png");
            Properties.Resources.C1_tile048.Save(_infoC.FullName + "\\C1_tile048.png");
            Properties.Resources.C1_tile049.Save(_infoC.FullName + "\\C1_tile049.png");
            Properties.Resources.C1_tile050.Save(_infoC.FullName + "\\C1_tile050.png");
            Properties.Resources.C1_tile051.Save(_infoC.FullName + "\\C1_tile051.png");
            Properties.Resources.C1_tile052.Save(_infoC.FullName + "\\C1_tile052.png");
            Properties.Resources.C1_tile053.Save(_infoC.FullName + "\\C1_tile053.png");
            Properties.Resources.C1_tile054.Save(_infoC.FullName + "\\C1_tile054.png");
            Properties.Resources.C1_tile055.Save(_infoC.FullName + "\\C1_tile055.png");
            Properties.Resources.C1_tile056.Save(_infoC.FullName + "\\C1_tile056.png");
            Properties.Resources.C1_tile057.Save(_infoC.FullName + "\\C1_tile057.png");
            Properties.Resources.C1_tile058.Save(_infoC.FullName + "\\C1_tile058.png");
            Properties.Resources.C1_tile059.Save(_infoC.FullName + "\\C1_tile059.png");
            Properties.Resources.C1_tile060.Save(_infoC.FullName + "\\C1_tile060.png");
            Properties.Resources.C1_tile061.Save(_infoC.FullName + "\\C1_tile061.png");
            Properties.Resources.C1_tile062.Save(_infoC.FullName + "\\C1_tile062.png");
            Properties.Resources.C1_tile063.Save(_infoC.FullName + "\\C1_tile063.png");
            Properties.Resources.C1_tile064.Save(_infoC.FullName + "\\C1_tile064.png");
            Properties.Resources.C1_tile065.Save(_infoC.FullName + "\\C1_tile065.png");
            Properties.Resources.C1_tile066.Save(_infoC.FullName + "\\C1_tile066.png");
            Properties.Resources.C1_tile067.Save(_infoC.FullName + "\\C1_tile067.png");
            Properties.Resources.C1_tile068.Save(_infoC.FullName + "\\C1_tile068.png");
            Properties.Resources.C1_tile069.Save(_infoC.FullName + "\\C1_tile069.png");
            Properties.Resources.C1_tile070.Save(_infoC.FullName + "\\C1_tile070.png");
            Properties.Resources.C1_tile071.Save(_infoC.FullName + "\\C1_tile071.png");
            Properties.Resources.C1_tile072.Save(_infoC.FullName + "\\C1_tile072.png");
            Properties.Resources.C1_tile073.Save(_infoC.FullName + "\\C1_tile073.png");
            Properties.Resources.C1_tile074.Save(_infoC.FullName + "\\C1_tile074.png");
            Properties.Resources.C1_tile075.Save(_infoC.FullName + "\\C1_tile075.png");
            Properties.Resources.C1_tile076.Save(_infoC.FullName + "\\C1_tile076.png");
            Properties.Resources.C1_tile077.Save(_infoC.FullName + "\\C1_tile077.png");
            Properties.Resources.C1_tile078.Save(_infoC.FullName + "\\C1_tile078.png");
            Properties.Resources.C1_tile079.Save(_infoC.FullName + "\\C1_tile079.png");
            Properties.Resources.C1_tile080.Save(_infoC.FullName + "\\C1_tile080.png");
            Properties.Resources.C1_tile081.Save(_infoC.FullName + "\\C1_tile081.png");
            Properties.Resources.C1_tile082.Save(_infoC.FullName + "\\C1_tile082.png");
            Properties.Resources.C1_tile083.Save(_infoC.FullName + "\\C1_tile083.png");
            Properties.Resources.C1_tile084.Save(_infoC.FullName + "\\C1_tile084.png");
            Properties.Resources.C1_tile085.Save(_infoC.FullName + "\\C1_tile085.png");
            Properties.Resources.C1_tile086.Save(_infoC.FullName + "\\C1_tile086.png");
            Properties.Resources.C1_tile087.Save(_infoC.FullName + "\\C1_tile087.png");
            Properties.Resources.C1_tile088.Save(_infoC.FullName + "\\C1_tile088.png");
            Properties.Resources.C1_tile089.Save(_infoC.FullName + "\\C1_tile089.png");
            Properties.Resources.C1_tile090.Save(_infoC.FullName + "\\C1_tile090.png");
            Properties.Resources.C1_tile091.Save(_infoC.FullName + "\\C1_tile091.png");
            Properties.Resources.C1_tile092.Save(_infoC.FullName + "\\C1_tile092.png");
            Properties.Resources.C1_tile093.Save(_infoC.FullName + "\\C1_tile093.png");
            Properties.Resources.C1_tile094.Save(_infoC.FullName + "\\C1_tile094.png");
            Properties.Resources.C1_tile095.Save(_infoC.FullName + "\\C1_tile095.png");
            Properties.Resources.C1_tile096.Save(_infoC.FullName + "\\C1_tile096.png");
            Properties.Resources.C1_tile097.Save(_infoC.FullName + "\\C1_tile097.png");
            Properties.Resources.C1_tile098.Save(_infoC.FullName + "\\C1_tile098.png");
            Properties.Resources.C1_tile099.Save(_infoC.FullName + "\\C1_tile099.png");
            Properties.Resources.C1_tile100.Save(_infoC.FullName + "\\C1_tile100.png");
            Properties.Resources.C1_tile101.Save(_infoC.FullName + "\\C1_tile101.png");
            Properties.Resources.C1_tile102.Save(_infoC.FullName + "\\C1_tile102.png");
            Properties.Resources.C1_tile103.Save(_infoC.FullName + "\\C1_tile103.png");
            Properties.Resources.C1_tile104.Save(_infoC.FullName + "\\C1_tile104.png");
            Properties.Resources.C1_tile105.Save(_infoC.FullName + "\\C1_tile105.png");
            Properties.Resources.C1_tile106.Save(_infoC.FullName + "\\C1_tile106.png");
            Properties.Resources.C1_tile107.Save(_infoC.FullName + "\\C1_tile107.png");
            Properties.Resources.C1_tile108.Save(_infoC.FullName + "\\C1_tile108.png");
            Properties.Resources.C1_tile109.Save(_infoC.FullName + "\\C1_tile109.png");
            Properties.Resources.C1_tile110.Save(_infoC.FullName + "\\C1_tile110.png");
            Properties.Resources.C1_tile111.Save(_infoC.FullName + "\\C1_tile111.png");
            Properties.Resources.C1_tile112.Save(_infoC.FullName + "\\C1_tile112.png");
            Properties.Resources.C1_tile113.Save(_infoC.FullName + "\\C1_tile113.png");
            Properties.Resources.C1_tile114.Save(_infoC.FullName + "\\C1_tile114.png");
            Properties.Resources.C1_tile115.Save(_infoC.FullName + "\\C1_tile115.png");
            Properties.Resources.C1_tile116.Save(_infoC.FullName + "\\C1_tile116.png");
            Properties.Resources.C1_tile117.Save(_infoC.FullName + "\\C1_tile117.png");
            Properties.Resources.C1_tile118.Save(_infoC.FullName + "\\C1_tile118.png");
            Properties.Resources.C1_tile119.Save(_infoC.FullName + "\\C1_tile119.png");
            Properties.Resources.C1_tile120.Save(_infoC.FullName + "\\C1_tile120.png");
            Properties.Resources.C1_tile121.Save(_infoC.FullName + "\\C1_tile121.png");
            Properties.Resources.C1_tile122.Save(_infoC.FullName + "\\C1_tile122.png");
            Properties.Resources.C1_tile123.Save(_infoC.FullName + "\\C1_tile123.png");
            Properties.Resources.C1_tile124.Save(_infoC.FullName + "\\C1_tile124.png");
            Properties.Resources.C1_tile125.Save(_infoC.FullName + "\\C1_tile125.png");
            Properties.Resources.C1_tile126.Save(_infoC.FullName + "\\C1_tile126.png");
            Properties.Resources.C1_tile127.Save(_infoC.FullName + "\\C1_tile127.png");
            Properties.Resources.C1_tile128.Save(_infoC.FullName + "\\C1_tile128.png");
            Properties.Resources.C1_tile129.Save(_infoC.FullName + "\\C1_tile129.png");
            Properties.Resources.C1_tile130.Save(_infoC.FullName + "\\C1_tile130.png");
            Properties.Resources.C1_tile131.Save(_infoC.FullName + "\\C1_tile131.png");
            Properties.Resources.C1_tile132.Save(_infoC.FullName + "\\C1_tile132.png");
            Properties.Resources.C1_tile133.Save(_infoC.FullName + "\\C1_tile133.png");
            Properties.Resources.C1_tile134.Save(_infoC.FullName + "\\C1_tile134.png");
            Properties.Resources.C1_tile135.Save(_infoC.FullName + "\\C1_tile135.png");
            Properties.Resources.C1_tile136.Save(_infoC.FullName + "\\C1_tile136.png");
            Properties.Resources.C1_tile137.Save(_infoC.FullName + "\\C1_tile137.png");
            Properties.Resources.C1_tile138.Save(_infoC.FullName + "\\C1_tile138.png");
            Properties.Resources.C1_tile139.Save(_infoC.FullName + "\\C1_tile139.png");
            Properties.Resources.C1_tile140.Save(_infoC.FullName + "\\C1_tile140.png");
            Properties.Resources.C1_tile141.Save(_infoC.FullName + "\\C1_tile141.png");
            Properties.Resources.C1_tile142.Save(_infoC.FullName + "\\C1_tile142.png");
            Properties.Resources.C1_tile143.Save(_infoC.FullName + "\\C1_tile143.png");
            Properties.Resources.C1_tile144.Save(_infoC.FullName + "\\C1_tile144.png");
            Properties.Resources.C1_tile145.Save(_infoC.FullName + "\\C1_tile145.png");
            Properties.Resources.C1_tile146.Save(_infoC.FullName + "\\C1_tile146.png");
            Properties.Resources.C1_tile147.Save(_infoC.FullName + "\\C1_tile147.png");
            Properties.Resources.C1_tile148.Save(_infoC.FullName + "\\C1_tile148.png");
            Properties.Resources.C1_tile149.Save(_infoC.FullName + "\\C1_tile149.png");
            Properties.Resources.C1_tile150.Save(_infoC.FullName + "\\C1_tile150.png");
            Properties.Resources.C1_tile151.Save(_infoC.FullName + "\\C1_tile151.png");
            Properties.Resources.C1_tile152.Save(_infoC.FullName + "\\C1_tile152.png");
            Properties.Resources.C1_tile153.Save(_infoC.FullName + "\\C1_tile153.png");
            Properties.Resources.C1_tile154.Save(_infoC.FullName + "\\C1_tile154.png");
            Properties.Resources.C1_tile155.Save(_infoC.FullName + "\\C1_tile155.png");
            Properties.Resources.C1_tile156.Save(_infoC.FullName + "\\C1_tile156.png");
            Properties.Resources.C1_tile157.Save(_infoC.FullName + "\\C1_tile157.png");
            Properties.Resources.C1_tile158.Save(_infoC.FullName + "\\C1_tile158.png");
            Properties.Resources.C1_tile159.Save(_infoC.FullName + "\\C1_tile159.png");
            Properties.Resources.C1_tile160.Save(_infoC.FullName + "\\C1_tile160.png");
            Properties.Resources.C1_tile161.Save(_infoC.FullName + "\\C1_tile161.png");
            Properties.Resources.C1_tile162.Save(_infoC.FullName + "\\C1_tile162.png");
            Properties.Resources.C1_tile163.Save(_infoC.FullName + "\\C1_tile163.png");
            Properties.Resources.C1_tile164.Save(_infoC.FullName + "\\C1_tile164.png");
            Properties.Resources.C1_tile165.Save(_infoC.FullName + "\\C1_tile165.png");
            Properties.Resources.C1_tile166.Save(_infoC.FullName + "\\C1_tile166.png");
            Properties.Resources.C1_tile167.Save(_infoC.FullName + "\\C1_tile167.png");
            Properties.Resources.C1_tile168.Save(_infoC.FullName + "\\C1_tile168.png");
            Properties.Resources.C1_tile169.Save(_infoC.FullName + "\\C1_tile169.png");
            Properties.Resources.C1_tile170.Save(_infoC.FullName + "\\C1_tile170.png");
            Properties.Resources.C1_tile171.Save(_infoC.FullName + "\\C1_tile171.png");
            Properties.Resources.C1_tile172.Save(_infoC.FullName + "\\C1_tile172.png");
            Properties.Resources.C1_tile173.Save(_infoC.FullName + "\\C1_tile173.png");
            Properties.Resources.C1_tile174.Save(_infoC.FullName + "\\C1_tile174.png");
            Properties.Resources.C1_tile175.Save(_infoC.FullName + "\\C1_tile175.png");
            Properties.Resources.C1_tile176.Save(_infoC.FullName + "\\C1_tile176.png");
            Properties.Resources.C1_tile177.Save(_infoC.FullName + "\\C1_tile177.png");
            Properties.Resources.C1_tile178.Save(_infoC.FullName + "\\C1_tile178.png");
            Properties.Resources.C1_tile179.Save(_infoC.FullName + "\\C1_tile179.png");
            Properties.Resources.C1_tile180.Save(_infoC.FullName + "\\C1_tile180.png");
            Properties.Resources.C1_tile181.Save(_infoC.FullName + "\\C1_tile181.png");
            Properties.Resources.C1_tile182.Save(_infoC.FullName + "\\C1_tile182.png");
            Properties.Resources.C1_tile183.Save(_infoC.FullName + "\\C1_tile183.png");
            Properties.Resources.C1_tile184.Save(_infoC.FullName + "\\C1_tile184.png");
            Properties.Resources.C1_tile185.Save(_infoC.FullName + "\\C1_tile185.png");
            Properties.Resources.C1_tile186.Save(_infoC.FullName + "\\C1_tile186.png");
            Properties.Resources.C1_tile187.Save(_infoC.FullName + "\\C1_tile187.png");
            Properties.Resources.C1_tile188.Save(_infoC.FullName + "\\C1_tile188.png");
            Properties.Resources.C1_tile189.Save(_infoC.FullName + "\\C1_tile189.png");
            Properties.Resources.C1_tile190.Save(_infoC.FullName + "\\C1_tile190.png");
            Properties.Resources.C1_tile191.Save(_infoC.FullName + "\\C1_tile191.png");
            Properties.Resources.C1_tile192.Save(_infoC.FullName + "\\C1_tile192.png");
            Properties.Resources.C1_tile193.Save(_infoC.FullName + "\\C1_tile193.png");
            Properties.Resources.C1_tile194.Save(_infoC.FullName + "\\C1_tile194.png");
            Properties.Resources.C1_tile195.Save(_infoC.FullName + "\\C1_tile195.png");
            Properties.Resources.C1_tile196.Save(_infoC.FullName + "\\C1_tile196.png");
            Properties.Resources.C1_tile197.Save(_infoC.FullName + "\\C1_tile197.png");
            Properties.Resources.C1_tile198.Save(_infoC.FullName + "\\C1_tile198.png");
            Properties.Resources.C1_tile199.Save(_infoC.FullName + "\\C1_tile199.png");
            Properties.Resources.C1_tile200.Save(_infoC.FullName + "\\C1_tile200.png");
            Properties.Resources.C1_tile201.Save(_infoC.FullName + "\\C1_tile201.png");
            Properties.Resources.C1_tile202.Save(_infoC.FullName + "\\C1_tile202.png");
            Properties.Resources.C1_tile203.Save(_infoC.FullName + "\\C1_tile203.png");
            Properties.Resources.C1_tile204.Save(_infoC.FullName + "\\C1_tile204.png");
            Properties.Resources.C1_tile205.Save(_infoC.FullName + "\\C1_tile205.png");
            Properties.Resources.C1_tile206.Save(_infoC.FullName + "\\C1_tile206.png");
            Properties.Resources.C1_tile207.Save(_infoC.FullName + "\\C1_tile207.png");
            Properties.Resources.C1_tile208.Save(_infoC.FullName + "\\C1_tile208.png");
            Properties.Resources.C1_tile209.Save(_infoC.FullName + "\\C1_tile209.png");
            Properties.Resources.C1_tile210.Save(_infoC.FullName + "\\C1_tile210.png");
            Properties.Resources.C1_tile211.Save(_infoC.FullName + "\\C1_tile211.png");
            Properties.Resources.C1_tile212.Save(_infoC.FullName + "\\C1_tile212.png");
            Properties.Resources.C1_tile213.Save(_infoC.FullName + "\\C1_tile213.png");
            Properties.Resources.C1_tile214.Save(_infoC.FullName + "\\C1_tile214.png");
            Properties.Resources.C1_tile215.Save(_infoC.FullName + "\\C1_tile215.png");
            Properties.Resources.C1_tile216.Save(_infoC.FullName + "\\C1_tile216.png");
            Properties.Resources.C1_tile217.Save(_infoC.FullName + "\\C1_tile217.png");
            Properties.Resources.C1_tile218.Save(_infoC.FullName + "\\C1_tile218.png");
            Properties.Resources.C1_tile219.Save(_infoC.FullName + "\\C1_tile219.png");
            Properties.Resources.C1_tile220.Save(_infoC.FullName + "\\C1_tile220.png");
            Properties.Resources.C1_tile221.Save(_infoC.FullName + "\\C1_tile221.png");
            Properties.Resources.C1_tile222.Save(_infoC.FullName + "\\C1_tile222.png");
            Properties.Resources.C1_tile223.Save(_infoC.FullName + "\\C1_tile223.png");
            Properties.Resources.C1_tile224.Save(_infoC.FullName + "\\C1_tile224.png");
            Properties.Resources.C1_tile225.Save(_infoC.FullName + "\\C1_tile225.png");
            Properties.Resources.C1_tile226.Save(_infoC.FullName + "\\C1_tile226.png");
            Properties.Resources.C1_tile227.Save(_infoC.FullName + "\\C1_tile227.png");
            Properties.Resources.C1_tile228.Save(_infoC.FullName + "\\C1_tile228.png");
            Properties.Resources.C1_tile229.Save(_infoC.FullName + "\\C1_tile229.png");
            Properties.Resources.C1_tile230.Save(_infoC.FullName + "\\C1_tile230.png");
            Properties.Resources.C1_tile231.Save(_infoC.FullName + "\\C1_tile231.png");
            Properties.Resources.C1_tile232.Save(_infoC.FullName + "\\C1_tile232.png");
            Properties.Resources.C1_tile233.Save(_infoC.FullName + "\\C1_tile233.png");
            Properties.Resources.C1_tile234.Save(_infoC.FullName + "\\C1_tile234.png");
            Properties.Resources.C1_tile235.Save(_infoC.FullName + "\\C1_tile235.png");
            Properties.Resources.C1_tile236.Save(_infoC.FullName + "\\C1_tile236.png");
            Properties.Resources.C1_tile237.Save(_infoC.FullName + "\\C1_tile237.png");
            Properties.Resources.C1_tile238.Save(_infoC.FullName + "\\C1_tile238.png");
            Properties.Resources.C1_tile239.Save(_infoC.FullName + "\\C1_tile239.png");
            Properties.Resources.C1_tile240.Save(_infoC.FullName + "\\C1_tile240.png");
            Properties.Resources.C1_tile241.Save(_infoC.FullName + "\\C1_tile241.png");
            Properties.Resources.C1_tile242.Save(_infoC.FullName + "\\C1_tile242.png");
            Properties.Resources.C1_tile243.Save(_infoC.FullName + "\\C1_tile243.png");
            Properties.Resources.C1_tile244.Save(_infoC.FullName + "\\C1_tile244.png");
            Properties.Resources.C1_tile245.Save(_infoC.FullName + "\\C1_tile245.png");
            Properties.Resources.C1_tile246.Save(_infoC.FullName + "\\C1_tile246.png");
            Properties.Resources.C1_tile247.Save(_infoC.FullName + "\\C1_tile247.png");
            Properties.Resources.C1_tile248.Save(_infoC.FullName + "\\C1_tile248.png");
            Properties.Resources.C1_tile249.Save(_infoC.FullName + "\\C1_tile249.png");
            Properties.Resources.C1_tile250.Save(_infoC.FullName + "\\C1_tile250.png");
            Properties.Resources.C1_tile251.Save(_infoC.FullName + "\\C1_tile251.png");
            Properties.Resources.C1_tile252.Save(_infoC.FullName + "\\C1_tile252.png");
            Properties.Resources.C1_tile253.Save(_infoC.FullName + "\\C1_tile253.png");
            Properties.Resources.C1_tile254.Save(_infoC.FullName + "\\C1_tile254.png");
            Properties.Resources.C1_tile255.Save(_infoC.FullName + "\\C1_tile255.png");
        }
        #endregion

    }
}




/*
 * using (MemoryStream memory = new MemoryStream())
                    {
                        BitmapImage img = new BitmapImage();

                        img.BeginInit();
                        img.StreamSource = memory;
                        img.UriSource = new Uri(tempImgLocation, UriKind.Absolute);
                        idToSpriteLocation.Add("A" + rep + "_" + number, img);
                        try
                        {
                        img.EndInit();
                        }
                        catch (Exception)
                        {
                            string s = "";
                        }
                    }

    */
