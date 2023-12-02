using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Windows;
using System.Reflection;
using UnityEngine.SceneManagement;

public class Manager : NetworkBehaviour
{
    public static Manager Instance;
    public enum GameState { Lobby, Game, PausedGame, Settings}
    public GameState gamestate { get; private set; }
    public string lobbyCode { get; private set; } = "";
    //public bool isHost { get; private set; } = false;


    public PlayerInput playerInput;
    public ConnectionManager connectionManager;

    public GameObject uiCam;

    public static event Action<int> buildMap;

    public static event Action gameStarted;
    public static event Action gamePaused;
    public static event Action gameContinued;
    ///<summary> boolean: isConnectionError </summary>
    public static event Action<bool> gameleft; 

    public string currentPlayerName = "";

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        //Debug.Log("PnlineLobbies: ");
        //OnlineLobbies.getLobbies();
    }

    public async void connectAsClient()
    {
        lobbyCode = UIManager.Instance.getLobbyCodeInput();

        UIManager.Instance.connectingScreen();
        bool connectionSuccessful = await RelayConnection.StartClientWithRelay(lobbyCode);
        if (connectionSuccessful)
        {
            //wait for map
            //buildMap?.Invoke(pMap);
            startGame();
        }
        else
        {
            //connection failed
            UIManager.Instance.connectionFailed();
        }
        //startGame();
    }

    public async void startHost(int pMap)
    {
        UIManager.Instance.connectingScreen();

        lobbyCode = await RelayConnection.StartHostWithRelay();

        if(lobbyCode == null)
        {
            //connection Failed
            UIManager.Instance.connectionFailed();
        }
        else
        {
            //connection successful
            buildMap?.Invoke(pMap);
            startGame();
        }
    }

    public void disconnect()
    {
        leaveGame();
        RelayConnection.StopConnection();
    }


    void startGame()
    {
        gamestate = GameState.Game;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        gameStarted?.Invoke();
        //UIGameManager.Instance.activate();
        uiCam.SetActive(false);
    }
    public void stopGame()
    {
        gamestate = GameState.PausedGame;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        gamePaused?.Invoke();
    }
    public void continueGame()
    {
        gamestate = GameState.Game;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        gameContinued?.Invoke();
    }

    void leaveGame()
    {
        gamestate = GameState.Lobby;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;

        gameleft?.Invoke(false);
        //UIGameManager.Instance.deactivate();
        //UIManager.Instance.gameLeft();
        uiCam.SetActive(true);
        //connectionManager.stopNetwork();
    }

    public void connectionLost()
    {
        gamestate = GameState.Lobby;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        gameleft?.Invoke(true);
        //UIGameManager.Instance.deactivate();
        //UIManager.Instance.connectionLost();
        uiCam.SetActive(true);
        //connectionManager.stopNetwork();
    }

    public void openSettings()
    {
        gamestate = GameState.Settings;
    }

    public void closeSettings()
    {
        if (uiCam.activeSelf)
            gamestate = GameState.Lobby;
        else
            gamestate = GameState.PausedGame;
    }

    public void copyLobbyCOde()
    {
        GUIUtility.systemCopyBuffer = lobbyCode;
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}