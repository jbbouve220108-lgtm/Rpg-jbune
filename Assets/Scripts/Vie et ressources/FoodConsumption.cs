using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodConsumption : MonoBehaviour
{
    [Header("Food Tick")]
    [Tooltip("Time (seconds) between each food loss")]
    public float tickInterval = 60f;   // ⏱️ ex: 60 secondes

    [Tooltip("Food lost each tick (GLOBAL)")]
    public int foodLostPerTick = 1;    // 🍖 ex: -1 nourriture

    [Header("Starvation")]
    [Tooltip("Life lost per unit when food is at 0")]
    public float lifeLostWhenStarving = 2f;

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

            // 🍖 CAS NORMAL : il reste de la nourriture
            if (PlayerResources.Instance.food > 0)
            {
                PlayerResources.Instance.SpendFood(foodLostPerTick);
                continue;
            }

            // 💔 CAS FAMINE : nourriture = 0
            if (SelectionManager.Instance == null)
                continue;

            List<SelectableUnit> units = SelectionManager.Instance.GetSelectedUnits();

            foreach (SelectableUnit unit in units)
            {
                Health h = unit.GetComponent<Health>();
                if (h)
                    h.TakeDamage(lifeLostWhenStarving);
            }
        }
    }
}