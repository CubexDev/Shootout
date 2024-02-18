using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public Button enterUsernameButton;

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
        triggerForUI.Add("Enter Username", new UITriggers(EnterUsername));
        triggerForUI.Add("Join a Game", new UITriggers(JoinAGame));
        //triggerForUI.Add("Join a GLOBAL Game", new UITriggers(JoinAGame));

        playerInput = Manager.Instance.playerInput;
        escapeAction = playerInput.actions["Escape"];
        anykeyAction = playerInput.actions["AnyKey"];
        enterAction = playerInput.actions["Enter"];
    }

    private void OnEnable(){
        Manager.gameleft += gameLeft;
        Manager.gameStarted += gameStarting;
    }
    private void OnDisable()
    {
        Manager.gameleft -= gameLeft;
        Manager.gameStarted -= gameStarting;
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

    #region treeMovement
    public void continueBranch_releasedEnter(GameObject nextUI)
    {
        StartCoroutine(I_continueBranch_releasedEnter(nextUI));
    }

    IEnumerator I_continueBranch_releasedEnter(GameObject nextUI)
    {
        yield return new WaitWhile(enterAction.IsPressed);
        continueBranch(nextUI);
    }

    public void continueBranch(GameObject nextUI)
    {
        Debug.Log(nextUI.name);
        if (uiStack.Peek().transform.Find("UI") != null)
            uiStack.Peek().transform.Find("UI").gameObject.SetActive(false);
        foreach (Transform eachChild in uiStack.Peek().transform)
        {
            if (eachChild == nextUI.transform)
            {
                eachChild.gameObject.SetActive(true);
                uiStack.Push(eachChild.gameObject);
                selectFirstButton(eachChild.transform);
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
            selectFirstButton(childO.transform);
            return;
        }
        Debug.LogWarning("UIManager: UI not available: " + nextUI);
    }

    void selectFirstButton(Transform panel)
    {
        Debug.Log("selectfunc" + panel.name);
        Transform uiChild = panel.Find("UI");
        if (uiChild != null)
        {
            Debug.Log("uifound" + panel.name);
            foreach (Transform eachChild in uiChild)
            {
                if (!eachChild.gameObject.activeSelf)
                    continue;
                Button btn = eachChild.GetComponent<Button>();
                if (btn != null)
                {
                    Debug.Log("btnfound" + btn.name);
                    btn.Select();
                    return;
                }
            }
        }
    }

    public void revertBranch()
    {
        uiStack.Peek().SetActive(false);
        uiStack.Pop();
        if (uiStack.Peek().transform.Find("UI") != null)
            uiStack.Peek().transform.Find("UI").gameObject.SetActive(true);
        selectFirstButton(uiStack.Peek().transform);
    }
    #endregion

    #region checkForEvents
    void EnterUsername()
    {
        string userName = transform.GetComponentInChildren<TMP_InputField>().text;
        bool validUsername = ValidateUserInput.isUsernameValid(userName) != "";

        transform.GetComponentInChildren<TMP_InputField>().ActivateInputField();
        transform.GetComponentInChildren<TMP_InputField>().Select();
        enterUsernameButton.interactable = validUsername;

        if (enterAction.IsPressed() && validUsername)
        {
            EnterUsernameBtn();
        }
    }

    public void EnterUsernameBtn()
    {
        string userName = transform.GetComponentInChildren<TMP_InputField>().text;
        string validUsername = ValidateUserInput.isUsernameValid(userName);
        if(validUsername != "")
        {
            //settings.setUsername(transform.GetComponentInChildren<TMP_InputField>().text);
            Manager.Instance.currentPlayerName = validUsername;
            continueBranch("Lobby Screen");
        }
    }

    void JoinAGame()
    {
        transform.GetComponentInChildren<TMP_InputField>().ActivateInputField();
        transform.GetComponentInChildren<TMP_InputField>().Select();
        if (enterAction.IsPressed())
        {
            Manager.Instance.connectAsClient();
        }
    }
    #endregion


    public string getLobbyCodeInput()
    {
        return transform.GetComponentInChildren<TMP_InputField>().text;
    }

    public void connectingScreen()
    {
        while (uiStack.Peek().name != "Lobby Screen")
            revertBranch();
        continueBranch("Connecting");
    }

    public void connectionFailed()
    {
        while(uiStack.Peek().name != "Lobby Screen")
            revertBranch();
        continueBranch("Couldnt connect");
    }

    public void gameStarting()
    {
        while (uiStack.Peek().name != "Lobby Screen")
            revertBranch();
        startingUI.SetActive(false);
    }

    public void gameLeft(bool isConnectionError)
    {
        startingUI.SetActive(true);
        while (uiStack.Peek().name != "Lobby Screen")
            revertBranch();
        if(isConnectionError)
            continueBranch("You Got Disconnected");
    }
}
