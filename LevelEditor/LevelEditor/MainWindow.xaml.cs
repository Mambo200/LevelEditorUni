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
        ///<summary>Current Level info</summary>
        public static Level level = new Level();
        public static string path;
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
        private Button[,] allButtons = null;
        ///<summary>Current Button</summary>
        private static Button CurrentButton = null;
        ///<summary>All RowDefinitions</summary>
        private RowDefinition[] allRows = null;
        ///<summary>All ColumnDefinitions</summary>
        private ColumnDefinition[] allColumns = null;

        public MainWindow()
        {
            InitializeComponent();

            // Test
            UIElementCollection u = Grid_GridButtons.Children;

            //GenerateGridWithButtons(10, 10);
            
            u = Grid_GridButtons.Children;
            var ItemInFirstRow = u.Cast<UIElement>().Where(i => Grid.GetRow(i) == 0);
            
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
            DeleteAllButtons();
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

        /// <summary>
        /// Open File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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
            level = LvlManager.LoadLevel(path);

            // delete old grid and buttons
            DeleteEverything();

            // set grid and buttons
            GenerateGridWithButtons(level.SizeY, level.SizeX);
            
            // loading complete. set statusbar
            SetStatus(Label_StatusbarOne, "Loading Complete", true);
        }

        /// <summary>
        /// Close Application
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (path == null)
                Button_SaveAs_Click(sender, e);
            else
            {
                lvlManager.SaveLevel(path, level);
                ConfirmClose = true;
            }
        }

        /// <summary>
        /// Save File As ...
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            h.SaveFile(out bool? succellful);
            if (succellful == true)
                ConfirmClose = true;
            else
                ConfirmClose = false;
        }


        #endregion

        #endregion

        #region GroupBox

        #region Tiles
        private void ListBox_Tiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)ListBox_Tiles.SelectedItem;
            
        }
        #endregion
        #endregion

        #region Properties Window

        #region Name
        private void TextBox_SpriteID_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Layer> newLayer = level.Layer;
            
            level.Name = TextBox_SpriteID.Text;
        }
        #endregion

        #region PosX
        private void TextBox_PosX_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #region PosY
        private void TextBox_PosY_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #region Comment
        private void TextBox_Comment_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #region Collision

        private void CheckBox_Collision_Checked(object sender, RoutedEventArgs e)
        {
            // return if no button was chosen
            if (CurrentButton == null)
                return;

            string Tag = CurrentButton.Tag.ToString();
        }

        private void CheckBox_Collision_Unchecked(object sender, RoutedEventArgs e)
        {
            // return if no button was chosen
            if (CurrentButton == null)
                return;

            string Tag = CurrentButton.Tag.ToString();

        }
        #endregion

        #region Tag
        private void TextBox_Tag_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #endregion

        #region Grid Buttons
        /// <summary>
        /// button click event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void bttn_Click(object sender, RoutedEventArgs e)
        {
            CurrentButton = (Button)sender;
            changed = true;
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

        #region Create Grid and Buttons
        /// <summary>
        /// Generates the grid with buttons.
        /// </summary>
        /// <param name="_rowDef">height of row (Y Size)</param>
        /// <param name="_columnDef">width of column (X Size)</param>
        private void GenerateGridWithButtons(int _rowDef, int _columnDef)
        {
            // generate All Buttons array
            allButtons = new Button[_rowDef, _columnDef];

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
                Grid_GridButtons.RowDefinitions.Add(r);
            }
            // add ColumnDefinition
            for (int col = 0; col < _columnDef; col++)
            {
                // create new ColumnDefinition
                ColumnDefinition c = new ColumnDefinition { Width = DefaultValue.DefaultGridLength };
                // save in array
                allColumns[col] = c;
                // add ColumnDefinition to grid
                Grid_GridButtons.ColumnDefinitions.Add(c);
            }

            // create buttons
            for (int row = 0; row < _rowDef; row++)
            {
                for (int col = 0; col < _columnDef; col++)
                {
                    // create Button
                    Button bttn = CreateButton(row, col, row + "|" + col);
                    // write Button in array
                    allButtons[row, col] = bttn;
                    // show button in grid
                    ShowButtonInGrid(row, col, bttn);
                }
            }

            SetStatus(Label_StatusbarOne, "Buttons created");
        }

        /// <summary>
        /// creates button
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_buttonContent">Content of the button.</param>
        /// <returns></returns>
        private Button CreateButton(int _row, int _col, string _buttonContent = "")
        {
            Button bttn = DefaultValue.DefaultButton;
            bttn.Tag = _row + "|" + _col;
            bttn.Content = _buttonContent;
            bttn.Click += bttn_Click;
            bttn.Focusable = false;

            return bttn;
        }
        #endregion

        #region Show Button Function
        /// <summary>
        /// Creates a button in Grid_GridButtons with margin = 2
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_buttonContent">content of the button.</param>
        private void ShowButtonInGrid(int _row, int _col, string _buttonContent = "")
        {
            Button bttn = CreateButton(_row, _col, _buttonContent);

            ShowButtonInGrid(_row, _col, Grid_GridButtons, bttn);
        }

        /// <summary>
        /// shows a button in Grid_GridButtons
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_button">the button</param>
        private void ShowButtonInGrid(int _row, int _col, Button _button)
        {
            ShowButtonInGrid(_row, _col, Grid_GridButtons, _button);
        }

        /// <summary>
        /// show button in grid
        /// </summary>
        /// <param name="_row">row of grid</param>
        /// <param name="_col">column of grid</param>
        /// <param name="_grid">the grid itself</param>
        /// <param name="_button">the button</param>
        private void ShowButtonInGrid(int _row, int _col, Grid _grid, Button _button)
        {
            _grid.Children.Add(_button);
            Grid.SetRow(_button, _row);
            Grid.SetColumn(_button, _col);
        }
        #endregion

        #region Delete Functions

        /// <summary>
        /// Deletes all buttons.
        /// </summary>
        private void DeleteAllButtons()
        {
            Grid_GridButtons.Children.RemoveRange(0, Grid_GridButtons.Children.Count);
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
            if (Grid_GridButtons.RowDefinitions.Count == 0)
                return;

            // delete RowDefinitions
            Grid_GridButtons.RowDefinitions.RemoveRange(0, Grid_GridButtons.RowDefinitions.Count);
        }

        /// <summary>
        /// Delete column
        /// </summary>
        private void DeleteColumn()
        {
            // check size of row definition
            if (Grid_GridButtons.ColumnDefinitions.Count == 0)
                return;

            // delete ColumnDefinitions
            Grid_GridButtons.ColumnDefinitions.RemoveRange(0, Grid_GridButtons.ColumnDefinitions.Count);
        }

        /// <summary>
        /// Delete Grid and Buttons
        /// </summary>
        private void DeleteEverything()
        {
            DeleteAllButtons();
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
    }
}
