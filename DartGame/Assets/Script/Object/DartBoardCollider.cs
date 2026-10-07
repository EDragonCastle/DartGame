using UnityEngine;

public class DartBoardCollider : MonoBehaviour, IChannel
{
    private readonly int[] sector = { 20, 1, 18, 4, 13, 6, 10, 16, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    private const float innerBullseye = 0.2f;
    private const float outerBullseye = 0.4f;
    private const float tripleRingInner = 1.7f;
    private const float tripleRingOuter = 2f;
    private const float doubleRingInner = 2.8f;
    private const float doubleRingOuter = 3.1f;

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.Score, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.Score, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.Score:
                if(information is ShotInformation shotInfo)
                    Score(shotInfo);
                break;
        }
    }

    // 여기서 점수처리를 해야 한다.
    public void Score(ShotInformation shotInfomaiton)
    {
        Vector3 hitPosition = shotInfomaiton.target;

        Vector3 position = hitPosition - this.transform.position;

        float radius = new Vector2(position.x, position.y).magnitude;
        shotInfomaiton.radius = radius;
        // 원래는 y, x지만 해당 과녁판에서는 문제가 생겼는지 역방향으로 출력되서 -x, y로 진행했다.
        float angle = Mathf.Atan2(-position.x, position.y) * Mathf.Rad2Deg;

        if (angle < 0f) angle += 360;

        shotInfomaiton.score = Calculator(radius, angle);
    }

    private int Calculator(float radius, float angle)
    {
        int index = Mathf.FloorToInt(((angle + 9f) % 360f) / 18f);
        int scoreNumber = sector[index];

        if (doubleRingInner <= radius && radius <= doubleRingOuter) {
            scoreNumber *= 2;
            return scoreNumber;
        }

        if(tripleRingInner <= radius && radius <= tripleRingOuter) {
            scoreNumber *= 3;
            return scoreNumber;
        }

        if(outerBullseye >= radius) {
            if (innerBullseye >= radius)
                scoreNumber = 50;
            else
                scoreNumber = 25;

            return scoreNumber;
        }

        if (doubleRingOuter <= radius)
            return 0;
        else
            return scoreNumber;
    }
}
