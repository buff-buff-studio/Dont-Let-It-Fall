using UnityEngine;

public class UIBox : MonoBehaviour
{
	public RectTransform fillImage;

	public float speed = 15f;

	private void Update()
	{
		float num = Time.time * speed % 16f;
		fillImage.offsetMin = new Vector2(num - 16f, fillImage.offsetMin.y);
	}
}
