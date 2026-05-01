using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class final_wall : MonoBehaviour
{
   
  List<bool> boollll = new List<bool>();
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
        foreach (var v in boollll)
        {
            if (v == false)
            {
                a = false;
            }
        }


        if (a==true)
        {
            Destroy(gameObject);
        }
        
        
        
        
        
    }
    
    
}
