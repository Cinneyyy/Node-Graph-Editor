using System;
using System.Collections.Generic;
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
		public bool partOfOutline;
	}

	[Serializable]
	public class Serializable
	{
		public string title, description, guid;
		public Color mainColor, accentColor;
		public Vector2 position;
		public bool accentOutline;


		public Serializable(Node node)
		{
			title = node.titleField.text;
			description = node.descField.text;
			mainColor = node.mainColor;
			accentColor = node.accentColor;
			position = node.transform.localPosition;
			guid = node.guid;
			accentOutline = node.accentOutline;
		}
	}

	[Serializable]
	public class Connection
	{
		public string from;
		public string to;
		[NonSerialized] public LineRenderer renderer;


		public Connection(Node starter, Node current, bool startedWithInput)
		{
			from = (startedWithInput ? current : starter).guid;
			to = (startedWithInput ? starter : current).guid;
		}


		public string GetOther(string node)
			=> from != node ? from : to;

		public Node GetNode(bool fromNode)
			=> ProjectManager.GetNode(fromNode ? from : to);


        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();


		public static bool operator ==(Connection from, Connection to) 
		{
			if(from is null)
				return to is null;
			else if(to is null)
				return false;
			
			return from.from == to.from && from.to == to.to;
		}
		public static bool operator !=(Connection from, Connection to) => !(from == to);
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

	public Transform connectorIn, connectorOut;
	public string title
	{
		get => titleField.text;
		set => titleField.text = value;
	}
	public string desc
	{
		get => descField.text;
		set => descField.text = value;
	}
	public Color mainColor = Color.white, accentColor = Color.white;
	public string guid;
	public bool accentOutline;
	public List<Connection> associatedConnections = new();

	private bool dragging;
	private Vector2 dragOffset;
	
	private static bool creatingConnection;
	private static Node connectionStarter;
	private static bool connectionStartedWithInput;


    private void Awake()
	{
		if(Application.isPlaying)
			ProjectManager.RegisterNode(this);
	}
	
    private void Update()
	{
		if(ViewMode.active)
			return;

		if(dragging)
			CalculateDraggedPos();

		UpdateVisuals();

		if(creatingConnection && connectionStarter == this)
		{
			var line = ProjectManager.instance.tempLine;
			line.gameObject.SetActive(true);
			line.SetPosition(0, (connectionStartedWithInput ? connectorIn : connectorOut).position);
			line.SetPosition(1, (Vector2)ProjectManager.instance.camera.ScreenToWorldPoint(Input.mousePosition));
			line.startColor = accentColor;
			line.endColor = Color.white;
		}
	}

	private void SetFinalSize(Vector2 newSize)
	{
		nodeRect.sizeDelta = newSize;
		titleRect.sizeDelta = new(newSize.x, titleRect.sizeDelta.y);

		descRect.sizeDelta = new(newSize.x, newSize.y - titleRect.sizeDelta.y);
		descRect.anchorMin = descRect.anchorMax = Vector2.zero;
		descRect.anchoredPosition = descRect.sizeDelta / 2f;

		ProjectManager.UpdateAssociatedConnectionLines(this);
	}

	private void CalculateDraggedPos()
	{
		nodeRect.localPosition = dragOffset + ProjectManager.GetScaledMousePos();
		ProjectManager.UpdateAssociatedConnectionLines(this);
	}


	public void UpdateVisuals()
	{
		Vector2 rv_desc = descTmp.GetRenderedValues();
		Vector2 rv_title = titleTmp.GetRenderedValues();
		Vector2 size = new(Mathf.Max(250f, rv_title.x + 50f, rv_desc.x + 50f),
						   Mathf.Max(125f, rv_desc.y + titleRect.sizeDelta.y + 12.5f));

		SetFinalSize(size);
    }

	public void UpdateColors()
	{
		foreach(var co in coloredObjects)
			co.obj.color = co.partOfOutline && accentOutline ? accentColor : co.tint * (co.useAccentColor ? accentColor : mainColor);
		outline.effectColor = toolbar.color;
		ProjectManager.UpdateAssociatedConnectionLines(this);
	}

	public void DestroyNode()
	{
		if(ViewMode.active)
			return;

		foreach(var ac in associatedConnections)
		{
			ProjectManager.GetNode(ac.GetOther(guid)).associatedConnections.Remove(ac);
			ProjectManager.instance.connections.Remove(ac);
			ProjectManager.instance.connectionLines.Remove(ac.renderer.gameObject);
			Destroy(ac.renderer.gameObject);
		}
		associatedConnections.Clear();

		ProjectManager.RemoveNode(this);
		Destroy(gameObject);
	}

	public void StartDrag()
	{
		if(ViewMode.active)
			return;

		if(!Input.GetKey(KeyCode.Mouse0))
			return;

		dragging = true;
		dragOffset = (Vector2)transform.localPosition - ProjectManager.GetScaledMousePos();

		SetAsLast();
	}

	public void EndDrag()
	{
		if(ViewMode.active)
			return;

		if(dragging)
		{
			CalculateDraggedPos();
			dragging = false;
		}

		SetAsLast();
	}

	public void OpenColorEditor()
	{
		if(ViewMode.active)
			return;

		var colorEditor = ProjectManager.instance.colorEditor;

		if(colorEditor.gameObject.activeSelf)
			colorEditor.Done();

		colorEditor.gameObject.SetActive(true);
		Vector2 min = new(-375f, -120f), max = new(375f, 60f), target = (Vector2)nodeRect.localPosition + (Vector2)nodeRect.parent.localPosition + Vector2.one * 100;
		colorEditor.transform.localPosition = new(Mathf.Clamp(target.x, min.x, max.x), Mathf.Clamp(target.y, min.y, max.y));
		colorEditor.selectedNode = this;
		colorEditor.OnOpen();

		SetAsLast();
	}

	public void SetAsLast()
		=> transform.SetAsLastSibling();

	public void ClickConnector(bool inputConnector)
	{
		if(ViewMode.active)
			return;

		if(creatingConnection)
		{
			if(connectionStarter == this)
				return;

			if(inputConnector == connectionStartedWithInput)
				return;

			if(ProjectManager.instance.connections.Exists(c => c == new Connection(connectionStarter, this, connectionStartedWithInput)))
				return;

			Connection connection = new(connectionStarter, this, connectionStartedWithInput);
			if(associatedConnections.Exists(c => c == connection))
				return;

            ProjectManager.instance.connections.Add(connection);
			creatingConnection = false;
			connectionStarter.associatedConnections.Add(connection);
			associatedConnections.Add(connection);
			ProjectManager.BuildConnectionLine(connection, false);
			ProjectManager.instance.tempLine.gameObject.SetActive(false);
			ContextMenu.suppress = false;
		}
		else
		{
			creatingConnection = true;
			connectionStarter = this;
			connectionStartedWithInput = inputConnector;
			ContextMenu.suppress = true;
		}
	}

	public void ClearConnections(bool inputConnector)
	{
		if(ViewMode.active)
			return;

		foreach(var ac in associatedConnections)
		{
			if(ac != null && (inputConnector ? ac.to : ac.from) == guid)
			{
				ProjectManager.instance.connections.Remove(ac);
				ProjectManager.instance.connectionLines.Remove(ac.renderer.gameObject);
				ProjectManager.GetNode(ac.GetOther(guid)).associatedConnections.Remove(ac);
				Destroy(ac.renderer.gameObject);
			}
		}

		ProjectManager.instance.connections.RemoveAll(c => c == null || c.renderer == null);
		ProjectManager.instance.connectionLines.RemoveAll(c => c == null);
		foreach(var ac in associatedConnections)
			if(ac != null && ac.renderer == null)
			{
				ProjectManager.GetNode(ac.from).associatedConnections.Remove(ac);
				ProjectManager.GetNode(ac.to).associatedConnections.Remove(ac);
			}
	}


	public static void CancelConnectionCreation()
	{
		if(ViewMode.active)
			return;

		creatingConnection = false;
		ContextMenu.suppress = false;
	}
}