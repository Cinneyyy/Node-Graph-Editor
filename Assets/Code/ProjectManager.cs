using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.IO;

[DisallowMultipleComponent]
public class ProjectManager : MonoBehaviour
{
    [Serializable]
    public class Project
    {
        public Node.Serializable[] nodes;
        public Node.Connection[] connections;
        public Vector2 cameraPos;


        public Project(Node.Serializable[] nodes, Node.Connection[] connections, Vector2 cameraPos)
        {
            this.nodes = nodes;
            this.connections = connections;
            this.cameraPos = cameraPos;
        }
    }


    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Transform nodeParent, connectionParent;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private TextAsset defaultProjectJson;

    public new Camera camera;
    public LineRenderer tempLine;
    public ColorEditor colorEditor;
    [HideInInspector] public List<Node.Connection> connections = new();
    public readonly List<GameObject> connectionLines = new();

    private readonly List<Node> nodes = new();

    public static ProjectManager instance { get; private set; }
    public static Vector2 canvasRes => instance.canvasScaler.referenceResolution;
    public static string projectName;
    public static bool createNewProject;


    private void Awake()
        => instance = this;

    private void Start()
    {
        string path = $"{Application.persistentDataPath}/{projectName}.json";
        string json;
        if(createNewProject)
            File.WriteAllText(path, json = defaultProjectJson.text);
        else
            json = File.ReadAllText(path);

        LoadJson(json);
    }


    public static void RemoveNode(Node node)
        => instance.nodes.Remove(node);

    public static void RegisterNode(Node node)
    {
        if(!instance.nodes.Contains(node))
            instance.nodes.Add(node);
    }

    public static string GetJson()
    {
        var nodes = (from node in instance.nodes
                    select new Node.Serializable(node))
                    .ToArray();

        var connections = instance.connections.ToArray();

        return JsonUtility.ToJson(new Project(nodes, connections, CameraMovement.instance.offset), true);
    }

    public static void LoadJson(string json)
    {
        var project = JsonUtility.FromJson<Project>(json);
        CameraMovement.instance.offset = project.cameraPos;

        foreach(var n in project.nodes)
        {
            var node = CreateNewNode();

            node.title = n.title;
            node.desc = n.description;
            node.guid = n.guid;
            node.mainColor = n.mainColor;
            node.accentColor = n.accentColor;
            node.transform.localPosition = n.position;

            node.UpdateColors();
            node.UpdateVisuals();
        }

        instance.connections = project.connections.ToList();
        BuildConnectionLines();
    }

    public static Vector2 GetScaledMousePos()
        => Input.mousePosition / instance.canvas.scaleFactor;

    public static Vector2 GetOffsetScaledMousePos()
        => GetScaledMousePos() - canvasRes / 2f;

    public static Node CreateNewNode()
    {
        var node = Instantiate(instance.nodePrefab, instance.nodeParent).GetComponent<Node>();
        node.transform.localPosition = GetOffsetScaledMousePos();
        node.guid = Guid.NewGuid().ToString();
        RegisterNode(node);
        return node;
    }

    public static Vector2 ScaleToCanvas(Vector2 v)
        => v / instance.canvas.scaleFactor;

    public static void BuildConnectionLines()
    {
        foreach(var cl in instance.connectionLines)
            Destroy(cl);
        instance.connectionLines.Clear();

        foreach(var c in instance.connections)
            BuildConnectionLine(c, false);
    }

    public static Node GetNode(string guid)
        => instance.nodes.Find(n => n.guid == guid);

    public static void BuildConnectionLine(Node.Connection connection, bool addToList)
    {
        var line = Instantiate(instance.linePrefab, instance.connectionParent).GetComponent<LineRenderer>();
        
        if(addToList)
            instance.connectionLines.Add(line.gameObject);

        Node from = GetNode(connection.from), to = GetNode(connection.to);

        from.associatedConnections.Add(connection);
        to.associatedConnections.Add(connection);

        line.startColor = from.accentColor;
        line.endColor = to.accentColor;

        line.SetPosition(0, (Vector2)from.connectorOut.position);
        line.SetPosition(1, (Vector2)to.connectorIn.position);

        connection.renderer = line;
    }

    public static void UpdateConnectionLine(Node.Connection connection)
    {
        Node from = GetNode(connection.from), to = GetNode(connection.to);

        connection.renderer.startColor = from.accentColor;
        connection.renderer.endColor = to.accentColor;

        connection.renderer.SetPosition(0, from.connectorOut.position);
        connection.renderer.SetPosition(1, to.connectorIn.position);
    }

    public static void UpdateAssociatedConnectionLines(Node node)
    {
        foreach(var ac in node.associatedConnections)
            UpdateConnectionLine(ac);
    }
}