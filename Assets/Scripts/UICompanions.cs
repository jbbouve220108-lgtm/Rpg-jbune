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
    // NAVIGATION ENTRE COMPAGNONS
    // =====================================================

    public void NextCompanion()
    {
        if (!HasCompanions())
            return;

        currentIndex++;
        ClampIndex();
        RefreshUI();
    }

    public void PreviousCompanion()
    {
        if (!HasCompanions())
            return;

        currentIndex--;
        ClampIndex();
        RefreshUI();
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

        ClampIndex();

        currentCompanion = CompanionManager.Instance.companions[currentIndex];

        if (currentCompanion == null)
        {
            ShowEmptyState();
            return;
        }

        // 🔹 Nom
        if (nameText != null)
            nameText.text = currentCompanion.companionName;

        // 🔹 Toggle Follow (IMPORTANT : interactable remis à true)
        if (followToggle != null)
        {
            followToggle.onValueChanged.RemoveAllListeners();

            followToggle.interactable = true; // ✅ CORRECTION DU BUG
            followToggle.isOn = currentCompanion.isFollowing;

            followToggle.onValueChanged.AddListener(OnFollowToggleChanged);
        }

        RefreshState();
    }

    void RefreshState()
    {
        if (stateText == null || currentCompanion == null)
            return;

        // Following explicite
        if (currentCompanion.isFollowing)
        {
            stateText.text = "Following";
            return;
        }

        // Ordre actif / formation
        NavMeshAgent agent = currentCompanion.GetComponent<NavMeshAgent>();
        if (agent != null && agent.hasPath)
        {
            stateText.text = "In Formation";
            return;
        }

        stateText.text = "Idle";
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

    void ClampIndex()
    {
        if (currentIndex < 0)
            currentIndex = 0;

        if (currentIndex >= CompanionManager.Instance.companions.Count)
            currentIndex = CompanionManager.Instance.companions.Count - 1;
    }
}