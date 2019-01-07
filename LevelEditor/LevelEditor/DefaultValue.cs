using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LevelEditor
{
    class DefaultValue
    {
        ///<summary>default info for button</summary>
        public static Button DefaultButton { get { return new Button { Margin = new Thickness(2) }; } }
        ///<summary>default info for Gridlength</summary>
        public static GridLength DefaultGridLength { get { return new GridLength(1d, GridUnitType.Star); } }
    }
}
