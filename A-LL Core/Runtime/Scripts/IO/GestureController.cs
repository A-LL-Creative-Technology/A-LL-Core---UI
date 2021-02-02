using UnityEngine;

public class GestureController : MonoBehaviour
{
	private static GestureController instance;

	public static GestureController GetInstance()
	{
		return instance;
	}

	[HideInInspector] public bool isGestureActive = false;

    private void Awake()
    {
		instance = this;
    }

    public void OnFingerRightSwipe()
	{
		if (!isGestureActive)
			return;

		NavigationController.GetInstance().OnStackClose();
	}
}
