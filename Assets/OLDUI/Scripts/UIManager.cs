using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static GameObject RenderItem(ItemStack itemStack, GameObject prefab, Transform parent, bool popWhenSpawn)
	{
		GameObject obj = Object.Instantiate(prefab, parent);
		obj.transform.GetChild(1).GetComponent<TMP_Text>().text = ((itemStack.amount == 1) ? "" : string.Concat(itemStack.amount));
		obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		UIItemStack component = obj.GetComponent<UIItemStack>();
		component.SetIcon(itemStack.item);
		component.SetDurability(itemStack.HasDurabilityBar(), itemStack.GetDurabilityLevel());
		if (popWhenSpawn)
		{
			component.ShouldPop();
		}
		return obj;
	}
}
