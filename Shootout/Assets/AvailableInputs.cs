using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AvailableInputs : MonoBehaviour
{
   [SerializeField] PlayerInput playerInput;

    void Start()
    {

        getInputs();
    }

    void getInputs()
    {
        ReadOnlyArray<InputDevice> inputDevices = playerInput.devices;
        for (int i = 0; i < inputDevices.Count; i++)
        {
            InputDevice device = inputDevices[i];
            Debug.Log(device.name + ", " + device.displayName + ", " + device.description + ", " + device.remote + ", " + device.added + ", " + device.deviceId);
        }
    }
}
