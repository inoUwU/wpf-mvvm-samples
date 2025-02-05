﻿using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
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
        WeakReferenceMessenger.Default.Register<MainWindowVm.FocusRequestMessage>(this, (r, m) =>
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
    }

    private void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not DataGridCell cell || cell.IsEditing) return;
        
        cell.IsEditing = true;
        switch (cell.Content)
        {
            case TextBox textBox:
                textBox.Focus();
                break;
            case ComboBox comboBox:
                comboBox.Focus();
                break;
        }
    }
}