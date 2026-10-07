using UnityEngine;
using Unity.Cinemachine;

public class CameraSetting : MonoBehaviour, IChannel
{
    [SerializeField]
    private CinemachineCamera mainCamera;
    [SerializeField]
    private CinemachineCamera targetCamera;

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.TargetCamera, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.TargetCamera, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.TargetCamera:
                if(information is CinemachineSetting setting)
                    TargetingCamera(setting);
                break;
        }
    }

    private void TargetingCamera(CinemachineSetting target)
    {
        mainCamera.gameObject.SetActive(!target.isTarget);
        targetCamera.gameObject.SetActive(target.isTarget);

        if(target.target != null)
            targetCamera.Target.TrackingTarget = target.target.transform;
    }
}


