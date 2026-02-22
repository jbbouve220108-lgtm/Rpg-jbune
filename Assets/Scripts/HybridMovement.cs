using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;
    public GameObject moveMarker;

    private NavMeshAgent agent;
    private Camera mainCamera;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;

        if (moveMarker)
            moveMarker.SetActive(false);
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleClickMovement();
        HandleArrival();
    }

    // ⌨️ ZQSD prioritaire
    void HandleKeyboardMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            if (agent.hasPath)
                agent.ResetPath();

            if (moveMarker)
                moveMarker.SetActive(false);

            Vector3 move = new Vector3(h, 0, v).normalized;
            agent.Move(move * keyboardSpeed * Time.deltaTime);

            if (move != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
        }
    }

    // 🖱️ Clic souris
    void HandleClickMovement()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);

                if (moveMarker)
                {
                    moveMarker.transform.position = hit.point + Vector3.up * 0.02f;
                    moveMarker.SetActive(true);
                }
            }
        }
    }

    // 🎯 Arrivée à destination
    void HandleArrival()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (agent.hasPath)
                agent.ResetPath();

            if (moveMarker)
                moveMarker.SetActive(false);
        }
    }
}