using UnityEngine;

public class Recruitable : MonoBehaviour
{
    [Header("Recruitment")]
    public int recruitCost = 50;

    private bool recruited = false;

    private Rigidbody rb;
    private bool wasKinematic;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // =====================================================
    // 👉 GESTION DU CLIC DIRECT SUR LE PERSONNAGE
    // =====================================================
    void OnMouseDown()
    {
        // volontairement vide
    }

    // =====================================================
    // 👉 CONDITIONS DE RECRUTEMENT
    // =====================================================
    public bool CanRecruit()
    {
        if (recruited)
            return false;

        if (PlayerResources.Instance == null)
            return false;

        return PlayerResources.Instance.gold >= recruitCost;
    }

    // =====================================================
    // 👉 RECRUTEMENT (UI)
    // =====================================================
    public void Recruit()
    {
        if (!CanRecruit())
            return;

        PlayerResources.Instance.gold -= recruitCost;
        recruited = true;

        Unit unit = GetComponent<Unit>();
        if (unit != null)
        {
            RenameUI.Instance.Open(unit);
        }

        Companion companion = GetComponent<Companion>();
        if (companion != null && unit != null)
        {
            companion.Recruit(unit.unitName);
        }

        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.DeselectAll();
        }

        this.enabled = false;
    }

    // =====================================================
    // 🔓 RESTORE PHYSICS
    // =====================================================
    public void RestorePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
        }
    }

    // =====================================================
    // 🔒 FREEZE PHYSICS FOR UI
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