using TMPro;
using UnityEngine;

namespace Creotly_Studios
{
    public class UITimeStamp : MonoBehaviour
    {
        public float remainingTime;
        [SerializeField] TextMeshProUGUI timerText;

        private void Awake()
        {
            timerText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void UITimeStamp_Updater(float delta)
        {
            TimerCountdown(delta);
        }

        private void TimerCountdown(float delta)
        {
            remainingTime -= delta;
            if(remainingTime <= 0.0f)
            {
                remainingTime = 0.0f;
            }

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            int milliSecond = Mathf.FloorToInt((remainingTime * 1000) % 1000);

            timerText.color = (remainingTime < 180f) ? Color.red : Color.white;
            timerText.text = string.Format("{0:00} : {1:00} : {2: 000}", minutes, seconds, milliSecond);
        }
    }
}