using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Recruitable : MonoBehaviour
{
    [Header("Recruitment")]
    public int recruitCost = 50;

    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
        unit.unitType = UnitType.Recruitable;
    }

    public void OnClicked()
    {
        if (RecruitUI.Instance != null)
        {
            RecruitUI.Instance.Open(this);
        }
    }

    public void Recruit()
    {
        // Sécurité
        if (PlayerResources.Instance == null)
            return;

        if (!PlayerResources.Instance.SpendGold(recruitCost))
        {
            Debug.Log("Not enough gold to recruit");
            return;
        }

        // Changement d’état
        unit.unitType = UnitType.Companion;

        Debug.Log($"Recruited {unit.unitName} for {recruitCost} gold");

        // IMPORTANT : on enlève le statut recruttable
        Destroy(this);

        // Fermeture UI
        if (RecruitUI.Instance != null)
            RecruitUI.Instance.Close();
    }
}