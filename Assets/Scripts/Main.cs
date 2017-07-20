using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {

        DontDestroyOnLoad(this.gameObject);

        if (Application.isEditor)
        {
            var debugHelper = this.gameObject.GetComponent<DebugHelper>();
            if (debugHelper == null)
            {
                this.gameObject.AddComponent<DebugHelper>();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
