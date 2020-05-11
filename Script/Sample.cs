using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Sample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var app = new ProcessStartInfo();
        app.FileName = "notepad";
        app.Arguments = "memo.txt";
        Process.Start(app);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
