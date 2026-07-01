using BCMHQModule.MonoBehaviours;
using BrutalCompanyMinus;
using BrutalCompanyMinus.Minus;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Menu related patches
    /// </summary>
    [HarmonyPatch]
    public class MenuManagerPatcher
    {
        private static TextMeshProUGUI? infoDisplay;

        [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(MenuManager __instance)
        {
            if (__instance.menuButtons == null || __instance.isInitScene) return;

            try
            {
                GameObject templateButtonObj = GameObject.Find("Canvas/MenuContainer/SettingsPanel/ControlsOptions/InvertYAxis");

                if (templateButtonObj == null)
                {
                    BCMHQModule.Logger.LogWarning("Failed to locate InvertYAxis.");
                    return;
                }

                GameObject uiContainer = new("BCMHQModule_ButtonContainer", typeof(RectTransform));
                uiContainer.transform.SetParent(__instance.menuButtons.transform, false);

                RectTransform containerRect = uiContainer.GetComponent<RectTransform>();
                containerRect.anchoredPosition = new Vector2(-70f, -120f);
                containerRect.sizeDelta = new Vector2(360f, 60f);
                containerRect.localScale = Vector3.one;

                VerticalLayoutGroup layout = uiContainer.AddComponent<VerticalLayoutGroup>();
                layout.childAlignment = TextAnchor.UpperCenter;
                layout.spacing = 10f;
                layout.childControlHeight = false;
                layout.childControlWidth = false;

                CreateSimulatedToggle(templateButtonObj, uiContainer.transform, "Internal Event Names", 0);
                CreateSimulatedToggle(templateButtonObj, uiContainer.transform, "SDC Mode", 1);

                BCMHQModule.Logger.LogInfo("Custom settings built and rendered successfully.");
            }
            catch (System.Exception ex)
            {
                BCMHQModule.Logger.LogError($"Failed to construct UI stack: {ex.Message}");
            }
        }

        private static void CreateSimulatedToggle(GameObject template, Transform parent, string titleText, int id)
        {
            GameObject rowItem = new($"BCMHQModule_Row_{id}", typeof(RectTransform));
            rowItem.transform.SetParent(parent, false);

            RectTransform rowRect = rowItem.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(100f, 15f);
            rowRect.localScale = Vector3.one;

            Sprite? block = template.transform.Find("Background").GetComponent<Image>()?.sprite;
            if (block == null) BCMHQModule.Logger.LogWarning("Block was null!");
            Sprite? checkmark = template.GetComponent<SettingsOption>()?.enabledImage;
            if (checkmark == null) BCMHQModule.Logger.LogWarning("Checkmark was null!");

            GameObject clonedButton = GameObject.Instantiate(template);
            clonedButton.name = $"BCMHQModule_ToggleButton_{id}";
            clonedButton.transform.SetParent(rowItem.transform, false);

            Button buttonComp = clonedButton.GetComponent<Button>();
            if (buttonComp == null) BCMHQModule.Logger.LogWarning("buttonComp was null!");
            else
            {
                buttonComp.onClick.RemoveAllListeners();
            }

            foreach (var script in clonedButton.GetComponents<MonoBehaviour>())
            {
                if (script is Button or Image) continue;
                GameObject.Destroy(script);
            }

            RectTransform buttonRect = clonedButton.GetComponent<RectTransform>();
            if (buttonRect == null) BCMHQModule.Logger.LogWarning("buttonRect was null!");
            else
            {
                buttonRect.anchorMin = new Vector2(1f, 1f);
                buttonRect.anchorMax = new Vector2(1f, 1f);
                buttonRect.pivot = new Vector2(1f, 0.5f);
                buttonRect.anchoredPosition = new Vector2(0f, -4f);
                buttonRect.sizeDelta = new Vector2(40f, 10f);
                buttonRect.localScale = Vector3.one;
            }

            Transform? labelTransform = clonedButton.transform.Find("Text (2)") ?? clonedButton.transform.GetComponentInChildren<TextMeshProUGUI>()?.transform;
            if (labelTransform == null) BCMHQModule.Logger.LogWarning("labelTransform was null!");
            else
            {
                labelTransform.SetParent(rowItem.transform, false);

                RectTransform textRect = labelTransform.GetComponent<RectTransform>();
                if (textRect == null) BCMHQModule.Logger.LogWarning("textRect was null!");
                else
                {
                    textRect.anchorMin = new Vector2(0f, 0.5f);
                    textRect.anchorMax = new Vector2(0f, 0.5f);
                    textRect.pivot = new Vector2(0f, 0.5f);
                    textRect.anchoredPosition = new Vector2(10f, 0f);
                    textRect.sizeDelta = new Vector2(320f, 10f);
                    textRect.localScale = Vector3.one;
                }

                TextMeshProUGUI textComp = labelTransform.GetComponent<TextMeshProUGUI>();
                if (textComp == null) BCMHQModule.Logger.LogWarning("textComp was null!");
                else
                {
                    textComp.text = titleText;
                    textComp.alignment = TextAlignmentOptions.MidlineLeft;
                    textComp.overflowMode = TextOverflowModes.Overflow;
                    textComp.enableWordWrapping = false;
                }
            }

            MenuToggleController toggleController = clonedButton.AddComponent<MenuToggleController>();
            toggleController.settingId = id;
            toggleController.baseTitle = titleText;
            toggleController.checkmarkSprite = checkmark;
            toggleController.buttonSprite = block;
        }

        [HarmonyPatch(typeof(Manager), nameof(Manager.ComputeDifficultyValues))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void ComputeDifficultyValues_Postfix()
        {
            if (BCMHQModule.sdcMode.Value) Manager.difficulty = 100f;
        }

        [HarmonyPatch(typeof(EventManager), nameof(EventManager.UpdateEventDescriptions))]
        [HarmonyPostfix]
        public static void UpdateEventDescriptions_Postfix(ref List<MEvent> events) 
        {
            if (!BCMHQModule.internalNames.Value || !Configuration.displayEvents.Value)
            {
                return;
            }
            EventManager.currentEventDescriptions.Clear();
            foreach (MEvent e in events)
            {
                if (e.Name() == "ChineseProduce") EventManager.currentEventDescriptions.Add($"<color={e.ColorHex}>CheapProduce</color>");
                else EventManager.currentEventDescriptions.Add($"<color={e.ColorHex}>{e.Name()}</color>");
            }
        } 

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.Start))]
        [HarmonyPostfix]
        private static void Start_Postfix()
        {
            if (!BCMHQModule.sdcMode.Value && BCMHQModule.correctVersion && !BCMHQModule.debugMode.Value)
            {
                return;
            }
            BCMHQModule.Logger.LogInfo($"SDC Mode: {BCMHQModule.sdcMode.Value}");
            BCMHQModule.Logger.LogInfo($"Debug Mode: {BCMHQModule.debugMode.Value}");
            if (infoDisplay == null)
            {
                BCMHQModule.Logger.LogInfo("infoDisplay is null. Creating a new infodisplay.");
                GameObject infoDisplayObject = new GameObject("BCMHQModule_infoDisplay");
                infoDisplayObject.transform.parent = HUDManager.Instance.weightCounter.transform.parent;
                TextMeshProUGUI weightCounter = HUDManager.Instance.weightCounter;
                infoDisplay = infoDisplayObject.AddComponent<TextMeshProUGUI>();
                infoDisplay.textStyle = weightCounter.textStyle;
                infoDisplay.tag = weightCounter.tag;
                infoDisplay.alignment = weightCounter.alignment;
                infoDisplay.color = weightCounter.color;
                infoDisplay.font = weightCounter.font;
                infoDisplay.fontSize = weightCounter.fontSize;
                infoDisplay.fontStyle = weightCounter.fontStyle;
                infoDisplay.fontWeight = weightCounter.fontWeight;
                infoDisplay.enableAutoSizing = weightCounter.enableAutoSizing;
                infoDisplay.fontSizeMin = weightCounter.fontSizeMin;
                infoDisplay.fontSizeMax = weightCounter.fontSizeMax;
                infoDisplay.isOverlay = weightCounter.isOverlay;
                infoDisplay.transform.position = weightCounter.transform.position;
                infoDisplay.text = "text";
                RectTransform infoDisplayTransform = infoDisplay.GetComponent<RectTransform>();
                if (infoDisplayTransform == null)
                {
                    BCMHQModule.Logger.LogError("Transform not found");
                    return;
                }
                infoDisplayTransform.offsetMin = weightCounter.GetComponent<RectTransform>().offsetMin;
                infoDisplayTransform.offsetMax = weightCounter.GetComponent<RectTransform>().offsetMax;
                infoDisplayTransform.anchoredPosition = new Vector2(67, -32);
                infoDisplayTransform.localScale = Vector3.one;
                infoDisplayTransform.localRotation = Quaternion.identity;
            }

            if (infoDisplay == null)
            {
                return;
            }
            if (BCMHQModule.debugMode.Value)
            {
                infoDisplay.text = "Debug Mode";
            }
            else if (!BCMHQModule.correctVersion)
                infoDisplay.text = $"Invalid Pack: BCM v{BCMHQModule.bcmVersionString}";
            else infoDisplay.text = "SDC Mode";

        }
    }
}
