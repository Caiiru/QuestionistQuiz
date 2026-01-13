using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Console = DeveloperConsole.Console;

public class LobbyManager : MonoBehaviour
{
    //Lobbies

    private Lobby _lobby;
    private Lobby _hostLobby;

    //Timers
    private float _heartbeatTimer;
    private float _lobbyUpdateTimer;

    //Player Connection Info
    public string playerName;
    public string joinCode;

    //Screens
    public Transform loadingScreenTransform;
    public Transform lobbyVisualTransform;

    public Transform joinScreenTransform;

    //Scripts
    private LobbyVisual _lobbyVisual;
    private Log logger;

    private async void Start()
    {
        //Logger
        logger = GetComponent<Log>();
        logger.prefix = "LobbyManager";


        playerName = "Miguelito " + UnityEngine.Random.Range(10, 99);
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            logger.PrintLog("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        AddCommands();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (_hostLobby != null)
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0)
            {
                float heartbeatTimerMax = 15;
                _heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId: _hostLobby.HostId);
            }
        }
    }

    private async void HandleLobbyPollUpdates()
    {
        if (_lobby != null)
        {
            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer <= 0)
            {
                float _lobbyUpdateTimerMax = 2;
                _lobbyUpdateTimer = _lobbyUpdateTimerMax;
                logger.PrintLog("Updating Lobby: " + _lobby.Name);

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId: _lobby.Id);
                _lobby = lobby;
                _lobbyVisual.UpdateLobby(_lobby);
            }
        }
    }

    private void Update()
    {
        // HandleLobbyHeartbeat();
        HandleLobbyPollUpdates();
    }


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

            _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
            _lobby = _hostLobby;
            // Debug.Log("Created Lobby");
            _lobbyVisual = lobbyVisualTransform.GetComponent<LobbyVisual>();
            // _lobbyVisual.PopulateLobbyHost(_hostLobby, playerName);
            _lobbyVisual.JoinLobby(_hostLobby);

            Console.PrintSuccess("Created Lobby successfully: " + _hostLobby.Name + " to " + maxPlayers + " players");
            Console.PrintSuccess("Lobby Code: " + _hostLobby.LobbyCode);

            Console.AddCommand("PrintLobby", PrintPlayersCommand);
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
            _lobbyVisual = lobbyVisualTransform.GetComponent<LobbyVisual>();
            _lobbyVisual.JoinLobby(joinedLobby);
            _lobby = joinedLobby;
            
            lobbyVisualTransform.gameObject.SetActive(true);
            loadingScreenTransform.gameObject.SetActive(false);


            Console.AddCommand("PrintLobby", PrintPlayersCommand);
            logger.PrintSuccess("Joined Lobby successfully: " + joinCode);
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

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            logger.PrintError(e.Message);
        }
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