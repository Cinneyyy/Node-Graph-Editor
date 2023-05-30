using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ProjectManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    public ColorEditor colorEditor;

    private List<Node> nodes = new();

    public static ProjectManager instance { get; private set; }


    private void Awake()
    {
        instance = this;
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
        return "";
    }

    public static Vector2 GetScaledMousePos()
        => Input.mousePosition / instance.canvas.scaleFactor;
}