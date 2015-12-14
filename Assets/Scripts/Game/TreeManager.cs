using NodeEditorFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TreeManager : MonoBehaviour
{
    public static TreeManager instance;

    public Dictionary<string, bool> tags;
    public Dictionary<string, bool> patterns;

    public bool showNodeEditor;

    [HideInInspector]
    public List<TagNode> tagNodes;
    [HideInInspector]
    public List<PatternNode> patternNodes;
    [HideInInspector]
    public List<MasteryNode> masteryNodes;

    private RuntimeNodeEditor nodeEditor;
    private NodeCanvas nodeCanvas;
    private bool canvasStart;

    public Dropdown assetBrowser;

    float delay = .5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        nodeEditor = GetComponent<RuntimeNodeEditor>();

        InitializeAssetBrowser();
    }

    private string rootPath = NodeEditor.editorPath + "Resources/Saves";

    void InitializeAssetBrowser()
    {
        string path = rootPath;
        path = path.Split('.')[0];
        path = path.Split(new string[] { "Resources/" }, StringSplitOptions.None)[1];
        NodeCanvas[] objects = Resources.LoadAll<NodeCanvas> (path);

        assetBrowser.options = new List<Dropdown.OptionData>();

        assetBrowser.options.Add(new Dropdown.OptionData("<none>"));
        foreach (NodeCanvas obj in objects)
        {
            assetBrowser.options.Add(new Dropdown.OptionData(obj.name));
        }

        assetBrowser.value = 0;
        OnValueChanged();
    }

    public void OnValueChanged()
    {
        StopAllCoroutines();

        if(assetBrowser.value == 0)
        {
            tags = null;
            tagNodes = new List<TagNode>(); ;
            patterns = null;
            patternNodes = new List<PatternNode>();
            masteryNodes = null;
            nodeEditor.canvas = null;
        }
        else
        {
            InitializeTags();
            InitializePatterns();
            InitializeMasteries();
            StartCoroutine(ReadTree());

            nodeEditor.CanvasString = rootPath + "/" + assetBrowser.options[assetBrowser.value].text + ".asset";
            nodeEditor.LoadNodeCanvas(nodeEditor.CanvasString);
        }

        canvasStart = false;
        assetBrowser.captionText.text = assetBrowser.options[assetBrowser.value].text;
    }

    void Update()
    {
        if(!canvasStart)
        {
            canvasStart = true;
            nodeCanvas = nodeEditor.canvas;
        }

        if (nodeCanvas != null)
        {
            NodeEditor.RecalculateAll(nodeCanvas);
        }

        assetBrowser.gameObject.SetActive(GameManager.instance.debugMode);
    }

    #region Tags
    private void InitializeTags()
    {
        tags = new Dictionary<string, bool>();

        foreach (string tag in Tag.values)
        {
            tags.Add(tag, false);
        }
    }

    public bool IsMechanicEnabled(string tag)
    {
        if(tags == null)
        {
            return true;
        }

        if(!tags[tag])
        {
            Debug.Log("Unresolved dependencies for tag: " + tag);
        }

        return tags[tag];
    }

    public void UpdateTag(TagNode node)
    {
        if (tags != null && node.tag != Tag.None)
        {
            tags[node.tag] = node.value;
        }
    }

    public void ReadTagNodes()
    {
        foreach (TagNode node in tagNodes)
        {
            UpdateTag(node);
        }
    }
    #endregion

    #region Patterns
    private void InitializePatterns()
    {
        patterns = new Dictionary<string, bool>();

        foreach (string pattern in PatternManager.instance.patternsName)
        {
            patterns.Add(pattern, false);
        }
    }

    public bool IsPatternEnabled(string pattern)
    {
        if(patterns == null)
        {
            return true;
        }

        return patterns[pattern];
    }

    public void UpdatePattern(PatternNode node)
    {
        patterns[node.pattern] = node.value;
    }

    public void ReadPatternNodes()
    {
        foreach (PatternNode node in patternNodes)
        {
            UpdatePattern(node);
        }
    }
    #endregion

    #region Mastery
    private void InitializeMasteries()
    {
        masteryNodes = new List<MasteryNode>();
    }

    public void UpdateMastery(MasteryNode node)
    {
        if(node.tagNode == null && node.patternNode == null)
        {
            return;
        }

        if(node.tagNode != null)
        {
            UpdateTag(node.tagNode);
        }
        else if(node.patternNode != null)
        {
            UpdatePattern(node.patternNode);
        }
    }

    public void AddMasteryNode(MasteryNode node)
    {
        if (!masteryNodes.Contains(node))
        {
            masteryNodes.Add(node);
        }
    }

    public bool IsMasteryResolved(MasteryNode masteryNode, PatternNode patternNode)
    {
        return PatternManager.instance.patternsInfo[patternNode.pattern].GetMastery() == masteryNode.mastery;
    }

    public bool IsMasteryResolved(MasteryNode masteryNode, TagNode tagNode)
    {
        return true;
    }
    #endregion

    IEnumerator ReadTree()
    {
        ReadTagNodes();
        ReadPatternNodes();

        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(ReadTree());
    }

    public void ToogleNodeEditor()
    {
        showNodeEditor = !showNodeEditor;

        if (showNodeEditor)
        {
            GameManager.instance.SetPause(true);
        }
    }

    void OnGUI()
    {
        if (nodeEditor != null)
        {
            nodeEditor.enabled = showNodeEditor;
        }
    }
}
