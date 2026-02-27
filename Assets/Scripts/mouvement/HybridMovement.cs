using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;
    public float blockCheckDistance = 0.8f; // distance de blocage devant le joueur

    private NavMeshAgent agent;

    // =====================================================
    // 🔥 ÉTAT DE MOUVEMENT (LECTURE EXTERNE)
    // =====================================================
    private bool isMoving = false;
    public bool IsMoving() => isMoving;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // =====================================================
        // 🔒 BLOCAGE TOTAL DU DÉPLACEMENT SI UI OUVERTE
        // =====================================================
        if (UIState.IsModalOpen)
        {
            isMoving = false;
            return;
        }
        // =====================================================

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;

        if (!isMoving)
            return;

        if (agent.hasPath)
            agent.ResetPath();

        Vector3 move = new Vector3(h, 0, v).normalized;

        // =====================================================
        // 🔒 BLOCAGE SI PNJ DEVANT (LOGIQUE EXISTANTE)
        // =====================================================
        if (IsBlockedByRecruitable(move))
        {
            isMoving = false;
            return;
        }
        // =====================================================

        agent.Move(move * keyboardSpeed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                Time.deltaTime * 10f
            );
        }
    }

    // =====================================================
    // 🔒 DÉTECTION SIMPLE D'UN PNJ DEVANT LE JOUEUR
    // =====================================================
    bool IsBlockedByRecruitable(Vector3 moveDir)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, moveDir);

        if (Physics.Raycast(ray, out RaycastHit hit, blockCheckDistance))
        {
            if (hit.collider.GetComponent<Recruitable>() != null)
                return true;
        }

        return false;
    }
}