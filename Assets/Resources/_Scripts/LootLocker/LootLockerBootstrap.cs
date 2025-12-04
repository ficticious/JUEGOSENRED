using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLockerBootstrap : MonoBehaviour
{
    public static bool SessionStarted { get; private set; }

    string playerIdentifier = System.Guid.NewGuid().ToString();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartGuest();
    }

    void StartGuest()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.LogError($"FALLO al iniciar sesión → code: {response.statusCode}, error: {response.errorData?.message}");
                return;
            }

            SessionStarted = true;
            //Debug.Log("Conectado correctamente a LootLocker");
        });
    }
}
