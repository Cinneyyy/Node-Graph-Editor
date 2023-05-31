using UnityEngine;
using CM = CameraMovement;

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

        float width = Mathf.Clamp(CM.zoomLevel, .2f, 5f) / 10f;
        foreach(var cl in ProjectManager.instance.connections)
        {
            if(cl?.renderer == null)
                continue;

            cl.renderer.SetPosition(0, cl.GetNode(true).connectorOut.position);
            cl.renderer.SetPosition(1, cl.GetNode(false).connectorIn.position);

            cl.renderer.widthMultiplier = width;
        }

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


    public void EnableCtxMenu()
        => ContextMenu.pSupress = false;


    public static void EnterViewMode()
    {
        instance.viewModeOverlay.SetActive(true);
        instance.toolbar.SetActive(false);
        
        if(ColorEditor.instance.gameObject.activeSelf)
            ColorEditor.instance.Done();

        ContextMenu.instance.Close();
        ContextMenu.pSupress = true;

        CM.zoomEnabled = true;

        active = true;
    }

    public static void ExitViewMode()
    {
        instance.viewModeOverlay.SetActive(false);
        instance.toolbar.SetActive(true);

        CM.zoomEnabled = false;
        CM.instance.nodeParent.localScale = Vector3.one;

        ProjectManager.UpdateAllConnectionLines();

        active = false;

        instance.Invoke(nameof(EnableCtxMenu), .1f);
    }
}