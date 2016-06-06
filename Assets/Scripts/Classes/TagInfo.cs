/*
	This class manages the window of attempts and mastery for each Mechanic
*/

using System.Collections.Generic;

public class TagInfo
{
    public string name;
    public List<int> attempts;

    public TagInfo(string name)
    {
        this.name = name;
        this.attempts = new List<int>(TagsManager.instance.attemptsCount);
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
            float weight = TagsManager.instance.attemptsWeights[index];

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
        return Mastery.FromAttempts(GetScore(), attempts.Count);
    }

    public void AddAttempt(bool success)
    {
        if (attempts.Count == TagsManager.instance.attemptsCount)
        {
            attempts.RemoveAt(0);
        }

        attempts.Add(success ? 1 : 0);
    }
}
