/*
 * 프로그래스바처리할 스프라이트 이미지 n개로 프로그래스바 처리.
 */

using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

namespace Game.GameStage2
{
    public class MultiImageProgressbar : MonoBehaviour
    {
        public Image progressBarImage;
        // 0인덱스: 0%, 마지막 인덱스: 100%
        public Sprite[] progressSprites;
        public Text healthText;
    
        /// <summary>
        /// 프로그래스바 데이터 셋
        /// </summary>
        /// <param name="progress">progress는 0에서 1 사이의 값.</param>
        /// <param name="health">표시할 HP</param>
        public void SetProgress(float progress, int health)
        {
            int index = Mathf.Clamp(Mathf.FloorToInt(progress * progressSprites.Length), 0, progressSprites.Length - 1);
            progressBarImage.sprite = progressSprites[index];

            healthText.text = health.ToString();
        }
    }
}