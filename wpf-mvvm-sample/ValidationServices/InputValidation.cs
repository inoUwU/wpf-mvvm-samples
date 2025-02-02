namespace wpf_mvvm_sample.ValidationServices;

public class InputValidation
{
    // ここに実際のバリデーションロジックを記述します
    // 例えば、名前が特定のパターンに一致するかどうかをチェックするなど
    public bool NameValidate(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }
}