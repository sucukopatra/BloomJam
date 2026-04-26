using System;
using UnityEngine;

public class wall_basic_material : MonoBehaviour
{
    public Material m;

    private void Start()
    {
        transform.GetChild(0).GetComponent<Renderer>().material = m;
        transform.GetChild(1).GetComponent<Renderer>().material = m;
        transform.GetChild(2).GetComponent<Renderer>().material = m;
        transform.GetChild(3).GetComponent<Renderer>().material = m;
    }
}
