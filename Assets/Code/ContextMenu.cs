using UnityEngine;

public class ContextMenu : MonoBehaviour
{
	[SerializeField] private GameObject menuObj;
	[SerializeField] private RectTransform menuRect;

	public bool isOpen { get; private set; }

	public static ContextMenu instance { get; private set; }
	public static bool suppress;


    private void Awake()
        => instance = this;


    public void Open()
	{
		if(ViewMode.active)
			return;

		if(suppress)
		{
			Node.CancelConnectionCreation();
			return;
		}

		menuRect.localPosition = ProjectManager.GetOffsetScaledMousePos();
		menuObj.SetActive(true);
	}
	
	public void Close()
	{
		if(ViewMode.active)
			return;

		menuObj.SetActive(false);
	}

	public void NewNode()
	{
		if(ViewMode.active)
			return;

		ProjectManager.CreateNewNode();
		Close();
	}
}