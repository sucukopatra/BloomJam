using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class final_wall : MonoBehaviour
{
    void Start()
    {
        
    }

    public GameObject enemi;
    // Update is called once per frame
    void Update()
    {
        bool a=false;
        bool hasChild = enemi.transform.childCount > 0;
        if (hasChild==false)
        {
            Destroy(gameObject); }
    
        
        
        
        
    }
    
    
}
