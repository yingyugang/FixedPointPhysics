using UnityEngine;
using System.Collections;

public class MobileDemoGUI : MonoBehaviour
{
	public GUISkin guiSkin;

    public string version = "1.0.0";
	public MobileSimpleRpgCamera rpgCamera;
	public MobileSimpleRpgPlayerController rpgPlayer;

	private Rect _window_rect;
	private Rect _char_rect;

	void Start()
	{
		_window_rect = new Rect(10, 10, Screen.width / 3f, 32);
		_char_rect = new Rect(Screen.width / 2f - 150, Screen.height, 300, 32);
	}

	void Update()
	{
		_char_rect.x = Mathf.Clamp(_char_rect.x, 0, Screen.width - _char_rect.width);
		_char_rect.y = Mathf.Clamp(_char_rect.y, 0, Screen.height - _char_rect.height - 10);

		rpgPlayer.InputX = 0;
		rpgPlayer.InputY = 0;
	}

	void OnGUI()
	{
		if(GUI.skin != guiSkin)
		{
			GUI.skin = guiSkin;
		}

		_window_rect = GUILayout.Window(0, _window_rect, DemoWindow, "Simple RPG Camera Demo");
		_char_rect = GUILayout.Window(1, _char_rect, CharWindow, "Character Control");

		if(_window_rect.Contains(Event.current.mousePosition) || _char_rect.Contains(Event.current.mousePosition))
		{
			rpgCamera.Controllable = false;
			rpgPlayer.Controllable = false;
		}
		else
		{
			rpgCamera.Controllable = true;
			rpgPlayer.Controllable = true;
		}
	}

	private void DemoWindow(int id)
	{
        GUILayout.Label("v" + version);
		rpgCamera.rotateObjects = GUILayout.Toggle(rpgCamera.rotateObjects, "Rotate Objects");
		rpgCamera.returnToOrigin = GUILayout.Toggle(rpgCamera.returnToOrigin, "Return To Origin");
        rpgCamera.invertX = GUILayout.Toggle(rpgCamera.invertX, "Invert X");
        rpgCamera.invertY = GUILayout.Toggle(rpgCamera.invertY, "Invert Y");
        rpgCamera.stayBehindTarget = GUILayout.Toggle(rpgCamera.stayBehindTarget, "Stay Behind Target");
		rpgCamera.fadeObjects = GUILayout.Toggle(rpgCamera.fadeObjects, "Fade Objects [" + rpgCamera.fadeDistance + "]");
        rpgCamera.fadeDistance = GUILayout.HorizontalSlider(rpgCamera.fadeDistance, rpgCamera.minDistance, rpgCamera.maxDistance);
		GUILayout.Label("Target Offset [X:" + rpgCamera.targetOffset.x.ToString("F2") + ", Y:" + rpgCamera.targetOffset.y.ToString("F2") + "]");
		rpgCamera.targetOffset.x = GUILayout.HorizontalSlider(rpgCamera.targetOffset.x, -2, 2);
		rpgCamera.targetOffset.y = GUILayout.HorizontalSlider(rpgCamera.targetOffset.y, 0, 2);

		GUILayout.Space(12);
		GUILayout.Label("Player:");
		rpgPlayer.clickToMove = GUILayout.Toggle(rpgPlayer.clickToMove, "Click To Move");
	}

	private void CharWindow(int id)
	{
		if(GUILayout.RepeatButton("Forward"))
		{
			rpgPlayer.InputY = 1;
		}

		GUILayout.BeginHorizontal();

		if(GUILayout.RepeatButton("Left"))
		{
			rpgPlayer.InputX = -1;
		}

		if(GUILayout.RepeatButton("Right"))
		{
			rpgPlayer.InputX = 1;
		}

		GUILayout.EndHorizontal();

		if(GUILayout.RepeatButton("Backward"))
		{
			rpgPlayer.InputY = -1;
		}

        GUILayout.Space(24);

        if(GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
	}
}