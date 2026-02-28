using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class FormationController : MonoBehaviour
{
    public GameObject formationMarkerPrefab;
    public GameObject flecheMarkerPrefab; // 🆕 Flèche
    public float spacing = 2f;

    [Header("Thresholds")]
    public float dragDistance = 10f;
    public float clickTime = 0.2f;

    [Header("Arrow Settings")]
    public float arrowForwardOffset = 1.8f;   // distance devant la formation
    public float arrowGroundOffset = 0.02f;   // très léger offset sol

    private Vector2 rightStart;
    private float rightDownTime;
    private bool forming;

    private Vector3 startPoint;
    private List<GameObject> markers = new List<GameObject>();

    // 🆕 Flèche runtime
    private GameObject arrowInstance;

    void Update()
    {
        // 🔒 Blocage global si une UI est ouverte
        if (UIState.IsModalOpen)
            return;

        HandleRightMouse();
    }

    void HandleRightMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightStart = Input.mousePosition;
            rightDownTime = Time.time;
            forming = false;
        }

        if (Input.GetMouseButton(1))
        {
            float dist = Vector2.Distance(rightStart, Input.mousePosition);
            float heldTime = Time.time - rightDownTime;

            if (!forming && (dist > dragDistance || heldTime > clickTime))
            {
                TryStartFormation();
            }

            if (forming)
                UpdateFormationPreview();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (forming)
            {
                ConfirmFormation();
                forming = false;
            }
        }
    }

    void TryStartFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();
        if (units.Count <= 1)
            return;

        if (!TryGetMouseGround(out startPoint))
            return;

        forming = true;
        CreateMarkers(units.Count);
        CreateArrow();
    }

    void UpdateFormationPreview()
    {
        if (!TryGetMouseGround(out Vector3 current))
            return;

        Vector3 dir = (current - startPoint).normalized;
        if (dir == Vector3.zero)
            dir = Vector3.forward;

        Vector3 right = Vector3.Cross(Vector3.up, dir);

        var units = SelectionManager.Instance.GetSelectedUnits();
        float half = (units.Count - 1) / 2f;

        // =============================
        // FORMATION
        // =============================
        for (int i = 0; i < units.Count; i++)
        {
            Vector3 pos = startPoint + right * (i - half) * spacing;
            markers[i].transform.position = pos + Vector3.up * 0.05f;
            markers[i].SetActive(true);
        }

        // =============================
        // FLÈCHE (DEVANT LA FORMATION)
        // =============================
        UpdateArrow(dir, right, units.Count, half);
    }

    void UpdateArrow(Vector3 dir, Vector3 right, int unitCount, float half)
    {
        if (arrowInstance == null)
            return;

        // centre de la formation
        Vector3 center =
            startPoint +
            right * (0 - half) * spacing +
            right * ((unitCount - 1) * spacing * 0.5f);

        // position devant
        Vector3 arrowPos = center + dir * arrowForwardOffset;

        // 🔥 Projection SOL
        if (Physics.Raycast(arrowPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
        {
            arrowPos = hit.point + Vector3.up * arrowGroundOffset;
        }

        arrowInstance.transform.position = arrowPos;

        // 🔥 Rotation SOL (toujours visible)
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        arrowInstance.transform.rotation = rot * Quaternion.Euler(90f, 0f, 0f);

        arrowInstance.SetActive(true);
    }

    void ConfirmFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();

        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i].GetComponent<Unit>();
            if (unit != null && unit.unitType == UnitType.Player)
            {
                // joueur ignore formation
            }

            Recruitable recruitable = units[i].GetComponent<Recruitable>();
            Companion companion = units[i].GetComponent<Companion>();

            if (recruitable != null && (companion == null || !companion.isRecruited))
                continue;

            if (companion != null)
                companion.OnFormationOrder();

            NavMeshAgent agent = units[i].GetComponent<NavMeshAgent>();
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                continue;

            agent.stoppingDistance = 0f;
            agent.SetDestination(markers[i].transform.position);
        }

        ClearMarkers();
        ClearArrow();
    }

    void CreateMarkers(int count)
    {
        ClearMarkers();

        for (int i = 0; i < count; i++)
        {
            GameObject m = Instantiate(formationMarkerPrefab);
            m.SetActive(false);
            markers.Add(m);
        }
    }

    void CreateArrow()
    {
        ClearArrow();

        if (flecheMarkerPrefab == null)
            return;

        arrowInstance = Instantiate(flecheMarkerPrefab);
        arrowInstance.SetActive(false);
    }

    void ClearMarkers()
    {
        foreach (var m in markers)
            Destroy(m);

        markers.Clear();
    }

    void ClearArrow()
    {
        if (arrowInstance != null)
            Destroy(arrowInstance);

        arrowInstance = null;
    }

    bool TryGetMouseGround(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}