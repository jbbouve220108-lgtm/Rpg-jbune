using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [Header("Thresholds")]
    public float dragDistance = 10f;
    public float clickTime = 0.2f;

    private Vector2 leftStart;
    private float leftDownTime;
    private bool leftDragging;
    private Rect selectionRect;

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
        // =====================================================
        // 🔒 BLOCAGE TOTAL SI UI MODALE
        // =====================================================
        if (UIState.IsModalOpen)
            return;
        // =====================================================

        HandleLeftMouse();
        HandleRightMouse();
    }

    // ================= LEFT CLICK =================
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
            // 🔒 CORRECTION CLÉ :
            // Si on a cliqué sur un Recruitable, ON NE TOUCHE PAS À LA SÉLECTION
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<Recruitable>() != null)
                {
                    leftDragging = false;
                    return;
                }
            }

            if (leftDragging)
            {
                SelectUnitsInRectangle();
            }
            else
            {
                IssueMoveOrder();
            }

            leftDragging = false;
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

    void SelectUnitsInRectangle()
    {
        DeselectAll();

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

    // ================= MOVE ORDER =================
    void IssueMoveOrder()
    {
        if (selectedUnits.Count == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        foreach (SelectableUnit unit in selectedUnits)
        {
            Recruitable recruitable = unit.GetComponent<Recruitable>();
            Companion companion = unit.GetComponent<Companion>();

            if (recruitable != null && (companion == null || !companion.isRecruited))
                continue;

            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    // ================= RIGHT CLICK =================
    void HandleRightMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightStart = Input.mousePosition;
            rightDownTime = Time.time;
            rightDragging = false;
        }

        if (Input.GetMouseButton(1))
        {
            float dist = Vector2.Distance(rightStart, Input.mousePosition);
            float held = Time.time - rightDownTime;

            if (dist > dragDistance || held > clickTime)
                rightDragging = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (!rightDragging)
                DeselectAll();
        }
    }

    // ================= API PUBLIQUE =================
    public List<SelectableUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    public void SelectUnit(SelectableUnit unit)
    {
        unit.Select();

        if (!unit.isSelected)
            return;

        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
        }
    }

    public void DeselectAll()
    {
        foreach (SelectableUnit unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }

    // ================= UTILS =================
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