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

        // 🔹 Pré-remplissage avec le nom actuel
        nameInput.text = unit.unitName;
        panel.SetActive(true);
    }

    public void Confirm()
    {
        if (currentUnit != null)
        {
            string newName = nameInput.text.Trim();

            // 🔹 Application DU nom ici (source de vérité)
            if (!string.IsNullOrEmpty(newName))
            {
                currentUnit.unitName = newName;
            }
        }

        // 🔴 FERMER AUSSI LA FENÊTRE DE RECRUTEMENT
        if (RecruitUI.Instance != null)
        {
            RecruitUI.Instance.Close();
        }

        Close();
    }

    public void Close()
    {
        currentUnit = null;
        panel.SetActive(false);
        UIState.IsModalOpen = false;
    }
}