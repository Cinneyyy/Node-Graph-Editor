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
		if(suppress)
			return;

		menuRect.localPosition = ProjectManager.GetOffsetScaledMousePos();
		menuObj.SetActive(true);
	}
	
	public void Close()
		=> menuObj.SetActive(false);

	public void NewNode()
	{
		ProjectManager.CreateNewNode();
		Close();
	}
}