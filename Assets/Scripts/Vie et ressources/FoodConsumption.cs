using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class FoodConsumption : MonoBehaviour
{
    [Header("Food Tick")]
    [Tooltip("Time (seconds) between each food distribution tick")]
    public float foodTickInterval = 60f;

    [Tooltip("Base food consumed per companion")]
    public float baseFoodPerCompanion = 1f;

    [Tooltip("Additional food consumed if companion is moving")]
    public float movementFoodBonus = 0.2f;

    [Tooltip("Minimal speed to consider a companion as moving")]
    public float movementThreshold = 0.1f;

    [Header("Starvation Tick")]
    [Tooltip("Time (seconds) between each life loss when starving")]
    public float starvationTickInterval = 8f;

    [Tooltip("Life lost per starvation tick")]
    public float lifeLostWhenStarving = 2f;

    // 🔒 Mémoire des compagnons affamés (1er tick)
    private HashSet<Companion> hungryCompanions = new HashSet<Companion>();

    void Start()
    {
        StartCoroutine(FoodRoutine());
        StartCoroutine(StarvationRoutine());
    }

    // =====================================================
    // 🍖 FOOD TICK
    // =====================================================
    IEnumerator FoodRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(foodTickInterval);

            if (PlayerResources.Instance == null ||
                CompanionManager.Instance == null)
                continue;

            List<Companion> companions = CompanionManager.Instance.companions;
            if (companions == null || companions.Count == 0)
                continue;

            int availableFood = PlayerResources.Instance.food;

            // ============================
            // PRIORITÉ JOUEUR
            // ============================
            if (availableFood > 0)
                availableFood -= 1;

            // ============================
            // CALCUL DES BESOINS
            // ============================
            Dictionary<Companion, int> needs = new Dictionary<Companion, int>();

            foreach (Companion companion in companions)
            {
                if (companion == null)
                    continue;

                float need = baseFoodPerCompanion;

                NavMeshAgent agent = companion.GetComponent<NavMeshAgent>();
                if (agent != null && agent.velocity.magnitude > movementThreshold)
                    need += movementFoodBonus;

                needs[companion] = Mathf.CeilToInt(need);
            }

            // ============================
            // DISTRIBUTION ALÉATOIRE
            // ============================
            List<Companion> shuffled = new List<Companion>(companions);
            for (int i = 0; i < shuffled.Count; i++)
            {
                Companion tmp = shuffled[i];
                int r = Random.Range(i, shuffled.Count);
                shuffled[i] = shuffled[r];
                shuffled[r] = tmp;
            }

            HashSet<Companion> fed = new HashSet<Companion>();

            foreach (Companion companion in shuffled)
            {
                if (!needs.ContainsKey(companion))
                    continue;

                int need = needs[companion];
                if (availableFood >= need)
                {
                    availableFood -= need;
                    fed.Add(companion);
                }
            }

            // ============================
            // APPLICATION DES ÉTATS
            // ============================
            foreach (Companion companion in companions)
            {
                if (companion == null)
                    continue;

                if (companion.CurrentState == CompanionState.Dying)
                    continue;

                if (fed.Contains(companion))
                {
                    hungryCompanions.Remove(companion);

                    if (companion.isFollowing)
                        companion.SetState(CompanionState.Following);
                    else
                        companion.SetState(CompanionState.Idle);

                    continue;
                }

                if (!hungryCompanions.Contains(companion))
                {
                    hungryCompanions.Add(companion);
                    companion.SetState(CompanionState.Hungry);
                }
                else
                {
                    companion.SetState(CompanionState.Starving);
                }
            }

            PlayerResources.Instance.food = Mathf.Max(0, availableFood);
        }
    }

    // =====================================================
    // ☠️ STARVATION DAMAGE TICK
    // =====================================================
    IEnumerator StarvationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(starvationTickInterval);

            if (CompanionManager.Instance == null)
                continue;

            foreach (Companion companion in CompanionManager.Instance.companions)
            {
                if (companion == null)
                    continue;

                if (companion.CurrentState != CompanionState.Starving)
                    continue;

                Health h = companion.GetComponent<Health>();
                if (h != null)
                {
                    // 🔥 FAMINE = PAS D’ATTAQUANT
                    h.TakeDamage(lifeLostWhenStarving, null);
                }
            }
        }
    }
}