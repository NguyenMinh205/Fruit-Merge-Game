using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoolingWithList 
{
    [SerializeField] private static List<GameObject> pools = new List<GameObject>();
     
     public static GameObject Spawn(GameObject gameObject, Vector3 position, Quaternion quaternion, Transform parent = null)
     {
         GameObject newObject = null;
         foreach (var gObject in pools)
         {
             if (gObject.name.Equals(gameObject.name))
             {
                 newObject = gObject;
                 pools.Remove(gObject);
                 break;
             }
         }

         if (newObject == null)
         {
             newObject = GameObject.Instantiate(gameObject, position, quaternion, parent);
             newObject.name = gameObject.name;
             return newObject;
         } 
         newObject.transform.SetPositionAndRotation(position,quaternion);
         newObject.transform.SetParent(parent);
         newObject.name = gameObject.name;
         newObject.SetActive(true);
         return newObject;
     }
     public static void Despawn(GameObject gameObject)
     {
         gameObject.SetActive(false);
         pools.Add(gameObject);
     }
}
