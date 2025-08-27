#nullable enable


using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Numerical_HUD
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get => log; private set => log = value; }
        internal static Character? localChar;
        internal static GUIManager? manager;
        internal static UITextObject? staminatext;
        internal static UITextObject? extraStam;
        internal static List<UITextObject> afflictions = [];
        internal static List<UITextObject> textToCleanup = [];
        private static ManualLogSource log = null!;


        private void OnDestroy()
        {
            textToCleanup.ForEach(Destroy);
        }
        private void Awake()
        {

            Log = Logger;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
        }

        private void FixedUpdate()
        {
            if (localChar == null)
            {
                localChar = Character.localCharacter;
            }
            if (manager == null)
            {
                GUIManager? checkManager = GUIManager.instance;
                if (checkManager != null)
                {
                    Log.LogInfo("Manager isn't null attempting binding");
                    manager = checkManager;

                }
            }
            if (localChar != null && manager != null && staminatext == null)
            {
                AttachText(manager, localChar);
            }
        }

        private void AttachText(GUIManager manager, Character localChar)
        {
            TMP_FontAsset font = manager.interactPromptText.font;

            manager.hudCanvas.gameObject.SetActive(false);

            CreateStaminaBarText(font, manager, localChar);
            CreateExtraStaminaBarText(font, manager, localChar);
            CreateAfflictionBarText(font, manager, localChar);
            CreateAltitudeText(font, manager, localChar);

            manager.hudCanvas.gameObject.SetActive(true);
        }

        private void CreateStaminaBarText(TMP_FontAsset font, GUIManager manager, Character localChar)
        {
            staminatext = manager.hudCanvas.gameObject.AddComponent<UITextObject>();
            staminatext.Logger = Log;
            staminatext.FontObject = font;
            staminatext.Parent = manager.bar.staminaBar.gameObject;
            staminatext.TextName = "Stamina_Bar";
            staminatext.UpdateFunc = () =>
            {
                return Math.Round(localChar.data.currentStamina * 100, 0, MidpointRounding.ToEven).ToString();
            };
            textToCleanup.AddItem(staminatext);
        }

        private void CreateExtraStaminaBarText(TMP_FontAsset font, GUIManager manager, Character localChar)
        {
            extraStam = manager.hudCanvas.gameObject.AddComponent<UITextObject>();
            extraStam.Logger = Log;
            extraStam.FontObject = font;
            extraStam.Parent = manager.bar.extraBarStamina.gameObject;
            extraStam.TextName = "Extra_Stamina_Bar";
            extraStam.UpdateFunc = () =>
            {
                return Math.Round(localChar.data.extraStamina * 100, 0, MidpointRounding.ToEven).ToString();
            };
            textToCleanup.AddItem(extraStam);
        }

        private void CreateAfflictionBarText(TMP_FontAsset font, GUIManager manager, Character localChar)
        {
            foreach (BarAffliction affliction in manager.bar.afflictions)
            {
                CharacterAfflictions.STATUSTYPE afflictionType = affliction.afflictionType;
                UITextObject afflictionText = manager.hudCanvas.gameObject.AddComponent<UITextObject>();
                afflictionText.Logger = Log;
                afflictionText.FontObject = font;
                afflictionText.Parent = affliction.gameObject;
                afflictionText.TextName = $"{affliction.afflictionType}_Bar";
                afflictionText.UpdateFunc = () =>
                {
                    return Math.Round(localChar.refs.afflictions.GetCurrentStatus(afflictionType) * 100, 0, MidpointRounding.ToEven).ToString();
                };
                afflictions.AddItem(afflictionText);
                textToCleanup.AddItem(afflictionText);
            }
        }
        private void CreateAltitudeText(TMP_FontAsset font, GUIManager manager, Character localChar)
        {
            UITextObject altitudeText = manager.hudCanvas.gameObject.AddComponent<UITextObject>();
            altitudeText.Logger = Log;
            altitudeText.FontObject = font;
            altitudeText.Parent = manager.bar.maxStaminaBar.gameObject;
            altitudeText.AdditionalTranslation = new Vector2(0, 75);
            altitudeText.TextName = $"Altitude_Text";
            altitudeText.UpdateFunc = () =>
            {
                return $"Altitude: {Math.Floor(localChar.refs.stats.heightInUnits)} ({Math.Floor(localChar.refs.stats.heightInMeters)}M)";
            };
            textToCleanup.AddItem(altitudeText);
        }
    }
}
