using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using wpf_mvvm_sample.Messages;
using wpf_mvvm_sample.Models;
using wpf_mvvm_sample.Utils;

namespace wpf_mvvm_sample.ViewModels;

public partial class MainWindowVm : ObservableValidator
{
    // 全ての選択可能なアイテム
    private readonly List<string> _allItems = ["選択肢1", "選択肢2", "選択肢3", "選択肢4", "選択肢5"];

    public MainWindowVm()
    {
        AddNewRow();
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

    [ObservableProperty] private ObservableCollection<Person>? _people = [];

    [ObservableProperty] private DateTime? _selectedYearMonth;

    [ObservableProperty] private bool _isEnabledYearMonthPicker;

    [ObservableProperty] private ObservableCollection<ComboBoxItem> _sexTypeComboBoxItems;

    [ObservableProperty] private ValidationViewModel? _validationViewModel;

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

    // 行を追加するコマンド
    [RelayCommand]
    private void AddNewRow()
    {
        if (People?.Count >= _allItems.Count) return;

        var newRow = new Person();
        newRow.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Person.SelectedValue))
            {
                // 選択値が変更されたら全ての行のComboBoxの選択可能アイテムを更新
                UpdateAvailableItems();
            }
        };
        People?.Add(newRow);
        UpdateAvailableItems();
    }

    // 各行の選択可能アイテムを更新
    private void UpdateAvailableItems()
    {
        // 現在選択されている値のリスト
        var selectedValues = People?
            .Where(item => !string.IsNullOrEmpty(item.SelectedValue))
            .Select(item => item.SelectedValue)
            .ToList();

        // 各行の選択可能アイテムを更新
        foreach (var item in People)
        {
            var availableItems = _allItems
                .Where(i => i == item.SelectedValue || !selectedValues.Contains(i))
                .ToList();

            item.UpdateAvailableItems(availableItems);
        }
    }
}