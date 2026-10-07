using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour, IChannel
{
    public GameObject backGround;
    public GameObject gameOver;
    public TextMeshProUGUI winnerText;

    private void Start()
    {
        gameOver.SetActive(false);
        backGround.SetActive(false);
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.GameOver, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.GameOver, HandleEvent);
    }


    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.GameOver:
                gameOver.SetActive(true);
                backGround.SetActive(true);

                if (information is bool isWinner)
                    Winner(isWinner);

                break;
        }
    }

    public void GameReset()
    {
        var dartFactory = Locator<DartFactory>.Get();
        dartFactory.DespawnAll();

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.GameReset);

        CinemachineSetting cameraSetting = new CinemachineSetting();
        cameraSetting.isTarget = false;

        int random = Random.Range(1, 1001);
        eventManager.Notify(ChannelInfo.ScoreSetting, random);

        eventManager.Notify(ChannelInfo.TargetCamera, cameraSetting);
        eventManager.Notify(ChannelInfo.DartReady);

        gameOver.SetActive(false);
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

    private void Winner(bool isWinner)
    {
        string winner = default;
        if (isWinner)
            winner = "Player 1";
        else
            winner = "Player 2";
        winnerText.text = $"{winner} 승리!";
    }
}
