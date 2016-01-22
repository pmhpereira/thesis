using System;
using System.Collections.Generic;

public class PaceInfo
{
    public string name;
    public int instancesCount;
    public List<int> attempts;

    public PaceInfo(string name, int instancesCount)
    {
        this.name = name;
        this.instancesCount = instancesCount;
        this.attempts = new List<int>(PaceManager.instance.attemptsCount * instancesCount);
    }

    public float GetScore()
    {
        float score = 0;
        float accumulator = 0;

        for (int i = 0; i < attempts.Count; i++)
        {
            int index = i;

            if(attempts.Count < attempts.Capacity)
            {
                index = i;
            }
            float weight = PaceManager.instance.attemptsWeights[index / instancesCount];

            accumulator += weight;
            score += attempts[i] * weight;
        }

        score /= accumulator;

        if(float.IsNaN(score))
        {
            score = 0;
        }

        return score;
    }

    public string GetMastery()
    {
        return Mastery.FromAttempts(GetScore(), attempts.Count / instancesCount);
    }

    public void AddAttempt(bool success)
    {
        for(int i = 0; i < instancesCount; i++)
        {
            if (attempts.Count == PaceManager.instance.attemptsCount * instancesCount)
            {
                attempts.RemoveAt(0);
            }

            attempts.Add(success ? 1 : 0);
        }
    }
}
