using UnityEngine;

public class Title : MonoBehaviour
{
    public GameObject title;
    public GameObject backGround;


    public void GameResetButton()
    {
        var dartFactory = Locator<DartFactory>.Get();
        dartFactory.DespawnAll();

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.GameReset);

        int random = Random.Range(1, 1001);
        eventManager.Notify(ChannelInfo.ScoreSetting, random);

        CinemachineSetting cameraSetting = new CinemachineSetting();
        cameraSetting.isTarget = false;
        eventManager.Notify(ChannelInfo.TargetCamera, cameraSetting);
        eventManager.Notify(ChannelInfo.DartReady);
    }
    
    public void GameStart()
    {
        title.SetActive(false);
        backGround.SetActive(false);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 실제 빌드된 앱에서 실행 중일 때
    Application.Quit();
#endif
    }

    public void GameReset()
    {
        title.gameObject.SetActive(false);
        backGround.SetActive(false);

        var dartFactory = Locator<DartFactory>.Get();
        dartFactory.DespawnAll();

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.GameReset);

        CinemachineSetting cameraSetting = new CinemachineSetting();
        cameraSetting.isTarget = false;
        eventManager.Notify(ChannelInfo.TargetCamera, cameraSetting);
        eventManager.Notify(ChannelInfo.DartReady);
    }

    public void Setting()
    {
        var eventMnager = Locator<EventManager>.Get();
        eventMnager.Notify(ChannelInfo.Setting);
    }
}