using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloomJam;
using UnityEngine;
using UnityEngine.SceneManagement;
using YigitcanCaliskan.ServiceLocator;

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

            ServiceLocator.ServiceLocator.Get<ISceneService>().LoadMainMenu();
        }
    }
}