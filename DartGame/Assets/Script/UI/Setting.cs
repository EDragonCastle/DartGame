using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Setting : MonoBehaviour, IChannel
{
    [SerializeField]
    private AudioMixer audioMixer;

    public Slider bgm;
    public Slider sfx;
    private float maxValue;

    public GameObject setting;
    public GameObject settingBackGround;

    private void Awake()
    {
        maxValue = bgm.maxValue;
        setting.SetActive(false);
        settingBackGround.SetActive(false);
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.Setting, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.Setting, HandleEvent);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            setting.SetActive(true);
            settingBackGround.SetActive(true);
        }
    }


    public void SetBGMVolum(float sliderValue)
    {
        SetVolume("BGM", sliderValue);

    }

public void SetSFXVolume(float sliderValue)
    {
        SetVolume("SFX", sliderValue);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    private void SetVolume(string parameterName, float sliderValue)
    {
        float normalize = sliderValue / maxValue;

        float db = normalize > 0.0001f ? Mathf.Log10(normalize) * 20f : -80f;

        audioMixer.SetFloat(parameterName, db);
    }

    public void GameScreen()
    {
        setting.SetActive(false);
        settingBackGround.SetActive(false);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.Setting:
                setting.SetActive(true);
                settingBackGround.SetActive(true);
                break;
        }
    }
}
