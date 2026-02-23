using UnityEngine;

public class Companion : MonoBehaviour
{
    public string companionName;

    public bool isRecruited { get; private set; }
    public bool isFollowing { get; private set; }

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void Recruit(string newName)
    {
        isRecruited = true;
        companionName = newName;

        CompanionManager.Instance.Register(this);
    }

    public void Follow()
    {
        isFollowing = true;
    }

    public void StopFollow()
    {
        isFollowing = false;
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            // version simple (on améliorera plus tard)
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                3f * Time.deltaTime
            );
        }
    }
}