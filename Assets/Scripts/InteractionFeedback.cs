using UnityEngine;
using TMPro;

public class InteractionFeedback : MonoBehaviour
{
    public static InteractionFeedback Instance;

    public TextMeshProUGUI messageText;
    public float displayDuration = 1.5f;

    private float timer;

    void Awake()
    {
        Instance = this;
        messageText.gameObject.SetActive(false);
    }

    public void ShowTooFar()
    {
        messageText.text = "Trop loin pour interagir";
        messageText.color = Color.red;
        messageText.gameObject.SetActive(true);
        timer = displayDuration;
    }

    void Update()
    {
        if (!messageText.gameObject.activeSelf)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}