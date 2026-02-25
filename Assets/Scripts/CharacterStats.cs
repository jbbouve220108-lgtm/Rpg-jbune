using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Stat
{
    [Range(0, 100)]
    public int value = 0;

    [Range(0f, 100f)]
    public float progress = 0f;
}

public class CharacterStats : MonoBehaviour
{
    [Header("Combat / Corps")]
    public Stat force = new Stat();
    public Stat athletisme = new Stat();
    public Stat resistance = new Stat();
    public Stat precision = new Stat();

    [Header("Social / Groupe")]
    public Stat commandement = new Stat();
    public Stat charisme = new Stat();
    public Stat chance = new Stat();

    [Header("Métiers")]
    public Stat commerce = new Stat();
    public Stat artisanat = new Stat();
    public Stat bucheron = new Stat();
    public Stat mineur = new Stat();

    // =====================================================
    // INITIALISATION
    // =====================================================
    void Awake()
    {
        ClampAll();
    }

    // =====================================================
    // API GÉNÉRIQUE
    // =====================================================
    public void AddProgress(Stat stat, float amount)
    {
        if (stat == null || amount <= 0f)
            return;

        if (stat.value >= 100)
            return;

        stat.progress += amount;

        while (stat.progress >= 100f)
        {
            stat.progress -= 100f;
            stat.value = Mathf.Clamp(stat.value + 1, 0, 100);

            if (stat.value >= 100)
            {
                stat.progress = 0f;
                break;
            }
        }
    }

    // =====================================================
    // UTILITAIRES
    // =====================================================
    public void ClampAll()
    {
        foreach (Stat stat in GetAllStats())
        {
            stat.value = Mathf.Clamp(stat.value, 0, 100);
            stat.progress = Mathf.Clamp(stat.progress, 0f, 100f);
        }
    }

    public List<Stat> GetAllStats()
    {
        return new List<Stat>
        {
            force,
            athletisme,
            resistance,
            precision,
            commandement,
            charisme,
            chance,
            commerce,
            artisanat,
            bucheron,
            mineur
        };
    }

    // =====================================================
    // DEBUG / DEV (OPTIONNEL)
    // =====================================================
#if UNITY_EDITOR
    [ContextMenu("Reset All Stats")]
    void ResetAll()
    {
        foreach (Stat stat in GetAllStats())
        {
            stat.value = 0;
            stat.progress = 0f;
        }
    }
#endif
}