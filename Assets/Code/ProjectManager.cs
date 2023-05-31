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

    private void OnApplicationQuit()
    {
        var dt = DateTime.Now;
        string time = $"{dt.Year:0000}-{dt.Month:00}-{dt.Day:00} {dt.Hour:00}-{dt.Minute:00}-{dt.Second:00}";
        File.WriteAllText($"{Application.persistentDataPath}/{projectName} (BCKP; {time}).json", GetJson());
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

        return JsonUtility.ToJson(new Project(nodes, connections, CameraMovement.offset), true);
    }

    public static void LoadJson(string json)
    {
        var project = JsonUtility.FromJson<Project>(json);
        CameraMovement.offset = project.cameraPos;

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
        node.transform.position -= node.transform.parent.position;
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

    public static bool UpdateConnectionLine(Node.Connection connection)
    {
        if(connection?.renderer == null)
            return false;

        Node from = GetNode(connection.from), to = GetNode(connection.to);

        connection.renderer.startColor = from.accentColor;
        connection.renderer.endColor = to.accentColor;

        connection.renderer.SetPosition(0, from.connectorOut.position);
        connection.renderer.SetPosition(1, to.connectorIn.position);

        connection.renderer.widthMultiplier = .1f;

        return true;
    }

    public static void UpdateAssociatedConnectionLines(Node node)
    {
        List<Node.Connection> remove = new();

        foreach(var ac in node.associatedConnections)
            if(!UpdateConnectionLine(ac))
                remove.Add(ac);

        if(remove.Count >= 1)
            foreach(var r in remove)
                node.associatedConnections.Remove(r);
    }

    public static void UpdateAllConnectionLines()
    {
        foreach(var cl in instance.connections)
            UpdateConnectionLine(cl);
    }
}