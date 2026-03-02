using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;
    public float blockCheckDistance = 0.8f;

    private NavMeshAgent agent;
    private Animator animator;

    private bool isMoving = false;
    public bool IsMoving() => isMoving;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (UIState.IsModalOpen)
        {
            isMoving = false;
            UpdateAnimator();
            return;
        }

        CombatController combat = GetComponent<CombatController>();
        Unit unit = GetComponent<Unit>();

        // 🔒 Combat bloque UNIQUEMENT les IA
        if (combat != null &&
            unit != null &&
            unit.unitType != UnitType.Player &&
            combat.State == CombatController.CombatState.Attacking)
        {
            isMoving = false;
            UpdateAnimator();
            return;
        }

        bool keyboardMoving = HandleKeyboardMovement();
        bool navmeshMoving = HandleNavMeshMovement();

        isMoving = keyboardMoving || navmeshMoving;
        UpdateAnimator();
    }

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

    bool HandleNavMeshMovement()
    {
        if (!agent.hasPath)
            return false;

        return agent.velocity.magnitude > 0.1f;
    }

    void UpdateAnimator()
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", isMoving ? 1f : 0f);
    }

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