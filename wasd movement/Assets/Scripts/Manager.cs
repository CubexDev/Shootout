using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Windows;
using System.Reflection;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public enum GameState { Lobby, Game, PausedGame}
    public GameState gamestate { get; private set; }
    public string ownIP { get; private set; } = "";
    public bool isHost { get; private set; } = false;

    public PlayerInput playerInput;
    public ConnectionManager connectionManager;

    public GameObject uiCam;

    public Action OnLobbyoppened;
    public Action OnGamestarted;
    public Action OnGamepaused;

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

    public void connectAsClient(string shortIP)
    {
        connectionManager.connectToHost(shortIP);
        //startGame();
    }

    public void startHost()
    {
        isHost = true;
        ownIP = connectionManager.connectAsHost();
        UIManager.Instance.gameStarting();
        startGame();
    }

    public void globalconnectAsClient(string longIP)
    {
        connectionManager.GlobalconnectToHost(longIP);
       // startGame();
    }

    public void globalstartHost()
    {
        isHost = true;
        ownIP = connectionManager.GlobalconnectAsHost();
        UIManager.Instance.gameStarting();
        startGame();
    }


    public void startGame()
    { 
        gamestate = GameState.Game;
        UIManager.Instance.gameStarting();
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        UIGameManager.Instance.activate();
        uiCam.SetActive(false);
    }
    public void stopGame()
    {
        gamestate = GameState.PausedGame;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
    }
    public void continueGame()
    {
        gamestate = GameState.Game;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void leaveGame()
    {
        gamestate = GameState.Lobby;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        UIGameManager.Instance.deactivate();
        UIManager.Instance.gameLeft();
        uiCam.SetActive(true);
        connectionManager.stopNetwork();
    }

    public void connectionLost()
    {
        gamestate = GameState.Lobby;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        UIGameManager.Instance.deactivate();
        UIManager.Instance.connectionLost();
        uiCam.SetActive(true);
        connectionManager.stopNetwork();
    }

    public void copyIPAdress()
    {
        GUIUtility.systemCopyBuffer = ownIP;
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}