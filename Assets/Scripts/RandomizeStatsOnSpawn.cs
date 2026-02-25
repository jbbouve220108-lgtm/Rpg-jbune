using UnityEngine;

[DisallowMultipleComponent]
public class RandomizeStatsOnSpawn : MonoBehaviour
{
    [Header("Random Stat Range")]
    [Tooltip("Minimum starting value for stats")]
    public int minValue = 0;

    [Tooltip("Maximum starting value for stats")]
    public int maxValue = 30;

    private bool initialized = false;

    void Awake()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        if (initialized)
            return;

        CharacterStats stats = GetComponent<CharacterStats>();
        if (stats == null)
            return;

        // 🔒 Génération UNE SEULE FOIS
        InitializeStat(stats.force);
        InitializeStat(stats.athletisme);
        InitializeStat(stats.resistance);
        InitializeStat(stats.precision);

        InitializeStat(stats.commandement);
        InitializeStat(stats.charisme);
        InitializeStat(stats.chance);

        InitializeStat(stats.commerce);
        InitializeStat(stats.artisanat);
        InitializeStat(stats.bucheron);
        InitializeStat(stats.mineur);

        initialized = true;
    }

    void InitializeStat(Stat stat)
    {
        if (stat == null)
            return;

        stat.value = Random.Range(minValue, maxValue + 1);
        stat.progress = 0f;
    }
}