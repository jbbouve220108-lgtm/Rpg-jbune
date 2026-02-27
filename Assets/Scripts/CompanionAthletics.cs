using UnityEngine;
using UnityEngine.AI;

public class CompanionAthletics : MonoBehaviour
{
    [Header("Gain Settings")]
    public float xpPerSecondMoving = 1f;

    private NavMeshAgent agent;
    private AthleticsProgression progression;

    // =====================================================
    // UNITY
    // =====================================================
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        progression = GetComponent<AthleticsProgression>();
    }

    void Update()
    {
        if (agent == null || progression == null)
            return;

        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        if (!IsMoving())
            return;

        progression.AddXp(Time.deltaTime * xpPerSecondMoving);
    }

    // =====================================================
    // DÉTECTION DE MOUVEMENT RÉEL (NAVMESH)
    // =====================================================
    bool IsMoving()
    {
        // Vitesse réelle du NavMeshAgent
        return agent.velocity.magnitude > 0.1f;
    }
}