// var 1.0.0
// takashicompany.com

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class GUIConsole : MonoBehaviour {

	public enum LogFilter
	{
		All = 0,
		Log = 1,
		Warning = 2,
		Error = 3
	}

	internal class LogData
	{
		public readonly string condition;
		public readonly string stackTrace;
		public readonly LogType type;

		public LogData  (string condition, string stackTrace, LogType type)
		{
			this.condition = condition;
			this.stackTrace	= stackTrace;
			this.type = type;
		}

		public override string ToString ()
		{
			return string.Format ("Type:{0} \nCondition:{1} \nStackTrace:{2}",type.ToString(),condition,stackTrace);
		}
	}

	[SerializeField]
	private Vector2 _consoleSize = new Vector2(400,300);
	
	private Vector2 _windowCenterPos = new Vector2(300,200);

	private Rect _consoleRect
	{
		get
		{

			float width =  Screen.width < _consoleSize.x ? Screen.width : _consoleSize.x;
			float height = Screen.height < _consoleSize.y ? Screen.height : _consoleSize.y;

			return new Rect(
				_windowCenterPos.x - width / 2,
				_windowCenterPos.y - height / 2,
				width,
				height
			);
		}
		set
		{
			_windowCenterPos.x = value.center.x;
			_windowCenterPos.y = value.center.y;
			_consoleSize.x = value.width;
			_consoleSize.y = value.height;
		}
	}

	private int _scrollHeight
	{
		get{
			return ((int)_consoleRect.height - 100) / 2;	// 100px : Header & Footer,etc...

		}
	}

	[SerializeField]
	private int _maxLogCount = 1000;

	public bool isShow{get; private set;}

	private Vector2 _listScrollPosition;
	private Vector2 _detailScrollPosition;
	
	private bool _autoListScroll = true;

	private Queue<LogData> _logQueue;
	private LogData _selectedLogData;
	private LogFilter _filter;

	private Rect _listScrollRect;

	private Rect _detailScrollRect;
	
	private bool isMouseClick;
	private Vector3 _prevMousePos;


	void Awake ()
	{
		_logQueue = new Queue<LogData>();
		_windowCenterPos = new Vector2(Screen.width / 2,Screen.height / 2);
	}

	void Update ()
	{
		// for PC and Editor
		if(Input.GetMouseButton(0))
		{
			if(isMouseClick)
			{
				Vector2 offset = Input.mousePosition - _prevMousePos;
				ScrollList(GetReverseInputPosition(Input.mousePosition),offset);
				ScrollDetail(GetReverseInputPosition(Input.mousePosition),offset);
			}
			else
			{
				isMouseClick = true;
			}
			_prevMousePos = Input.mousePosition;
		}else{
			isMouseClick = false;
		}


		// for Mobile Device.
		if (0 < Input.touchCount)
		{
			Touch touch = Input.touches[0];

			if (touch.phase == TouchPhase.Moved)
			{

				ScrollList(GetReverseInputPosition(touch.position),touch.deltaPosition);
				ScrollDetail(GetReverseInputPosition(touch.position),touch.deltaPosition);
			}
		}
	}

	void OnEnable ()
	{
		Application.RegisterLogCallback(LogCallBack);
	}

	void OnDisable ()
	{
		Application.RegisterLogCallback(null);
	}

	public void Show ()
	{
		_autoListScroll = true;
		isShow = true;
	}

	public void Hide ()
	{
		isShow = false;
	}

	void ScrollList(Vector2 touchPosition, Vector2 offset)
	{

		Rect rect = new Rect(
			_consoleRect.x + _listScrollRect.x,
			_consoleRect.y + _listScrollRect.y,
			_listScrollRect.width,
			_listScrollRect.height);

		if (rect.Contains(touchPosition))
		{
			_autoListScroll = false;
			_listScrollPosition += offset;
		}
	}

	void ScrollDetail(Vector2 touchPosition, Vector2 offset)
	{
		Rect rect = new Rect(
			_consoleRect.x + _detailScrollRect.x,
			_consoleRect.y + _detailScrollRect.y,
			_detailScrollRect.width,
			_detailScrollRect.height);

		if (rect.Contains(touchPosition))
		{
			_detailScrollPosition += offset;
		}
	}

	Vector2 GetReverseInputPosition(Vector2 position)
	{
		return new Vector2(position.x,Screen.height - position.y);
	}


	void LogCallBack (string condition, string stackTrace, LogType type)
	{
		stackTrace = "";
		System.Diagnostics.StackTrace systemStackTrace = new System.Diagnostics.StackTrace(true);
		stackTrace = systemStackTrace.ToString();
		AddLog(new LogData(condition, stackTrace, type));
	}

	void AddLog (LogData item)
	{
		_logQueue.Enqueue(item);
		while (0 < _maxLogCount && _maxLogCount <= _logQueue.Count)
		{
			_logQueue.Dequeue();
		}
	}

	void OnGUI ()
	{
		if(isShow)
		{
			_consoleRect = GUILayout.Window(19890611, _consoleRect,DrawConsoleWindow,"Console (Move by dragging here.)",GUILayout.MaxWidth(_consoleRect.width));
		}
	}

	void DrawConsoleWindow(int id)
	{
		Color defaultContentColor = GUI.contentColor;

		GUIStyle buttonStyle = new GUIStyle("button");
		buttonStyle.alignment = TextAnchor.MiddleLeft;
		buttonStyle.fixedHeight = 30;
		buttonStyle.margin = new RectOffset(0,0,0,0);

		GUI.contentColor = Color.white;

		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Logs");

		GUILayout.Space(20);

		_filter = (LogFilter)GUILayout.SelectionGrid((int)_filter,Enum.GetNames(typeof(LogFilter)),Enum.GetValues(typeof(LogFilter)).Length);

		GUILayout.EndHorizontal();

		List<LogData> dataList = new List<LogData>(_logQueue.ToArray()).FindAll(m => _filter == LogFilter.All || m.type.GetFilter() == _filter);

		int listItemHeight = (int)buttonStyle.fixedHeight;	// TODO Use Margin.
		int listScrollBottomPos = (listItemHeight * dataList.Count) - (int)_listScrollRect.height;

		Vector2 scrollPos = new Vector2(_listScrollPosition.x, _autoListScroll ? listScrollBottomPos :_listScrollPosition.y);
		_listScrollPosition = GUILayout.BeginScrollView(scrollPos,false,true,GUILayout.MaxHeight(_scrollHeight));

		if(listScrollBottomPos <= _listScrollPosition.y)
		{
			_autoListScroll = true;
		}		

		GUILayout.Box(GUIContent.none,GUIStyle.none,GUILayout.ExpandWidth(true),GUILayout.Height(0));
		Rect listRect =  GUILayoutUtility.GetLastRect();

		GUI.contentColor = defaultContentColor;

		foreach(LogData data in dataList)
		{
			GUI.contentColor = data.type.GetFilter().GetContentColor();

			if (GUILayout.Button(data.condition,buttonStyle,GUILayout.Width(listRect.width),GUILayout.ExpandWidth(true)))
			{
				_selectedLogData = data;
			}

			GUI.contentColor = defaultContentColor;
		}

		GUILayout.EndScrollView();

		Rect listLastRect = GUILayoutUtility.GetLastRect();
		if(!(listLastRect.x == 0 && listLastRect.y == 0 && listLastRect.width == 1 && listLastRect.height == 1))
		{
			_listScrollRect = GUILayoutUtility.GetLastRect();
		}

		GUILayout.Space(10);
		GUILayout.Label("Detail");
		_detailScrollPosition = GUILayout.BeginScrollView(_detailScrollPosition,false,true,GUILayout.MaxHeight(_scrollHeight));
		if (_selectedLogData != null)
		{
			GUI.contentColor = _selectedLogData.type.GetFilter().GetContentColor();

			GUILayout.Label(_selectedLogData.condition + "\n" + _selectedLogData.stackTrace);

			GUI.contentColor = defaultContentColor;
		}

		GUILayout.EndScrollView();

		_detailScrollRect = GUILayoutUtility.GetLastRect();


		GUILayout.Space(10);

		// Export Menu
		GUILayout.BeginHorizontal();

		GUI.contentColor = defaultContentColor;

		if (_selectedLogData == null)
		{
			GUI.contentColor = Color.gray;
			GUILayout.Button("Log Export (Select a Log!)");
		}
		else
		{
			if (GUILayout.Button("Log Export"))
			{
				ExportLog(_selectedLogData);
			}
		}

		GUI.contentColor = defaultContentColor;

		GUILayout.EndHorizontal();

		// Clear & Close
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Clear"))
		{
			_selectedLogData = null;
			_logQueue = new Queue<LogData>();
		}

		if(GUILayout.Button("Close"))
		{
			isShow = false;
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
		GUI.DragWindow(new Rect(0,0,_consoleRect.width,30));

		GUI.contentColor = defaultContentColor;
	}

	private void ExportLog (LogData log)
	{
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//Instead of the Clipboard
			string str = WWW.EscapeURL(_selectedLogData.ToString());
			Application.OpenURL("mailto:?subject=&body=" + str);
		}
		else
		{
			TextEditor te = new TextEditor();
			te.content = new GUIContent(log.ToString());
			te.SelectAll();
			te.Copy();
		}
	}
}


internal static class LogTypeExtension
{
	internal static GUIConsole.LogFilter GetFilter(this LogType self)
	{
		switch(self)
		{
			case LogType.Assert:
				return GUIConsole.LogFilter.Error;
			case LogType.Error:
				return GUIConsole.LogFilter.Error;
			case LogType.Exception:
				return GUIConsole.LogFilter.Error;
			case LogType.Log:
				return GUIConsole.LogFilter.Log;
			case LogType.Warning:
				return GUIConsole.LogFilter.Warning;
		}

		return GUIConsole.LogFilter.All;
	}
}

internal static class LogFilterExtension
{
	internal static Color GetContentColor (this GUIConsole.LogFilter self)
	{
		switch(self)
		{
			case GUIConsole.LogFilter.Log:
				return Color.white;
			case GUIConsole.LogFilter.Warning:
				return Color.yellow;
			case GUIConsole.LogFilter.Error:
				return Color.red;
		}

		return Color.white;
	}
}