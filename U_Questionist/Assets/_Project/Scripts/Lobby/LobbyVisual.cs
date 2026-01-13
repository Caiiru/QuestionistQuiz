using System.Collections.Generic;
using DeveloperConsole;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyVisual : MonoBehaviour
{
    public GameObject playerEntry;
    public Transform playerEntryTransform;
    public TextMeshProUGUI playerNameText;
    [Header("Code")] public TextMeshProUGUI joinCodeText;

    public void PopulateLobby(Lobby lobby)
    {
        Console.Print($"Lobby-HostID :{lobby.HostId}");
        List<Player> players = lobby.Players;
        playerNameText.text = $"{players[0].Data["PlayerName"].Value}'s room";
        foreach (Player p in players)
        {
            Console.Print($"ID:{p.Id}");
            if (p.Id == lobby.HostId)
            {
                Console.Print("I'll not create the host");
                continue;
            }

            var entry = Instantiate(playerEntry, playerEntryTransform);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = p.Data["PlayerName"].Value;
        }
    }

    public void PopulateLobbyHost(string code, string playerName)
    {
        playerNameText.text = $"{playerName}'s room";
        joinCodeText.text = code;
    }
}