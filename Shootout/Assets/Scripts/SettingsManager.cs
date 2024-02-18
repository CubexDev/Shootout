using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public GameObject settingsScreen;
    public NetworkManager networkManager;
    public UnityTransport netTransport;

    [SerializeField] TMP_Text versionTxt;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        Manager.gameContinued += activation;
        Manager.gameStarted += activation;

        versionTxt.text = "v" + Application.version;
    }

    private void Update()
    {
        if (changeUsernameField.isFocused) checkUsernameInput();
        updateTxts();
    }

    public void openSettings()
    {
        Manager.Instance.openSettings();
        setTab(0);
        settingsScreen.SetActive(true);
        updateControlls();
    }
    public void closeSettings()
    {
        Manager.Instance.closeSettings();
        settingsScreen.SetActive(false);

        //netTransport.MaxPacketQueueSize = int.Parse(v_packetInput.text);
        //netTransport.MaxPayloadSize = int.Parse(v_payloadInput.text);
    }

    #region controllsSetttings
    public Slider c_SensiSlider;
    public static float c_Sensi = 3f;
    public void changeSensi()
    {
        c_Sensi = c_SensiSlider.value;
    }
    public Toggle v_fpsToggle;
    public static bool v_fps = false;
    public TMP_Text v_fpsText;
    public void changeFPS()
    {
        v_fps = v_fpsToggle.isOn;
    }
    public Toggle v_netToggle;
    public static bool v_net = false;
    public TMP_Text v_tickText;
    public TMP_Text v_packetText;
    public TMP_Text v_payloadText;
    public TMP_InputField v_tickInput;
    public TMP_InputField v_packetInput;
    public TMP_InputField v_payloadInput;
    public void changeNET()
    {
        v_net = v_netToggle.isOn;
    }

    void updateControlls()
    {
        c_SensiSlider.value = c_Sensi;
        v_fpsToggle.isOn = v_fps;
        v_netToggle.isOn = v_net;
    }

    void updateTxts()
    {
        if(Manager.Instance.gamestate == Manager.GameState.Game)
        {
            if (v_fps) v_fpsText.text = "FPS: " + (int)(1 / Time.unscaledDeltaTime);
        }
    }

    void activation()
    {
        v_fpsText.transform.parent.gameObject.SetActive(v_fps);
        v_tickText.transform.parent.gameObject.SetActive(v_net);
        if (v_net)
        {
            v_tickText.text = "Tick: " + networkManager.NetworkTickSystem.TickRate;
            v_packetText.text = "Packet Que: " + netTransport.MaxPacketQueueSize;
            v_payloadText.text = "Payload: " + netTransport.MaxPayloadSize;
        }
    }
    #endregion

    #region tabs
    int currentTab;
    [System.Serializable] public class SettingsTab { public Button button; public GameObject screen; }
    public SettingsTab[] tabs;
    public TMP_InputField changeUsernameField;
    public Button changeUsernameBtn;
    public TMP_Text changeUsernameBtnTxt;

    public void setTab(int index)
    {
        if(index != currentTab)
        {
            tabs[currentTab].screen.SetActive(false);

            tabs[currentTab].button.interactable = true;
            tabs[index].button.interactable = false;
            tabs[index].screen.SetActive(true);
            currentTab = index;
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
