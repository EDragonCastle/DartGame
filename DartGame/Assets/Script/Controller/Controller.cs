using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IChannel
{
    [Header("Throw Data")]
    public float holdDistance = 1.5f;
    public Vector2 dartAngle;
    public Vector2 offset;

    public float minThrowSpeed = 1.5f;
    public float maxThrowSpeed = 6f;
    public float maxShift = 1f;
    

    private Camera cam;
    private bool isThrow;
    private bool isdragging = false;
    private Vector2 swipeVelocity;

    private EventManager eventManager;

    private bool isPlayer = true;

    private void Awake()
    {
        cam = Camera.main;
        eventManager = Locator<EventManager>.Get();
    }

    private void OnEnable()
    {
        eventManager.Subscription(ChannelInfo.IsPlayerTurn, HandleEvent);
        eventManager.Subscription(ChannelInfo.GameReset, HandleEvent);
        eventManager.Subscription(ChannelInfo.Controller, HandleEvent);
    }

    private void OnDisable()
    {
        eventManager.Unsubscription(ChannelInfo.IsPlayerTurn, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.GameReset, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.Controller, HandleEvent);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isThrow)
            return;

        isdragging = true;
        isThrow = true;
        swipeVelocity = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isdragging) return;

        Vector3 screen = new Vector3(eventData.position.x + offset.x, eventData.position.y + offset.y, holdDistance);
        Vector3 world = cam.ScreenToWorldPoint(screen);
        world += cam.transform.TransformDirection(offset);

        Vector3 viewDir = (world - cam.transform.position).normalized;
        DartTransformInformation dartTransform = new DartTransformInformation();
        dartTransform.position = world;
        dartTransform.rotation = Quaternion.LookRotation(viewDir, cam.transform.up)
                            * Quaternion.Euler(-dartAngle.x, dartAngle.y, 0f);

        eventManager.Notify(ChannelInfo.MovingDart, dartTransform);

        Vector2 instant = eventData.delta / Screen.height / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        swipeVelocity = Vector2.Lerp(swipeVelocity, instant, 0.5f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isdragging) return;

        isdragging = false;

        if (swipeVelocity.y > 0f && swipeVelocity.magnitude >= minThrowSpeed)
        {
            // 마우스 위치로부터 계산
            MousePosition(eventData.position);
        }
        else
        {
            eventManager.Notify(ChannelInfo.ErrorLog);
            isThrow = false;
        }
    }

    private void MousePosition(Vector3 position)
    {
        Ray ray = cam.ScreenPointToRay(position);

        if(!Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            isThrow = false;
            return;
        }
        else
        {
            float power = Mathf.InverseLerp(minThrowSpeed, maxThrowSpeed, swipeVelocity.y);
            float shift = Mathf.Lerp(-maxShift, maxShift, power);

            Vector3 up = Vector3.ProjectOnPlane(cam.transform.up, hit.normal).normalized;
            Vector3 target = hit.point + up * shift;
            
            ShotInformation shotInfo = new ShotInformation() 
            { target = target, power = power, isPlayer = isPlayer, isSlowAction = false, score = 0 };

            // Shooting에서 Data를 보내면 알 수 있다.
            eventManager.Notify(ChannelInfo.Score, shotInfo);

            // Score에서 UI로 보내기 전에 SlowAction을 진행할 지 판별한다.
            eventManager.Notify(ChannelInfo.CheckingScoreUI, shotInfo);

            // 그 다음 Shooting을 한다.
            eventManager.Notify(ChannelInfo.Shooting, shotInfo);
        }
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.IsPlayerTurn:
                isPlayer = !isPlayer;
                var eventManager = Locator<EventManager>.Get();
                eventManager.Notify(ChannelInfo.Turn, isPlayer);
                break;
            case ChannelInfo.GameReset:
                isPlayer = true;
                isThrow = false;
                break;
            case ChannelInfo.Controller:
                isThrow = false;
                break;
        }
    }
}


public class ShotInformation
{
    public Vector3 target;
    public bool isPlayer;
    public int score;
    public bool isSlowAction;
    public float power;
    public float radius;
}

