using UnityEngine;

public class ViewMode : MonoBehaviour
{
    [SerializeField] private GameObject viewModeOverlay, toolbar;

	public static ViewMode instance { get; private set; }
    public static bool active { get; private set; }


    private void Awake()
        => instance = this;

    private void Update()
    {
        if(!active)
            return;

        foreach(var kc in new[] {
            KeyCode.Space,
            KeyCode.Return,
            KeyCode.Mouse0
        })
            if(Input.GetKeyDown(kc))
            {
                viewModeOverlay.SetActive(!viewModeOverlay.activeSelf);
                break;
            }

        foreach(var kc in new[] {
            KeyCode.Escape,
            KeyCode.Mouse1,
        })
            if(Input.GetKeyDown(kc))
            {
                ExitViewMode();
                break;
            }
    }


    public static void EnterViewMode()
    {
        instance.viewModeOverlay.SetActive(true);
        instance.toolbar.SetActive(false);
        
        if(ColorEditor.instance.gameObject.activeSelf)
            ColorEditor.instance.Done();

        ContextMenu.instance.Close();

        CameraMovement.zoomEnabled = true;

        active = true;
    }

    public static void ExitViewMode()
    {
        instance.viewModeOverlay.SetActive(false);
        instance.toolbar.SetActive(true);

        CameraMovement.zoomEnabled = false;
        CameraMovement.instance.nodeParent.localScale = Vector3.one;

        active = false;
    }
}