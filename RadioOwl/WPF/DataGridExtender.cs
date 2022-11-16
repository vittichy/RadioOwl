using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace WPF
{
    /// <summary>
    /// 
    /// http://stackoverflow.com/questions/22868445/wpf-binding-selecteditems-in-mvvm
    /// </summary>
    public class DataGridExtender : DataGrid
    {

        public DataGridExtender()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(DataGridExtender), new PropertyMetadata(null));

        #endregion
    }
}
