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

        initialized = true;
    }

    void RandomizeStat(Stat stat)
    {
        if (stat == null)
            return;

        // 🔥 MODIF : Max random fixé à 20
        stat.value = Random.Range(1, 21); 
        stat.progress = 0f;
    }
}