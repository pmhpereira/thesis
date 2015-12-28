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

    [HideInInspector]
    public List<TagNode> tagNodes;
    [HideInInspector]
    public List<PatternNode> patternNodes;
    [HideInInspector]
    public List<MasteryNode> masteryNodes;
    [HideInInspector]
    public List<PaceNode> paceNodes;

    private RuntimeNodeEditor nodeEditor;
    private NodeCanvas nodeCanvas;
    private bool canvasStart;

    public Dropdown assetBrowser;
    
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
            InitializePaces();

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

    public string GetTagMastery(string pattern)
    {
        return Mastery.MASTERED;
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
        if(patterns != null)
        {
            patterns[node.pattern] = node.value;
        }
    }

    public string GetPatternMastery(string pattern)
    {
        return PatternManager.instance.patternsInfo[pattern].GetMastery();
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
        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], PatternManager.instance.patternsInfo[patternNode.pattern].GetMastery(), masteryNode.mastery);
    }

    public bool IsMasteryResolved(MasteryNode masteryNode, TagNode tagNode)
    {
        // TODO: implement IsMasteryResolved
        return true;
    }
    #endregion

    #region Pace
    private void InitializePaces()
    {
        paceNodes = new List<PaceNode>();
    }

    public void UpdatePaceNode(PaceNode paceNode)
    {
        int index = paceNodes.IndexOf(paceNode);

        if(index < 0)
        {
            paceNodes.Add(paceNode);
        }
        else
        {
            paceNodes[index] = paceNode;
        }
    }

    public PaceNode GetRandomActivePaceNode()
    {
        List<PaceNode> nodes = new List<PaceNode>();
        foreach(PaceNode paceNode in paceNodes)
        {
            if(paceNode.value)
            {
                nodes.Add(paceNode);
            }
        }

        if(nodes.Count == 0)
        {
            return null;
        }

        return nodes[UnityEngine.Random.Range(0, nodes.Count)];
    }
    #endregion

    public void ToogleNodeEditor()
    {
        nodeEditor.ToogleSplitscreen();
    }
}
