using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [Header("Audio Sources")]
    public AudioSource SFXAudioSource;

    [Space(10f)]
    [Header("Audio Clipes")]

    [SerializeField]
    private AudioClip MoveAudioClip;
    [SerializeField]
    private AudioClip WinAudioClip;
    [SerializeField]
    private AudioClip LoseAudioClip;
    [SerializeField]
    private AudioClip ClickAudioClip;
    [SerializeField]
    private AudioClip RandomAudioClip;
    [SerializeField]
    private AudioClip SuperCheckerAudioClip;

    public void playClickSoundSES()
    {
        SFXAudioSource.PlayOneShot(ClickAudioClip);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void PlayOneShotAudio(AudioType type)
    {
        switch (type)
        {
            case AudioType.Move:
                SFXAudioSource.PlayOneShot(MoveAudioClip);
                break;
            case AudioType.Win:
                SFXAudioSource.PlayOneShot(WinAudioClip);
                break;
            case AudioType.Lose:
                SFXAudioSource.PlayOneShot(LoseAudioClip);
                break;
            case AudioType.ClickButton:
                SFXAudioSource.PlayOneShot(ClickAudioClip);
                break;
            case AudioType.Random:
                SFXAudioSource.PlayOneShot(RandomAudioClip);
                break;
            case AudioType.SuperChecker:
                SFXAudioSource.PlayOneShot(SuperCheckerAudioClip);
                break;
        }
    }

    public enum AudioType
    {
        Move,
        Win,
        Lose,
        ClickButton,
        Random,
        SuperChecker
    }

}
