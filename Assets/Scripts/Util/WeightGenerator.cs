/*
	This class is used to generate the respective weights 
	when the user presses the %, Lin, Qua or Log buttons.
*/

using UnityEngine;

public static class WeightGenerator
{
    public static float[] Quadratic(int size, float start)
    {
        float[] weights = new float[size];

        weights[0] = start;
        for(int i = 1; i < size; i++)
        {
            weights[i] = weights[i-1] * weights[i-1];
        }

        return weights;
    }

    public static float[] Logarithmic(int size, float b)
    {
        float[] weights = new float[size];

        for(int i = 0; i < size; i++)
        {
            weights[i] = Mathf.Max(Mathf.Log(i+1, b), 0.01f);
        }

        return weights;
    }

    public static float[] Linear(int size, int repetition)
    {
        float[] weights = new float[size];

        int value = 1;

        for(int i = 0; i < size; i += repetition, value++)
        {
            for(int r = 0; r < repetition; r++)
            {
                if(i+r < size)
                {
                    weights[i+r] = value;
                }
            }
        }

        return weights;
    }

    public static float[] Percentage(int size)
    {
        float[] weights = new float[size];

        for(int i = 0; i < size; i++)
        {
            weights[i] = 1;
        }

        return weights;
    }
}
