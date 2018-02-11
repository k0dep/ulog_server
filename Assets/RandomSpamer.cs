using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSpamer : MonoBehaviour
{
    public int Mod = 100;

	void Update ()
    {
        var rnd = Random.Range(0, int.MaxValue);

        if(rnd % Mod != 0)
            return;

        var type = Random.Range(0, 5);

        switch (type)
        {
            case 0:
                Debug.Log("just log " + Time.deltaTime + ", rnd val: " + rnd);
                break;

            case 1:
                Debug.LogAssertion("Assertation! " + Time.deltaTime + ", rnd val: " + rnd);
                break;

            case 2:
                Debug.LogError("ERRRRROOOOORRRRRR!!!!!!1!!1! " + Time.deltaTime + ", rnd val: " + rnd);
                break;

            case 3:
                Debug.LogWarning("Warning. *.* " + Time.deltaTime + ", rnd val: " + rnd);
                break;

            case 4:
                Debug.LogException(new NullReferenceException("not setted value"));
                break;
        }
    }
}
