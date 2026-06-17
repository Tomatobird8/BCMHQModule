using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BCMHQModule.MonoBehaviours
{
    public class MenuToggleController : MonoBehaviour
    {
        private Button? button;
        private TextMeshProUGUI? buttonText;
        private Image? checkmarkImage;

        public Sprite? buttonSprite;
        public Sprite? checkmarkSprite;

        public int settingId = 0;
        public string baseTitle = "Setting";

        void Awake()
        {
            button = GetComponent<Button>();
            checkmarkImage = transform.Find("Background/Checkmark")?.GetComponent<Image>();
            buttonText = transform.parent.Find("Text (2)").GetComponent<TextMeshProUGUI>();
            if (button == null) BCMHQModule.Logger.LogWarning("checkmarkImage was null!");
            else
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }

        void Start()
        {
            UpdateVisualState();
        }


        void OnButtonClicked()
        {
            if (settingId == 0)
            {
                BCMHQModule.internalNames.Value = !BCMHQModule.internalNames.Value;
                BCMHQModule.internalNames.ConfigFile.Save();
            }
            else
            {
                BCMHQModule.sdcMode.Value = !BCMHQModule.sdcMode.Value;
                BCMHQModule.sdcMode.ConfigFile.Save();
            }
            UpdateVisualState();
        }

        void UpdateVisualState()
        {
            if (buttonText == null) return;

            buttonText.text = $"{baseTitle}";

            if (checkmarkImage == null) return;

            bool isActive = (settingId == 0) ? BCMHQModule.internalNames.Value : BCMHQModule.sdcMode.Value;

            if (isActive)
            {
                checkmarkImage.overrideSprite = checkmarkSprite;
            }
            else
            {
                checkmarkImage.overrideSprite = null;
            }
        }
    }
}
