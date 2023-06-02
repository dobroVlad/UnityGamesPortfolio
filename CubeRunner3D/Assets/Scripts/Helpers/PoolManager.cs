using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoolManager
{
    private static Transform _parentForObject;
    private static Dictionary<string, LinkedList<GameObject>> _pooledObject =
        new Dictionary<string, LinkedList<GameObject>>();

    public static void PutObject(GameObject gameObject)
    {
        gameObject.SetActive(false);

        if (_pooledObject.ContainsKey(gameObject.name))
        {
            _pooledObject[gameObject.name].AddLast(gameObject);
        }
        else
        {
            _pooledObject[gameObject.name] = new LinkedList<GameObject>();
            _pooledObject[gameObject.name].AddLast(gameObject);
        }
        gameObject.transform.SetParent(_parentForObject, !gameObject.TryGetComponent<RectTransform>(out var rt));
    }
    public static GameObject GetObject(GameObject gameObject)
    {
        if (_pooledObject.ContainsKey(gameObject.name))
        {
            if (_pooledObject[gameObject.name].Count > 0)
            {
                var result = _pooledObject[gameObject.name].Last;
                _pooledObject[gameObject.name].RemoveLast();
                return result.Value;
            }
            else
            {
                return InstantiateObject(gameObject);
            }
        }
        else
        {
            _pooledObject[gameObject.name] = new LinkedList<GameObject>();
            return InstantiateObject(gameObject);
        }
    }
    public static void InitializePool(GameObject gameObjectPrefab, int count)
    {
        _pooledObject = new Dictionary<string, LinkedList<GameObject>>();
        for (int i = 0; i < count; i++)
        {
            var gameObject = InstantiateObject(gameObjectPrefab);
            PutObject(gameObject);
        }
    }
    public static void SetParentForObject(Transform parent)
    {
        _parentForObject = parent;
    }
    private static GameObject InstantiateObject(GameObject gameObject)
    {
        var result = GameObject.Instantiate(gameObject, _parentForObject);
        result.name = gameObject.name;
        return result;
    }
}
