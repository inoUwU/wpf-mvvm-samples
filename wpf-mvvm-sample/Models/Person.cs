using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using wpf_mvvm_sample.ValidationServices;

namespace wpf_mvvm_sample.Models;

public partial class Person : ObservableValidator
{
    private readonly InputValidation _service = new InputValidation();

    /// <summary>
    /// サイズ
    /// </summary>
    [ObservableProperty] private string? _size;

    // memo: NotifyDataErrorInfoを使用しないとエラーが検証されない
    [NotifyDataErrorInfo] [ObservableProperty] [CustomValidation(typeof(Person), nameof(ValidateName))]
    private string? _name;

    [ObservableProperty] [NotifyDataErrorInfo] [CustomValidation(typeof(Person), nameof(ValidateAge))]
    private int _age;

    [NotifyDataErrorInfo] [Required(ErrorMessage = "性別を選択してください")] [ObservableProperty]
    private int _sex;


    // 選択可能なアイテムリスト
    [ObservableProperty] private List<string>? _availableItems = new();

    // 選択されたアイテム
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsSelectionValid))]
    private string _selectedValue;

    // 選択が有効かどうか（バリデーションなどに利用可能）
    public bool IsSelectionValid => !string.IsNullOrEmpty(SelectedValue);

    /// <summary>
    /// 名前のバリデーション
    /// </summary>
    public static ValidationResult? ValidateName(string name, ValidationContext context)
    {
        // 共通化されたバリデーションロジックを呼び出す
        var instance = (Person)context.ObjectInstance;
        var isValid = instance._service.NameValidate(name);
        return isValid
            ? ValidationResult.Success
            : new ValidationResult("The name must be a non-empty string.", ["Name"]);
    }

    /// <summary>
    /// 年齢のバリデーション
    /// </summary>
    public static ValidationResult? ValidateAge(int age, ValidationContext context)
    {
        if (age <= 0 || age > 120)
        {
            return new ValidationResult("Age must be a positive number between 1 and 120.", ["Age"]);
        }

        return ValidationResult.Success;
    }

    /// <summary>
    /// プロパティのエラーを取得する
    /// </summary>
    public IEnumerable<string> GetErrors(string propertyName)
    {
        return base.GetErrors(propertyName)?.OfType<string>() ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// オブジェクト全体のエラーを取得する
    /// </summary>
    public IEnumerable<string> GetAllErrors()
    {
        return base.GetErrors(string.Empty).OfType<string>();
    }

    // 利用可能なアイテムリストを更新
    public void UpdateAvailableItems(List<string> items)
    {
        AvailableItems = items;

        // 選択値が有効でなくなった場合にリセット
        if (SelectedValue != null && !items.Contains(SelectedValue))
        {
            SelectedValue = null;
        }
    }
}