using Liudax.LoopScrollView;

public class SimpleLoopItemView : LoopItemView<string>
{
    public TMPro.TMP_Text text;
    public override void ItemUpdate()
    {
        text.text = Data;
    }
}
