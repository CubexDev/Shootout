using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    /*public static UIManager Instance {
        get { }
        private set { if (Instance != null) { Debug.LogWarning("UIManager already exists"); Instance = value; } else Instance = value; }  }*/
    Stack<GameObject> uiStack = new Stack<GameObject>();
    public GameObject startingUI;
    public GameObject gameScreen;
    delegate void UITriggers();
    Dictionary<string, UITriggers> triggerForUI = new Dictionary<string, UITriggers>();

    private void Awake()
    {
        //Instance = this;
    }

    private void Start()
    {
        uiStack.Push(startingUI);
        triggerForUI.Add("Start Screen", new UITriggers(StartScreenToEnterUsername));
        triggerForUI.Add("Enter Username", new UITriggers(EnterUsernameToLobbyScreen));
        triggerForUI.Add("Lobby Screen", new UITriggers(LobbyScreen));
        triggerForUI.Add("Join a LAN Game", new UITriggers(JoinALANGameToJoiningGame));
        triggerForUI.Add("Host a LAN Game", new UITriggers(gameScreenActive));
        triggerForUI.Add("Joining Game", new UITriggers(gameScreenActive));
    }

    private void Update()
    {
        if(triggerForUI.ContainsKey(uiStack.Peek().name))
            triggerForUI[uiStack.Peek().name]();
        if (Input.GetKeyDown(KeyCode.Escape) && uiStack.Count > 1)
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

    void StartScreenToEnterUsername()
    { if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape)) continueBranch("Enter Username"); }

    void EnterUsernameToLobbyScreen()
    {
        transform.GetComponentInChildren<TMP_InputField>().ActivateInputField();
        transform.GetComponentInChildren<TMP_InputField>().Select();
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //settings.setUsername(transform.GetComponentInChildren<TMP_InputField>().text);
            continueBranch("Lobby Screen");
        }
    }

    void LobbyScreen()
    {
        if (gameScreen.activeSelf)
        {
            gameScreen.SetActive(false);
            Application.Quit();
            //GetComponent<ConnectionManager>().disconnect();
        }
    }

    void JoinALANGameToJoiningGame()
    {
        if (gameScreen.activeSelf)
        { 
            gameScreen.SetActive(false);
            Application.Quit();
            //GetComponent<ConnectionManager>().disconnect();
        }
        transform.GetComponentInChildren<TMP_InputField>().ActivateInputField();
        transform.GetComponentInChildren<TMP_InputField>().Select();
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //settings.setUsername(transform.GetComponentInChildren<TMP_InputField>().text);
            continueBranch("Joining Game");
            GetComponent<ConnectionManager>().connectToHost(transform.GetComponentInChildren<TMP_InputField>().text);
        }
    }

    void gameScreenActive()
    {
        if (!gameScreen.activeSelf)
        {
            gameScreen.SetActive(true);
            if(uiStack.Peek().name == "Host a LAN Game")
                GetComponent<ConnectionManager>().connectAsHost();
        }
    }
}
