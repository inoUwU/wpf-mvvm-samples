﻿using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using wpf_mvvm_sample.Messages;
using wpf_mvvm_sample.Models;
using wpf_mvvm_sample.Utils;
using wpf_mvvm_sample.ValidationServices;

namespace wpf_mvvm_sample.ViewModels;

public partial class MainWindowVm : ObservableValidator
{
    public MainWindowVm()
    {
        People =
        [
            new Person() { Name = "山田太郎", Age = 30, Sex = 0 },
            new Person() { Name = "鈴木花子", Age = 25, Sex = 1 },
        ];

        SexTypeComboBoxItems =
        [
            new ComboBoxItem()
            {
                Name = "男性",
                Value = 0
            },
            new ComboBoxItem()
            {
                Name = "女性",
                Value = 1
            }
        ];
    }

    [ObservableProperty] private ObservableCollection<Person> _people;

    [ObservableProperty] private DateTime? _selectedYearMonth;

    [ObservableProperty] private bool _isEnabledYearMonthPicker = true;

    [ObservableProperty] private ObservableCollection<ComboBoxItem> _sexTypeComboBoxItems;

    partial void OnSelectedYearMonthChanged(DateTime? value)
    {
        IsEnabledYearMonthPicker = false;
    }

    [RelayCommand]
    private void CancelDate()
    {
        SelectedYearMonth = null;
    }

    [RelayCommand]
    private void SetFocus()
    {
        if (People.Any())
        {
            // 1行目のNameカラムにフォーカスを設定
            // CurrentCell = new DataGridCellInfo(People[0], DataGridColumn.CanUserReorderProperty);
            WeakReferenceMessenger.Default.Send(new GridCellFocusRequestMessage
            {
                RowIndex = 0,
                ColumnIndex = 0
            });
        }
    }

    [RelayCommand]
    private void SetFocusToYearMonthPicker()
    {
        WeakReferenceMessenger.Default.Send(new FocusElementMessage("YmPicker"));
    }

    [RelayCommand]
    private void Save()
    {
        var errors = new StringBuilder();
        foreach (var person in _people)
        {
            var personErrors = person.GetErrors();
            if (personErrors.Any())
            {
                errors.AppendLine($"Person: {person.Name}");
                foreach (var error in personErrors)
                {
                    errors.AppendLine($" - {error}");
                }
            }
        }

        if (errors.Length > 0)
        {
            MessageBox.Show(errors.ToString(), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            // 保存処理
            MessageBox.Show("All data is valid. Saving data...");
        }
    }
}