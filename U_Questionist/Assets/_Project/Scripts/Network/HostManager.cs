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
        Debug.Log($"[HostManager.StartHost Line 17]");
    }

    public void StopHost()
    {
        _networkManager.Shutdown();
    }
}
