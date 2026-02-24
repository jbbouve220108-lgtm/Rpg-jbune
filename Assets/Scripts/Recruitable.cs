using UnityEngine;

public class Recruitable : MonoBehaviour
{
    [Header("Recruitment")]
    public int recruitCost = 50;

    private bool recruited = false;

    // 👉 Appelé quand on clique sur le PNJ
    public void OnClicked()
    {
        if (recruited)
            return;

        // 🟢 Ouvre UNIQUEMENT l'UI de recrutement
        RecruitUI.Instance.Open(this);
    }

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

        // 🔹 Récupération de l'Unit
        Unit unit = GetComponent<Unit>();
        if (unit != null)
        {
            // 🔹 Ouverture du renommage (le nom peut changer APRÈS)
            RenameUI.Instance.Open(unit);
        }

        // 🔹 Récupération du Companion
        Companion companion = GetComponent<Companion>();
        if (companion != null && unit != null)
        {
            // 🔹 Initialisation (le nom sera synchronisé ensuite)
            companion.Recruit(unit.unitName);
        }

        // 🔹 Désactivation du composant une fois recruté
        this.enabled = false;
    }
}