using UnityEngine;

public class Recruitable : MonoBehaviour
{
    [Header("Recruitment")]
    public int recruitCost = 50;

    private bool recruited = false;

    // 🔒 Références physiques (EXISTANT)
    private Rigidbody rb;
    private bool wasKinematic;

    void Awake()
    {
        // 🔹 On récupère le Rigidbody existant (s’il existe)
        rb = GetComponent<Rigidbody>();
    }

    // 👉 GESTION DU CLIC DIRECT SUR LE PERSONNAGE
    void OnMouseDown()
    {
        // 🔒 Si une UI est déjà ouverte → on ignore
        if (UIState.IsModalOpen)
            return;

        // 🔒 Si déjà recruté → on ignore
        if (recruited)
            return;

        // =====================================================
        // 🔴 BLOQUAGE SI TROP LOIN (EXISTANT)
        // =====================================================
        Companion companion = GetComponent<Companion>();
        if (companion != null && !companion.IsPlayerInInteractionRange())
        {
            if (InteractionFeedback.Instance != null)
            {
                InteractionFeedback.Instance.ShowTooFar();
            }
            return; // ⛔ L’UI NE S’OUVRE PAS
        }
        // =====================================================

        // 🔒 GEL PHYSIQUE TEMPORAIRE (EXISTANT)
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 🟢 Ouverture de l’UI de recrutement (INCHANGÉ)
        RecruitUI.Instance.Open(this);
    }

    // 👉 Vérification des conditions de recrutement (INCHANGÉ)
    public bool CanRecruit()
    {
        if (recruited)
            return false;

        if (PlayerResources.Instance == null)
            return false;

        return PlayerResources.Instance.gold >= recruitCost;
    }

    // 👉 Appelé UNIQUEMENT par le bouton "Recruter"
    public void Recruit()
    {
        if (!CanRecruit())
            return;

        PlayerResources.Instance.gold -= recruitCost;
        recruited = true;

        // 🔹 Récupération de l'Unit (INCHANGÉ)
        Unit unit = GetComponent<Unit>();
        if (unit != null)
        {
            RenameUI.Instance.Open(unit);
        }

        // 🔹 Récupération du Companion (INCHANGÉ)
        Companion companion = GetComponent<Companion>();
        if (companion != null && unit != null)
        {
            companion.Recruit(unit.unitName);
        }

        // =====================================================
        // 🔥 AJOUT DEMANDÉ : INVALIDATION DE LA SÉLECTION
        // =====================================================
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.DeselectAll();
        }
        // =====================================================

        // 🔹 Désactivation du composant une fois recruté (INCHANGÉ)
        this.enabled = false;
    }

    // 🔓 APPELÉ LORS DE LA FERMETURE DE L’UI (INCHANGÉ)
    public void RestorePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
        }
    }
}