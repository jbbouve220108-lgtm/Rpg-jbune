using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class FormationController : MonoBehaviour
{
    public GameObject formationMarkerPrefab;
    public float spacing = 2f;

    [Header("Thresholds")]
    public float dragDistance = 10f;
    public float clickTime = 0.2f;

    private Vector2 rightStart;
    private float rightDownTime;
    private bool forming;

    private Vector3 startPoint;
    private List<GameObject> markers = new List<GameObject>();

    void Update()
    {
    if (MerchantUI.Instance != null && MerchantUI.Instance.IsOpen())
        return;
    

    HandleRightMouse();
 }

    void HandleRightMouse()
    {
        // Mouse down → on mémorise, MAIS on ne démarre PAS la formation
        if (Input.GetMouseButtonDown(1))
        {
            rightStart = Input.mousePosition;
            rightDownTime = Time.time;
            forming = false;
        }

        // Mouse hold → on vérifie si on ENTRE en formation
        if (Input.GetMouseButton(1))
        {
            float dist = Vector2.Distance(rightStart, Input.mousePosition);
            float heldTime = Time.time - rightDownTime;

            if (!forming && (dist > dragDistance || heldTime > clickTime))
            {
                TryStartFormation();
            }

            if (forming)
            {
                UpdateFormationPreview();
            }
        }

        // Mouse up
        if (Input.GetMouseButtonUp(1))
        {
            if (forming)
            {
                ConfirmFormation();
                forming = false;
            }
        }
    }

    // =========================
    // 🟢 DÉMARRAGE FORMATION
    // =========================
    void TryStartFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();
        if (units.Count <= 1)
            return;

        if (!TryGetMouseGround(out startPoint))
            return;

        forming = true;
        CreateMarkers(units.Count);
    }

    // =========================
    // 🔵 PREVIEW FORMATION
    // =========================
    void UpdateFormationPreview()
    {
        if (!TryGetMouseGround(out Vector3 current))
            return;

        Vector3 dir = (current - startPoint).normalized;
        if (dir == Vector3.zero)
            dir = Vector3.forward;

        Vector3 right = Vector3.Cross(Vector3.up, dir);

        var units = SelectionManager.Instance.GetSelectedUnits();
        int count = units.Count;
        float half = (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = startPoint + right * (i - half) * spacing;
            markers[i].transform.position = pos + Vector3.up * 0.05f;
            markers[i].SetActive(true);
        }
    }

    // =========================
    // ✅ CONFIRMATION
    // =========================
    void ConfirmFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();

        for (int i = 0; i < units.Count; i++)
        {
            NavMeshAgent agent = units[i].GetComponent<NavMeshAgent>();
            if (agent)
                agent.SetDestination(markers[i].transform.position);
        }

        ClearMarkers();
    }

    // =========================
    // 🧱 MARKERS
    // =========================
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

    // =========================
    // 🎯 SOL
    // =========================
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