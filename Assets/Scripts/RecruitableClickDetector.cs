using UnityEngine;

public class RecruitableClickDetector : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask recruitableLayer;
    public float maxDistance = 10f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
            Debug.LogError("[RecruitableClickDetector] Camera.main introuvable");
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        // 🔒 Sécurité : une UI modale bloque toute interaction monde
        if (UIState.IsModalOpen)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, maxDistance, recruitableLayer))
            return;

        Recruitable recruitable = hit.collider.GetComponent<Recruitable>();
        if (recruitable == null)
            return;

        // 🔒 Vérifie l'existence de l'UI
        if (RecruitUI.Instance == null)
        {
            Debug.LogError("[RecruitableClickDetector] RecruitUI.Instance introuvable");
            return;
        }

        // 🔒 Vérification distance via Companion (LOGIQUE EXISTANTE)
        Companion companion = recruitable.GetComponent<Companion>();
        if (companion != null && !companion.IsPlayerInInteractionRange())
        {
            if (InteractionFeedback.Instance != null)
            {
                InteractionFeedback.Instance.ShowTooFar();
            }
            return;
        }

        // 🔒 IMPORTANT : invalider toute sélection AVANT ouverture UI
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.DeselectAll();
        }

        // 🔒 Gel physique AVANT ouverture UI
        recruitable.FreezePhysicsForUI();

        // 🟢 Ouverture UI avec la BONNE instance
        RecruitUI.Instance.Open(recruitable);
    }
}