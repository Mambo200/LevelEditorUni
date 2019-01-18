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
using System.Reflection;
using System.Resources;
using System.Data;

namespace LevelEditor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ///<summary>directory info of File</summary>
        public static DirectoryInfo fileInfo = null;
        ///<summary>filter for file</summary>
        public static string filter = "Level File|*.lvl|XML File|*.xml";
        ///<summary>Current Level Level info</summary>
        public static Level level = new Level();
        ///<summary>Current Level Layer info</summary>
        public static Layer[] levelLayer;
        ///<summary>Current Level Tile A info</summary>
        public static Tile[] levelTileA;
        ///<summary>Current Level Tile B info</summary>
        public static Tile[] levelTileB;
        ///<summary>Current Level Tile C info</summary>
        public static Tile[] levelTileC;
        ///<summary>Current Level Tile array position</summary>
        public static int currentTileArrayPos = 0;
        ///<summary>Current Sprite ID</summary>
        public static string CurrentSpriteID;
        ///<summary>Current Border</summary>
        private static Border CurrentBorder = null;
        ///<summary>Current Image</summary>
        private static Image CurrentImage = null;

        private static Image[] allBImages = null;
        private static Image[] allCImages = null;
        public static string path;

        ///<summary>A1 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationA1 = new Dictionary<string, string>();
        ///<summary>A2 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationA2 = new Dictionary<string, string>();
        ///<summary>A3 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationA3 = new Dictionary<string, string>();
        ///<summary>A4 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationA4 = new Dictionary<string, string>();
        ///<summary>A5 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationA5 = new Dictionary<string, string>();
        ///<summary>B1 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationB1 = new Dictionary<string, string>();
        ///<summary>C1 Sprite Location</summary>
        private Dictionary<string, string> idToSpriteLocationC1 = new Dictionary<string, string>();


        ///<summary>true if something was changed</summary>
        public static bool changed = false;
        /// <summary>Confirm to close Window</summary>
        public static bool ConfirmClose;

        ///<summary>level manager private static</summary>
        private static LevelManager lvlManager = new LevelManager();
        ///<summary>level manager public static</summary>
        public static LevelManager LvlManager { get { return lvlManager; } }
        ///<summary>helper class</summary>
        private static Helper h = new Helper();
        ///<summary>All buttons</summary>
        private Border[,] allBorders = null;

        ///<summary>All RowDefinitions</summary>
        private RowDefinition[] allRows = null;
        ///<summary>All ColumnDefinitions</summary>
        private ColumnDefinition[] allColumns = null;

        public MainWindow()
        {
            // export files if they are missing
            h.ExportFiles();

            InitializeComponent();
            CreateLayerA();
            CreateLayerB();
            CreateLayerC();

            AddToRecentFilesHeader();
        }
        
        /// <summary>
        /// Create test file (Test purpose)
        /// </summary>
        private void TestFile()
        {
            Level TestLevel = new Level();
            TestLevel.Name = "MyTestLevel";
            TestLevel.SizeX = 20;
            TestLevel.SizeY = 8;
            TestLevel.Layer = new List<Layer>();
            for (int y = 0; y < TestLevel.SizeY; y++)
            {
                for (int x = 0; x < TestLevel.SizeX; x++)
                {
                    Layer layer = new Layer();
                    Tile tile = new Tile();
                    layer.Tiles = new List<Tile>();
                    for (int i = 0; i < 2; i++)
                    {
                        tile.PosX = x;
                        tile.PosY = y;
                        layer.Tiles.Add(tile);
                    }
                    TestLevel.Layer.Add(layer);
                }
            }
            LvlManager.SaveLevel(Environment.CurrentDirectory + ".lvl", TestLevel);
            LvlManager.SaveLevelXML(Environment.CurrentDirectory + ".xml", TestLevel);
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
        #region New Button

        /// <summary>
        /// Create new Level File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            bool? result;
            bool changedTmp = WasChanged(out result);

            if (changedTmp == true)
            {
                switch (result)
                {
                    // save
                    case true:
                        // if file already exist
                        if (fileInfo == null)
                        {
                            h.SaveFile(out bool? work);

                            // if file was not saved returrn
                            if (work != true)
                                return;
                        }
                        break;
                    // jump directly to creating new file
                    case false:
                        break;
                    // Cancel creating file
                    case null:
                        return;
                }
            }

            // prepare new level
            NewLevel prepare = new NewLevel();
            prepare.ShowDialog();
            List<Tile> tilesList = new List<Tile>();
            List<Layer> layerList = new List<Layer>();
            Layer layer = new Layer();
            layer.ZOrder = 0;
            layerList.Add(layer);

            // return if levelname is null
            if (prepare.levelName == null)
                return;

            // delete old grid and buttons
            DeleteBorders();
            DeleteGrid();

            // generate new level
            level = new Level();
            level.Name = prepare.levelName;
            level.SizeX = prepare.xSize;
            level.SizeY = prepare.ySize;
            level.Layer = layerList;
            GenerateGridWithButtons(level.SizeY, level.SizeX);
            changed = true;
            path = null;
        }

        #endregion

        #region Open

        /// <summary>
        /// Open File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            // temp level
            Level tmpLevel = new Level();

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
                path = file.FileName;
                SetStatus(Label_StatusbarOne, "Loading File...", true, path);
            }
            // User canceled window
            else
            {
                SetStatus(Label_StatusbarOne, "Canceled", true);
                return;
            }
            // loading File
            if (file.FilterIndex == 1)
                level = LvlManager.LoadLevel(path);
            else if (file.FilterIndex == 2)
                level = LvlManager.LoadLevelXML(path);

            // save to temp level
            tmpLevel = level;

            if (allBorders != null)
            {
                // delete old grid and buttons
                DeleteEverything();
            }

            // set grid and buttons
            GenerateGridWithButtons(level.SizeY, level.SizeX);

            // set level, temp layer and Tiles
            level = tmpLevel;
            levelLayer = level.Layer.ToArray();
            levelTileA = levelLayer[0].Tiles.ToArray();
            levelTileB = levelLayer[1].Tiles.ToArray();
            levelTileC = levelLayer[2].Tiles.ToArray();

            SetImages();

            // loading complete. set statusbar
            SetStatus(Label_StatusbarOne, "Loading Complete", true);


        }

        #endregion

        #region Quit

        /// <summary>
        /// Close Application
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            if (!WasChanged(out bool? result))
            {
                Application.Current.Shutdown();
            }

            if (result == true)
            {
                Application.Current.Shutdown();
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            levelLayer[0].Tiles = levelTileA.ToList();
            levelLayer[1].Tiles = levelTileB.ToList();
            levelLayer[2].Tiles = levelTileC.ToList();

            level.Layer = levelLayer.ToList();

            if (path == null)
                Button_SaveAs_Click(sender, e);
            else
            {
                lvlManager.SaveLevel(path, level);
                ConfirmClose = true;
                changed = false;
                h.AddPathToTextFile(path);
                AddToRecentFilesHeader();
            }
            SetStatus(Label_StatusbarOne, "Successful saved to " + path, true, "Save");
        }

        #endregion

        #region Save As

        /// <summary>
        /// Save File As ...
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            levelLayer[0].Tiles = levelTileA.ToList();
            levelLayer[1].Tiles = levelTileB.ToList();
            levelLayer[2].Tiles = levelTileC.ToList();

            level.Layer = levelLayer.ToList();

            h.SaveFile(out bool? succellful);
            if (succellful == true)
            {
                ConfirmClose = true;
                h.AddPathToTextFile(path);
                AddToRecentFilesHeader();
                SetStatus(Label_StatusbarOne, "Successful saved to " + path, true, "Save As");
            }
            else
                ConfirmClose = false;


        }

        #endregion

        #endregion

        #endregion

        #region GroupBox

        #region Tiles
        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            CurrentSpriteID = img.Tag.ToString();
            string statusText = "";
            statusText += FirstTwoChars(CurrentSpriteID);
            statusText += "-";
            statusText += GetLastChars(CurrentSpriteID, 3);
            SetStatus(Label_StatusbarThree, statusText + " chosen");
        }
        #endregion

        #region Properties Window

        #region Sprite ID
        private void TextBox_SpriteID_TextChanged(object sender, TextChangedEventArgs e)
        {
            // return if button = null
            if (CurrentBorder == null)
                return;

            string BttnTag = CurrentBorder.Tag.ToString();
            string[] tagSplit = BttnTag.Split('|');
            // check if input was number
            bool work = int.TryParse(TextBox_SpriteID.Text.ToString(), out int number);
            if (work)
            {
                levelTileA[currentTileArrayPos].SpriteID = TextBox_SpriteID.Text.ToString();
                TextBox_SpriteID.BorderBrush = Brushes.Gray;
                TextBox_SpriteID.Background = Brushes.White;
            }
            else
            {
                TextBox_SpriteID.BorderBrush = Brushes.Red;
                TextBox_SpriteID.Background = Brushes.Red;
            }
            
            level.Name = TextBox_SpriteID.Text;
        }
        #endregion

        #region Comment
        private void TextBox_Comment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentBorder == null)
                return;

            levelTileA[currentTileArrayPos].Commentary = TextBox_Comment.Text.ToString();
        }
        #endregion

        #region Collision

        private void CheckBox_Collision_Checked(object sender, RoutedEventArgs e)
        {
            // return if no button was chosen
            if (CurrentBorder == null)
                return;

            levelTileA[currentTileArrayPos].HasCollision = true;
        }

        private void CheckBox_Collision_Unchecked(object sender, RoutedEventArgs e)
        {
            // return if no button was chosen
            if (CurrentBorder == null)
                return;

            levelTileA[currentTileArrayPos].HasCollision = false;

        }
        #endregion

        #region Tag
        private void TextBox_Tag_TextChanged(object sender, TextChangedEventArgs e)
        {
            levelTileA[currentTileArrayPos].Tag = TextBox_Tag.Text.ToString();
        }
        #endregion

        #endregion

        #endregion

        #region Grid Buttons
        /// <summary>
        /// left button click event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Img_ClickLeft(object sender, RoutedEventArgs e)
        {
            // return if no sprite was chosen
            if (CurrentSpriteID == null)
            {
                return;
            }
            // get current border
            CurrentBorder = (Border)sender;
            string BttnTag = CurrentBorder.Tag.ToString();
            string[] tagSplit = BttnTag.Split('|');

            // fill PosX and PosY
            LayerArrayPos(tagSplit[0], tagSplit[1]);
            
            char firstLetter = CurrentSpriteID[0];
            // set new sprite ID
            switch (firstLetter)
            {
                case 'A':
                    levelTileA[currentTileArrayPos].SpriteID = CurrentSpriteID;
                    CurrentImage = (Image)CurrentBorder.Child;
                    break;
                case 'B':
                    levelTileB[currentTileArrayPos].SpriteID = CurrentSpriteID;
                    CurrentImage = allBImages[currentTileArrayPos];
                    break;
                case 'C':
                    levelTileC[currentTileArrayPos].SpriteID = CurrentSpriteID;
                    CurrentImage = allCImages[currentTileArrayPos];
                    break;
                default:
                    break;
            }
            
            changed = true;




            // set Text at Properties
            SetNewInfoForTextBox();

            // get image location
            CurrentImage.Source = new BitmapImage(new Uri(TagToImageLocation(CurrentSpriteID)));

            SetStatus(Label_StatusbarTwo, "changes sprites of " + BttnTag);
            SetStatus(Label_StatusbarOne, "Idle");
        }

        /// <summary>
        /// right button click event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Img_ClickRight(object sender, RoutedEventArgs e)
        {
            CurrentBorder = (Border)sender;
          
            // fill PosX and PosY
            string BttnTag = CurrentBorder.Tag.ToString();
            string[] tagSplit = BttnTag.Split('|');
            LayerArrayPos(tagSplit[0], tagSplit[1]);

            // set Text at Properties
            SetNewInfoForTextBox();
            SetStatus(Label_StatusbarOne, "Idle");

        }

        private void Img_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Img_ClickLeft(sender, new RoutedEventArgs());
            }
        }
        #endregion
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConfirmClose = false;
            bool changed = WasChanged(out bool? result);
            if (changed == false)
                ConfirmClose = true;

            else if (result == true)
                Button_Save_Click(sender, new RoutedEventArgs());
            else if (result == false)
                ConfirmClose = true;
            else
            {
                e.Cancel = ConfirmClose;
            }

            e.Cancel = !ConfirmClose;
        }

        // ----------------------------------------------------------------------------------------------------- //

        #region Create Grid and Border
        /// <summary>
        /// Generates the grid with buttons.
        /// </summary>
        /// <param name="_rowDef">height of row (Y Size)</param>
        /// <param name="_columnDef">width of column (X Size)</param>
        private void GenerateGridWithButtons(int _rowDef, int _columnDef)
        {
            // add layer
            levelLayer = new Layer[3];
            // generate All Buttons array
            allBorders = new Border[_columnDef, _rowDef];
            

            allRows = new RowDefinition[_rowDef];
            allColumns = new ColumnDefinition[_columnDef];

            // add RowDefinition
            for (int row = 0; row < _rowDef; row++)
            {
                // create new RowDefinition
                RowDefinition r = new RowDefinition { Height = DefaultValue.DefaultGridLength };
                // save in array
                allRows[row] = r;
                // add RowDefinition to grid
                Grid_GridBorder.RowDefinitions.Add(r);
            }
            // add ColumnDefinition
            for (int col = 0; col < _columnDef; col++)
            {
                // create new ColumnDefinition
                ColumnDefinition c = new ColumnDefinition { Width = DefaultValue.DefaultGridLength };
                // save in array
                allColumns[col] = c;
                // add ColumnDefinition to grid
                Grid_GridBorder.ColumnDefinitions.Add(c);
            }

            // create Tiles
            levelTileA = new Tile[_rowDef * _columnDef];
            levelTileB = new Tile[_rowDef * _columnDef];
            levelTileC = new Tile[_rowDef * _columnDef];

            allBImages = new Image[_rowDef * _columnDef];
            allCImages = new Image[_rowDef * _columnDef];

            // counter
            int counter = 0;

            // create image
            for (int row = 0; row < _rowDef; row++)
            {
                for (int col = 0; col < _columnDef; col++)
                {
                    // create Image
                    Image img = CreateImage(row, col, col + "|" + row);
                    // show button in grid
                    ShowPictureInBorderInGrid(row, col);

                    // create Tile
                    Tile t = new Tile();
                    t.PosX = col;
                    t.PosY = row;
                    t.SpriteID = "0";
                    t.Commentary = "";
                    t.HasCollision = false;
                    t.Tag = "";
                    // copy tile to array
                    levelTileA[counter] = t;

                    // create Tile
                    t = new Tile();
                    t.PosX = col;
                    t.PosY = row;
                    t.SpriteID = "0";
                    t.Commentary = "";
                    t.HasCollision = false;
                    t.Tag = "";
                    // copy tile to array
                    levelTileB[counter] = t;

                    // create Tile
                    t = new Tile();
                    t.PosX = col;
                    t.PosY = row;
                    t.SpriteID = "0";
                    t.Commentary = "";
                    t.HasCollision = false;
                    t.Tag = "";
                    // copy tile to array
                    levelTileC[counter] = t;

                    Image img1 = CreateImage(row, col, col + "|" + row);
                    Image img2 = CreateImage(row, col, col + "|" + row);

                    allBImages[counter] = img1;
                    allCImages[counter] = img2;
                    counter++;
                }


                // set layer
                levelLayer[0].ZOrder = 0;
                levelLayer[0].Tiles = levelTileA.ToList();
                levelLayer[1].ZOrder = 1;
                levelLayer[1].Tiles = levelTileB.ToList();
                levelLayer[2].ZOrder = 2;
                levelLayer[2].Tiles = levelTileC.ToList();
            }
            // set status
            SetStatus(Label_StatusbarOne, "Images created");

            // reset Textboxes
            ResetTextBox();
        }

        /// <summary>
        /// creates button
        /// </summary>
        /// <param name="_row">row of grid (for Tag)</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_imageSource">Content of the button.</param>
        /// <returns></returns>
        private Image CreateImage(int _row, int _col, string _imageSource)
        {
            Image img = new Image();
            img.BeginInit();
            img.Tag = _col + "|" + _row;
            
            img.Focusable = false;

            img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Sprites\\empty.png"));
            img.Stretch = Stretch.Uniform;
            img.EndInit();


            return img;
        }

        /// <summary>
        /// Creates a black thickness 1 border
        /// </summary>
        /// <param name="_row">Current row (for Tag)</param>
        /// <param name="_col">Current column (for Tag)</param>
        /// <returns>black border</returns>
        private Border CreateBorder(int _row, int _col)
        {
            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.MouseLeftButtonDown += Img_ClickLeft;
            border.MouseRightButtonDown += Img_ClickRight;
            border.MouseEnter += Img_MouseEnter;
            border.Tag = _col + "|" + _row;

            return border;
        }

        #endregion

        #region Show Button Function
        /// <summary>
        /// Creates an image in a border in Grid_GridBorder with margin = 2
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_imageSource">content of the button.</param>
        private void ShowPictureInBorderInGrid(int _row, int _col, string _imageSource = "")
        {
            Image img = CreateImage(_row, _col, _imageSource);
            Border border = CreateBorder(_row, _col);
            border.Child = img;
            allBorders[_col, _row] = border;
            ShowPictureInBorderInGrid(_row, _col, Grid_GridBorder, border);
        }

        private void ShowPictureInGrid(int _row, int _col, string _imageSource = "")
        {
            Image img = CreateImage(_row, _col, _imageSource);
            ShowPictureInGrid(_row, _col, Grid_GridBorder, img);
        }

        /// <summary>
        /// shows a button in Grid_GridBorder
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_border">the button</param>
        private void ShowPictureInBorderInGrid(int _row, int _col, Border _border)
        {
            ShowPictureInBorderInGrid(_row, _col, Grid_GridBorder, _border);
        }

        /// <summary>
        /// show button in grid
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_grid">the grid itself</param>
        /// <param name="_border">the border with image</param>
        private void ShowPictureInBorderInGrid(int _row, int _col, Grid _grid, Border _border)
        {
            _grid.Children.Add(_border);
            Grid.SetRow(_border, _row);
            Grid.SetColumn(_border, _col);
        }
        private void ShowPictureInGrid(int _row, int _col, Grid _grid, Image _image)
        {
            Grid.SetRow(_image, _row);
            Grid.SetColumn(_image, _col);
        }
        #endregion

        #region Delete Functions

        /// <summary>
        /// Deletes all buttons.
        /// </summary>
        private void DeleteBorders()
        {
            if (allBorders == null)
            {
                return;
            }

            // set images to null
            for (int i = 0; i < allBorders.GetLength(0); i++)
            {
                for (int y = 0; y < allBorders.GetLength(1); y++)
                {
                    Image img = (Image)allBorders[i, y].Child;
                    img = null;
                }
            }

            // delete borders
            Grid_GridBorder.Children.RemoveRange(0, Grid_GridBorder.Children.Count);
        }

        /// <summary>
        /// Delete complete grid (WITHOUT BUTTONS).
        /// </summary>
        private void DeleteGrid()
        {
            // Delete Rows
            DeleteRow();
            // Delete Columns
            DeleteColumn();
        }

        /// <summary>
        /// Delete row
        /// </summary>
        private void DeleteRow()
        {
            // check size of row definition
            if (Grid_GridBorder.RowDefinitions.Count == 0)
                return;

            // delete RowDefinitions
            Grid_GridBorder.RowDefinitions.RemoveRange(0, Grid_GridBorder.RowDefinitions.Count);
        }

        /// <summary>
        /// Delete column
        /// </summary>
        private void DeleteColumn()
        {
            // check size of row definition
            if (Grid_GridBorder.ColumnDefinitions.Count == 0)
                return;

            // delete ColumnDefinitions
            Grid_GridBorder.ColumnDefinitions.RemoveRange(0, Grid_GridBorder.ColumnDefinitions.Count);
        }

        /// <summary>
        /// Delete Grid and Buttons
        /// </summary>
        private void DeleteEverything()
        {
            DeleteBorders();
            DeleteGrid();
        }
        #endregion

        /// <summary>
        /// check if file was changed and ask to save if file was changed.
        /// </summary>
        /// <param name="MsgBoxResult">what User pressed</param>
        /// <returns>true: file was changed</returns>
        public static bool WasChanged(out bool? MsgBoxResult)
        {
            MsgBoxResult = null;
            if(changed == false)
            {
                return false;
            }

            MessageBoxResult result = MessageBox.Show("Would you like to save?", "Unsaved Changes", MessageBoxButton.YesNoCancel);

            if (result == System.Windows.MessageBoxResult.Yes)
                MsgBoxResult = true;
            else if (result == System.Windows.MessageBoxResult.No)
                MsgBoxResult = false;
            else
                MsgBoxResult = null;

            return true;
        }

        /// <summary>
        /// Get array position in Tile Array
        /// </summary>
        /// <param name="_xPos">x position button</param>
        /// <param name="_yPos">y position button</param>
        /// <returns>position in Tile Array</returns>
        public static void LayerArrayPos(int _xPos, int _yPos)
        {
            int pos = level.SizeX * _yPos;
            pos += _xPos;
            currentTileArrayPos = pos;
        }

        /// <summary>
        /// Get array position in Tile Array
        /// </summary>
        /// <param name="_xPos">x position button</param>
        /// <param name="_yPos">y position button</param>
        /// <returns>position in Tile Array</returns>
        public static void LayerArrayPos(string _xPos, string _yPos)
        {
            LayerArrayPos(Convert.ToInt32(_xPos), Convert.ToInt32(_yPos));
        }

        /// <summary>
        /// Reset Text of Textboxes
        /// </summary>
        public void ResetTextBox()
        {
            TextBox_PosX.Text = "";
            TextBox_PosY.Text = "";
            TextBox_SpriteID.Text = "";
            TextBox_Comment.Text = "";
            CheckBox_Collision.IsChecked = false;
            TextBox_Tag.Text = "";
        }

        private void SetNewInfoForTextBox()
        {
            TextBox_PosX.Text = levelTileA[currentTileArrayPos].PosX.ToString();
            TextBox_PosY.Text = levelTileA[currentTileArrayPos].PosY.ToString();
            TextBox_SpriteID.Text = levelTileA[currentTileArrayPos].SpriteID.ToString();
            TextBox_SpriteID.Text += " | " + levelTileB[currentTileArrayPos].SpriteID.ToString();
            TextBox_SpriteID.Text += " | " + levelTileC[currentTileArrayPos].SpriteID.ToString();
            TextBox_SpriteID.BorderBrush = Brushes.Gray;
            TextBox_SpriteID.Background = Brushes.White;
            TextBox_Comment.Text = levelTileA[currentTileArrayPos].Commentary.ToString();
            CheckBox_Collision.IsChecked = levelTileA[currentTileArrayPos].HasCollision;
            TextBox_Tag.Text = levelTileA[currentTileArrayPos].Tag.ToString();

        }

        #region Show images in Layer

        #region Layer A
        private void CreateLayerA()
        {
            for (int rep = 1; rep < 6; rep++)
            {
                string imageLocation = Environment.CurrentDirectory + "\\Sprites\\Outside_A" + rep + "\\Frames\\";
                string[] allFiles = Directory.GetFiles(imageLocation, "*.png", SearchOption.TopDirectoryOnly);
                int fileCount = allFiles.GetLength(0);

                LoadImagesToLayer(fileCount, imageLocation, "A", rep);
            }
        }
        #endregion

        #region Layer B
        private void CreateLayerB()
        {
            string imageLocation = Environment.CurrentDirectory + "\\Sprites\\Outside_B\\Frames\\";
            string[] allFiles = Directory.GetFiles(imageLocation, "*.png", SearchOption.TopDirectoryOnly);
            int fileCount = allFiles.GetLength(0);
            LoadImagesToLayer(fileCount, imageLocation, "B", 1);
        }
        #endregion

        #region Layer C
        private void CreateLayerC()
        {
            string imageLocation = Environment.CurrentDirectory + "\\Sprites\\Outside_C\\Frames\\";
            string[] allFiles = Directory.GetFiles(imageLocation, "*.png", SearchOption.TopDirectoryOnly);
            int fileCount = allFiles.GetLength(0);
            LoadImagesToLayer(fileCount, imageLocation, "C", 1);
        }

        /// <summary>
        /// Load images to layer
        /// </summary>
        /// <param name="_fileCount">how many files are in the folder</param>
        /// <param name="_imageLocation">location of Image</param>
        /// <param name="_layer">only the Letter if he first 2 letters. egs: A1 --> A, B5 --> B</param>
        /// <param name="_folderCount">only the number of the first 2 letters. egs: A1 --> 1, B5 --> 5</param>
        private void LoadImagesToLayer(int _fileCount, string _imageLocation, string _layer, int _folderCount)
        {
            for (int i = 0; i < _fileCount; i++)
            {
                string tempImgLocation = _imageLocation;
                string name = _layer + _folderCount + "_tile";
                string number = "";
                if (i < 10)
                    number += "00";
                else if (i < 100)
                    number += "0";
                number += i;
                name += number + ".png";

                tempImgLocation += name;
                string fullNumber = _layer + _folderCount + "_" + number;
                // add to dictionary
                switch (_layer + _folderCount)
                {
                    case "A1":
                        idToSpriteLocationA1.Add(fullNumber, tempImgLocation);
                        break;
                    case "A2":
                        idToSpriteLocationA2.Add(fullNumber, tempImgLocation);
                        break;
                    case "A3":
                        idToSpriteLocationA3.Add(fullNumber, tempImgLocation);
                        break;
                    case "A4":
                        idToSpriteLocationA4.Add(fullNumber, tempImgLocation);
                        break;
                    case "A5":
                        idToSpriteLocationA5.Add(fullNumber, tempImgLocation);
                        break;
                    case "B1":
                        idToSpriteLocationB1.Add(fullNumber, tempImgLocation);
                        break;
                    case "C1":
                        idToSpriteLocationC1.Add(fullNumber, tempImgLocation);
                        break;
                    default:
                        break;
                }

                LoadImage(tempImgLocation, fullNumber.First().ToString(), fullNumber);

            }

        }

        private int GetFileCount(string _path)
        {
            return GetFileCount(new DirectoryInfo(Environment.CurrentDirectory + "\\Sprites\\Outside_A1\\Frames\\"));
        }

        private int GetFileCount(DirectoryInfo _dInfo)
        {
            FileInfo[] fInfo = _dInfo.GetFiles("*.png");
            return fInfo.GetLength(0);
        }
        #endregion

        #endregion

        #region Load Sprites
        private void LoadImage(string _path, string _layer, string _tag)
        {
            WrapPanel panel = null;
            switch (_layer)
            {
                case "A":
                    panel = WrapPanel_LayerA;
                    break;
                case "B":
                    panel = WrapPanel_LayerB;
                    break;
                case "C":
                    panel = WrapPanel_LayerC;
                    break;
                default:
                    return;
            }

            Image img = new Image();
            img.BeginInit();
            img.Tag = _tag;
            img.Focusable = false;

            img.MouseLeftButtonDown += Img_MouseLeftButtonDown;
            img.Source = new BitmapImage(new Uri(_path));
            img.Height = 50;
            img.Width = 50;
            img.Stretch = Stretch.Uniform;


            panel.Children.Add(img);
            img.EndInit();
        }
        #endregion

        /// <summary>
        /// looks up in dictionary to convert from tag to location
        /// </summary>
        /// <param name="_tag">Tag of Image</param>
        /// <returns>location of image</returns>
        private string TagToImageLocation(string _tag)
        {
            string chars = FirstTwoChars(_tag);
            string imageLocation = "";
            switch (chars)
            {
                case "A1":
                    imageLocation = idToSpriteLocationA1[_tag];
                    break;
                case "A2":
                    imageLocation = idToSpriteLocationA2[_tag];
                    break;
                case "A3":
                    imageLocation = idToSpriteLocationA3[_tag];
                    break;
                case "A4":
                    imageLocation = idToSpriteLocationA4[_tag];
                    break;
                case "A5":
                    imageLocation = idToSpriteLocationA5[_tag];
                    break;
                case "B1":
                    imageLocation = idToSpriteLocationB1[_tag];
                    break;
                case "C1":
                    imageLocation = idToSpriteLocationC1[_tag];
                    break;
                default:
                    break;
            }

            return imageLocation;
        }

        private string FirstTwoChars(string _word)
        {
            char[] c = _word.ToCharArray();
            string s = (c[0].ToString() + c[1].ToString()).ToString();
            return s;
        }

        private void SetImages()
        {
            int count = -1;
            foreach (Border border in Grid_GridBorder.Children)
            {
                count++;
                // get image from border
                Image img = (Image)border.Child;
                // if ID is empty continue
                if (levelTileA[count].SpriteID == "0")
                {
                    continue;
                }
                // set image
                img.Source = new BitmapImage(new Uri(TagToImageLocation(levelTileA[count].SpriteID)));
            }
        }

        #region Recent Files
        private void AddToRecentFilesHeader()
        {
            // remove all header if header exists
            while (true)
            {

                if (Button_RecentFiles.HasItems)
                {
                    Button_RecentFiles.Items.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            // read Text File
            string[] files = h.ReadTextFile();

            // add Header
            for (int LinesAmount = 0; LinesAmount < files.Length; LinesAmount++)
            {
                // check if string is empty, break out
                if (files[LinesAmount] == "" || files[LinesAmount] == null)
                {
                    break;
                }

                // create new menu item
                MenuItem item = new MenuItem();
                item.Header = files[LinesAmount];
                item.Click += OpenFile;
                Button_RecentFiles.Items.Add(item);
            }

            // if recent files is empty create header wit empty
            if (Button_RecentFiles.HasItems == false)
            {
                MenuItem empty = new MenuItem();
                empty.Header = "(empty)";
                empty.IsEnabled = false;
                Button_RecentFiles.Items.Add(empty);
            }



        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            MenuItem clickedItem = (MenuItem)sender;
            LoadFile(clickedItem.Header.ToString());
        }

        private void LoadFile(string _path)
        {
            if (File.Exists(_path) == false)
            {
                MessageBox.Show("File could not be found: " + _path, "Something went wrong", MessageBoxButton.OK);
                return;
            }
            Level level = new Level();
            Level tmpLevel = new Level();
            if (_path.EndsWith(".lvl".ToLower()))
            {
                level = lvlManager.LoadLevel(_path);
            }
            else
            {
                level = lvlManager.LoadLevelXML(_path);
            }

            // save to temp level
            tmpLevel = level;

            if (allBorders != null)
            {
                // delete old grid and buttons
                DeleteEverything();
            }

            // set grid and buttons
            GenerateGridWithButtons(level.SizeY, level.SizeX);

            // set level, temp layer and Tiles
            level = tmpLevel;
            levelLayer = level.Layer.ToArray();
            levelTileA = levelLayer[0].Tiles.ToArray();
            levelTileB = levelLayer[1].Tiles.ToArray();
            levelTileC = levelLayer[2].Tiles.ToArray();

            SetImages();

            path = _path;

            // loading complete. set statusbar
            SetStatus(Label_StatusbarOne, "Loading Complete", true);
        }
        #endregion

        private string GetLastChars(string _id, int _charCount)
        {
            char[] idArray = _id.ToCharArray();
            string toReturn = "";
            for (int i = 0; i < _charCount; i++)
            {
                toReturn = idArray[idArray.GetLength(0) - (1 + i)] + toReturn;
            }
            return toReturn;
        }

    }
}
