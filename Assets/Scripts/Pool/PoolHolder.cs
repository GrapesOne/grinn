using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHolder : MonoBehaviour {

	void Awake () => Pool._Init (transform);
}
