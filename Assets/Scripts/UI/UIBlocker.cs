using UnityEngine;
using System.Collections.Generic;

public class UIBlocker : MonoBehaviour
{
    public static UIBlocker Instance { get; private set; }

    // Panels actuellement bloqués
    private HashSet<CanvasGroup> blockedPanels = new HashSet<CanvasGroup>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Bloque UN panel précis
    public void Block(CanvasGroup panel)
    {
        if (panel == null)
            return;

        if (blockedPanels.Add(panel))
        {
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
    }

    // Débloque UN panel précis
    public void Unblock(CanvasGroup panel)
    {
        if (panel == null)
            return;

        if (blockedPanels.Remove(panel))
        {
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }
    }
}