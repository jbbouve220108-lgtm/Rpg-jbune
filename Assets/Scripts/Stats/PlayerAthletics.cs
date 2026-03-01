using UnityEngine;
using UnityEngine.AI;

public class PlayerAthletics : MonoBehaviour
{
    [Header("Gain Settings")]
    public float xpPerSecondMoving = 1f;

    private NavMeshAgent agent;
    private HybridMovement movement;
    private AthleticsProgression progression;

    // =====================================================
    // UNITY
    // =====================================================
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<HybridMovement>();
        progression = GetComponent<AthleticsProgression>();
    }

    void Update()
    {
        if (progression == null)
            return;

        if (!IsMoving())
            return;

        progression.AddXp(Time.deltaTime * xpPerSecondMoving);
    }

    // =====================================================
    // DÉTECTION DE MOUVEMENT RÉEL
    // =====================================================
    bool IsMoving()
    {
        // Priorité au déplacement clavier
        if (movement != null && movement.IsMoving())
            return true;

        // Fallback NavMesh
        if (agent != null)
            return agent.velocity.magnitude > 0.1f;

        return false;
    }
}