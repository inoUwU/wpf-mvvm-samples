using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace wpf_mvvm_sample.ViewModels;

public partial class ValidationViewModel : ObservableObject, INotifyDataErrorInfo
{
    // エラー情報を保持するディクショナリ
    private readonly ConcurrentDictionary<string, List<string>> _errors = new ConcurrentDictionary<string, List<string>>();
    
    // INotifyDataErrorInfoの実装
    public bool HasErrors => _errors.Any(kv => kv.Value?.Count > 0);
    
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return Enumerable.Empty<string>();
            
        return _errors.GetValueOrDefault(propertyName, new List<string>());
    }
    
    // エラーを追加するメソッド
    protected void SetError(string propertyName, string error)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();
            
        if (!_errors[propertyName].Contains(error))
        {
            _errors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }
    
    // エラーをクリアするメソッド
    protected void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.TryRemove(propertyName, out _);
            OnErrorsChanged(propertyName);
        }
    }
    
    // 全てのエラーをクリア
    protected void ClearAllErrors()
    {
        foreach (var propertyName in _errors.Keys.ToList())
        {
            ClearErrors(propertyName);
        }
    }
    
    // ErrorsChangedイベントを発火
    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }
    
    // バリデーション用プロパティの例
    [ObservableProperty]
    private string _inputCode = string.Empty;
    
    // InputCodeプロパティ変更後の処理（これはオプショナル）
    partial void OnInputCodeChanged(string value)
    {
        // 即時バリデーション（必要に応じて）
        if (string.IsNullOrWhiteSpace(value))
        {
            SetError(nameof(InputCode), "コードは必須です。");
        }
        else
        {
            ClearErrors(nameof(InputCode));
        }
    }

    [RelayCommand]
    private async Task Validate()
    {
      await ValidateInputCodeAsync();
    }
    
    // LostFocus時の非同期バリデーション
    private async Task ValidateInputCodeAsync()
    {
        // 入力チェック
        if (string.IsNullOrWhiteSpace(InputCode))
        {
            SetError(nameof(InputCode), "コードは必須です。");
            return;
        }
        
        try
        {
            // DBチェック（非同期）
            bool exists = await CheckIfCodeExistsInDbAsync(InputCode);
            
            if (!exists)
            {
                SetError(nameof(InputCode), $"コード '{InputCode}' はDBに存在しません。");
            }
            else
            {
                ClearErrors(nameof(InputCode));
            }
        }
        catch (Exception ex)
        {
            SetError(nameof(InputCode), $"検証中にエラーが発生しました: {ex.Message}");
        }
    }
    
    // DB検索のダミー実装（実際のロジックに置き換えてください）
    private async Task<bool> CheckIfCodeExistsInDbAsync(string code)
    {
        // 実際のDB検索ロジックを実装
        await Task.Delay(500); // シミュレーション用の遅延
        
        // テスト用のロジック（実際の実装では置き換えてください）
        return code == "12345" || code == "67890";
    }
}
