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
        int accumulator = 0;

        for(int i = 0; i < attempts.Count; i++)
        {
            int index = i;

            if(attempts.Count < attempts.Capacity)
            {
                index = attempts.Capacity - i - 1;
            }
            int weight = PatternManager.instance.attemptsWeights[index];

            accumulator += weight;
            score += attempts[i] * weight;
        }

        for (int index = attempts.Count; index < attempts.Capacity; index++)
        {
            accumulator += PatternManager.instance.attemptsWeights[index];
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
