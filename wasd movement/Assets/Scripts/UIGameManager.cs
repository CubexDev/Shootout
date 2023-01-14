using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;

    PlayerInput playerInput;
    InputAction escapeAction;

    public GameObject gameScreen;
    public GameObject pauseScreen;

    #region pausedGame
    public TMP_Text lobbyCode;
    public Button copyIP;
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
        escapeAction = playerInput.actions["Escape"];
        Hitmarker.SetActive(false);
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

    public void shotPlayer(Playermanager ownerPlayer)
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

    public void gotShot(Playermanager ownerPlayer)
    {
        DeathsText.text = "Deaths: " + ownerPlayer.deaths.Value.ToString();
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
