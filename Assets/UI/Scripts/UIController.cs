using UnityEngine;

public class UIController
{
	public static bool ControllingViaScreenCursor = true;

	public static Vector3 ScreenCursorPos = new Vector3(0f, 0f, 0f);

	public static bool ShowingScreenController = true;

	public static void RestartScreenCursorPosition()
	{
		ScreenCursorPos = new Vector2(Screen.width / 2, Screen.height / 2);
	}

	public static Vector3 GetCursorPosition()
	{
		if (!ControllingViaScreenCursor)
		{
			return Input.mousePosition;
		}
		return ScreenCursorPos;
	}
}
