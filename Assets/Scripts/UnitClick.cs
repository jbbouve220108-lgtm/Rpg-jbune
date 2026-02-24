using UnityEngine;

public class UnitClick : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 🔥 On récupère TOUS les colliders sous la souris
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);

        if (hits.Length == 0)
            return;

        // 🔥 On trie par distance (le plus proche d'abord)
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // 🔥 On cherche le PREMIER Recruitable valide
        foreach (RaycastHit hit in hits)
        {
            Recruitable recruit = hit.collider.GetComponent<Recruitable>();

            if (recruit != null)
            {
                recruit.OnClicked();
                return;
            }
        }
    }
}