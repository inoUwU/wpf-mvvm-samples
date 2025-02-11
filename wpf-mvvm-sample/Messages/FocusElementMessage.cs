namespace wpf_mvvm_sample.Messages;

public class FocusElementMessage(string elementName)
{
    public string ElementName { get; } = elementName;
}