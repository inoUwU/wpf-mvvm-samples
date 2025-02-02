using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using wpf_mvvm_sample.ValidationServices;

namespace wpf_mvvm_sample.ViewModels;

// Models
public partial class Person : ObservableValidator
{
    private readonly InputValidation _service;

    public Person(InputValidation service)
    {
        this._service = service;
    }

    [ObservableProperty]
    [CustomValidation(typeof(Person), nameof(ValidateName))]
    private string? _name;

    [ObservableProperty]
    [CustomValidation(typeof(Person), nameof(ValidateAge))]
    private int _age;

    /// <summary>
    /// 名前のバリデーション
    /// </summary>
    public static ValidationResult? ValidateName(string name, ValidationContext context)
    {
        // 共通化されたバリデーションロジックを呼び出す
        var instance = (Person)context.ObjectInstance;
        var isValid = instance._service.NameValidate(name);
        return isValid ? ValidationResult.Success : new ValidationResult("The name was not validated by the fancy service");
    }

    /// <summary>
    /// 年齢のバリデーション
    /// </summary>
    public static ValidationResult? ValidateAge(int age, ValidationContext context)
    {
        if (age <= 0 || age > 120)
        {
            return new ValidationResult("Age must be a positive number between 1 and 120.");
        }

        return ValidationResult.Success;
    }
}

public partial class MainWindowVm : ObservableValidator
{
    public MainWindowVm()
    {
       var service = new InputValidation(); 
        
        People = new ObservableCollection<Person>
        {
            new Person(service) { Name = "山田太郎", Age = 30 },
            new Person(service) { Name = "鈴木花子", Age = 25 }
        };
    }

    [ObservableProperty] private ObservableCollection<Person> _people;

    [ObservableProperty] private DateTime? _selectedYearMonth;

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