using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public const float MinZoom = 0.1f, MaxZoom = 5f;

    [SerializeField] private Transform moveObject, scaleObject;

    private Vector2 lastCursorPos;
    private new Camera camera;

    public static float zoomLevel
    {
        get => _zoomLevel;
        set {
            _zoomLevel = value;
            instance.scaleObject.localScale = zoomVec;
        }
    }
    public static Vector3 zoomVec => new(zoomLevel, zoomLevel, 1f);
    public static Vector2 offset;
    public static CameraMovement instance { get; private set; }
    public static bool zoomEnabled;

    [Range(MinZoom, MaxZoom)] private static float _zoomLevel = 1f;


    private void Awake()
    {
        camera = GetComponent<Camera>();
        instance = this;
    }

    private void Update()
    {
        Vector2 stw = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = lastCursorPos - stw;

        if(Input.GetKey(KeyCode.Mouse2))
            offset -= delta;

        moveObject.position = offset;
        lastCursorPos = stw;

        if(!zoomEnabled)
            return;

        float scroll = Input.mouseScrollDelta.y;
        if(scroll != 0f)
        {
            float newZoomLevel = zoomLevel + scroll * Mathf.Pow(Mathf.Min(zoomLevel, .7f), 2.5f);
            zoomLevel = Mathf.Clamp(newZoomLevel, MinZoom, MaxZoom);
        }
    }
}