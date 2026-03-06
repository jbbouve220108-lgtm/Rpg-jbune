using UnityEngine;

public class CharacterPreviewSystem : MonoBehaviour
{
    [Header("Preview")]
    public Transform spawnPoint;

    private GameObject previewCharacter;

    public void ShowCharacter(Unit unit)
    {
        if (unit == null || spawnPoint == null)
            return;

        // supprimer ancien preview
        if (previewCharacter != null)
        {
            Destroy(previewCharacter);
        }

        // cloner le modèle du joueur
        previewCharacter = Instantiate(unit.gameObject, spawnPoint.position, spawnPoint.rotation);

        // mettre le clone dans layer Preview
        SetLayerRecursively(previewCharacter, LayerMask.NameToLayer("Preview"));

        // désactiver scripts gameplay
        DisableGameplay(previewCharacter);
    }

    public void HideCharacter()
    {
        if (previewCharacter != null)
        {
            Destroy(previewCharacter);
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void DisableGameplay(GameObject obj)
    {
        MonoBehaviour[] scripts = obj.GetComponentsInChildren<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
    }
}