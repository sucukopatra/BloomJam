using System;
using System.Collections.Generic;
using BloomJam.Player;
using UnityEngine;

public class healt : MonoBehaviour
{
   public List<GameObject> xxxxx = new List<GameObject>();
   private GameObject playerGo;
   private void Awake()
   {
       playerGo = GameObject.FindGameObjectWithTag("Player");

   }

   private void Update()
   {
       float y = playerGo.GetComponent<PlayerHealth>().CurrentHealth;

      if (y == 100)
      {
          xxxxx[0].SetActive(true);
       
      }
      else
      {
          xxxxx[0].SetActive(false);
          if (y >75)
          {
              xxxxx[1].SetActive(true);
       
          }
          else
          {
              xxxxx[1].SetActive(false);
              if (y >50)
              {
                  xxxxx[2].SetActive(true);
       
              }
              else
              {
                  xxxxx[2].SetActive(false);
                  if (y >25)
                  {
                      xxxxx[3].SetActive(true);
       
                  }
                  else
                  {
                      if (0 >=y)
                      {
                          xxxxx[3].SetActive(false);
                      }
                      else
                      {
                   
          
                      }
                  }
                  
              }
          }
      }
       
   }
}
