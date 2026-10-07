using UnityEngine;

public class Shooter : MonoBehaviour, IChannel
{
    // GameObject -> Dart
    private Dart dart;

    private async void Start()
    {
        await Locator<GameManager>.Get().ResourceInitalize;

        var dartFactory = Locator<DartFactory>.Get();
        PrepareDart(dartFactory.Spawn(this.transform.position, this.transform.rotation));
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.Shooting, HandleEvent);
        eventManager.Subscription(ChannelInfo.MovingDart, HandleEvent);
        eventManager.Subscription(ChannelInfo.DartReady, HandleEvent);
        eventManager.Subscription(ChannelInfo.GameReset, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.Shooting, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.MovingDart, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.DartReady, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.GameReset, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.Shooting:
                if(information is ShotInformation shotInfo)
                {
                    Shoot(shotInfo);
                }
                break;
            case ChannelInfo.MovingDart:
                if(information is DartTransformInformation dartInfo) {
                    if(dart != null)
                    {
                        dart.transform.position = dartInfo.position;
                        dart.transform.rotation = dartInfo.rotation;
                    }

                    this.transform.position = dartInfo.position;
                    this.transform.rotation = dartInfo.rotation;
                }
                break;
            case ChannelInfo.DartReady:
                if(information is bool active)
                {
                    dart = null;
                }

                var dartFactory = Locator<DartFactory>.Get();
                var preDart = dartFactory.Spawn(Vector3.zero, Quaternion.identity);
                PrepareDart(preDart);
                break;
            case ChannelInfo.GameReset:
                dart = null;
                break;
        }
    }

    private void PrepareDart(Dart prepareDart)
    {
        if(dart == null)
        {
            dart = prepareDart;
            dart.ReadyDart();
        }
        else
        {
            // prepareDart는 반납한다.
            var dartFactory = Locator<DartFactory>.Get();
            dartFactory.Despawn(prepareDart);
        }
    }

    private void Shoot(ShotInformation shotInfo)
    {
        if (dart == null) return;

        var shotDart = dart;
        dart = null;
        shotDart.Shot(shotInfo);
    }
}

public struct DartTransformInformation
{
    public Vector3 position;
    public Quaternion rotation;
}