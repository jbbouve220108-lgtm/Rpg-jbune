using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class FoodConsumption : MonoBehaviour
{
    [Header("Food Tick")]
    public float tickInterval = 60f;

    [Tooltip("Base food consumed per companion")]
    public float baseFoodPerCompanion = 1f;

    [Tooltip("Additional food consumed if companion is moving")]
    public float movementFoodBonus = 0.2f;

    [Tooltip("Minimal speed to consider a companion as moving")]
    public float movementThreshold = 0.1f;

    [Header("Starvation")]
    public float lifeLostWhenStarving = 2f;

    // 🔒 Mémoire des compagnons déjà affamés
    private HashSet<Companion> hungryCompanions = new HashSet<Companion>();

    void Start()
    {
        StartCoroutine(FoodRoutine());
    }

    IEnumerator FoodRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);

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
                availableFood -= 1; // 1 nourriture réservée au joueur

            // ============================
            // CALCUL DES BESOINS COMPAGNONS
            // ============================
            Dictionary<Companion, int> companionNeeds = new Dictionary<Companion, int>();

            foreach (Companion companion in companions)
            {
                if (companion == null)
                    continue;

                float need = baseFoodPerCompanion;

                NavMeshAgent agent = companion.GetComponent<NavMeshAgent>();
                if (agent != null && agent.velocity.magnitude > movementThreshold)
                    need += movementFoodBonus;

                companionNeeds[companion] = Mathf.CeilToInt(need);
            }

            // ============================
            // DISTRIBUTION ALÉATOIRE
            // ============================
            List<Companion> shuffled = new List<Companion>(companions);
            for (int i = 0; i < shuffled.Count; i++)
            {
                Companion temp = shuffled[i];
                int randomIndex = Random.Range(i, shuffled.Count);
                shuffled[i] = shuffled[randomIndex];
                shuffled[randomIndex] = temp;
            }

            HashSet<Companion> fedCompanions = new HashSet<Companion>();

            foreach (Companion companion in shuffled)
            {
                if (!companionNeeds.ContainsKey(companion))
                    continue;

                int need = companionNeeds[companion];
                if (availableFood >= need)
                {
                    availableFood -= need;
                    fedCompanions.Add(companion);
                }
            }

            // ============================
            // APPLICATION DES ÉTATS
            // ============================
            foreach (Companion companion in companions)
            {
                if (companion == null)
                    continue;

                // ☠️ Priorité absolue
                if (companion.CurrentState == CompanionState.Dying)
                    continue;

                if (fedCompanions.Contains(companion))
                {
                    hungryCompanions.Remove(companion);
                    continue; // retour Idle / Following via Companion
                }

                // ❌ PAS MANGÉ
                if (!hungryCompanions.Contains(companion))
                {
                    hungryCompanions.Add(companion);
                    companion.SetState(CompanionState.Hungry);
                }
                else
                {
                    companion.SetState(CompanionState.Starving);

                    Health h = companion.GetComponent<Health>();
                    if (h != null)
                        h.TakeDamage(lifeLostWhenStarving);
                }
            }

            // ============================
            // MISE À JOUR NOURRITURE JOUEUR
            // ============================
            PlayerResources.Instance.food = Mathf.Max(0, availableFood);
        }
    }
}