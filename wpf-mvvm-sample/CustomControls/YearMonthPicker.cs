using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_mvvm_sample.CustomControls;

/// <summary>
/// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
///
/// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
/// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
/// 追加します:
///
///     xmlns:MyNamespace="clr-namespace:wpf_mvvm_sample.CustomControls"
///
///
/// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
/// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
/// 追加します:
///
///     xmlns:MyNamespace="clr-namespace:wpf_mvvm_sample.CustomControls;assembly=wpf_mvvm_sample.CustomControls"
///
/// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
/// リビルドして、コンパイル エラーを防ぐ必要があります:
///
///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
///
///
/// 手順 2)
/// コントロールを XAML ファイルで使用します。
///
///     <MyNamespace:YearMonthPicker/>
///
/// </summary>
/// 
public class YearMonthPicker : DatePicker
{
    public YearMonthPicker()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(YearMonthPicker), new FrameworkPropertyMetadata(typeof(YearMonthPicker)));
        //this.Loaded += OnLoaded;
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        var textBox = GetTemplateTextBox(this);
        if (textBox is not null)
        {
            textBox.PreviewKeyDown += TextBoxOnPreviewKeyDown;
            textBox.LostFocus += Lost;
            // 新しいバインディングを作成
            var binding = new Binding("SelectedDate")
            {
                Source = this,
                StringFormat = "yyyy/MM",
                Mode = BindingMode.TwoWay
            };
            // バインディングを適用
            textBox.SetBinding(TextBox.TextProperty, binding);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var textBox = GetTemplateTextBox(this);
        if (textBox is not null)
        {
            textBox.PreviewKeyDown += TextBoxOnPreviewKeyDown;
            textBox.LostFocus += Lost;
            // 新しいバインディングを作成
            var binding = new Binding("SelectedDate")
            {
                Source = this,
                StringFormat = "yyyy/MM",
                Mode = BindingMode.TwoWay
            };
            // バインディングを適用
            textBox.SetBinding(TextBox.TextProperty, binding);
        }
    }


    protected override void OnCalendarOpened(RoutedEventArgs e)
    {
        base.OnCalendarOpened(e);
        Popup popup = (Popup)this.Template.FindName("PART_Popup", this);
        if (popup != null)
        {
            Calendar cal = (Calendar)popup.Child;
            cal.DisplayModeChanged += Calendar_DisplayModeChanged;
            cal.DisplayMode = CalendarMode.Decade;
        }

        var textBox = GetTemplateTextBox(this);
        if (textBox is not null && this.SelectedDate.HasValue)
        {
            textBox.Text = this.SelectedDate.Value.ToString("yyyy/MM");
        }
    }

    protected override void OnCalendarClosed(RoutedEventArgs e)
    {
        base.OnCalendarClosed(e);
        Popup popup = (Popup)this.Template.FindName("PART_Popup", this);
        if (popup != null)
        {
            Calendar cal = (Calendar)popup.Child;
            cal.DisplayMode = CalendarMode.Month;
        }

        var textBox = GetTemplateTextBox(this);
        if (textBox is not null && this.SelectedDate.HasValue)
        {
            textBox.Text = this.SelectedDate.Value.ToString("yyyy/MM");
        }
    }

    protected override void OnSelectedDateChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectedDateChanged(e);
        var textBox = GetTemplateTextBox(this);
        if (textBox is not null && this.SelectedDate.HasValue)
        {
            textBox.Text = this.SelectedDate.Value.ToString("yyyy/MM");
        }
    }


    private static void Lost(object sender, EventArgs e)
    {

        var textBox = (TextBox)sender;
        var datePicker = (DatePicker)textBox.TemplatedParent;
        var dateStr = textBox.Text;
        if (DateTime.TryParse(dateStr, out var date))
        {
            datePicker.SelectedDate = date;
        }
    }


    private static void TextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Return) return;

        e.Handled = true;

        var textBox = (TextBox)sender;
        var datePicker = (DatePicker)textBox.TemplatedParent;
        var dateStr = textBox.Text;
        if (DateTime.TryParse(dateStr, out var date))
        {
            datePicker.SelectedDate = date;
        }
    }

    private static TextBox? GetTemplateTextBox(Control control)
    {
        control.ApplyTemplate();
        return (TextBox?)control.Template?.FindName("PART_TextBox", control);
    }

    private void Calendar_DisplayModeChanged(object? sender, CalendarModeChangedEventArgs e)
    {
        if (sender is not Calendar calendar) return;

        if (calendar.DisplayMode == CalendarMode.Month)
        {
            calendar.SelectedDate = calendar.DisplayDate;
            this.IsDropDownOpen = false;
        }
    }
}
