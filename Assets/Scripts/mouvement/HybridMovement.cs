using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;
    public float blockCheckDistance = 0.8f; // distance de blocage devant le joueur

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            if (agent.hasPath)
                agent.ResetPath();

            Vector3 move = new Vector3(h, 0, v).normalized;

            // =====================================================
            // 🔒 AJOUT : BLOCAGE SI PNJ DEVANT
            // =====================================================
            if (IsBlockedByRecruitable(move))
                return;
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
            {
                return true; // on bloque le déplacement
            }
        }

        return false;
    }
}