using UnityEngine;
using UnityEngine.AI;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
    if (UIBlocker.Instance != null && UIBlocker.Instance.IsBlocked())
        return;
    if (MerchantUI.Instance != null && MerchantUI.Instance.IsOpen())
        return;

        HandleKeyboardMovement();
    }

    void HandleKeyboardMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            if (agent.hasPath)
                agent.ResetPath();

            Vector3 move = new Vector3(h, 0, v).normalized;
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
}