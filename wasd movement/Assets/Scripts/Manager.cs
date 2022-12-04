using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public enum GameState { Lobby, Game, PausedGame}
    public GameState gamestate { get; private set; }

    public PlayerInput playerInput;
    public ConnectionManager connectionManager;

    public Action OnLobbyoppened;
    public Action OnGamestarted;
    public Action OnGamepaused;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
        connectionManager = GetComponent<ConnectionManager>();
    }

    public void connectAsClient(string shortIP)
    {
        connectionManager.connectToHost(shortIP);
        UIManager.Instance.gameStarting();
        startGame();
    }

    public void startHost()
    {
        connectionManager.connectAsHost();
        UIManager.Instance.gameStarting();
        startGame();
    }

    public void startGame()
    {
        gamestate = GameState.Game;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        UIGameManager.Instance.activate();
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
}
