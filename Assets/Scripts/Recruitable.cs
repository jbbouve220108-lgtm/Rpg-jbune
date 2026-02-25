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

    // =====================================================
    // 👉 GESTION DU CLIC DIRECT SUR LE PERSONNAGE
    // ⚠️ DÉSACTIVÉ VOLONTAIREMENT
    // Le clic est désormais centralisé via RecruitableClickDetector
    // =====================================================
    void OnMouseDown()
    {
        // Intentionnellement vide
        // (évite le double déclenchement et le crash du singleton RecruitUI)
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

    // 👉 Appelé UNIQUEMENT par le bouton "Recruter" (INCHANGÉ)
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
        // 🔥 INVALIDATION DE LA SÉLECTION (INCHANGÉ)
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

    // =====================================================
    // 🔒 APPELÉ AVANT L’OUVERTURE UI (par RecruitableClickDetector)
    // =====================================================
    public void FreezePhysicsForUI()
    {
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}