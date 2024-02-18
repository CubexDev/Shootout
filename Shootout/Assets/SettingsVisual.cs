using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.EventSystems;

public class SettingsVisual : MonoBehaviour
{
    [SerializeField] GameObject tabsPanel;
    [SerializeField] GameObject viewPort;
    Button[] tabButtons;
    List<List<List<Selectable>>> options = new List<List<List<Selectable>>>();

    PlayerInput playerInput;
    InputAction navigateAction;
    InputAction submitAction;
    InputAction cancelAction;

    bool isViewPortActive = true, sliderActive = false;
    int selectedTabButton = 0;
    int selectedViewPort = 0, selectedItem = 0, selectedSubItem = 0; //viewport

    private void Awake()
    {
        getTabBtns();
        getOptionsBtns();
    }

    private void Start()
    {
        playerInput = Manager.Instance.playerInput;
        navigateAction = playerInput.actions["Navigate"];
        submitAction = playerInput.actions["Submit"];
        cancelAction = playerInput.actions["Cancel"];

        setViewPort(0);
    }

    private void OnEnable()
    {
        isViewPortActive = true;
        selectedTabButton = 0;
        selectedViewPort = 0;
        selectedItem = 0;
        selectedSubItem = 0;
        options[selectedViewPort][selectedItem][selectedSubItem].Select();
        // switch to right viewport
    }

    private void Update()
    {
        Vector2 input = navigateAction.ReadValue<Vector2>();
        checkSliderInterAction();
        if (navigateAction.triggered && input != Vector2.zero)
        {
            if (isViewPortActive) viewport_ButtonsMovement(input);
            else tab_ButtonsMovement(input);
        }
    }

    void checkSliderInterAction()
    {
        if (!isViewPortActive) return;

        if (!sliderActive)
        {
            if (submitAction.triggered)
            {
                sliderActive = true;
                selectSelectable();
            }
        }
        else if (cancelAction.triggered)
        {
            sliderActive = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void viewport_ButtonsMovement(Vector2 direction)
    {
        if (!isViewPortActive) return;

        if (direction == Vector2.up)
        {
            if (selectedItem <= 0) return;
            selectedItem--;
            selectedSubItem = 0;
            sliderActive = false;
        }
        else if (direction == Vector2.down)
        {
            if (selectedItem >= options[selectedViewPort].Count - 1) return;
            selectedItem++;
            selectedSubItem = 0;
            sliderActive = false;
        }
        else if (direction == Vector2.right && !sliderActive)
        {
            if (selectedSubItem >= options[selectedViewPort][selectedItem].Count - 1) return;
            selectedSubItem++;
        }
        else if (direction == Vector2.left && !sliderActive)
        {
            if (selectedSubItem <= 0)
            {
                selectedTabButton = selectedViewPort;
                selectedItem = 0;
                selectedSubItem = 0;
                isViewPortActive = false;
            }
            else
            {
                selectedSubItem--;
            }
        }

        selectSelectable();
    }

    void tab_ButtonsMovement(Vector2 direction)
    {
        if (isViewPortActive) return;

        if(direction == Vector2.up)
        {
            if (selectedTabButton <= 0) return;
            selectedTabButton--;
        }
        else if (direction == Vector2.down)
        {
            if (selectedTabButton >= tabButtons.Length - 1) return;
            selectedTabButton++;
        }
        else if (direction == Vector2.right)
        {
            selectedTabButton = selectedViewPort;
            selectedItem = 0;
            selectedSubItem = 0;
            isViewPortActive = true;
        }

        selectSelectable();
    }

    void selectSelectable()
    {
        Debug.Log(isViewPortActive + ", " + selectedTabButton + ", " + selectedViewPort + ", " + selectedItem + ", " + selectedSubItem + "/" + options[selectedViewPort][selectedItem].Count);
        if (isViewPortActive)
        {
            if (options[selectedViewPort][selectedItem][selectedSubItem] is Slider)
            {
                if (sliderActive)
                    options[selectedViewPort][selectedItem][selectedSubItem].Select();
            }
            else
                options[selectedViewPort][selectedItem][selectedSubItem].Select();
        }
        else
            tabButtons[selectedTabButton].Select();
    }

    public void setViewPort(int viewPortNr)
    {
        selectedViewPort = viewPortNr;
        options[selectedViewPort][selectedItem][selectedSubItem].Select();
        if(options[selectedViewPort][selectedItem][selectedSubItem] is Slider)
            sliderActive = true;
        Debug.Log(isViewPortActive + ", " + selectedTabButton + ", " + selectedViewPort + ", " + selectedItem + ", " + selectedSubItem + "/" + options[selectedViewPort][selectedItem].Count);
    }

    void getTabBtns()
    {
        List<Button> tabButtonsList = new List<Button>();
        foreach (Transform child in tabsPanel.transform)
        {
            Button b = child.GetComponent<Button>();
            if (child.gameObject.activeSelf && b != null) tabButtonsList.Add(b);
        }
        tabButtons = tabButtonsList.ToArray();
    }

    void getOptionsBtns()
    {
        int tabs = viewPort.transform.childCount;
        for (int tabIndex = 0; tabIndex < tabs; tabIndex++)
        {
            Transform tab = viewPort.transform.GetChild(tabIndex).transform;

            options.Add(new List<List<Selectable>>());

            int settings = tab.childCount;

            for (int settingIndex = 0; settingIndex < settings; settingIndex++)
            {
                Transform setting = tab.GetChild(settingIndex).transform;
                if (!setting.gameObject.activeSelf) continue;

                options[tabIndex].Add(new List<Selectable>());

                int subSettings = setting.childCount;

                for (int subSettingIndex = 0; subSettingIndex < subSettings; subSettingIndex++)
                {
                    Selectable subSetting = setting.transform.GetChild(subSettingIndex).GetComponent<Selectable>();
                    if (subSetting == null) continue;

                    options[tabIndex][settingIndex].Add(subSetting);
                }
            }
        }

    }
}
