using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

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

    private int currentIndex = 0;
    private Companion currentCompanion;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        RefreshUI();
    }

    // =====================================================
    // OUVERTURE / FERMETURE
    // =====================================================
    public void TogglePanel()
    {
        if (panel == null)
            return;

        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);

        if (!isActive)
            RefreshUI();
    }

    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // =====================================================
    // NAVIGATION ENTRE COMPAGNONS (CIRCULAIRE)
    // =====================================================
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

    // =====================================================
    // FOLLOW (INDIVIDUEL)
    // =====================================================
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

    // =====================================================
    // RAFRAÎCHISSEMENT UI
    // =====================================================
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

        // 🔹 Nom
        if (nameText != null)
            nameText.text = currentCompanion.companionName;

        // 🔹 Toggle Follow
        if (followToggle != null)
        {
            followToggle.onValueChanged.RemoveAllListeners();

            followToggle.interactable = true;
            followToggle.SetIsOnWithoutNotify(currentCompanion.isFollowing);

            followToggle.onValueChanged.AddListener(OnFollowToggleChanged);
        }

        RefreshState();
    }

    // =====================================================
    // ÉTAT / STATS (PRIORITÉS RESPECTÉES)
    // =====================================================
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

        if (nameText != null)
            nameText.text = "Aucun compagnon";

        if (stateText != null)
            stateText.text = "";

        if (followToggle != null)
        {
            followToggle.onValueChanged.RemoveAllListeners();
            followToggle.isOn = false;
            followToggle.interactable = false;
        }
    }

    // =====================================================
    // UTILS
    // =====================================================
    bool HasCompanions()
    {
        return CompanionManager.Instance != null &&
               CompanionManager.Instance.companions != null &&
               CompanionManager.Instance.companions.Count > 0;
    }
}