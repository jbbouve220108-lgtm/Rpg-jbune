using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public bool isSelected = false;
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend)
            originalColor = rend.material.color;
    }

    public void Select()
    {
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