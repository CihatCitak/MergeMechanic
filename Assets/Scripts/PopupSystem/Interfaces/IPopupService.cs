namespace Game.PopupSystem
{
    public interface IPopupService
    {
        T Show<T>() where T : BasePopupView;
        void Close();
        void CloseAll();
    }
}
