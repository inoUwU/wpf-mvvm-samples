using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using wpf_mvvm_sample.Models;
using wpf_mvvm_sample.Utils;
using wpf_mvvm_sample.ValidationServices;

namespace wpf_mvvm_sample.ViewModels;

public partial class MainWindowVm : ObservableValidator
{
    public MainWindowVm()
    {
       var service = new InputValidation(); 
        
        People =
        [
            new Person(service) { Name = "山田太郎", Age = 30,Sex = 0},
            new Person(service) { Name = "鈴木花子", Age = 25, Sex = 1},
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
    
    [ObservableProperty] private ObservableCollection<ComboBoxItem> _sexTypeComboBoxItems;

    // DataGridのフォーカス制御用のメッセージ
    public class FocusRequestMessage
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
    }

    [RelayCommand]
    private void SetFocus()
    {
        if (People.Any())
        {
            // 1行目のNameカラムにフォーカスを設定
            // CurrentCell = new DataGridCellInfo(People[0], DataGridColumn.CanUserReorderProperty);
            WeakReferenceMessenger.Default.Send(new FocusRequestMessage
            {
                RowIndex = 0,
                ColumnIndex = 0
            });
        }
    }
}