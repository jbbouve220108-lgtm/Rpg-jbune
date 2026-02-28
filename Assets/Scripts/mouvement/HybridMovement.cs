using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;
    public float blockCheckDistance = 0.8f;

    private NavMeshAgent agent;
    private Animator animator;

    // =====================================================
    // 🔥 ÉTAT DE MOUVEMENT (LECTURE EXTERNE)
    // =====================================================
    private bool isMoving = false;
    public bool IsMoving() => isMoving;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("[HybridMovement] Animator introuvable dans les enfants");
    }

    void Update()
    {
        // =====================================================
        // 🔒 BLOCAGE TOTAL SI UI OUVERTE
        // =====================================================
        if (UIState.IsModalOpen)
        {
            isMoving = false;
            UpdateAnimator();
            return;
        }
        // =====================================================

        bool keyboardMoving = HandleKeyboardMovement();
        bool navmeshMoving = HandleNavMeshMovement();

        isMoving = keyboardMoving || navmeshMoving;

        UpdateAnimator();
    }

    // =====================================================
    // ⌨️ DÉPLACEMENT CLAVIER
    // =====================================================
    bool HandleKeyboardMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
            return false;

        if (agent.hasPath)
            agent.ResetPath();

        Vector3 move = new Vector3(h, 0, v).normalized;

        if (IsBlockedByRecruitable(move))
            return false;

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

        return true;
    }

    // =====================================================
    // 🧭 DÉPLACEMENT NAVMESH (FORMATION / SÉLECTION)
    // =====================================================
    bool HandleNavMeshMovement()
    {
        if (!agent.hasPath)
            return false;

        // seuil très bas pour éviter les micro-oscillations
        return agent.velocity.magnitude > 0.1f;
    }

    // =====================================================
    // 🎞️ ANIMATION
    // =====================================================
    void UpdateAnimator()
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", isMoving ? 1f : 0f);
    }

    // =====================================================
    // 🔒 DÉTECTION PNJ DEVANT
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