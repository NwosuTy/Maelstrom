using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Creotly_Studios
{
    public class UIBar : MonoBehaviour
    {
        private Slider barSlider;
        private TMP_Text valueText;

        [field: Header("Status")]
        [field: SerializeField] public CharacterType characterType {get; private set;}

        public virtual void Awake()
        {
            if(characterType == CharacterType.Player)
            {
                barSlider = GetComponentInChildren<Slider>();
                valueText = GetComponentInChildren<TMP_Text>();
                return;
            }
            barSlider = GetComponent<Slider>();
        }

        public virtual void SetCurrentValue(float value)
        {
            if(characterType == CharacterType.Player)
            {
                valueText.text = $"{Mathf.RoundToInt(value)}%";
            }
            barSlider.value = value;
        }

        public virtual void SetMaxValue(float value)
        {
            if(characterType == CharacterType.Player)
            {
                valueText.text = $"{Mathf.RoundToInt(value)}%";
            }
            barSlider.maxValue = value;
            barSlider.value = value;
        }
    }
}
