using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace wpf_mvvm_sample.ViewModels;

// Models
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public partial class MainWindowVm : ObservableValidator
{
    public MainWindowVm()
    {
        People = new ObservableCollection<Person>
        {
            new Person { Name = "山田太郎", Age = 30 },
            new Person { Name = "鈴木花子", Age = 25 }
        };
    }

    [ObservableProperty] private ObservableCollection<Person> _people;

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