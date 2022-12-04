using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;

    PlayerInput playerInput;
    InputAction escapeAction;

    public GameObject gameScreen;
    public GameObject pauseScreen;
    public TMP_Text lobbyCode;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        playerInput = Manager.Instance.playerInput;
        escapeAction = playerInput.actions["Escape"];
    }

    private void Update()
    {
        checkEscape();
    }

    void checkEscape()
    {
        if (!escapeAction.WasPerformedThisFrame())
            return;
        if (Manager.Instance.gamestate == Manager.GameState.Game)
        {
            Manager.Instance.stopGame();
            escapeAction = playerInput.actions["Escape"];
            pauseScreen.SetActive(true);
        }
        else
        {
            Manager.Instance.continueGame();
            escapeAction = playerInput.actions["Escape"];
            pauseScreen.SetActive(false);
        }
    }

    public void activate()
    {
        gameScreen.SetActive(true);
        escapeAction = playerInput.actions["Escape"];
        if(Manager.Instance.isHost)
        {
            lobbyCode.gameObject.SetActive(true);
            lobbyCode.text = "Your Lobbycode is: " + Manager.Instance.ownIP;
        }
    }

    public void deactivate()
    {
        gameScreen.SetActive(false);
    }
}
