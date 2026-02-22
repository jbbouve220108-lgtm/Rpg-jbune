using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HybridMovement : MonoBehaviour
{
    public float keyboardSpeed = 4f;
    public GameObject moveMarker;
    public float markerDuration = 1.5f;

    private NavMeshAgent agent;
    private Camera mainCamera;
    private Coroutine markerCoroutine;

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

            HideMarker();

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
                ShowMarker(hit.point);
            }
        }
    }

    // 🎯 Affiche le marker temporairement
    void ShowMarker(Vector3 position)
    {
        if (!moveMarker) return;

        moveMarker.transform.position = position + Vector3.up * 0.02f;
        moveMarker.SetActive(true);

        if (markerCoroutine != null)
            StopCoroutine(markerCoroutine);

        markerCoroutine = StartCoroutine(HideMarkerAfterDelay());
    }

    IEnumerator HideMarkerAfterDelay()
    {
        yield return new WaitForSeconds(markerDuration);
        HideMarker();
    }

    void HideMarker()
    {
        if (!moveMarker) return;

        moveMarker.SetActive(false);

        if (markerCoroutine != null)
        {
            StopCoroutine(markerCoroutine);
            markerCoroutine = null;
        }
    }
}