using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteManager : MonoBehaviour
{
    public Toggle ToggleON;
    public Toggle ToggleOFF;
    //bool isMuted;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    AudioListener.volume = 0.0f;
    //}
    private void Awake()
    {
        if (PlayerPrefs.HasKey("mute"))
        {
            if (PlayerPrefs.GetInt("mute") == 1)
            {
                ToggleON.isOn = false;
                ToggleOFF.isOn = true;
                mutePressed();
            }
            else
            {
                ToggleON.isOn = true;
                ToggleOFF.isOn = false;
                UnmutePressed();
            }
        }
    }
    
    public void mutePressed()
    {
        PlayerPrefs.SetInt("mute", 1);
        AudioListener.volume = 0.0f;
    }
    public void UnmutePressed()
    {
        PlayerPrefs.SetInt("mute", 0);
        AudioListener.volume = 1.0f;
    }
}
