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

    // =====================================================
    // 🆕 FLÈCHE DE DIRECTION
    // =====================================================
    [Header("Direction Arrow")]
    public GameObject arrowPrefab;
    public float arrowForwardOffset = 2.5f;   // distance DEVANT la formation
    public float arrowGroundOffset = 0.02f;   // légère élévation sol

    private GameObject arrowInstance;

    private Vector2 rightStart;
    private float rightDownTime;
    private bool forming;

    private Vector3 startPoint;
    private List<GameObject> markers = new List<GameObject>();

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

        // 🆕 Création de la flèche
        if (arrowPrefab != null && arrowInstance == null)
        {
            arrowInstance = Instantiate(arrowPrefab);
            arrowInstance.SetActive(true);
        }
    }

    void UpdateFormationPreview()
    {
        if (!TryGetMouseGround(out Vector3 current))
            return;

        Vector3 dir = current - startPoint;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f)
            dir = Vector3.forward;

        dir.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, dir);

        var units = SelectionManager.Instance.GetSelectedUnits();
        float half = (units.Count - 1) / 2f;

        // ================= FORMATION =================
        for (int i = 0; i < units.Count; i++)
        {
            Vector3 pos = startPoint + right * (i - half) * spacing;
            markers[i].transform.position = pos + Vector3.up * 0.05f;
            markers[i].SetActive(true);
        }

        // ================= FLÈCHE (DEVANT LA FORMATION) =================
        if (arrowInstance != null)
        {
            Vector3 arrowPos = startPoint + dir * arrowForwardOffset;
            arrowPos.y += arrowGroundOffset;

            arrowInstance.transform.position = arrowPos;
            arrowInstance.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    void ConfirmFormation()
    {
        var units = SelectionManager.Instance.GetSelectedUnits();

        for (int i = 0; i < units.Count; i++)
        {
            // 🔒 Le joueur ignore la formation (LOGIQUE EXISTANTE)
            Unit unit = units[i].GetComponent<Unit>();
            if (unit != null && unit.unitType == UnitType.Player)
            {
                // joueur autorisé (clavier prioritaire)
            }

            // 🔒 Unité non recrutée ignorée (LOGIQUE EXISTANTE)
            Recruitable recruitable = units[i].GetComponent<Recruitable>();
            Companion companion = units[i].GetComponent<Companion>();

            if (recruitable != null && (companion == null || !companion.isRecruited))
                continue;

            // 🔹 Coupure du follow (LOGIQUE EXISTANTE)
            if (companion != null)
                companion.OnFormationOrder();

            NavMeshAgent agent = units[i].GetComponent<NavMeshAgent>();
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                continue;

            agent.stoppingDistance = 0f;
            agent.SetDestination(markers[i].transform.position);
        }

        ClearMarkers();

        // 🆕 Suppression propre de la flèche
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
            arrowInstance = null;
        }
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