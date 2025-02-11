using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using wpf_mvvm_sample.Messages;
using wpf_mvvm_sample.ViewModels;

namespace wpf_mvvm_sample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // メッセージの受信登録
        WeakReferenceMessenger.Default.Register<GridCellFocusRequestMessage>(this, (r, m) =>
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MainDataGrid.Items.Count > m.RowIndex &&
                    MainDataGrid.Columns.Count > m.ColumnIndex)
                {
                    // ※SelectionUnlit="Cell"の場合は行の選択は行えずエラーになる
                    //  MainDataGrid.SelectedItem = MainDataGrid.Items[m.RowIndex];

                    // セルへフォーカスする
                    var cell = new DataGridCellInfo(
                        MainDataGrid.Items[m.RowIndex],
                        MainDataGrid.Columns[m.ColumnIndex]
                    );

                    // セルを選択
                    MainDataGrid.CurrentCell = cell;
                    MainDataGrid.Focus();

                    // セルを編集モードにする場合は以下を追加
                    MainDataGrid.BeginEdit();
                }
            }));
        });
        
        // 好きな要素にフォーカスを設定する
        // メッセージの受信機能
       WeakReferenceMessenger.Default.Register<Messages.FocusElementMessage>(this, (r, m) =>
       {
           Dispatcher.BeginInvoke(new Action(() =>
           {
                var element = this.FindName(m.ElementName);
                if(element is FrameworkElement frameworkElement)
                    frameworkElement.Focus();
           }));
       });
    }

    private void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not DataGridCell cell || cell.IsEditing) return;
        // セルを編集モードにする
        cell.IsEditing = true;
        Dispatcher.BeginInvoke(new Action(() =>
        {
            switch (cell.Content)
            {
                case TextBox textBox:
                    textBox.Focus();
                    break;
                case ComboBox comboBox:
                    comboBox.Focus();
                    break;
            }
        }), DispatcherPriority.Input);
    }

    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.GotFocus += DataGridRow_GotFocus;
    }

    private void DataGridRow_GotFocus(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not DataGridCell cell || cell.IsEditing) return;
        if (sender is not DataGrid dataGrid) return;

        // 行内のセルを編集モードにする   
        dataGrid.BeginEdit(e);
        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (cell.Content is TextBox textBox)
            {
                textBox.Focus();
            }
            else if (cell.Content is ComboBox comboBox)
            {
                comboBox.Focus();
            }
        }), DispatcherPriority.Input);
    }
}