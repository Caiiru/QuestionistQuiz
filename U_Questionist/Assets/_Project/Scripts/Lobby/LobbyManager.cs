using System;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Console = DeveloperConsole.Console;

public class LobbyManager : MonoBehaviour
{
    private Lobby _lobby;
    private float _heartbeatTimer;
    public string playerName;
    public string joinCode;

    public Transform loadingScreenTransform;
    public Transform lobbyVisualTransform;
    public Transform joinScreenTransform;


    private async void Start()
    {
        playerName = "Miguelito " + UnityEngine.Random.Range(10, 99);
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Console.Print("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        AddCommands();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (_lobby != null)
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0)
            {
                float heartbeatTimerMax = 15;
                _heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId: _lobby.HostId);
            }
        }
    }

    // private void Update()
    // {
    //     HandleLobbyHeartbeat();
    // }


    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "New Lobby";
            int maxPlayers = 4;

            PlayerDataObject playerNameData =
                new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName);
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();

            playerData.Add("PlayerName", playerNameData);

            Player hostPlayer = new Player(id: AuthenticationService.Instance.PlayerId, data: playerData);


            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                Player = hostPlayer,
            };

            _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            // Debug.Log("Created Lobby");
            LobbyVisual _lobbyVisual = lobbyVisualTransform.GetComponent<LobbyVisual>();
            _lobbyVisual.PopulateLobbyHost(_lobby.LobbyCode, playerName);
            Console.PrintSuccess("Created Lobby successfully: " + _lobby.Name + " to " + maxPlayers + " players");
            Console.PrintSuccess("Lobby Code: " + _lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public void JoinLobbyByCodeWithName(string username)
    {
        playerName = username;
        JoinLobbyByCode();
    }

    public async void JoinLobbyByCode()
    {
        joinScreenTransform.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(joinCode))
        {
            loadingScreenTransform.gameObject.SetActive(false);
            joinScreenTransform.gameObject.SetActive(true);
            Console.PrintWarning("Code is null or empty");
            return;
        }

        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(),
            };

            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode, joinLobbyByCodeOptions);
            Console.PrintSuccess("Joined Lobby successfully: " + joinCode);
            LobbyVisual _lobbyVisual = lobbyVisualTransform.GetComponent<LobbyVisual>();
            _lobbyVisual.PopulateLobby(joinedLobby);
            
            lobbyVisualTransform.gameObject.SetActive(true);
            loadingScreenTransform.gameObject.SetActive(false);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Console.PrintWarning("Failed to join lobby: " + e.Message);
        }
    }

    #region Player

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
            }
        };
    }

    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    #endregion

    public void SetJoinCode(string newCode)
    {
        joinCode = newCode;
    }

    void PrintPlayers(Lobby lobby)
    {
        Console.Print("Players in Lobby: " + lobby.Name);
        foreach (var p in lobby.Players)
        {
            Console.Print(p.Id + " " + p.Data["PlayerName"].Value);
        }
    }

    #region Commands

    private void AddCommands()
    {
        Console.AddCommand("CreateLobby", CreateLobbyCommand);
        Console.AddCommand("JoinLobby", JoinLobbyByCodeCommand);
    }

    void CreateLobbyCommand(string[] args)
    {
        CreateLobby();

        Console.AddCommand("PrintLobby", PrintPlayersCommand);
    }

    void JoinLobbyByCodeCommand(string[] args)
    {
        // Console.Print(args[0]);
        joinCode = args[0];
        if (args.Length > 1)
        {
            JoinLobbyByCodeWithName(args[1]);
            Console.AddCommand("PrintLobby", PrintPlayersCommand);
        }
        else
        {
            JoinLobbyByCode();
            Console.AddCommand("PrintLobby", PrintPlayersCommand);
        }
    }

    void PrintPlayersCommand(string[] args)
    {
        if (_lobby != null)
            PrintPlayers(_lobby);
    }

    #endregion
}