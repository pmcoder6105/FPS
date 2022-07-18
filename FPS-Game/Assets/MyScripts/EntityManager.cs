using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EntityManager : MonoBehaviour
{
    Dictionary<int, GameObject> entities = new Dictionary<int, GameObject>();
    public static EntityManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            Debug.Log("An Instance of EntityManager already exists. Destroying this EntityManager.");
            return;
        }
        Instance = this;
    }

    public void AddEntity(GameObject entityObject, PhotonView entityView)
    {
        int id = entityView.ViewID;

        if (entities.ContainsKey(id))
        {
            Debug.LogWarning("Cannot AddEntity. There is already an entity with ID " + id);
            return;
        }

        entities.Add(entityView.ViewID, entityObject);
    }

    public void RemoveEntity(PhotonView entityView)
    {
        int id = entityView.ViewID;

        if (!entities.ContainsKey(id))
        {
            Debug.LogWarning("Cannot RemoveEntity. There is no entity with ID " + id);
            return;
        }

        entities.Remove(id);
    }

    /// <summary>
    /// Checks for any null entity objects, and removes them.
    /// </summary>
    public void CheckEntities()
    {
        List<int> removedEntities = new List<int>();

        foreach (KeyValuePair<int, GameObject> entity in entities)
        {
            if (entity.Value == null)
            {
                removedEntities.Add(entity.Key);
            }
        }

        for (int i = 0; i < removedEntities.Count; i++)
        {
            entities.Remove(removedEntities[i]);
        }
    }

    public GameObject GetEntity(PhotonView entityView)
    {
        return GetEntity(entityView.ViewID);
    }

    public GameObject GetEntity(int entityViewID)
    {
        if (entities.ContainsKey(entityViewID))
        {
            return entities[entityViewID];
        }
        else
        {
            Debug.Log("Cannot GetEntity. There is no entity with ID " + entityViewID);
            return null;
        }
    }
}