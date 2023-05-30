using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform nodeParent;

    [Range(.1f, 3f)] public float zoomLevel = 500f;
    public Vector2 zoomVec => new(zoomLevel, zoomLevel);
    public Vector2 offset;

    private Vector2 lastCursorPos;
    private new Camera camera;

    public static CameraMovement instance { get; private set; }


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

        lastCursorPos = stw;

       // if(Input.mouseScrollDelta.y != 0f)
       // {
       //     zoomLevel = Mathf.Clamp(zoomLevel + 0.3f * Input.mouseScrollDelta.y * (Mathf.Pow(Map(zoomLevel, 0.1f, 3f, 0f, 1f), 2.7f) + .25f), 0.1f, 3f);
       //     nodeParent.localScale = zoomVec;
       // }

        nodeParent.position = offset;
    }


   // private static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
   //     => ((value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow)) + toLow;
}