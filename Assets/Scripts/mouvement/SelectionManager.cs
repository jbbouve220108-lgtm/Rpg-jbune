using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [Header("Drag Selection")]
    public float dragDistance = 10f;

    // ─────────────────────────────
    // ÉTAT SOURIS
    // ─────────────────────────────
    private Vector2 leftStart;
    private bool leftDragging;
    private Rect selectionRect;

    // ─────────────────────────────
    // SÉLECTION
    // ─────────────────────────────
    private readonly List<SelectableUnit> selectedUnits = new();

    // ─────────────────────────────
    // FORMATION GUARD
    // ─────────────────────────────
    // Utilisé par FormationController
    public bool BlockNextRightClickDeselect { get; set; }

    // ─────────────────────────────
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (UIState.IsModalOpen)
            return;

        HandleLeftMouse();
        HandleRightMouse();
    }

    // =====================================================
    // 🖱️ LEFT CLICK — SELECT / DRAG RECTANGLE
    // =====================================================
    void HandleLeftMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftStart = Input.mousePosition;
            leftDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (!leftDragging &&
                Vector2.Distance(leftStart, Input.mousePosition) > dragDistance)
            {
                leftDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (leftDragging)
            {
                SelectUnitsInRectangle();
            }
            else
            {
                TrySelectSingleUnit();
            }

            leftDragging = false;
        }
    }

    // =====================================================
    // 🖱️ RIGHT CLICK — DESELECT (SI AUTORISÉ)
    // =====================================================
    void HandleRightMouse()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (BlockNextRightClickDeselect)
            {
                BlockNextRightClickDeselect = false;
                return;
            }

            DeselectAll();
        }
    }

    // =====================================================
    // 🎯 SINGLE UNIT SELECTION
    // =====================================================
    void TrySelectSingleUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        SelectableUnit unit = hit.collider.GetComponentInParent<SelectableUnit>();
        if (unit == null)
            return;

        DeselectAll();
        SelectUnit(unit);
    }

    // =====================================================
    // 📦 RECTANGLE SELECTION
    // =====================================================
    void SelectUnitsInRectangle()
    {
        DeselectAll();

        selectionRect = GetScreenRect(leftStart, Input.mousePosition);

        foreach (SelectableUnit unit in FindObjectsByType<SelectableUnit>(FindObjectsSortMode.None))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (screenPos.z < 0)
                continue;

            if (selectionRect.Contains(screenPos, true))
            {
                SelectUnit(unit);
            }
        }
    }

    void OnGUI()
    {
        if (leftDragging)
        {
            selectionRect = GetScreenRect(leftStart, Input.mousePosition);
            DrawSelectionRect(selectionRect);
        }
    }

    // =====================================================
    // 🟩 API PUBLIQUE
    // =====================================================
    public List<SelectableUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    public void SelectUnit(SelectableUnit unit)
    {
        if (unit == null)
            return;

        unit.Select();

        if (!unit.isSelected)
            return;

        if (!selectedUnits.Contains(unit))
            selectedUnits.Add(unit);
    }

    public void DeselectAll()
    {
        foreach (SelectableUnit unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }

    // =====================================================
    // 🧰 UTILS
    // =====================================================
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
    }
}