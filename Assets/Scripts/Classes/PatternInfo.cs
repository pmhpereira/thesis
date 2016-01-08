using System;
using System.Collections.Generic;

public class PatternInfo
{
    public string name;
    public List<List<string>> tags;
    public List<List<Dependency>> dependencies;
    public List<List<int>> attempts;

    public PatternInfo(string name)
    {
        this.name = name;
        this.tags = new List<List<string>>();
        this.attempts = new List<List<int>>();
        this.dependencies = new List<List<Dependency>>();
    }

    public float GetScore(int tagIndex = 0)
    {
        float score = 0;
        float accumulator = 0;

        List<int> tries = attempts[tagIndex];

        for (int i = 0; i < tries.Count; i++)
        {
            int index = i;

            if(tries.Count < tries.Capacity)
            {
                index = i;
            }
            float weight = PatternManager.instance.attemptsWeights[index];

            accumulator += weight;
            score += tries[i] * weight;
        }

        score /= accumulator;

        if(float.IsNaN(score))
        {
            score = 0;
        }

        return score;
    }

    public string GetScoresString()
    {
        #warning disabled tags
        return Math.Round(GetScore(), 3) + " | " + Mastery.ToId(GetMastery()) + "\n";

        /* UNUSED * /
        string scores = "";

        for(int i = 0; i < tags.Count; i++)
        {
            float score = GetScore(i);
            scores += Math.Round(score, 3) + " | " + Mastery.ToId(Mastery.FromAttempts(score, attempts[i].Count)) + "\n";
        }

        return scores;
        /**/
    }

    public string GetMastery(int tagIndex = 0)
    {
        return Mastery.FromAttempts(GetScore(tagIndex), attempts[tagIndex].Count);
    }

    /* UNUSED * /
    public void AddAttempt(bool success, PlayerState[] playerStates)
    {
        int tagIndex = ResolveTagIndex(playerStates);
        AddAttempt(success, tagIndex);
    }
    /**/

    public void AddAttempt(bool success)
    {
        for(int i = 0; i < tags.Count; i++)
        {
            AddAttempt(success, i);
        }
    }

    public void AddAttempt(bool success, int tagIndex)
    {
        if(tagIndex < 0 || tagIndex >= attempts.Count)
        {
            return;
        }

        List<int> tries = attempts[tagIndex];

        if (tries.Count == PatternManager.instance.attemptsCount)
        {
            tries.RemoveAt(0);
        }

        tries.Add(success ? 1 : 0);
    }

    public void AddTags(params string[] newTags)
    {
        tags.Add(new List<string>(newTags));
        attempts.Add(new List<int>(PatternManager.instance.attemptsCount));
    }

    public void AddDependencies(params Dependency[] newDependencies)
    {
        dependencies.Add(new List<Dependency>(newDependencies));
    }

    /* UNUSED * /
    public int ResolveTagIndex(PlayerState[] playerStates)
    {
        // WARN: disabled tags
        return 0;

        int tagIndex = 0, bestScore = 0;

        for(int t = 0; t < tags.Count; t++)
        {
            List<string> tagList = tags[t];
            int score = 0;

            List<string> playerStateTags = PlayerStateToTags(playerStates);

            foreach(string patternTag in tagList)
            {
                foreach (string playerTag in playerStateTags) {
                    if(playerTag == patternTag)
                        score++;
                }
            }

            if(score > bestScore)
            {
                tagIndex = t;
                bestScore = score;
            }
        }

        return tagIndex;
    }
    /**/
}
