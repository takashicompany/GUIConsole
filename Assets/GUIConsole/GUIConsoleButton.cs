// takashicompany.com

using UnityEngine;
using System.Collections;

public class GUIConsoleButton : MonoBehaviour {

	public GUIConsole console;

	[SerializeField]
	private Rect _buttonRect = new Rect(0f,0f,100f,100f);

	void Awake ()
	{
		if (console == null)
		{
			console = GetComponent<GUIConsole>();
		}
	}

	void OnGUI ()
	{
		if (console != null)
		{
			string label = string.Format("Console:{0}",console.isShow);
			if (GUI.Button(_buttonRect,label))
			{
				if(console.isShow)
				{
					console.Hide();
				}
				else
				{
					console.Show();
				}
			}

		}
	}

}
