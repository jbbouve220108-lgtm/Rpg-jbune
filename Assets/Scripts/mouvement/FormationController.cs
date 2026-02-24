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
        // 🔒 Blocage global si une UI est ouverte (existant)
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

        for (int i = 0; i < units.Count; i++)
        {
            Vector3 pos = startPoint + right * (i - half) * spacing;
            markers[i].transform.position = pos + Vector3.up * 0.05f;
            markers[i].SetActive(true);
        }
    }

    void ConfirmFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();

        for (int i = 0; i < units.Count; i++)
        {
            // =====================================================
            // 🔹 AJOUT DEMANDÉ : COUPURE DU FOLLOW
            // =====================================================
            Companion companion = units[i].GetComponent<Companion>();
            if (companion != null)
            {
                companion.OnFormationOrder(); // 🔥 clé
            }
            // =====================================================

            NavMeshAgent agent = units[i].GetComponent<NavMeshAgent>();
            if (agent)
                agent.SetDestination(markers[i].transform.position);
        }

        ClearMarkers();
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