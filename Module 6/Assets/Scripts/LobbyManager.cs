using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{

    public LobbyPlayerPanel playerPanelPrefab;
    public GameObject playerScrollContent;
    public TMPro.TMP_Text txtPlayerNumber;
    public Button btnStart;
    public Player playerPrefab;

    private NetworkList<PlayerInfo> allPlayers = new NetworkList<PlayerInfo>();
    private List<LobbyPlayerPanel> playerPanels = new List<LobbyPlayerPanel>();


    public void Start() {
        if (IsHost) {
            AddPlayerToList(NetworkManager.LocalClientId);
            RefreshPlayerPanels();
        }
    }

    public override void OnNetworkSpawn() {
        

        if (IsHost) {
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
            //NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;
            
        }
        // Must be after Host Connects to signals
        base.OnNetworkSpawn();

        if (IsClient){
            allPlayers.OnListChanged += ClientOnAllPlayersChanged;
        }
        txtPlayerNumber.text = $"Player #{NetworkManager.LocalClientId}";
    }
    // -----------------
    // Private
    // -----------------
    private void AddPlayerToList(ulong clientId) {
        allPlayers.Add(new PlayerInfo(clientId, Color.red));
    }

    private void AddPlayerPanel(PlayerInfo info) {
        LobbyPlayerPanel newPanel = Instantiate(playerPanelPrefab);
        newPanel.transform.SetParent(playerScrollContent.transform, false);
        newPanel.SetName($"Player {info.clientId.ToString()}");
        newPanel.SetColor(info.color);
        playerPanels.Add(newPanel);

    }

    private void RefreshPlayerPanels() {
        foreach (LobbyPlayerPanel panel in playerPanels) {
            Destroy(panel.gameObject);
        }
        playerPanels.Clear();

        foreach (PlayerInfo pi in allPlayers) {
            AddPlayerPanel(pi);
        }
    }
    // ------------------
    // Events
    // ------------------
    private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent) {
        RefreshPlayerPanels();
    }

    private void HostOnClientConnected(ulong clientId) {
        AddPlayerToList(clientId);
        RefreshPlayerPanels();
    }
    
}
