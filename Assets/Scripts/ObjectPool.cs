using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public int poolSize;

    private static ObjectPool _instance;

    public static ObjectPool Instance { get { return _instance; } }

    Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    [Header("Prefabs")]
    public GameObject dronePrefab;
    public GameObject laserMissilePrefab;
    public GameObject droneExplosionPrefab;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

    private void Start()
    {
        CreateStartupPools();
    }

    void CreateStartupPools()
    {
        CreatePool(dronePrefab, poolSize);
        CreatePool(laserMissilePrefab, poolSize*4);
        CreatePool(droneExplosionPrefab, poolSize);
    }

    void CreatePool(GameObject prefab, int newSize)
    {
		if (prefab != null && !_instance.pooledObjects.ContainsKey(prefab))
		{
			var list = new List<GameObject>();
			_instance.pooledObjects.Add(prefab, list);

			if (newSize > 0)
			{
				bool active = prefab.activeSelf;
				prefab.SetActive(false);
				Transform parent = _instance.transform;
				while (list.Count < newSize)
				{
					var obj = (GameObject)Object.Instantiate(prefab);
                    obj.transform.parent = _instance.transform;

					list.Add(obj);
				}
				prefab.SetActive(active);
			}
		}
    }

	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		List<GameObject> list;
		Transform trans;
		GameObject obj;
		if (_instance.pooledObjects.TryGetValue(prefab, out list))
		{
			obj = null;
			if (list.Count > 0)
			{
				while (obj == null && list.Count > 0)
				{
					obj = list[0];
					list.RemoveAt(0);
				}
				if (obj != null)
				{
					trans = obj.transform;
                    trans.parent = parent;
					trans.position = position;
					trans.localRotation = rotation;
					obj.SetActive(true);
					_instance.spawnedObjects.Add(obj, prefab);
					return obj;
				}
			}
			obj = (GameObject)Object.Instantiate(prefab);
			trans = obj.transform;
            trans.parent = parent;
			trans.position = position;
			trans.localRotation = rotation;
			_instance.spawnedObjects.Add(obj, prefab);
			return obj;
		}
		else
		{
			obj = (GameObject)Object.Instantiate(prefab);
			trans = obj.GetComponent<Transform>();
			trans.parent = parent;
			trans.position = position;
			trans.localRotation = rotation;
			return obj;
		}
	}

	public static void Recycle(GameObject obj)
	{
		GameObject prefab;
		if (_instance.spawnedObjects.TryGetValue(obj, out prefab))
			Recycle(obj, prefab);
		else
			Object.Destroy(obj);
	}

	static void Recycle(GameObject obj, GameObject prefab)
	{
		_instance.pooledObjects[prefab].Add(obj);
		_instance.spawnedObjects.Remove(obj);
        obj.transform.parent = _instance.transform;
		obj.SetActive(false);
	}

	//public static void RecycleAll(GameObject prefab)
	//{
	//	foreach (var item in instance.spawnedObjects)
	//		if (item.Value == prefab)
	//			tempList.Add(item.Key);
	//	for (int i = 0; i < tempList.Count; ++i)
	//		Recycle(tempList[i]);
	//	tempList.Clear();
	//}


}
