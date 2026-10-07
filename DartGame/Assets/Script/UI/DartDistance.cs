using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DartDistance : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI distanceText;

    private float distance;

    public void SetDistance(float distance)
    {
        this.distance = distance;
        distanceText.text = $"{Math.Round(distance, 4) * 5}m";

        CancelInvoke(nameof(DisactiveDistance));
        Invoke(nameof(DisactiveDistance), 1f);
    }

    public void CompareDistance(bool isPlayer, float compareDistance)
    {
        var eventManager = Locator<EventManager>.Get();

        if(isPlayer)
        {
            if (distance > compareDistance)
                eventManager.Notify(ChannelInfo.GameOver, isPlayer);
            else
                eventManager.Notify(ChannelInfo.GameOver, !isPlayer);
        }
        else
        {
            if (distance > compareDistance)
                eventManager.Notify(ChannelInfo.GameOver, isPlayer);
            else
                eventManager.Notify(ChannelInfo.GameOver, !isPlayer);
        }
    }

    public IEnumerator CompareDis(bool isPlayer, float compareDistance, float delay)
    {
        CancelInvoke(nameof(DisactiveDistance));
        Invoke(nameof(DisactiveDistance), 1f);
        yield return new WaitForSeconds(delay);

        var eventManager = Locator<EventManager>.Get();
        bool winner = distance > compareDistance ? isPlayer : !isPlayer;
        eventManager.Notify(ChannelInfo.GameOver, winner);
    }


    public void DisactiveDistance()
    {
        var eventManager = Locator<EventManager>.Get();

        CinemachineSetting cameraSetting = new CinemachineSetting();
        cameraSetting.isTarget = false;
        cameraSetting.target = null;

        eventManager.Notify(ChannelInfo.TargetCamera, cameraSetting);
        eventManager.Notify(ChannelInfo.IsPlayerTurn);
        eventManager.Notify(ChannelInfo.Controller);

        this.gameObject.SetActive(false);
    }


    public void GameOver(bool isWinner)
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.GameOver, isWinner);
    }
}
