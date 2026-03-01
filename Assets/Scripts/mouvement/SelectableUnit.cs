using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public bool isSelected = false;
    public bool selectOnStart = false;

    private Renderer rend;
    private Color originalColor;

    private Recruitable recruitable;
    private Companion companion;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend)
            originalColor = rend.material.color;

        recruitable = GetComponent<Recruitable>();
        companion = GetComponent<Companion>();
    }

    void Start()
    {
        if (selectOnStart && SelectionManager.Instance != null)
        {
            if (CanBeSelected())
                SelectionManager.Instance.SelectUnit(this);
        }
    }

    bool CanBeSelected()
    {
        if (recruitable == null)
            return true;

        if (companion == null || !companion.isRecruited)
            return false;

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

    // =====================================================
    // 🔥 NETTOYAGE AUTO À LA MORT
    // =====================================================
    void OnDestroy()
    {
        if (SelectionManager.Instance != null)
            SelectionManager.Instance.RemoveUnit(this);
    }
}