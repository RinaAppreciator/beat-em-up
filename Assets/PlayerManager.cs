using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<Transform> players = new List<Transform>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPlayer(Transform playerTransform)
    {
        if (!players.Contains(playerTransform))
        {
            players.Add(playerTransform);
        }
    }

    public void UnregisterPlayer(Transform playerTransform)
    {
        if (players.Contains(playerTransform))
        {
            players.Remove(playerTransform);
        }
    }

    public List<Transform> GetPlayers()
    {
        return players;
    }
}

