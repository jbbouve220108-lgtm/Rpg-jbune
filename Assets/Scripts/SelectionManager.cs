using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [Header("Click / Drag")]
    public float dragDistance = 10f;
    public float clickTime = 0.2f;

    [Header("Move Marker")]
    public GameObject moveMarker;
    public float markerDuration = 1.2f;

    private Vector2 mouseStart;
    private float mouseDownTime;
    private bool isDragging;
    private Rect selectionRect;

    private List<SelectableUnit> selectedUnits = new List<SelectableUnit>();
    private Coroutine markerRoutine;

    void Awake()
    {
        Instance = this;
        if (moveMarker)
            moveMarker.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStart = Input.mousePosition;
            mouseDownTime = Time.time;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(mouseStart, Input.mousePosition) > dragDistance ||
                Time.time - mouseDownTime > clickTime)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
                SelectUnitsInRectangle();
            else
                HandleSimpleClick();
        }
    }

    void OnGUI()
    {
        if (Input.GetMouseButton(0) && isDragging)
        {
            selectionRect = GetScreenRect(mouseStart, Input.mousePosition);
            DrawSelectionRect(selectionRect);
        }
    }

    // 🖱️ CLIC SIMPLE → DÉPLACEMENT
    void HandleSimpleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        // 🔍 Récupère le player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SelectableUnit playerUnit = player ? player.GetComponent<SelectableUnit>() : null;

        // 🟢 CAS 1 : aucune unité sélectionnée → le player seul bouge
        if (selectedUnits.Count == 0)
        {
            if (playerUnit)
            {
                NavMeshAgent agent = playerUnit.GetComponent<NavMeshAgent>();
                if (agent)
                    agent.SetDestination(hit.point);
            }

            ShowMarker(hit.point);
            return;
        }

        // 🟡 CAS 2 : des unités sont sélectionnées
        foreach (SelectableUnit unit in selectedUnits)
        {
                NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
                if (agent)
                    agent.SetDestination(hit.point);
        }

        ShowMarker(hit.point);
    }

    // 🟩 SÉLECTION RECTANGLE
    void SelectUnitsInRectangle()
    {
        DeselectAll();

        foreach (SelectableUnit unit in FindObjectsByType<SelectableUnit>(FindObjectsSortMode.None))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (selectionRect.Contains(screenPos, true))
            {
                SelectUnit(unit);
            }
        }
    }

    public void SelectUnit(SelectableUnit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            unit.Select();
            selectedUnits.Add(unit);
        }
    }

    void DeselectAll()
    {
        foreach (SelectableUnit unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }

    // 🔵 MARKER
    void ShowMarker(Vector3 pos)
    {
        if (!moveMarker) return;

        moveMarker.transform.position = pos + Vector3.up * 0.02f;
        moveMarker.SetActive(true);

        if (markerRoutine != null)
            StopCoroutine(markerRoutine);

        markerRoutine = StartCoroutine(HideMarker());
    }

    IEnumerator HideMarker()
    {
        yield return new WaitForSeconds(markerDuration);
        moveMarker.SetActive(false);
    }

    // 🔧 UTILS
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

    public List<SelectableUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }
}