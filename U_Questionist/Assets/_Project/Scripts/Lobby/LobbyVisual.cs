using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Console = DeveloperConsole.Console;

public class LobbyVisual : MonoBehaviour
{
    public GameObject playerEntry;
    public Transform playerEntryTransform;
    [Header("Texts")] public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI joinCodeText;
    public TextMeshProUGUI playersCountText;

    //Lobby
    private Lobby _connectedLobby;
    private ILobbyEvents _lobbyEvents;

    //Log
    private Log logger;

    private void OnEnable()
    {
        if (TryGetComponent<Log>(out logger))
        {
            logger.prefix = "Lobby Visual";
        }
    }

    public void JoinLobby(Lobby lobby)
    {
        _connectedLobby = lobby;
        PopulateLobby();
        SetupLobbyCallbacks();


        joinCodeText.text = _connectedLobby.LobbyCode;
    }

    private async void SetupLobbyCallbacks()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += LobbyChanged;

        try
        {
            _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(_connectedLobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            switch (e.Reason)
            {
                case LobbyExceptionReason.AlreadySubscribedToLobby:
                    logger.PrintError(
                        $"Already subscribed to lobby[{_connectedLobby.Id}]. We did not need to try and subscribe again. Exception Message: {e.Message}");
                    break;
                case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy:
                    logger.PrintError(
                        $"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {e.Message}");
                    throw;
                case LobbyExceptionReason.LobbyEventServiceConnectionError:
                    logger.PrintError($"Failed to connect to lobby events. Exception Message: {e.Message}");
                    throw;
                default: throw;
            }
        }
    }

    public void UpdateLobby(Lobby lobby)
    {
        _connectedLobby = lobby;
        CleanupVisual();
        PopulateLobby();
    }

    private void PopulateLobby()
    {
        List<Player> players = _connectedLobby.Players;
        playerNameText.text = $"{players[0].Data["PlayerName"].Value}'s room";
        foreach (Player p in players)
        {
            if (p.Id == _connectedLobby.HostId)
            {
                continue;
            }

            var entry = Instantiate(playerEntry, playerEntryTransform);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = p.Data["PlayerName"].Value;
        }
    }

    private void CleanupVisual()
    {
        for (int i = 0; i < playerEntryTransform.childCount; i++)
        {
            Destroy(playerEntryTransform.GetChild(i).gameObject);
        }
    }

    private void LobbyChanged(ILobbyChanges changes)
    {
        PopulateLobby();
    }

    public void PopulateLobbyHost(Lobby lobby, string playerName)
    {
        _connectedLobby = lobby;
        SetupLobbyCallbacks();
        playerNameText.text = $"{playerName}'s room";
        joinCodeText.text = _connectedLobby.LobbyCode;
    }
}