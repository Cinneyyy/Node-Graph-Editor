using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(RectTransform))]
public class ActualRectPos : MonoBehaviour
{
    public Vector2 localPosition, globalPosition;
    public bool get, setLocal, setGlobal;

    private RectTransform rectTransform;


    private void Awake()
        => rectTransform = GetComponent<RectTransform>();

    private void Update()
    {
        if(get)
        {
            localPosition = rectTransform.localPosition;
            globalPosition = rectTransform.position;
        }
        else if(setLocal)
            rectTransform.localPosition = localPosition;
        else if(setGlobal)
            rectTransform.position = globalPosition;

        if(get)
            setLocal = setGlobal = false;
    }
}