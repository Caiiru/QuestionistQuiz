using Unity.Netcode;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
    }

    public void StartClient()
    {
        _networkManager.StartClient();
        
    }

    public void EndClient()
    {
        _networkManager.Shutdown();
    }
}
