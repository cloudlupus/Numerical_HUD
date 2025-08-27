#nullable enable


using System;
using BepInEx.Logging;
using TMPro;
using UnityEngine;


namespace Numerical_HUD
{
    public class UITextObject : MonoBehaviour
    {
        private readonly GameObject textGameObject = new();
        private TextMeshProUGUI? textObject;
        private RectTransform? textTransform;

        public GameObject? Parent { get; set; }
        public Func<string>? UpdateFunc { get; set; }
        public TMP_FontAsset? FontObject { get; set; }
        public ManualLogSource? Logger { get; set; }

        public string TextName { get; set; } = "Default";

        public Vector2 AdditionalTranslation { get; set; } = Vector2.zero;

        private void Start()
        {
            textObject = textGameObject.AddComponent<TextMeshProUGUI>();
            textObject.text = "Init";
            textObject.fontSize = 50;
            textObject.color = Color.white;
            textObject.fontStyle = FontStyles.Normal;
            textObject.outlineColor = new Color(50, 50, 50);
            textObject.outlineWidth = 25;
            textObject.font = FontObject;
            textObject.fontSizeMin = 50;
            textObject.fontSizeMax = 50;
            textObject.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textObject.verticalAlignment = VerticalAlignmentOptions.Capline;
            textObject.textWrappingMode = TextWrappingModes.NoWrap;

            var parentObj = Parent ?? gameObject;
            textGameObject.transform.SetParent(parentObj.transform);
            textTransform = textGameObject.GetComponent<RectTransform>();
            textTransform.offsetMax = new Vector2(1, 1);
            textTransform.offsetMin = new Vector2(0, 0);
            textTransform.sizeDelta = new Vector2(400, 50);
            textTransform.Translate(AdditionalTranslation.x, AdditionalTranslation.y, 0);
            Logger?.LogInfo($"UITextObject({TextName}) started");

        }

        private void OnDestroy()
        {
            Destroy(textGameObject);
            Destroy(textObject);
            Destroy(textTransform);
            Logger?.LogInfo($"UITextObject({TextName}) Destroyed");
        }

        private void Update()
        {
            if (textObject && UpdateFunc != null)
            {
                textObject.text = UpdateFunc();
            }
        }
    }
}