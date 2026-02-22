using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [Header("Thresholds")]
    public float dragDistance = 10f;
    public float clickTime = 0.2f;

    // ---- LEFT CLICK ----
    private Vector2 leftStart;
    private float leftDownTime;
    private bool leftDragging;
    private Rect selectionRect;

    // ---- RIGHT CLICK ----
    private Vector2 rightStart;
    private float rightDownTime;
    private bool rightDragging;

    private List<SelectableUnit> selectedUnits = new List<SelectableUnit>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (UIBlocker.Instance != null && UIBlocker.Instance.IsBlocked())
            return;
        if (MerchantUI.Instance != null && MerchantUI.Instance.IsOpen())
            return;

        HandleLeftMouse();
        HandleRightMouse();
    }

    // ======================================================
    // 🟩 LEFT CLICK — selection / order
    // ======================================================
    void HandleLeftMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftStart = Input.mousePosition;
            leftDownTime = Time.time;
            leftDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            float dist = Vector2.Distance(leftStart, Input.mousePosition);
            float held = Time.time - leftDownTime;

            if (dist > dragDistance || held > clickTime)
                leftDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (leftDragging)
                SelectUnitsInRectangle();
            else
                IssueOrder();
        }
    }

    void OnGUI()
    {
        if (Input.GetMouseButton(0) && leftDragging)
        {
            selectionRect = GetScreenRect(leftStart, Input.mousePosition);
            DrawSelectionRect(selectionRect);
        }
    }

    void IssueOrder()
    {
        if (selectedUnits.Count == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        foreach (SelectableUnit unit in selectedUnits)
        {
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent)
                agent.SetDestination(hit.point);
        }
    }

    void SelectUnitsInRectangle()
    {
        DeselectAll();

        foreach (SelectableUnit unit in FindObjectsByType<SelectableUnit>(FindObjectsSortMode.None))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(screenPos, true))
                SelectUnit(unit);
        }
    }

    // ======================================================
    // 🟥 RIGHT CLICK — deselect / formation
    // ======================================================
    void HandleRightMouse()
    {
        // Mouse down
        if (Input.GetMouseButtonDown(1))
        {
            rightStart = Input.mousePosition;
            rightDownTime = Time.time;
            rightDragging = false;
        }

        // Mouse hold
        if (Input.GetMouseButton(1))
        {
            float dist = Vector2.Distance(rightStart, Input.mousePosition);
            float held = Time.time - rightDownTime;

            if (dist > dragDistance || held > clickTime)
                rightDragging = true;
        }

        // Mouse up
        if (Input.GetMouseButtonUp(1))
        {
            // 🔴 PRIORITÉ ABSOLUE : Shift + unité
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    SelectableUnit unit = hit.collider.GetComponentInParent<SelectableUnit>();
                    if (unit)
                    {
                        DeselectUnit(unit);
                        return;
                    }
                }
                // Shift + clic droit sur sol → rien
                return;
            }

            // 🟢 clic droit maintenu → formation (gérée ailleurs)
            if (rightDragging)
                return;

            // 🟡 clic droit court → désélection totale
            DeselectAll();
        }
    }

    // ======================================================
    // 🧩 API
    // ======================================================
    public void SelectUnit(SelectableUnit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            unit.Select();
            selectedUnits.Add(unit);
        }
    }

    public void DeselectUnit(SelectableUnit unit)
    {
        if (selectedUnits.Contains(unit))
        {
            unit.Deselect();
            selectedUnits.Remove(unit);
        }
    }

    public void DeselectAll()
    {
        foreach (SelectableUnit unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }

    public List<SelectableUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    // ======================================================
    // 🔧 UTILS
    // ======================================================
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
}