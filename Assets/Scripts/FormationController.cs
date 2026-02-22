using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class FormationController : MonoBehaviour
{
    public GameObject formationMarkerPrefab;
    public float spacing = 2f;

    private List<GameObject> markers = new List<GameObject>();
    private Vector3 startPoint;
    private bool forming = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            StartFormation();

        if (Input.GetMouseButton(1) && forming)
            UpdateFormationPreview();

        if (Input.GetMouseButtonUp(1) && forming)
            ConfirmFormation();
    }

    void StartFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();
        if (units.Count <= 1) return;

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
        forming = false;
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