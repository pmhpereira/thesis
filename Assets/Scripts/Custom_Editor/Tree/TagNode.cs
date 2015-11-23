using UnityEngine;
using NodeEditorFramework;
using UnityEditor;

[Node(false, "Patterns Tree/Tag Node", false)]
public class TagNode : BaseNode
{
    public const string ID = "tagNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor;

    [HideInInspector]
    public string tag;
    [HideInInspector]
    public int tagIndex;

    private bool oldValue;

    public override Node Create(Vector2 pos)
    {
        TagNode node = CreateInstance<TagNode>();
        node.rect = new Rect(pos.x, pos.y, 150, 50);
        node.name = "Tag";

        tagIndex = 0;

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        base.DrawNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        tag = Tag.values[tagIndex];

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        for(int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }

        GUILayout.EndVertical();

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.fontSize = 16;
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontStyle = FontStyle.Bold;
        GUILayout.Label(tag.ToString(), guiStyle);

        GUILayout.BeginVertical();

        for (int i = 0; i < Outputs.Count; i++)
        {
            Outputs[i].DisplayLayout();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    public override bool Calculate()
    {
        if(Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }

        Outputs[0].SetValue<bool>(value);
        
        if(value)
        {
            nodeColor = new Color(1f, .65f, 0f);
        }
        else
        {
            nodeColor = new Color(1f, 0f, 0f);
        }

        if(value != oldValue)
        {
            oldValue = value;
            UpdateTreeManager();
        }

        return true;
    }

    void UpdateTreeManager()
    {
        if(Application.isPlaying)
        {
            TreeManager.instance.UpdateTag(this);
        }
    }
}

[CustomEditor(typeof(TagNode))]
public class TagNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int index = ((TagNode)target).tagIndex;

        EditorGUILayout.BeginVertical();
        index = EditorGUILayout.Popup("Tag", index, Tag.values.ToArray(), EditorStyles.popup);
        EditorGUILayout.EndVertical();

        ((TagNode)target).tagIndex = index;
    }
}