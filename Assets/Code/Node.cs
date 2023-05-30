using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Node : MonoBehaviour
{
	[Serializable]
	public class ColorPair
	{
		public Graphic obj;
		public Color tint;
		public bool useAccentColor;
	}


	[Header("Rect Transforms")]
	[SerializeField] private RectTransform nodeRect;
	[SerializeField] private RectTransform titleRect, descRect;
	[Header("Input Fields")]
	[SerializeField] private TMP_InputField titleField;
	[SerializeField] private TMP_InputField descField;
	[Header("TMPs")]
	[SerializeField] private TextMeshProUGUI titleTmp;
	[SerializeField] private TextMeshProUGUI descTmp;

	[Header("Other")]
	[SerializeField] private Outline outline; 
	[SerializeField] private Image toolbar;
	[SerializeField] private ColorPair[] coloredObjects = { };

	public string title
	{
		get => titleField.text;
		set => titleField.text = value;
	}
	public Color mainColor = Color.white, accentColor = Color.white;

	private bool dragging;
	private Vector2 dragOffset;


    private void Awake()
	{
		if(Application.isPlaying)
			ProjectManager.RegisterNode(this);
	}
	
    private void Update()
	{
		if(dragging)
			CalculateDraggedPos();

		UpdateVisuals();
	}

	private void SetFinalSize(Vector2 newSize)
	{
		nodeRect.sizeDelta = newSize;
		titleRect.sizeDelta = new(newSize.x, titleRect.sizeDelta.y);

		descRect.sizeDelta = new(newSize.x, newSize.y - titleRect.sizeDelta.y);
		descRect.anchorMin = descRect.anchorMax = Vector2.zero;
		descRect.anchoredPosition = descRect.sizeDelta / 2f;
	}

	private void CalculateDraggedPos()
		=> nodeRect.localPosition = dragOffset + ProjectManager.GetScaledMousePos();


	public void UpdateVisuals()
	{
#if false
		var desc = descTmp.text;
		descTmp.text = "_" + desc + "_";
		descTmp.ForceMeshUpdate();
		Vector2 rv_desc = descTmp.GetRenderedValues();
		descTmp.text = desc;
		descTmp.ForceMeshUpdate();
#else
		Vector2 rv_desc = descTmp.GetRenderedValues();
#endif

		Vector2 rv_title = titleTmp.GetRenderedValues();
		Vector2 size = new(Mathf.Max(250f, rv_title.x + 50f, rv_desc.x + 50f),
						   Mathf.Max(125f, rv_desc.y + titleRect.sizeDelta.y + 12.5f));

		SetFinalSize(size);
    }

	public void UpdateColors()
	{
		foreach(var co in coloredObjects)
			co.obj.color = co.tint * (co.useAccentColor ? accentColor : mainColor);
		outline.effectColor = toolbar.color;
	}

	public void DestroyNode()
	{
		ProjectManager.RemoveNode(this);
		Destroy(gameObject);
	}

	public void StartDrag()
	{
		dragging = true;
		dragOffset = (Vector2)transform.localPosition - ProjectManager.GetScaledMousePos();
	}

	public void EndDrag()
	{
		if(dragging)
		{
			CalculateDraggedPos();
			dragging = false;
		}
	}

	public void OpenColorEditor()
	{
		var colorEditor = ProjectManager.instance.colorEditor;

		if(colorEditor.gameObject.activeSelf)
			colorEditor.Done();

		colorEditor.gameObject.SetActive(true);
		colorEditor.transform.localPosition = (Vector2)transform.position + Vector2.one * 100;
		colorEditor.selectedNode = this;
		colorEditor.OnOpen();
	}
}