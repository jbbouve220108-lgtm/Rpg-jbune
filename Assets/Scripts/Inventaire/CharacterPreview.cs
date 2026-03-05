using UnityEngine;
using UnityEngine.AI;

public class CharacterPreviewSystem : MonoBehaviour
{
    public Transform spawnPoint;

    GameObject currentCharacter;

    public void ShowCharacter(Unit unit)
    {
        Debug.Log("Preview du personnage : " + unit);

        if (unit == null)
            return;

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint non assigné !");
            return;
        }

        if (currentCharacter != null)
            Destroy(currentCharacter);

        currentCharacter = Instantiate(unit.gameObject, spawnPoint);

        currentCharacter.transform.localPosition = Vector3.zero;
        currentCharacter.transform.localRotation = Quaternion.identity;

        // mettre le personnage dans le layer preview
        SetLayerRecursively(currentCharacter, LayerMask.NameToLayer("CharacterPreview"));

        // désactiver gameplay
        NavMeshAgent agent = currentCharacter.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        HybridMovement move = currentCharacter.GetComponent<HybridMovement>();
        if (move != null)
            move.enabled = false;

        CombatController combat = currentCharacter.GetComponent<CombatController>();
        if (combat != null)
            combat.enabled = false;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}