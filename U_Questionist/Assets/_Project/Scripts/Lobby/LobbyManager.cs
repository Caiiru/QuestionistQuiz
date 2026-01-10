using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using AtomicConsole;

public class LobbyManager : MonoBehaviour
{
    private Lobby _lobby;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        
        
        
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    [AtomicCommand(name:"CreateLobby")]
    private async void CreateLobby()
    {

        try
        {
            string lobbyName = "New Lobby";
            int maxPlayers = 4;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            Debug.Log("Created Lobby");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

}
