using UnityEngine;

public class Recruitable : MonoBehaviour
{
    [Header("Recruitment")]
    public int recruitCost = 50;

    private bool recruited = false;

    // 🔒 Références physiques (AJOUT, pas remplacement)
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

        // 🔒 GEL PHYSIQUE TEMPORAIRE (CAUSE DU BUG)
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

    // 👉 Appelé UNIQUEMENT par le bouton "Recruter" (INCHANGÉ)
    public void Recruit()
    {
        if (!CanRecruit())
            return;

        PlayerResources.Instance.gold -= recruitCost;
        recruited = true;

        // 🔹 Récupération de l'Unit
        Unit unit = GetComponent<Unit>();
        if (unit != null)
        {
            RenameUI.Instance.Open(unit);
        }

        // 🔹 Récupération du Companion
        Companion companion = GetComponent<Companion>();
        if (companion != null && unit != null)
        {
            companion.Recruit(unit.unitName);
        }

        // 🔹 Désactivation du composant une fois recruté
        this.enabled = false;
    }

    // 🔓 APPELÉ LORS DE LA FERMETURE DE L’UI
    public void RestorePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
        }
    }
}