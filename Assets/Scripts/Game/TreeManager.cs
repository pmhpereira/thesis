using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager instance;

    public Dictionary<string, bool> tags;
    public Dictionary<string, bool> patterns;

    public Dictionary<string, string> tagsMastery;

    public List<TagNode> tagNodes;
    public List<PatternNode> patternNodes;
    public List<TagMasteryNode> tagMasteryNodes;
    public List<PatternMasteryNode> patternMasteryNodes;

    float delay = .5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        InitializeTags();
        InitializePatterns();
        InitializeTagsMastery();
        InitializePatternsMastery();

        StartCoroutine(ReadTree());
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
        if(!tags[tag])
        {
            Debug.Log("Unresolved dependencies for tag: " + tag);
        }

        return tags[tag];
    }

    public void UpdateTag(TagNode node)
    {
        if (node.tag != Tag.None)
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

    #region Tags Mastery
    private void InitializeTagsMastery()
    {
        tagMasteryNodes = new List<TagMasteryNode>();

        tagsMastery = new Dictionary<string, string>();

        foreach (string tag in Tag.values)
        {
            tagsMastery.Add(tag, Mastery.UNEXERCISED);
        }
    }

    public bool IsTagMasteryResolved(TagMasteryNode tagMasteryNode)
    {
        //tagMasteryNode.tag;
        //tagMasteryNode.mastery;

        // TODO: implement IsTagMasteryResolved()
        return true;
    }

    public void AddTagMasteryNode(TagMasteryNode node)
    {
        if(!tagMasteryNodes.Contains(node))
        {
            tagMasteryNodes.Add(node);
        }
    }

    public void UpdateTagMasteryNodes()
    {
        foreach(TagMasteryNode node in tagMasteryNodes)
        {
            node.Calculate();
        }
    }
    #endregion

    #region Patterns Mastery
    private void InitializePatternsMastery()
    {
        patternMasteryNodes = new List<PatternMasteryNode>();
    }

    public bool IsPatternMasteryResolved(PatternMasteryNode patternMasteryNode)
    {
        return PatternManager.instance.patternsInfo[patternMasteryNode.pattern].GetMastery() ==  patternMasteryNode.mastery;
    }

    public void AddPatternMasteryNode(PatternMasteryNode node)
    {
        if (!patternMasteryNodes.Contains(node))
        {
            patternMasteryNodes.Add(node);
        }
    }

    public void UpdatePatternMasteryNodes()
    {
        foreach(PatternMasteryNode node in patternMasteryNodes)
        {
            node.Calculate();
        }
    }

    #endregion

    IEnumerator ReadTree()
    {
        ReadTagNodes();
        ReadPatternNodes();

        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(UpdateTree());
    }

    IEnumerator UpdateTree()
    {
        UpdatePatternMasteryNodes();
        UpdateTagMasteryNodes();

        if(NodeEditorFramework.NodeEditor.Repaint != null)
        {
            NodeEditorFramework.NodeEditor.Repaint();
        }

        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(ReadTree());
    }
}
