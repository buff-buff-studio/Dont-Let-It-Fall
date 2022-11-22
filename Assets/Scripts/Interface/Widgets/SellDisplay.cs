using UnityEngine;
using TMPro;

namespace DLIFR.Interface.Widgets
{
    public class SellDisplay : MonoBehaviour 
    {
        [Header("SETTINGS")]
        public Vector3 worldPosition;
        public float moveUpOffset = 100f;
        public float fadeOutSpeed = 1;

        [Header("REFERENCES")]
        public TMP_Text labelPrice;
        
        private CanvasGroup _canvasGroup;

        private void OnEnable() 
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetPrice(int price)
        {
            labelPrice.text = $"{price}";

            if(price == 0)
            {
                labelPrice.color = Color.red;
            }
            else
            {
                labelPrice.color = new Color(1f, 0.75f, 0f);
            }
        }

        private void Update() 
        {
            float deltaTime = Time.unscaledDeltaTime;
            float alpha = _canvasGroup.alpha;
            _canvasGroup.alpha = alpha -= fadeOutSpeed * deltaTime;

            Vector3 pos = Camera.main.WorldToScreenPoint(worldPosition);

            transform.position = new Vector3(pos.x, pos.y) + Vector3.up * moveUpOffset * (1f - alpha);

            if(alpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}