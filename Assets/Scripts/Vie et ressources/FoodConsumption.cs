using UnityEngine;
using System.Collections;

public class FoodConsumption : MonoBehaviour
{
    public float tickTime = 10f;
    public float starvationDamage = 2f;

    void Start()
    {
        StartCoroutine(FoodTick());
    }

    IEnumerator FoodTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);

            int unitCount = SelectionManager.Instance.GetSelectedUnits().Count;

            if (unitCount == 0)
                continue;

            bool hasFood = PlayerResources.Instance.SpendFood(unitCount);

            if (!hasFood)
            {
                // Famine → dégâts
                foreach (var unit in SelectionManager.Instance.GetSelectedUnits())
                {
                    Health h = unit.GetComponent<Health>();
                    if (h)
                        h.TakeDamage(starvationDamage);
                }
            }
        }
    }
}