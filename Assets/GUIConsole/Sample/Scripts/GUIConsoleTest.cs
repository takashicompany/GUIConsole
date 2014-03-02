using UnityEngine;
using System;
using System.Collections;

public class GUIConsoleTest : MonoBehaviour {

	private int _debugCount = 0;

	void OnGUI ()
	{
		if (GUI.Button(new Rect(Screen.width - 100,0,100,100),"Log"))
		{
			Debug.Log("Log:"  + _debugCount);
			_debugCount++;
		}

		if (GUI.Button(new Rect(Screen.width - 100,100,100,100),"Warning"))
		{
			Debug.LogWarning("Log:"  + _debugCount);
			_debugCount++;
		}


		if (GUI.Button(new Rect(Screen.width - 100,200,100,100),"Error"))
		{
			Debug.LogError("Log:"  + _debugCount);
			_debugCount++;
		}

		if (GUI.Button(new Rect(Screen.width - 100,300,100,100),"Exception"))
		{
			Debug.LogException(new Exception());
			_debugCount++;
		}
	}
}
