using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public int _Seed = 42;

	float random ()
	{
        _Seed = (51769 * _Seed * 34033) % 32911;
        return ((float)_Seed) / 32911.0f;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(_Seed + ": " + random());
        }
	}
}
