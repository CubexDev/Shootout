using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;
    public bool _isDead => Playermanager.ownerPlayer.isDead.Value;

    PlayerInput playerInput;
    InputAction escapeAction;
    InputAction spaceAction;

    public GameObject gameScreen;
    public GameObject gameUI;
    public GameObject pauseUI;
    public GameObject deathUI;

    #region pausedGame
    public TMP_Text lobbyCode;
    public Button copyIP;

    public TMP_Text killedByField;
    #endregion


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
        Hitmarker.SetActive(false);
    }

    private void Update()
    {
        checkEscape();
    }

    void checkEscape()
    {
        if (Manager.Instance.gamestate == Manager.GameState.Game)
        {
            if (escapeAction.WasPerformedThisFrame())
            {
                Manager.Instance.stopGame();
                pauseUIOn();
            }
        }
        else if (Manager.Instance.gamestate == Manager.GameState.PausedGame)
        {
            if (escapeAction.WasPerformedThisFrame() && !_isDead)
            {
                Manager.Instance.continueGame();
                gameUIOn();
            } else if(_isDead)
            {
                if (spaceAction.WasPerformedThisFrame())
                {
                    Manager.Instance.continueGame();
                    gameUIOn();
                    Playermanager.ownerPlayer.respawn();
                }
            }
        }
    }

    void gameUIOn()
    {
        gameUI.SetActive(true);
        pauseUI.SetActive(false);
        deathUI.SetActive(false);
        escapeAction = playerInput.actions["Escape"];
    }

    void pauseUIOn()
    {
        gameUI.SetActive(true);
        pauseUI.SetActive(true);
        deathUI.SetActive(false);
        escapeAction = playerInput.actions["Escape"];
    }

    void deathUIOn()
    {
        gameUI.SetActive(true);
        pauseUI.SetActive(false);
        deathUI.SetActive(true);
        escapeAction = playerInput.actions["Escape"];
        spaceAction = playerInput.actions["Space"];
    }

    public void activate()
    {
        gameScreen.SetActive(true);
        gameUIOn();
        if(Manager.Instance.isHost)
        {
            copyIP.gameObject.SetActive(true);
            lobbyCode.gameObject.SetActive(true);
            lobbyCode.text = "Your Lobbycode is: " + Manager.Instance.ownIP;
        }
    }

    public void deactivate()
    {
        gameScreen.SetActive(false);
    }

    #region inGame

    public TMP_Text killsText;
    public TMP_Text DeathsText;
    public GameObject Hitmarker;
    public Slider fireCoolDown;

    public void shotPlayer(Playermanager ownerPlayer, string affectedPlayer)
    {
        killsText.text = "Kills: " + ownerPlayer.kills.Value.ToString();
        StartCoroutine(animateHitmarker());
    }
    IEnumerator animateHitmarker()
    {
        Hitmarker.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Hitmarker.SetActive(false);
    }

    public void gotShot(Playermanager ownerPlayer, string shooterPlayer)
    {
        DeathsText.text = "Deaths: " + ownerPlayer.deaths.Value.ToString();
        killedByField.text = shooterPlayer;
        deathUIOn();
    }

    public void coolDown(float value)
    {
        if (value >= 1)
            fireCoolDown.gameObject.SetActive(false);
        else if (Manager.Instance.gamestate != Manager.GameState.Lobby)
        {
            if (fireCoolDown.gameObject.activeSelf)
                fireCoolDown.value = value;
            else {
                fireCoolDown.gameObject.SetActive(true);
                fireCoolDown.value = value;
            }
        }
    }
    #endregion
}
