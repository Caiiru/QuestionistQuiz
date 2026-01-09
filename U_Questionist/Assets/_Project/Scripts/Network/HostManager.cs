using System;
using UnityEngine;
using Unity.Netcode;

public class HostManager : MonoBehaviour
{
    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
    }

    public void StartHost()
    {
        _networkManager.StartServer();

    }

    public void StopHost()
    {
        _networkManager.Shutdown();
    }
}
