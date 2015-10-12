using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternInfo {

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

        for(int index = 0; index < attempts.Count; index++)
        {
            int weight = PatternManager.instance.attemptsWeights[index];

            accumulator += weight;
            score += attempts[index] * weight;
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
