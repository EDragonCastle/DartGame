using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    public int totalScore = 101;
    private int originScore;

    [SerializeField]
    private DartUI[] darts;

    [SerializeField]
    private GameObject turn;

    public int scoreIndex = 0;
    public int rememberScore;

    private EventManager eventManager;

    private float allClearTime = 1.5f;

    private void Start()
    {
        originScore = totalScore;
        SetTextingScore(totalScore);
        turn.SetActive(false);
        eventManager = Locator<EventManager>.Get();
    }

    public void CheckingScoreBoard(ShotInformation shotInfo)
    {
        // 그러면 여기서 확인한다.
        int endScore = totalScore - shotInfo.score;
        
        if (endScore == 0)
            shotInfo.isSlowAction = true;
        else
            shotInfo.isSlowAction = false;
    }

    public void Score(ScoreInfo score)
    {
        // 총 3번을 쏜다.
        int calculatorScore = totalScore - score.score;

        if(calculatorScore < 0)
        {
            // 음수면 안된다. 점수를 초기화 시킨다.
            AllClearDartUI();
            scoreIndex = 0;
            totalScore = rememberScore;
            SetTextingScore(totalScore);
        }
        else if(calculatorScore == 0)
        {
            // 이긴건 확정인데
            // 적에게 다트를 던질 기회를 준다.
            darts[scoreIndex].SetScore(score.score);
            SetTextingScore(0);
            scoreIndex = 0;

            eventManager.Notify(ChannelInfo.FinalGame);
            CancelInvoke(nameof(AllClearDartUI));
            Invoke(nameof(AllClearDartUI), allClearTime);

            CancelInvoke(nameof(SwitchingMainCamera));
            Invoke(nameof(SwitchingMainCamera), allClearTime/2);
        }
        else
        {
            darts[scoreIndex].SetScore(score.score);
            totalScore = calculatorScore;
            SetTextingScore(totalScore);

            scoreIndex++;

            if (scoreIndex >= darts.Length)
            {
                // 적의 턴으로 넘긴다.
                scoreIndex = 0;

                // darts들을 원점으로 되돌린다.
                CancelInvoke(nameof(AllClearDartUI));
                Invoke(nameof(AllClearDartUI), allClearTime);
            }
            else {
                eventManager.Notify(ChannelInfo.Controller);
            }
        }
    }

    public void FinalScore(ScoreInfo score)
    {
        // 이미 쏴서 맞은거니까
        int calculatorScore = totalScore - score.score;

        // 최대로 낼 수 있는 점수가 120인데
        if (calculatorScore > 120 - (scoreIndex * 60))
        {
            score.isEnd = true;
            return;
        }

        if (calculatorScore < 0)
        {
            // 음수면 게임 끝
            scoreIndex = 0;
            score.isEnd = true;
            return;
        }
        else if (calculatorScore == 0)
        {
            // 똑같은 점수에 들어왔다.
            darts[scoreIndex].SetScore(score.score);
            SetTextingScore(0);
            scoreIndex = 0;

            // 연장전에 들어간다.
            CancelInvoke(nameof(AllClearDartUI));
            Invoke(nameof(AllClearDartUI), allClearTime);

            CancelInvoke(nameof(SwitchingMainCamera));
            Invoke(nameof(SwitchingMainCamera), allClearTime/2);
        
            eventManager.Notify(ChannelInfo.OverTime);
        }
        else
        {
            darts[scoreIndex].SetScore(score.score);
            totalScore = calculatorScore;
            SetTextingScore(totalScore);

            scoreIndex++;

            if (scoreIndex >= darts.Length)
            {
                // 게임 끝
                eventManager.Notify(ChannelInfo.GameOver);
                score.isEnd = true;
                return;
            }
            else
            {
                eventManager.Notify(ChannelInfo.Controller);
            }
        }
    }

    public void CurrentTurn(bool isActive)
    {
        turn.SetActive(isActive);
    }

    public void SetTextingScore(int score)
    {
        scoreText.text = $"score : {score}";
    }

    public void AllClearDartUI()
    {
        foreach(var dart in darts) {
            dart.Clear();
        }

        var factory = Locator<DartFactory>.Get();
        factory.DespawnAll();

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.IsPlayerTurn);
        eventManager.Notify(ChannelInfo.Controller);
        eventManager.Notify(ChannelInfo.DartReady, true);
    }

    public void RememberScore()
    {
        if (scoreIndex == 0)
            rememberScore = totalScore;
    }
    
    public void ScoreSetting(int score)
    {
        originScore = score;
        totalScore = score;
        SetTextingScore(totalScore);
    }

    public void ResetScore()
    {
        foreach (var dart in darts) {
            dart.Clear();
        }

        scoreIndex = 0;
        totalScore = originScore;
        SetTextingScore(originScore);
    }

    private void SwitchingMainCamera()
    {
        CinemachineSetting cameraSetting = new CinemachineSetting();
        cameraSetting.isTarget = false;
        cameraSetting.target = null;

        eventManager.Notify(ChannelInfo.TargetCamera, cameraSetting);
    }
}

