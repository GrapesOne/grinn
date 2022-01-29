using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class Pool  {

	private static Dictionary<string , LinkedList<GameObject>> poolsDictionary;
	private static Transform deactivatedObjectsParent;
	private static MonoBehaviour holderBehaviour;

	public static void _Init ( Transform pooledObjectsContainer)
	{
		deactivatedObjectsParent = pooledObjectsContainer;
		holderBehaviour = deactivatedObjectsParent.GetComponent<MonoBehaviour>();
		poolsDictionary = new Dictionary<string, LinkedList<GameObject>> ();
	}

	public static GameObject GetGameObject(GameObject prefab, Transform parent  = null)
	{
		if (!poolsDictionary.ContainsKey (prefab.name)) poolsDictionary[prefab.name] = new LinkedList<GameObject>();

		GameObject result;
		if (parent == null) parent = deactivatedObjectsParent;
		if (poolsDictionary [prefab.name].Count > 0) 
		{
			result = poolsDictionary [prefab.name].First.Value;
			poolsDictionary [prefab.name].RemoveFirst ();
			result.transform.SetParent(parent);
			
			return result;
		}

		result = Object.Instantiate (prefab, parent);
		result.SetActive (false);
		result.name = prefab.name;
		return result;
	}

	private static CancellationTokenSource tokenSource = new CancellationTokenSource();

	public static void CallPutChildes(Transform parent, int count)
	{
		holderBehaviour.StartCoroutine(CallPutChildesCoroutine(parent, count));
	}
	public static IEnumerator CallPutChildesCoroutine(Transform parent, int count)
	{
		tokenSource.Cancel();
		tokenSource = new CancellationTokenSource();
		yield return holderBehaviour.StartCoroutine(PutChildes(parent.transform, count, tokenSource.Token));
	}
	
	private static IEnumerator PutChildes(Transform parent, int count, CancellationToken token)
	{
		while (parent.childCount != 0)
		{
			if (token.IsCancellationRequested) yield break;
			if (parent.childCount % count == 0) yield return new WaitForEndOfFrame();
			PutGameObject(parent.GetChild(0).gameObject);
		}
	} 
	public static void PutGameObject(GameObject target)
	{
		if (!target) return;
		if (poolsDictionary[target.name] == null) return;
		if (poolsDictionary[target.name].Count >= 100)
		{
			target.transform.SetParent(deactivatedObjectsParent);
			Object.Destroy(target);
			return;
		}
		poolsDictionary[target.name].AddFirst(target);
		target.transform.SetParent(deactivatedObjectsParent);
		target.SetActive(false);
	}
}
