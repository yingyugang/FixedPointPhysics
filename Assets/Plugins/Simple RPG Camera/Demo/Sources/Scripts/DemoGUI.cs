using UnityEngine;
using System.Collections;

public class DemoGUI : MonoBehaviour
{
    public string version = "1.0.0";
	public SimpleRpgCamera rpgCamera;
	public SimpleRpgPlayerController rpgPlayer;

	private Rect _window_rect;

	void Start()
	{
		_window_rect = new Rect(10, 10, 200, 32);
	}

	void OnGUI()
	{
		_window_rect = GUILayout.Window(0, _window_rect, DemoWindow, "Simple RPG Camera Demo");

		if(_window_rect.Contains(Event.current.mousePosition))
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
		rpgCamera.allowRotation = GUILayout.Toggle(rpgCamera.allowRotation, "Allow Rotation");
		rpgCamera.lockCursor = GUILayout.Toggle(rpgCamera.lockCursor, "Lock Cursor");
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
		rpgPlayer.keyboardControl = GUILayout.Toggle(rpgPlayer.keyboardControl, "Keyboard Control");
	}
}