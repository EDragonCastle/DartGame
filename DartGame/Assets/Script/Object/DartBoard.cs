using UnityEngine;

public class DartBoard : MonoBehaviour
{
    private Camera cam;

    public Vector2 referenceResolution = new Vector2(1080, 1920);

    private Vector3 refNdc;      // x,y: 기준 화면에서의 위치(-1~1), z: 카메라와의 깊이(고정)
    private Vector3 refScale;
    private float refAspect;
    private float lastAspect;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        refAspect = referenceResolution.x / referenceResolution.y;
        refScale = this.transform.localScale;

        Vector3 local = cam.transform.InverseTransformPoint(this.transform.position);
        float halfH = local.z * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        refNdc = new Vector3(local.x / (halfH * refAspect), local.y / halfH, local.z);

        Fit();
    }

    private void Update()
    {
        if (!Mathf.Approximately(cam.aspect, lastAspect)) Fit();
    }

    private void Fit()
    {
        lastAspect = cam.aspect;

        // 세로 FOV는 고정이라 화면 비율이 바뀌면 "가로로 보이는 폭"만 달라진다
        float halfH = refNdc.z * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfW = halfH * cam.aspect;

        // 위치: 기준 화면에서 보이던 곳과 같은 화면 위치로
        Vector3 local = new Vector3(refNdc.x * halfW, refNdc.y * halfH, refNdc.z);
        this.transform.position = cam.transform.TransformPoint(local);

        // 크기: 기준보다 화면이 좁아지면 가로폭에 맞춰 줄이고, 넓어져도 기준 크기를 넘지 않게
        this.transform.localScale = refScale * Mathf.Min(1f, cam.aspect / refAspect);
    }
}
