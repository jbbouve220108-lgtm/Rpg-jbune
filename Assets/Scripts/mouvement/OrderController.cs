using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class OrderController : MonoBehaviour
{
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
    // 🖱️ LEFT CLICK — MOVE
    // =====================================================
    void HandleLeftClick()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        // 🔒 clic consommé par la sélection
        if (SelectionManager.Instance.ConsumeNextLeftClick)
        {
            SelectionManager.Instance.ConsumeNextLeftClick = false;
            return;
        }

        var units = SelectionManager.Instance.GetSelectedUnits();
        if (units.Count == 0)
            return;

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

        List<Vector3> targets = ComputeSpreadPositions(center, units.Count);

        for (int i = 0; i < units.Count; i++)
        {
            NavMeshAgent agent = units[i].GetComponent<NavMeshAgent>();
            Companion comp = units[i].GetComponent<Companion>();
            Recruitable rec = units[i].GetComponent<Recruitable>();

            if (rec != null && (comp == null || !comp.isRecruited))
                continue;

            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                continue;

            agent.SetDestination(targets[i]);
        }
    }

    // =====================================================
    // 📐 DISPERSION
    // =====================================================
    List<Vector3> ComputeSpreadPositions(Vector3 center, int count)
    {
        List<Vector3> points = new List<Vector3>();

        if (count == 1)
        {
            points.Add(center);
            return points;
        }

        float radius = spacing * Mathf.Sqrt(count);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;

            // ✅ CORRECTION SYNTAXIQUE UNIQUEMENT
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
    // 📍 MARKER
    // =====================================================
    void ShowMarker(Vector3 pos)
    {
        if (moveMarkerPrefab == null)
            return;

        if (markerInstance != null)
            Destroy(markerInstance);

        markerInstance = Instantiate(moveMarkerPrefab, pos, Quaternion.identity);
    }

    // =====================================================
    // 🧠 UTILS
    // =====================================================
    bool IsMouseOverSelectableUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit)
               && hit.collider.GetComponentInParent<SelectableUnit>() != null;
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