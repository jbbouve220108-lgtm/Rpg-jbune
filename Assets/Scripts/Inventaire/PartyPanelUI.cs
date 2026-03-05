using UnityEngine;

public class PartyUI : MonoBehaviour
{
    public static PartyUI Instance;

    public Transform partyPanel;
    public GameObject partySlotPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void Refresh()
    {
        foreach (Transform child in partyPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Unit unit in PartyManager.Instance.partyMembers)
        {
            GameObject slot = Instantiate(partySlotPrefab, partyPanel);

            PartySlotUI slotUI = slot.GetComponent<PartySlotUI>();

            if (slotUI != null)
                slotUI.Setup(unit);
        }
    }
}