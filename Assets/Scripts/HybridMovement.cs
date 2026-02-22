using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;

    private NavMeshAgent agent;
    private Camera mainCamera;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleClickMovement();
    }

    // ⌨️ ZQSD / WASD (prioritaire)
    void HandleKeyboardMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            // Stop déplacement par clic
            if (agent.hasPath)
                agent.ResetPath();

            Vector3 move = new Vector3(h, 0, v).normalized;
            agent.Move(move * keyboardSpeed * Time.deltaTime);

            // Oriente le personnage vers la direction
            if (move != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );
            }
        }
    }

    // 🖱️ Clic souris
    void HandleClickMovement()
    {
        // Si le clavier est utilisé → on ignore le clic
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}