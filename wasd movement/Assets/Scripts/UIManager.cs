using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    Stack<GameObject> uiStack = new Stack<GameObject>();
    public GameObject startingUI;
    //public GameObject gameScreen;
    
    delegate void UITriggers();
    Dictionary<string, UITriggers> triggerForUI = new Dictionary<string, UITriggers>();
    
    PlayerInput playerInput;
    InputAction escapeAction;
    InputAction anykeyAction;
    InputAction enterAction;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        uiStack.Push(startingUI);
        triggerForUI.Add("Start Screen", new UITriggers(StartScreen));
        triggerForUI.Add("Enter Username", new UITriggers(EnterUsername));
        //triggerForUI.Add("Lobby Screen", new UITriggers(LobbyScreen));
        triggerForUI.Add("Join a LAN Game", new UITriggers(JoinALANGame));
        //triggerForUI.Add("Host a LAN Game", new UITriggers(gameScreenActive));
        //triggerForUI.Add("Joining Game", new UITriggers(gameScreenActive));

        playerInput = Manager.Instance.playerInput;
        escapeAction = playerInput.actions["Escape"];
        anykeyAction = playerInput.actions["AnyKey"];
        enterAction = playerInput.actions["Enter"];
    }

    private void Update()
    {
        if (Manager.Instance.gamestate != Manager.GameState.Lobby)
            return;

        if(triggerForUI.ContainsKey(uiStack.Peek().name))
            triggerForUI[uiStack.Peek().name]();
        if (escapeAction.WasPerformedThisFrame() && uiStack.Count > 1)
            revertBranch();
    }

    public void continueBranch(GameObject nextUI)
    {
        if (uiStack.Peek().transform.Find("UI") != null)
            uiStack.Peek().transform.Find("UI").gameObject.SetActive(false);
        foreach (Transform eachChild in uiStack.Peek().transform)
        {
            if (eachChild == nextUI.transform)
            {
                eachChild.gameObject.SetActive(true);
                uiStack.Push(eachChild.gameObject);
                return;
            }    
        }
        Debug.LogWarning("UIManager: UI not available");
    }

    public void continueBranch(string nextUI)
    {
        if (uiStack.Peek().transform.Find("UI") != null)
            uiStack.Peek().transform.Find("UI").gameObject.SetActive(false);

        Transform childO = uiStack.Peek().transform.Find(nextUI);
        if (childO != null)
        {
            childO.gameObject.SetActive(true);
            uiStack.Push(childO.gameObject);
            return;
        }
        Debug.LogWarning("UIManager: UI not available: " + nextUI);
    }

    public void revertBranch()
    {
        uiStack.Peek().SetActive(false);
        uiStack.Pop();
        if (uiStack.Peek().transform.Find("UI") != null)
            uiStack.Peek().transform.Find("UI").gameObject.SetActive(true);
    }


    void StartScreen()
    { 
        if (anykeyAction.IsPressed() && !escapeAction.IsPressed())
            continueBranch("Enter Username");
    }

    void EnterUsername()
    {
        transform.GetComponentInChildren<TMP_InputField>().ActivateInputField();
        transform.GetComponentInChildren<TMP_InputField>().Select();
        if (enterAction.IsPressed())
        {
            //settings.setUsername(transform.GetComponentInChildren<TMP_InputField>().text);
            continueBranch("Lobby Screen");
        }
    }

    //void LobbyScreen()
    //{
    //    if (gameScreen.activeSelf)
    //    {
    //        gameScreen.SetActive(false);
    //        Application.Quit();
    //        //GetComponent<ConnectionManager>().disconnect();
    //    }
    //}

    void JoinALANGame()
    {
        //if (gameScreen.activeSelf)
        //{ 
        //    gameScreen.SetActive(false);
        //    Application.Quit();
        //    //GetComponent<ConnectionManager>().disconnect();
        //}
        transform.GetComponentInChildren<TMP_InputField>().ActivateInputField();
        transform.GetComponentInChildren<TMP_InputField>().Select();
        if (enterAction.IsPressed())
        {
            connectAsClientBtn();
        }
    }

    public void connectAsClientBtn()
    {
        Manager.Instance.connectAsClient(transform.GetComponentInChildren<TMP_InputField>().text);
    }

    //void gameScreenActive()
    //{
    //    if (!gameScreen.activeSelf)
    //    {
    //        gameScreen.SetActive(true);
    //        if (uiStack.Peek().name == "Host a LAN Game")
    //        {
    //            GetComponent<ConnectionManager>().connectAsHost();
    //            Manager.Instance.startGame();
    //        }
    //    }
    //}

    public void gameStarting()
    {
        while (uiStack.Peek().name != "Lobby Screen")
            revertBranch();
        startingUI.SetActive(false);
    }
}
