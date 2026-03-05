using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;

    public List<Unit> partyMembers = new List<Unit>();

    public Unit SelectedUnit;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AddPlayer();
    }

    void AddPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj == null)
            return;

        Unit playerUnit = playerObj.GetComponent<Unit>();

        if (playerUnit == null)
            return;

        partyMembers.Add(playerUnit);

        SelectUnit(playerUnit);

        PartyUI.Instance.Refresh();

        // 🔥 AJOUT : afficher le preview au démarrage
        if (CharacterUI.Instance != null)
        {
            CharacterUI.Instance.Refresh(playerUnit);
        }
    }

    public void AddCompanion(Unit unit)
    {
        if (unit == null)
            return;

        if (!partyMembers.Contains(unit))
        {
            partyMembers.Add(unit);

            PartyUI.Instance.Refresh();
        }
    }

    public void SelectUnit(Unit unit)
    {
        if (unit == null)
            return;

        SelectedUnit = unit;

        Debug.Log("Selected : " + unit.unitName);
    }
}