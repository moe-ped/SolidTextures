using UnityEngine;
using System.Collections;

public class Panner : MonoBehaviour 
{
    Renderer Renderer;

    void Start ()
    {
        Renderer = GetComponent<Renderer>();
    }

	// Update is called once per frame
	void Update () 
    {
        Renderer.material.SetFloat("_Time", Time.time);
	}
}
