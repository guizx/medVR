namespace CC
{
    public interface ICustomizerUI
    {
        void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util parentUI);

        void RefreshUIElement();
    }
}