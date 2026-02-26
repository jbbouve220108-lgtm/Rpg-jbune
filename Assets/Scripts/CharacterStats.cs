using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Combat")]
    public Stat force;
    public Stat athletisme;
    public Stat resistance;
    public Stat precision;

    [Header("Social")]
    public Stat commandement;
    public Stat charisme;
    public Stat chance;

    [Header("Metiers")]
    public Stat commerce;
    public Stat artisanat;
    public Stat bucheron;
    public Stat mineur;

    void Awake()
    {
        // 🔒 OPTION B :
        // On FORCE la recréation des stats pour éviter toute valeur sérialisée du prefab
        InitStat(ref force, true);
        InitStat(ref athletisme, true);
        InitStat(ref resistance, true);
        InitStat(ref precision, true);

        InitStat(ref commandement, true);
        InitStat(ref charisme, true);
        InitStat(ref chance, true);

        InitStat(ref commerce, true);
        InitStat(ref artisanat, true);
        InitStat(ref bucheron, true);
        InitStat(ref mineur, true);
    }

    void InitStat(ref Stat stat, bool forceReset)
    {
        if (forceReset || stat == null)
            stat = new Stat();
    }
}