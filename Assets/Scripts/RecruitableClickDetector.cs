using UnityEngine;

public class RecruitableClickDetector : MonoBehaviour
{
    public LayerMask recruitableLayer;
    public float maxDistance = 1000f;

    void Update()
    {
        if (UIState.IsModalOpen)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 🔥 On cherche TOUS les hits sous la souris
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);

        if (hits.Length == 0)
            return;

        // 🔥 On trie par distance
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // 🔥 On cherche le PREMIER Recruitable, même s’il est derrière
        foreach (RaycastHit hit in hits)
        {
            // On filtre par layer pour éviter les faux positifs
            if ((recruitableLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
                continue;

            Recruitable recruit = hit.collider.GetComponentInParent<Recruitable>();
            if (recruit != null && recruit.enabled)
            {
                RecruitUI.Instance.Open(recruit);
                return; // ⛔ priorité absolue
            }
        }
    }
}