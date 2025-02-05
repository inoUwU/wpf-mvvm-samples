using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using wpf_mvvm_sample.ValidationServices;

namespace wpf_mvvm_sample.Models;

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
    
    [ObservableProperty]
    private int _sex;

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
