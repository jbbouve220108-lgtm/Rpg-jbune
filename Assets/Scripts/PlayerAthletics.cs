using UnityEngine;
using UnityEngine.AI;

public class PlayerAthletics : MonoBehaviour
{
    [Header("Athletics Settings")]
    public float xpPerSecondMoving = 1f;
    public float baseXpRequired = 10f;
    public float exponentialFactor = 1.15f; // 🔥 difficulté
    public int maxAthleticsLevel = 100;

    [Header("Speed Bonus")]
    public float speedBonusPerLevel = 0.05f;

    private NavMeshAgent agent;
    private CharacterStats stats;
    private HybridMovement movement;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
        movement = GetComponent<HybridMovement>();
    }

    void Update()
    {
        if (stats == null || stats.athletisme == null)
            return;

        if (!IsMoving())
            return;

        GainAthleticsXp(Time.deltaTime * xpPerSecondMoving);
    }

    // =====================================================
    // DÉTECTION DE MOUVEMENT RÉEL
    // =====================================================
    bool IsMoving()
    {
        if (movement != null)
            return movement.IsMoving();

        if (agent != null)
            return agent.velocity.magnitude > 0.1f;

        return false;
    }

    // =====================================================
    // XP / LEVEL
    // =====================================================
    void GainAthleticsXp(float amount)
    {
        Stat ath = stats.athletisme;

        if (ath.value >= maxAthleticsLevel)
            return;

        ath.progress += amount;

        float xpNeeded = GetXpRequiredForNextLevel(ath.value);

        if (ath.progress >= xpNeeded)
        {
            ath.progress -= xpNeeded;
            ath.value++;

            OnAthleticsLevelUp(ath.value);
        }
    }

    float GetXpRequiredForNextLevel(int currentLevel)
    {
        return baseXpRequired * Mathf.Pow(exponentialFactor, currentLevel);
    }

    void OnAthleticsLevelUp(int newLevel)
    {
        ApplySpeedBonus(newLevel);
    }

    // =====================================================
    // BONUS DE VITESSE
    // =====================================================
    void ApplySpeedBonus(int level)
    {
        float multiplier = 1f + level * speedBonusPerLevel;

        if (agent != null)
            agent.speed = 4f * multiplier;
    }
}