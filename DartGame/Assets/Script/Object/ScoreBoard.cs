using UnityEngine;

public class ScoreBoard : MonoBehaviour, IChannel
{
    [SerializeField]
    private ScoreUI[] scores;

    // Distance
    [SerializeField]
    private DartDistance dartDistance;

    private bool isFinal = false;
    private bool isPlayer;
    [SerializeField]
    private bool isOverTime = false;
    private float distance;
    private bool isThrowingOverTime = false;

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.ScoreUI, HandleEvent);
        eventManager.Subscription(ChannelInfo.ScoreSetting, HandleEvent);
        eventManager.Subscription(ChannelInfo.CheckingScoreUI, HandleEvent);
        eventManager.Subscription(ChannelInfo.FinalGame, HandleEvent);
        eventManager.Subscription(ChannelInfo.OverTime, HandleEvent);
        eventManager.Subscription(ChannelInfo.GameReset, HandleEvent);
        eventManager.Subscription(ChannelInfo.Turn, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.ScoreUI, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.ScoreSetting, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.CheckingScoreUI, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.FinalGame, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.OverTime, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.GameReset, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.Turn, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch (channel)
        {
            case ChannelInfo.CheckingScoreUI:
                ShotInformation shotInfo = information as ShotInformation;
                if (shotInfo != null)
                {
                    if (!isOverTime)
                    {
                        isPlayer = shotInfo.isPlayer;

                        if (shotInfo.isPlayer)
                            scores[0].CheckingScoreBoard(shotInfo);
                        else
                            scores[1].CheckingScoreBoard(shotInfo);
                    }
                    else
                    {
                        shotInfo.isSlowAction = true;
                    }

                    distance = shotInfo.radius;
                }
                break;
            case ChannelInfo.ScoreUI:
                if (information is ScoreInfo scoreInfo)
                {
                    if (!isOverTime)
                    {
                        if (scoreInfo.isPlayer)
                        {
                            scores[0].RememberScore();

                            if (!isFinal)
                                scores[0].Score(scoreInfo);
                            else
                                scores[0].FinalScore(scoreInfo);
                        }
                        else
                        {
                            scores[1].RememberScore();

                            if (!isFinal)
                                scores[1].Score(scoreInfo);
                            else
                                scores[1].FinalScore(scoreInfo);
                        }

                        if (scoreInfo.isEnd)
                            Winner(!scoreInfo.isPlayer);
                    }
                    else
                    {
                        dartDistance.gameObject.SetActive(true);
                        dartDistance.SetDistance(distance);

                        // 결과가 바로 나온다.
                        if (isThrowingOverTime)
                            StartCoroutine(dartDistance.CompareDis(!scoreInfo.isPlayer, distance, 2));

                        isThrowingOverTime = true;
                    }
                }
                break;
            case ChannelInfo.ScoreSetting:
                if (information is int score)
                {
                    foreach (var scoreUI in scores)
                    {
                        scoreUI.ScoreSetting(score);
                    }
                }
                break;
            case ChannelInfo.FinalGame:
                // 턴이 아직 안 바뀐 상태.
                if (isPlayer)
                {
                    if (scores[1].totalScore > 180)
                        Winner(true);
                }
                else
                {
                    if (scores[0].totalScore > 180)
                        Winner(false);
                }

                isFinal = !isFinal;
                break;
            case ChannelInfo.OverTime:
                isOverTime = true;
                isThrowingOverTime = false;
                break;
            case ChannelInfo.GameReset:
                foreach(var scoreUI in scores) {
                    scoreUI.ResetScore();
                }

                dartDistance.DisactiveDistance();

                isThrowingOverTime = false;
                isFinal = false;
                isOverTime = false;
                distance = 0;

                break;
            case ChannelInfo.Turn:
                if(information is bool isPlayerTurn)
                {
                    if (isPlayerTurn)
                    {
                        scores[0].CurrentTurn(true);
                        scores[1].CurrentTurn(false);
                    }
                    else
                    {
                        scores[0].CurrentTurn(false);
                        scores[1].CurrentTurn(true);
                    }
                }
                break;
        }
    }

    private void Winner(bool isPlayer)
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.GameOver, isPlayer);
    }
}
