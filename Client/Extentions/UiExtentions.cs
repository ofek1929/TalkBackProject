using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Extentions
{
    public static class UiExtentions
    {
        public static void SetElementToGrid(this Grid grid, UIElement element, int row = 0, int col = 0, bool setColSpan = false, bool setRowSpan = false)
        {
            if (!setColSpan && !setRowSpan)
            {
                Grid.SetRow(element, row);
                Grid.SetColumn(element, col);
                grid.Children.Add(element);
            }
            else if (setColSpan)
            {
                Grid.SetRow(element, row);
                Grid.SetColumnSpan(element, col);
                grid.Children.Add(element);
            }
            else if (setRowSpan)
            {
                Grid.SetRowSpan(element, row);
                Grid.SetColumn(element, col);
                grid.Children.Add(element);
            }
        }

        public static void SetElementToGridWithSpan(this Grid grid, UIElement element, int row = 0, int col = 0, int setColSpan = 0, int setRowSpan = 0)
        {
            if (setColSpan == 0)
            {
                Grid.SetRow(element, row);
                Grid.SetRowSpan(element, setRowSpan);
                Grid.SetColumn(element, col);
                grid.Children.Add(element);
            }
            else
            {
                Grid.SetRow(element, row);
                Grid.SetRowSpan(element, setRowSpan);
                Grid.SetColumn(element, col);
                Grid.SetColumnSpan(element, setColSpan);
                grid.Children.Add(element);
            }

        }

    }
}
