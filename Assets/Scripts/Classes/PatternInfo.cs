using System.Collections.Generic;

public class PatternInfo
{
    public string name;
    public List<List<string>> tags;
    public List<List<Dependency>> dependencies;

    public List<int> attempts;

    public PatternInfo(string name)
    {
        this.name = name;
        this.attempts = new List<int>(PatternManager.instance.savedAttempts);
        this.tags = new List<List<string>>();
        this.dependencies = new List<List<Dependency>>();
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

    public void AddTags(params string[] newTags)
    {
        tags.Add(new List<string>(newTags));
    }

    public void AddDependencies(params Dependency[] newDependencies)
    {
        dependencies.Add(new List<Dependency>(newDependencies));
    }
}
