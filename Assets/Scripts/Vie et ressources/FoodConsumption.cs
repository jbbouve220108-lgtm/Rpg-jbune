using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class FoodConsumption : MonoBehaviour
{
    [Header("Food Tick")]
    [Tooltip("Time (seconds) between each food tick")]
    public float tickInterval = 60f;   // ⏱️ ex: 60 secondes

    [Tooltip("Base food consumed per companion")]
    public float baseFoodPerCompanion = 1f;   // 🍖 1 nourriture par compagnon

    [Tooltip("Additional food consumed if companion is moving")]
    public float movementFoodBonus = 0.2f;    // 🏃 petit bonus si déplacement

    [Header("Starvation")]
    [Tooltip("Life lost per companion when food is at 0")]
    public float lifeLostWhenStarving = 2f;

    [Tooltip("Minimal speed to consider a companion as moving")]
    public float movementThreshold = 0.1f;

    void Start()
    {
        StartCoroutine(FoodRoutine());
    }

    IEnumerator FoodRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);

            if (PlayerResources.Instance == null)
                continue;

            if (CompanionManager.Instance == null ||
                CompanionManager.Instance.companions == null ||
                CompanionManager.Instance.companions.Count == 0)
                continue;

            List<Companion> companions = CompanionManager.Instance.companions;

            float totalFoodNeeded = 0f;

            // ============================
            // CALCUL DE CONSOMMATION
            // ============================
            foreach (Companion companion in companions)
            {
                if (companion == null)
                    continue;

                totalFoodNeeded += baseFoodPerCompanion;

                NavMeshAgent agent = companion.GetComponent<NavMeshAgent>();
                if (agent != null && agent.velocity.magnitude > movementThreshold)
                {
                    totalFoodNeeded += movementFoodBonus;
                }
            }

            int foodToConsume = Mathf.CeilToInt(totalFoodNeeded);

            // ============================
            // CAS NORMAL : NOURRITURE OK
            // ============================
            if (PlayerResources.Instance.food >= foodToConsume)
            {
                PlayerResources.Instance.SpendFood(foodToConsume);
                continue;
            }

            // ============================
            // CAS FAMINE : NOURRITURE = 0
            // ============================
            if (PlayerResources.Instance.food <= 0)
            {
                foreach (Companion companion in companions)
                {
                    if (companion == null)
                        continue;

                    Health h = companion.GetComponent<Health>();
                    if (h != null)
                        h.TakeDamage(lifeLostWhenStarving);
                }
            }
        }
    }
}