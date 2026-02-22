using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    private Vector2 startPos;
    private Rect selectionRect;

    private List<SelectableUnit> selectedUnits = new List<SelectableUnit>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Début sélection
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }

        // Fin sélection
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnitsInRectangle();
        }
    }

    void OnGUI()
    {
        if (Input.GetMouseButton(0))
        {
            selectionRect = GetScreenRect(startPos, Input.mousePosition);
            DrawSelectionRect(selectionRect);
        }
    }

    void SelectUnitsInRectangle()
    {
        DeselectAll();

        foreach (SelectableUnit unit in FindObjectsByType<SelectableUnit>(FindObjectsSortMode.None))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (selectionRect.Contains(screenPos, true))
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }
    }

    void DeselectAll()
    {
        foreach (SelectableUnit unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }

    // 🔧 Utils
    Rect GetScreenRect(Vector2 p1, Vector2 p2)
    {
        p1.y = Screen.height - p1.y;
        p2.y = Screen.height - p2.y;
        return Rect.MinMaxRect(
            Mathf.Min(p1.x, p2.x),
            Mathf.Min(p1.y, p2.y),
            Mathf.Max(p1.x, p2.x),
            Mathf.Max(p1.y, p2.y)
        );
    }

    void DrawSelectionRect(Rect rect)
    {
        GUI.color = new Color(0, 1, 0, 0.25f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.green;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1, rect.height), Texture2D.whiteTexture);
    }

    // 🔓 Accès futur (ordres)
    public List<SelectableUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }
}