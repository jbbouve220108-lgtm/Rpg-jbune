using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Panels")]
    public GameObject inventoryRoot;
    public GameObject hudPanel;

    private bool isOpen = false;

    void Awake()
    {
        Instance = this;

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
    }

    // =====================================================
    // 👉 OUVRIR
    // =====================================================
    public void Open()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (inventoryRoot != null)
            inventoryRoot.SetActive(true);

        if (hudPanel != null)
            hudPanel.SetActive(false);

        UIState.OpenModal();
    }

    // =====================================================
    // 👉 FERMER
    // =====================================================
    public void Close()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        UIState.CloseModal();
    }

    // =====================================================
    // 👉 TOGGLE
    // =====================================================
    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }
}