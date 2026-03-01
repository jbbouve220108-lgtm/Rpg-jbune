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

        if (panel != null)
            panel.SetActive(false);
    }

    // =====================================================
    // 👉 OUVERTURE UI (BLOQUE LE MONDE)
    // =====================================================
    public void Open(Unit unit)
    {
        if (unit == null)
            return;

        currentUnit = unit;

        // 🔒 BLOCAGE CENTRALISÉ DES INPUTS MONDE
        UIState.OpenModal();

        // 🔹 Pré-remplissage avec le nom actuel
        if (nameInput != null)
            nameInput.text = unit.unitName;

        if (panel != null)
            panel.SetActive(true);
    }

    // =====================================================
    // 👉 CONFIRMATION (INCHANGÉE)
    // =====================================================
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

        // 🔴 FERMER AUSSI LA FENÊTRE DE RECRUTEMENT (COMPORTEMENT EXISTANT)
        if (RecruitUI.Instance != null)
        {
            RecruitUI.Instance.Close();
        }

        Close();
    }

    // =====================================================
    // 👉 FERMETURE UI (DÉBLOQUE LE MONDE)
    // =====================================================
    public void Close()
    {
        currentUnit = null;

        if (panel != null)
            panel.SetActive(false);

        // 🔓 RESTITUTION DES INPUTS MONDE
        UIState.CloseModal();
    }
}