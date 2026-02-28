using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class OrderController : MonoBehaviour
{
    [Header("Move Marker")]
    public GameObject moveMarkerPrefab;

    [Header("Dispersion")]
    public float spacing = 2f;
    public float navMeshSampleRadius = 2.5f;

    private GameObject markerInstance;

    void Update()
    {
        if (UIState.IsModalOpen)
            return;

        HandleLeftClick();
    }

    // =====================================================
    // 🖱️ LEFT CLICK — MOVE ORDER ONLY
    // =====================================================
    void HandleLeftClick()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        // Sécurité : SelectionManager requis
        if (SelectionManager.Instance == null)
            return;

        List<SelectableUnit> units = SelectionManager.Instance.GetSelectedUnits();
        if (units.Count == 0)
            return;

        // Si la souris est sur une unité → PAS un ordre
        if (IsMouseOverSelectableUnit())
            return;

        if (!TryGetMouseGround(out Vector3 center))
            return;

        IssueMoveOrder(units, center);
    }

    // =====================================================
    // 🚶‍♂️ MOVE ORDER
    // =====================================================
    void IssueMoveOrder(List<SelectableUnit> units, Vector3 center)
    {
        ShowMarker(center);

        List<Vector3> destinations = ComputeSpreadPositions(center, units.Count);

        for (int i = 0; i < units.Count; i++)
        {
            SelectableUnit unit = units[i];

            Recruitable recruitable = unit.GetComponent<Recruitable>();
            Companion companion = unit.GetComponent<Companion>();
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();

            // Règles métier conservées
            if (recruitable != null && (companion == null || !companion.isRecruited))
                continue;

            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                continue;

            agent.SetDestination(destinations[i]);
        }
    }

    // =====================================================
    // 📐 DISPERSION AUTOUR DU POINT
    // =====================================================
    List<Vector3> ComputeSpreadPositions(Vector3 center, int count)
    {
        List<Vector3> points = new();

        if (count == 1)
        {
            points.Add(center);
            return points;
        }

        float radius = spacing * Mathf.Sqrt(count);
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle),
                0f,
                Mathf.Sin(angle)
            ) * radius;

            Vector3 candidate = center + offset;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
                points.Add(hit.position);
            else
                points.Add(center);
        }

        return points;
    }

    // =====================================================
    // 📍 MARKER VISUEL
    // =====================================================
    void ShowMarker(Vector3 position)
    {
        if (moveMarkerPrefab == null)
            return;

        if (markerInstance != null)
            Destroy(markerInstance);

        markerInstance = Instantiate(
            moveMarkerPrefab,
            position,
            Quaternion.identity
        );
    }

    // =====================================================
    // 🧠 UTILS
    // =====================================================
    bool IsMouseOverSelectableUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        return hit.collider.GetComponentInParent<SelectableUnit>() != null;
    }

    bool TryGetMouseGround(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, mask))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}