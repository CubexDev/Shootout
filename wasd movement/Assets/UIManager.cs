using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /*public static UIManager Instance {
        get { }
        private set { if (Instance != null) { Debug.LogWarning("UIManager already exists"); Instance = value; } else Instance = value; }  }*/
    Stack<GameObject> uiStack = new Stack<GameObject>();
    public GameObject startingUI;
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
    }

    private void Update()
    {
        if(triggerForUI.ContainsKey(uiStack.Peek().name))
            triggerForUI[uiStack.Peek().name]();
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
                Debug.Log(eachChild.gameObject.name);
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
            Debug.Log(childO.gameObject.name);
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
        Debug.Log(uiStack.Peek().transform.Find("UI").gameObject.name);
    }

    void StartScreenToEnterUsername()
    { if (Input.anyKey && !Input.GetKey(KeyCode.Escape)) continueBranch("Enter Username"); }
}
