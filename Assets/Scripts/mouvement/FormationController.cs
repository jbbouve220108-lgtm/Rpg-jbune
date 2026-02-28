using UnityEngine;
using UnityEngine.AI;
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
    public float arrowForwardOffset = 2.5f;
    public float groundOffset = 0.05f;

    [Header("Ground Projection")]
    public float groundRayHeight = 10f;
    public float groundRayDistance = 30f;

    [Header("Rotation")]
    public float rotationSpeed = 6f;

    private Vector2 rightStart;
    private float rightDownTime;
    private bool forming;

    private Vector3 startPoint;
    private Vector3 formationForward = Vector3.forward;

    private readonly List<GameObject> markers = new();
    private GameObject flecheInstance;

    // 🔥 rotations finales
    private readonly Dictionary<Transform, Vector3> finalLookDirections = new();

    void Update()
    {
        if (UIState.IsModalOpen)
            return;

        HandleRightMouse();
    }

    // =====================================================
    // 🖱️ RIGHT CLICK — FORMATION
    // =====================================================
    void HandleRightMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightStart = Input.mousePosition;
            rightDownTime = Time.time;
            forming = false;

            // 🔒 IMPORTANT : dès qu’on commence à appuyer
            SelectionManager.Instance.BlockNextRightClickDeselect = true;
        }

        if (Input.GetMouseButton(1))
        {
            float dist = Vector2.Distance(rightStart, Input.mousePosition);
            float heldTime = Time.time - rightDownTime;

            if (!forming && (dist > dragDistance || heldTime > clickTime))
                TryStartFormation();

            if (forming)
                UpdateFormationPreview();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (forming)
            {
                ConfirmFormation();
            }

            // 🔒 IMPORTANT : on protège AUSSI le relâchement
            SelectionManager.Instance.BlockNextRightClickDeselect = true;
            forming = false;
        }
    }

    // =====================================================
    // FORMATION START
    // =====================================================
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

    // =====================================================
    // PREVIEW
    // =====================================================
    void UpdateFormationPreview()
    {
        if (!TryGetMouseGround(out Vector3 current))
            return;

        formationForward = current - startPoint;
        formationForward.y = 0f;

        if (formationForward.sqrMagnitude < 0.001f)
            formationForward = Vector3.forward;
        else
            formationForward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, formationForward).normalized;

        var units = SelectionManager.Instance.GetSelectedUnits();
        float half = (units.Count - 1) / 2f;

        for (int i = 0; i < units.Count; i++)
        {
            Vector3 logicalPos = startPoint + right * (i - half) * spacing;
            PlaceOnGround(markers[i], logicalPos, formationForward);
            markers[i].SetActive(true);
        }

        UpdateArrow();
    }

    // =====================================================
    // CONFIRM
    // =====================================================
    void ConfirmFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();

        for (int i = 0; i < units.Count; i++)
        {
            NavMeshAgent agent = units[i].GetComponent<NavMeshAgent>();
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                continue;

            agent.stoppingDistance = 0f;
            agent.SetDestination(markers[i].transform.position);

            finalLookDirections[units[i].transform] = formationForward;
        }

        ClearAll();
    }

    // =====================================================
    // ROTATION APRÈS ARRIVÉE
    // =====================================================
    void LateUpdate()
    {
        if (finalLookDirections.Count == 0)
            return;

        List<Transform> done = new();

        foreach (var kvp in finalLookDirections)
        {
            Transform unit = kvp.Key;
            Vector3 dir = kvp.Value;

            if (unit == null)
            {
                done.Add(unit);
                continue;
            }

            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                done.Add(unit);
                continue;
            }

            if (!agent.pathPending &&
                agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                agent.updateRotation = false;

                Quaternion targetRot = Quaternion.LookRotation(dir);
                unit.rotation = Quaternion.Slerp(
                    unit.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );

                if (Quaternion.Angle(unit.rotation, targetRot) < 1f)
                {
                    unit.rotation = targetRot;
                    agent.updateRotation = true;
                    done.Add(unit);
                }
            }
        }

        foreach (var t in done)
            finalLookDirections.Remove(t);
    }

    // =====================================================
    // CLEANUP
    // =====================================================
    void ClearAll()
    {
        foreach (var m in markers)
            Destroy(m);

        markers.Clear();

        if (flecheInstance != null)
            Destroy(flecheInstance);
    }

    // =====================================================
    // UTILS
    // =====================================================
    void UpdateArrow()
    {
        if (flecheInstance == null)
            return;

        Vector3 arrowPos = startPoint + formationForward * arrowForwardOffset;
        PlaceOnGround(flecheInstance, arrowPos, formationForward);
    }

    void CreateMarkers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject m = Instantiate(formationMarkerPrefab);
            m.SetActive(false);
            m.layer = LayerMask.NameToLayer("Ignore Raycast");
            markers.Add(m);
        }
    }

    void CreateArrow()
    {
        if (flechePrefab == null)
            return;

        flecheInstance = Instantiate(flechePrefab);
        flecheInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    void PlaceOnGround(GameObject obj, Vector3 logicalPosition, Vector3 forward)
    {
        Vector3 rayOrigin = logicalPosition + Vector3.up * groundRayHeight;
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRayDistance, layerMask))
        {
            obj.transform.position = hit.point + hit.normal * groundOffset;

            Vector3 projectedForward = Vector3.ProjectOnPlane(forward, hit.normal);
            if (projectedForward.sqrMagnitude < 0.001f)
                projectedForward = Vector3.ProjectOnPlane(Vector3.forward, hit.normal);

            obj.transform.rotation =
                Quaternion.LookRotation(projectedForward, hit.normal) *
                Quaternion.Euler(90f, 0f, 0f);
        }
    }

    bool TryGetMouseGround(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, layerMask))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}