using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UICompanions : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    [Header("Texts")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stateText;

    [Header("Controls")]
    public Toggle followToggle;
    public Button prevButton;
    public Button nextButton;

    [Header("Stats UI")]
    public List<StatRowUI> statRows = new List<StatRowUI>();

    private int currentIndex = 0;
    private Companion currentCompanion;

    private bool isOpen = false;

    void Awake()
    {
        statRows.Clear();
        statRows.AddRange(GetComponentsInChildren<StatRowUI>(true));

        if (panel != null)
            panel.SetActive(false);
    }

    void Start()
    {
        RefreshUI();
    }

    public void TogglePanel()
    {
        if (panel == null)
            return;

        bool newState = !panel.activeSelf;
        panel.SetActive(newState);

        if (newState)
        {
            UIState.OpenModal();
            isOpen = true;
            RefreshUI();
        }
        else
        {
            UIState.CloseModal();
            isOpen = false;
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            isOpen = false;
            UIState.CloseModal();
        }
    }

    void Update()
    {
        if (!isOpen || currentCompanion == null)
            return;

        CharacterStats stats = currentCompanion.GetComponent<CharacterStats>();
        if (stats == null)
            return;

        foreach (var row in statRows)
        {
            if (row != null)
                row.SetStat(stats);
        }

        RefreshState();
    }

    public void NextCompanion()
    {
        if (!HasCompanions())
            return;

        currentIndex++;
        WrapIndex();
        RefreshUI();
    }

    public void PreviousCompanion()
    {
        if (!HasCompanions())
            return;

        currentIndex--;
        WrapIndex();
        RefreshUI();
    }

    void WrapIndex()
    {
        int count = CompanionManager.Instance.companions.Count;
        if (currentIndex < 0)
            currentIndex = count - 1;
        else if (currentIndex >= count)
            currentIndex = 0;
    }

    void OnFollowToggleChanged(bool follow)
    {
        if (currentCompanion == null)
            return;

        if (follow)
            currentCompanion.Follow();
        else
            currentCompanion.StopFollow();

        RefreshState();
    }

    void RefreshUI()
    {
        if (!HasCompanions())
        {
            ShowEmptyState();
            return;
        }

        int count = CompanionManager.Instance.companions.Count;
        if (currentIndex < 0 || currentIndex >= count)
            currentIndex = 0;

        currentCompanion = CompanionManager.Instance.companions[currentIndex];

        if (currentCompanion == null)
        {
            ShowEmptyState();
            return;
        }

        if (nameText != null)
            nameText.text = currentCompanion.companionName;

        if (followToggle != null)
        {
            followToggle.onValueChanged.RemoveAllListeners();
            followToggle.interactable = true;
            followToggle.SetIsOnWithoutNotify(currentCompanion.isFollowing);
            followToggle.onValueChanged.AddListener(OnFollowToggleChanged);
        }

        CharacterStats stats = currentCompanion.GetComponent<CharacterStats>();
        if (stats != null)
        {
            foreach (var row in statRows)
            {
                if (row != null)
                    row.SetStat(stats);
            }
        }

        RefreshState();
    }

    void RefreshState()
    {
        if (stateText == null || currentCompanion == null)
            return;

        switch (currentCompanion.CurrentState)
        {
            case CompanionState.Dying:
                stateText.text = "En train de mourir";
                break;
            case CompanionState.Starving:
                stateText.text = "Famine";
                break;
            case CompanionState.Hungry:
                stateText.text = "À faim";
                break;
            case CompanionState.Following:
                stateText.text = "Following";
                break;
            case CompanionState.Idle:
            default:
                stateText.text = "Idle";
                break;
        }
    }

    void ShowEmptyState()
    {
        currentCompanion = null;
        if (nameText != null) nameText.text = "Aucun compagnon";
        if (stateText != null) stateText.text = "";
        if (followToggle != null)
        {
            followToggle.onValueChanged.RemoveAllListeners();
            followToggle.isOn = false;
            followToggle.interactable = false;
        }
        foreach (var row in statRows)
        {
            if (row != null) row.SetStat(null);
        }
    }

    bool HasCompanions()
    {
        return CompanionManager.Instance != null &&
               CompanionManager.Instance.companions != null &&
               CompanionManager.Instance.companions.Count > 0;
    }
}