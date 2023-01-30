using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pool : MonoBehaviour
{
    public List<PoolList> pools = new List<PoolList>();

    public static Pool main;

    private void Awake()
    {
        main = this;
        Init();
    }


    public void Init()
    {
        foreach (var p in pools)
        {
            GameObject parent = new GameObject(p.name + "_parent");
            parent.transform.parent = transform;

            p.parent = parent.transform;


            for (int i = 0; i < p.amount; i++)
            {
                var go = Instantiate(p.prefab, p.parent);

                go.name = p.name + "_" + i.ToString();

                go.SetActive(false);
            }
        }
    }


    public GameObject Get(PoolObjectTypes _type)
    {
        var pool = pools[(int)_type];
        var childGO = pool.parent.GetChild(0).gameObject;

        if (childGO.activeSelf)
        {
            //instantiate a new one
            return Instantiate(pool.prefab);
        }
        else
        {
            childGO.SetActive(true);
            childGO.transform.parent = null;
            return childGO;
        }
    }


    public void Return(GameObject _go, PoolObjectTypes _type)
    {
        var pool = pools[(int)_type];
        _go.SetActive(false);
        _go.transform.parent = pool.parent;
        _go.transform.SetAsFirstSibling();
    }
}


[Serializable]
public class PoolList
{
    public string name;
    public GameObject prefab;
    public PoolObjectTypes poolObjectType = new PoolObjectTypes();
    [HideInInspector]
    public Transform parent;
    public int amount;

    public PoolList()
    {
        
    }

}

public enum PoolObjectTypes
{
    BulletRenderer
}
