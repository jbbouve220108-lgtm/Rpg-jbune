public static class UIState
{
    public static bool IsModalOpen { get; private set; }

    public static void OpenModal()
    {
        IsModalOpen = true;
    }

    public static void CloseModal()
    {
        IsModalOpen = false;
    }
}