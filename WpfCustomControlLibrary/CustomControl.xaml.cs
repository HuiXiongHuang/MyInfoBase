using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WPFCustomControlLibrary
{
    public partial class CustomControl
    {
 #region DataGrid事件
        double width = 0;

        private void ColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)

        {

            if (e.Source == null)

                return;

            DataGridColumnHeader header = e.Source as DataGridColumnHeader;

            DataGridColumn column = header.Column;

            if (column != null)

                width = column.Width.Value;

        }

        private void data_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)

        {
            if(e.Source is DataGrid)
            {
                DataGrid data = e.Source as DataGrid;
                for (int i = 0; i < data.Columns.Count; i++)
                      data.Columns[i].Width = width;
            }
           

        }
        #endregion


    }
}
