using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public static CompanionManager Instance;

    public List<Companion> companions = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(Companion companion)
    {
        if (companion == null)
            return;

        if (!companions.Contains(companion))
        {
            companions.Add(companion);
        }
    }
}