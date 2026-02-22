using UnityEngine;
using TMPro;

public class RenameUI : MonoBehaviour
{
    public static RenameUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_InputField nameInput;

    private Unit currentUnit;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(Unit unit)
    {
        if (unit == null)
            return;

        currentUnit = unit;

        UIState.IsModalOpen = true;

        nameInput.text = unit.unitName;
        panel.SetActive(true);
    }

    public void Confirm()
    {
        if (currentUnit != null && !string.IsNullOrWhiteSpace(nameInput.text))
        {
            currentUnit.unitName = nameInput.text.Trim();
        }

        Close();
    }

    void Close()
    {
        currentUnit = null;
        panel.SetActive(false);

        UIState.IsModalOpen = false;
    }
}