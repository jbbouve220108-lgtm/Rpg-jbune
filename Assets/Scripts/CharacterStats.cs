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
        InitStat(ref force);
        InitStat(ref athletisme);
        InitStat(ref resistance);
        InitStat(ref precision);

        InitStat(ref commandement);
        InitStat(ref charisme);
        InitStat(ref chance);

        InitStat(ref commerce);
        InitStat(ref artisanat);
        InitStat(ref bucheron);
        InitStat(ref mineur);
    }

    void InitStat(ref Stat stat)
    {
        if (stat == null)
            stat = new Stat();
    }
}