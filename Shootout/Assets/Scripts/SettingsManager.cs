using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public GameObject settingsScreen;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Update()
    {
        if (changeUsernameField.isFocused) checkUsernameInput();
    }

    public void openSettings()
    {
        Manager.Instance.openSettings();
        settingsScreen.SetActive(true);
        updateControlls();
    }
    public void closeSettings()
    {
        Manager.Instance.closeSettings();
        settingsScreen.SetActive(false);
    }

    #region controllsSetttings
    public Slider c_SensiSlider;
    public static float c_Sensi = 3f;
    public void changeSensi()
    {
        c_Sensi = c_SensiSlider.value;
    }

    void updateControlls()
    {
        c_SensiSlider.value = c_Sensi;
    }
    #endregion

    #region tabs
    int currentTab;
    public Button[] tabButtons;
    public TMP_InputField changeUsernameField;
    public Button changeUsernameBtn;
    public TMP_Text changeUsernameBtnTxt;

    public void setTab(int btn)
    {
        if(btn != currentTab)
        {
            tabButtons[currentTab].interactable = true;
            tabButtons[btn].interactable = false;
            currentTab = btn;
        }
    }

    void checkUsernameInput()
    {
        string validatedUserName = ValidateUserInput.isUsernameValid(changeUsernameField.text);
        if (validatedUserName != "" && validatedUserName != Manager.Instance.currentPlayerName)
        {
            changeUsernameField.text = validatedUserName;
            changeUsernameBtn.interactable = true;
            changeUsernameBtnTxt.color = Color.HSVToRGB(0,0,0.78f);
        }
        else
        {
            changeUsernameBtn.interactable = false;
            changeUsernameBtnTxt.color = Color.HSVToRGB(0, 0, 0.38f);
        }
    }

    public void setUserName()
    {
        string validatedUserName = ValidateUserInput.isUsernameValid(changeUsernameField.text);
        if (validatedUserName != "" && validatedUserName != Manager.Instance.currentPlayerName)
        {
            Manager.Instance.currentPlayerName = validatedUserName;
            if(Manager.Instance.gamestate == Manager.GameState.Game || Manager.Instance.gamestate == Manager.GameState.PausedGame)
                Playermanager.ownerPlayer._playerName.Value = validatedUserName;
            checkUsernameInput();
        }
    }
    #endregion
}
