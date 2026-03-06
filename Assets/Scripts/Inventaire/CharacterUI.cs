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
        if (unit == null)
            return;

        if (preview == null)
        {
            Debug.LogError("CharacterPreviewSystem non assigné !");
            return;
        }

        preview.ShowCharacter(unit);
    }
}