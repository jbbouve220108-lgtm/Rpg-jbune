using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public bool isSelected = false;
    public bool selectOnStart = false;

    private Renderer rend;
    private Color originalColor;

    // 🔹 AJOUT : lien vers Recruitable / Companion
    private Recruitable recruitable;
    private Companion companion;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend)
            originalColor = rend.material.color;

        // 🔹 Cache des composants
        recruitable = GetComponent<Recruitable>();
        companion = GetComponent<Companion>();
    }

    void Start()
    {
        if (selectOnStart && SelectionManager.Instance != null)
        {
            // 🔒 Ne sélectionner au start que si autorisé
            if (CanBeSelected())
                SelectionManager.Instance.SelectUnit(this);
        }
    }

    // 🔹 NOUVEAU : règle centrale
    bool CanBeSelected()
    {
        // Cas 1 : unité NON recruitable → toujours OK
        if (recruitable == null)
            return true;

        // Cas 2 : recruitable mais PAS encore recrutée → interdit
        if (companion == null || !companion.isRecruited)
            return false;

        // Cas 3 : recruitable + recrutée → OK
        return true;
    }

    public void Select()
    {
        if (!CanBeSelected())
            return;

        isSelected = true;
        if (rend)
            rend.material.color = Color.green;
    }

    public void Deselect()
    {
        isSelected = false;
        if (rend)
            rend.material.color = originalColor;
    }
}