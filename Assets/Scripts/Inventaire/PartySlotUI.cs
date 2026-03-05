using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartySlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    private Unit unit;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(Unit u)
    {
        unit = u;

        if (nameText != null)
            nameText.text = u.unitName;
    }

    void OnClick()
    {
        PartyManager.Instance.SelectUnit(unit);

        // 🔥 AJOUT : rafraîchit le preview du personnage
        if (CharacterUI.Instance != null)
        {
            CharacterUI.Instance.Refresh(unit);
        }
    }
}