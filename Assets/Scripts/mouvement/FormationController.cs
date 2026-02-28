using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class FormationController : MonoBehaviour
{
    public GameObject formationMarkerPrefab;
    public GameObject flechePrefab;

    public float spacing = 2f;

    [Header("Thresholds")]
    public float dragDistance = 10f;
    public float clickTime = 0.2f;

    [Header("Arrow")]
    public float arrowForwardOffset = 1.5f;
    public float arrowGroundOffset = 0.02f;
    public float arrowFlatRotationX = 90f; // 🔹 flèche couchée

    private Vector2 rightStart;
    private float rightDownTime;
    private bool forming;

    private Vector3 startPoint;
    private Vector3 formationForward = Vector3.forward;

    private List<GameObject> markers = new();
    private GameObject flecheInstance;

    void Update()
    {
        // 🔒 Blocage global si UI ouverte
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
            return;

        formationForward = dir;

        Vector3 right = Vector3.Cross(Vector3.up, dir);
        var units = SelectionManager.Instance.GetSelectedUnits();
        float half = (units.Count - 1) / 2f;

        for (int i = 0; i < units.Count; i++)
        {
            Vector3 pos = startPoint + right * (i - half) * spacing;
            markers[i].transform.position = pos + Vector3.up * 0.05f;
            markers[i].SetActive(true);
        }

        // =====================================================
        // 🔹 FLÈCHE DEVANT LA FORMATION (COUCHÉE AU SOL)
        // =====================================================
        Vector3 arrowPos = startPoint + dir * arrowForwardOffset;
        arrowPos.y += arrowGroundOffset;

        flecheInstance.transform.position = arrowPos;

        // 👉 rotation à plat + direction
        float yRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;
        flecheInstance.transform.rotation = Quaternion.Euler(
            arrowFlatRotationX,
            yRotation,
            0f
        );

        flecheInstance.SetActive(true);
    }

    void ConfirmFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();

        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i].GetComponent<Unit>();
            if (unit != null && unit.unitType == UnitType.Player)
            {
                // 🔹 Le joueur ignore la formation (logique existante)
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

            // 🔹 Orientation finale vers la flèche
            StartCoroutine(RotateOnArrival(units[i].transform, formationForward));
        }

        ClearMarkers();
        ClearArrow();
    }

    IEnumerator RotateOnArrival(Transform unit, Vector3 forward)
    {
        NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
        if (agent == null)
            yield break;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.05f)
            yield return null;

        agent.isStopped = true;

        Quaternion startRot = unit.rotation;
        Quaternion targetRot = Quaternion.LookRotation(forward, Vector3.up);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            unit.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        agent.isStopped = false;
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

    void ClearMarkers()
    {
        foreach (var m in markers)
            Destroy(m);

        markers.Clear();
    }

    void CreateArrow()
    {
        if (flecheInstance != null)
            Destroy(flecheInstance);

        flecheInstance = Instantiate(flechePrefab);
        flecheInstance.SetActive(false);
    }

    void ClearArrow()
    {
        if (flecheInstance != null)
            Destroy(flecheInstance);
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