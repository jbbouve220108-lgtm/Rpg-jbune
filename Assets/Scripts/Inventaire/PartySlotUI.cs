using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlotUI : MonoBehaviour
{
    public Button button;

    private Unit unit;

    private TMP_Text tmpText;
    private Text uiText;

    void Awake()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }

        if (button != null)
        {
            tmpText = button.GetComponentInChildren<TMP_Text>();
            uiText = button.GetComponentInChildren<Text>();
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

        if (tmpText != null)
        {
            tmpText.text = displayName;
        }

        if (uiText != null)
        {
            uiText.text = displayName;
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