using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YigitcanCaliskan.Bootstrap
{
    
    public sealed class BootstrapRunner : MonoBehaviour
    {

        private void Awake()
        {
           
            DontDestroyOnLoad(gameObject);
         
            var sceneServices = GetComponentsInChildren<IBootstrapService>(includeInactive: true);
            foreach (var service in sceneServices)
            {
                service.Register();
            }
        }

        private void Start()
        {
           
            SceneManager.LoadScene(1);
        }
    }
}