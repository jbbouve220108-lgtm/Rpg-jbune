using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlotUI : MonoBehaviour
{
    public Button button;

    // 🔥 texte assigné dans l'inspector
    public TMP_Text nameText;

    private Unit unit;

    void Awake()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }
    }

    public void Setup(Unit newUnit)
    {
        unit = newUnit;

        string displayName = "";

        if (unit != null)
        {
            if (unit.unitType == UnitType.Player)
                displayName = "Vous";
            else
                displayName = unit.unitName;
        }

        Debug.Log("PartySlotUI text = " + displayName);

        if (nameText != null)
        {
            nameText.text = displayName;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (unit != null)
        {
            Debug.Log("Selected unit : " + unit.unitName);

            if (PartyManager.Instance != null)
            {
                PartyManager.Instance.SelectUnit(unit);
            }

            if (CharacterUI.Instance != null)
            {
                CharacterUI.Instance.Refresh(unit);
            }
        }
    }
}