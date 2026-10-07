using UnityEngine;

public class Dart : MonoBehaviour, IEntity
{
    public GameObject dartPin;

    private Vector3 localScale;
    private Rigidbody rigidbody;

    private bool isFlying;

    [SerializeField]
    private float maxAngle = 15f;

    private Vector3 start, target;
    private Quaternion startRotation, lookRotation;
    private float duration;
    private int score;
    private bool isPlayer;

    private Vector3 pinLocalOffset;

    private float slowTime = 0.5f;
    private float fastTime = 0.1f;


    private float slowStart = 0.3f;
    private float slowSpeed = 0.2f;
    private float slowBlend = 0.1f;
    private bool isSlowAction;
    private float progress;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void OnDespawn()
    {
        this.transform.localScale = localScale;
        ReadyDart();
    }

    public void OnSpawn()
    {
        ReadyDart();
    }

    public void SetTransform(Vector3 position, Quaternion rotation, float multiplier = 1, Transform parent = null)
    {
        if (parent != null)
            this.transform.SetParent(parent);

        this.transform.localPosition = position;
        this.transform.localRotation = rotation;
        localScale = this.transform.localScale;
        this.transform.localScale = localScale * multiplier;
    }

    public void Shot(ShotInformation shotInfo = default)
    {
        start = dartPin.transform.position;
        startRotation = this.transform.rotation;
        target = shotInfo.target;

        Vector3 direction = target - start;
        if (direction.sqrMagnitude < 1e-6f)
            direction = transform.forward;

        Quaternion aimRotation = Quaternion.LookRotation(direction.normalized);
        float angleDiff = Quaternion.Angle(startRotation, aimRotation);

        maxAngle = 15f;
        float ratido = Mathf.Clamp01(maxAngle/Mathf.Max(angleDiff, 0.01f));
        lookRotation = Quaternion.Slerp(startRotation, aimRotation, ratido);

        duration = Mathf.Lerp(slowTime, fastTime, shotInfo.power);
        isFlying = true;
        pinLocalOffset = Quaternion.Inverse(transform.rotation) * (start -transform.position);

        isSlowAction = shotInfo.isSlowAction;
        score = shotInfo.score;
        isPlayer = shotInfo.isPlayer;

        if (isSlowAction)
        { 
            var eventManager = Locator<EventManager>.Get();
            CinemachineSetting setting = new CinemachineSetting { isTarget = true, target = this.gameObject };
            eventManager.Notify(ChannelInfo.TargetCamera, setting);
        }

        progress = 0f;
    }


    public void ReadyDart()
    {
        isFlying = false;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    private void Update()
    {
        if (!isFlying) return;

        float speed = 1f;

        if(isSlowAction)
        {
            float blend = Mathf.InverseLerp(slowStart, slowStart+slowBlend, progress);
            speed = Mathf.Lerp(1f, slowSpeed, blend);
        }

        progress = Mathf.Min(1f, progress + Time.deltaTime / duration * speed);

        float t = progress;

        transform.rotation = Quaternion.Slerp(startRotation, lookRotation, t);
        Vector3 pinPosition = Vector3.Lerp(start, target, t);
        transform.position = pinPosition - transform.rotation * pinLocalOffset;

        if(t >= 1f)
        {
            isFlying = false;

            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.DartReady);

            ScoreInfo scoreInfo = new ScoreInfo();
            scoreInfo.score = score;
            scoreInfo.isPlayer = isPlayer;
            
            eventManager.Notify(ChannelInfo.ScoreUI, scoreInfo);

            var soundFactory = Locator<SFXFactory>.Get();
            soundFactory.Spawn(Vector3.zero, Quaternion.identity);
        }
    }

}
