using System.Collections.Generic;

public class PatternInfo
{
    public string name;

    public List<int> attempts;

    public PatternInfo(string name)
    {
        this.name = name;
        this.attempts = new List<int>(PatternManager.instance.savedAttempts);
    }

    public float GetScore()
    {
        float score = 0;
        float accumulator = 0;

        for(int i = 0; i < attempts.Count; i++)
        {
            int index = i;

            if(attempts.Count < attempts.Capacity)
            {
                index = i;
            }
            float weight = PatternManager.instance.attemptsWeights[index];

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

    public void AddAttempt(bool success)
    {
        if(attempts.Count == PatternManager.instance.savedAttempts)
        {
            attempts.RemoveAt(0);
        }

        attempts.Add(success ? 1 : 0);
    }
}
