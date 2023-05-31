using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public const float MinZoom = 0.1f, MaxZoom = 5f;

    public Transform nodeParent;

    [Range(MinZoom, MaxZoom)] public static float zoomLevel = 1f;
    public static Vector3 zoomVec => new(zoomLevel, zoomLevel, 1f);
    public static Vector2 offset;

    private Vector2 lastCursorPos;
    private new Camera camera;

    public static CameraMovement instance { get; private set; }
    public static bool zoomEnabled;


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

        nodeParent.position = offset;
        lastCursorPos = stw;

        if(!zoomEnabled)
            return;

        float scroll = Input.mouseScrollDelta.y;
        if(scroll != 0f)
        {
            float newZoomLevel = zoomLevel + scroll * Mathf.Pow(Mathf.Min(zoomLevel, .7f), 2.5f);
            zoomLevel = Mathf.Clamp(newZoomLevel, MinZoom, MaxZoom);
            nodeParent.localScale = zoomVec;
        }
    }
}