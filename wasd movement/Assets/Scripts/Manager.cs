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
    public string ownIP { get; private set; } = "";
    //public bool isHost { get; private set; } = false;

    public GameObject oldMapPrefab;
    public GameObject newMapPrefab;
    public GameObject oldMap;
    public GameObject newMap;

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
        oldMap = Instantiate(oldMapPrefab);
        oldMap.SetActive(true);
        newMap = Instantiate(newMapPrefab);
        newMap.SetActive(false);
    }

    public void connectAsClient(string shortIP)
    {
        ownIP = connectionManager.connectToHost(shortIP);
        //startGame();
    }

    public void startHost(int pMap)
    {
        //isHost = true;
        ownIP = connectionManager.connectAsHost(pMap);
        UIManager.Instance.gameStarting();
        startGame();
    }

    public void globalconnectAsClient(string longIP)
    {
        ownIP = connectionManager.GlobalconnectToHost(longIP);
       // startGame();
    }

    public void globalstartHost(int pMap)
    {
        //isHost = true;
        ownIP = connectionManager.GlobalconnectAsHost(pMap);
    }

    public void getMap()
    {
        if (connectionManager.chosenMap.Value == 1)
        {
            Debug.Log("getMap1");
            oldMap.SetActive(true);
            Destroy(newMap);
        }
        else
        {
            Debug.Log("getMap2");
            newMap.SetActive(true);
            Destroy(oldMap);
        }
    }

    void returnMap()
    {
        if(oldMap != null)
        {
            oldMap.SetActive(true);
            newMap = Instantiate(newMapPrefab);
            newMap.SetActive(false);
        }
        else
        {
            newMap.SetActive(false);
            oldMap = Instantiate(oldMapPrefab);
            oldMap.SetActive(true);
        }
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
        returnMap();
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

    public void copyIPAdress()
    {
        GUIUtility.systemCopyBuffer = ownIP;
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}