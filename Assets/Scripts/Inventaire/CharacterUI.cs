using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    public static CharacterUI Instance;

    public CharacterPreviewSystem preview;

    void Awake()
    {
        Instance = this;
    }

    public void Refresh(Unit unit)
    {
        Debug.Log("CharacterUI Refresh appelé avec : " + unit);

        if (unit == null)
            return;

        if (preview == null)
        {
            Debug.LogError("CharacterPreviewSystem non assigné dans CharacterUI !");
            return;
        }

        preview.ShowCharacter(unit);
    }
}