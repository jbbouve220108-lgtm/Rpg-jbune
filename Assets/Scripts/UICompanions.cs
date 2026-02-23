using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICompanions : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public Toggle followToggle;

    private Companion current;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // Appelé par le bouton HUD "Compagnons"
    public void TogglePanel()
    {
        if (panel == null)
            return;

        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);

        if (!isActive)
            LoadFirstCompanion();
    }

    void LoadFirstCompanion()
    {
        // 🔒 Sécurité : CompanionManager absent
        if (CompanionManager.Instance == null)
        {
            Debug.LogWarning("[UICompanions] CompanionManager manquant");
            ShowEmptyState("Aucun compagnon");
            return;
        }

        // 🔒 Aucun compagnon recruté
        if (CompanionManager.Instance.companions.Count == 0)
        {
            ShowEmptyState("Aucun compagnon");
            return;
        }

        current = CompanionManager.Instance.companions[0];

        // 🔒 Companion invalide
        if (current == null)
        {
            ShowEmptyState("Compagnon invalide");
            return;
        }

        // ✅ Companion valide
        nameText.text = current.companionName;
        followToggle.interactable = true;
        followToggle.isOn = current.isFollowing;

        followToggle.onValueChanged.RemoveAllListeners();
        followToggle.onValueChanged.AddListener(OnFollowChanged);
    }

    void ShowEmptyState(string message)
    {
        current = null;

        if (nameText != null)
            nameText.text = message;

        if (followToggle != null)
        {
            followToggle.isOn = false;
            followToggle.interactable = false;
            followToggle.onValueChanged.RemoveAllListeners();
        }
    }

    void OnFollowChanged(bool follow)
    {
        if (current == null)
            return;

        if (follow)
            current.Follow();
        else
            current.StopFollow();
    }
}