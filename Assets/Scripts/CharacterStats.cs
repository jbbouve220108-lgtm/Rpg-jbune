using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("State")]
    public bool initialized = false;

    [Header("Combat")]
    public Stat force = new Stat();
    public Stat athletisme = new Stat();
    public Stat resistance = new Stat();
    public Stat precision = new Stat();

    [Header("Social")]
    public Stat charisme = new Stat();
    public Stat commandement = new Stat();
    public Stat chance = new Stat();

    [Header("Metiers")]
    public Stat mineur = new Stat();
    public Stat bucheron = new Stat();
    public Stat artisanat = new Stat();
    public Stat commerce = new Stat();

    private void Awake()
    {
        EnsureInitialized();
    }

    public void EnsureInitialized()
    {
        if (initialized)
            return;

        // 🔹 Réinitialisation propre
        force = new Stat();
        athletisme = new Stat();
        resistance = new Stat();
        precision = new Stat();

        charisme = new Stat();
        commandement = new Stat();
        chance = new Stat();

        mineur = new Stat();
        bucheron = new Stat();
        artisanat = new Stat();
        commerce = new Stat();

        // 🔍 Détection joueur
        Unit unit = GetComponent<Unit>();
        bool isPlayer = (unit != null && unit.unitType == UnitType.Player);

        if (isPlayer)
        {
            // 🧍 JOUEUR → TOUT À 0
            SetAllStatsToZero();
        }
        else
        {
            // 👥 COMPAGNONS / PNJ → RANDOM
            RandomizeStat(force);
            RandomizeStat(athletisme);
            RandomizeStat(resistance);
            RandomizeStat(precision);

            RandomizeStat(charisme);
            RandomizeStat(commandement);
            RandomizeStat(chance);

            RandomizeStat(mineur);
            RandomizeStat(bucheron);
            RandomizeStat(artisanat);
            RandomizeStat(commerce);
        }

        initialized = true;
    }

    // =====================================================
    // UTILS
    // =====================================================
    void RandomizeStat(Stat stat)
    {
        if (stat == null)
            return;

        stat.value = Random.Range(1, 21); // comme avant
        stat.progress = 0f;
    }

    void SetAllStatsToZero()
    {
        SetZero(force);
        SetZero(athletisme);
        SetZero(resistance);
        SetZero(precision);

        SetZero(charisme);
        SetZero(commandement);
        SetZero(chance);

        SetZero(mineur);
        SetZero(bucheron);
        SetZero(artisanat);
        SetZero(commerce);
    }

    void SetZero(Stat stat)
    {
        if (stat == null)
            return;

        stat.value = 0;
        stat.progress = 0f;
    }
}