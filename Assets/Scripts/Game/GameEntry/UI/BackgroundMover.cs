/*
 * 배열에 지정된 3장의 이미지를 지정된 속도로 이동시키면서 지나간 배경은 다시 뒤쪽으로 이동시키는 기능을 구현한 스크립트입니다.
 */

using System;
using System.Linq;
using UnityEngine;
using Game.Util.ScriptFieldSetup;

namespace Game.GameEntry.UI
{
    public class BackgroundMover : MonoBehaviour
    {
        public enum Direction
        {
            Left,
            Right
        }
        
        [Serializable]
        public class BackgroundInfo
        {
            public Transform transform;
            public SpriteRenderer spriteRenderer;
        }
        
        [SerializeField]
        private BackgroundInfo[] backGroundList;
        
        [SerializeField]
        private float moveSpeed = 1.0f;
        
        [SerializeField]
        private Direction direction = Direction.Left;

        // 배경 이미지의 너비
        private float spriteWidth;
        // 좌표 이동후 배경을 이동처리할 기준값 (배경 이미지 width / 2)
        private float rePositioningValue;
        
        private void Awake()
        {
            RepositionImages();
        }

        private void Update()
        {
            if (backGroundList == null || backGroundList.Length == 0)
                return;
            
            if (direction == Direction.Left)
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            
                // 가운데 배경의 월드포지션 체크
                if (backGroundList[0].transform.position.x < -rePositioningValue)
                {
                    // 부모 오브젝트를 배경의 너비만큼 오른쪽으로 이동
                    transform.position += new Vector3(spriteWidth, 0, 0);
                }    
            }
            else
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            
                // 가운데 배경의 월드포지션 체크
                if (backGroundList[0].transform.position.x > rePositioningValue)
                {
                    // 부모 오브젝트를 배경의 너비만큼 왼쪽으로 이동
                    transform.position -= new Vector3(spriteWidth, 0, 0);
                } 
            }
        }

        private void RepositionImages()
        {
            if (backGroundList == null || backGroundList.Length == 0)
            {
                Debug.LogWarning("배경정보가 없습니다.");
                return;
            }
            
            // 첫 번째 배경을 중앙에 배치
            backGroundList[0].transform.position = new Vector3(0, backGroundList[0].transform.position.y, backGroundList[0].transform.position.z);

            spriteWidth = backGroundList[0].spriteRenderer.bounds.size.x;
            
            for (int i = 1; i < backGroundList.Length; i++)
            {
                float xPosition;
            
                if (i % 2 == 0) // 짝수 인덱스 (오른쪽에 배치)
                {
                    xPosition = spriteWidth * (i / 2);
                }
                else // 홀수 인덱스 (왼쪽에 배치)
                {
                    xPosition = -spriteWidth * ((i + 1) / 2);
                }

                backGroundList[i].transform.position = new Vector3(xPosition, backGroundList[i].transform.position.y, backGroundList[i].transform.position.z);
            }
            
            rePositioningValue = spriteWidth / 2;
        }
        
#if UNITY_EDITOR
        // 코드를 작성할때 의도된대로 필드에 원하는 데이터를 할당하도록 합니다.
        [FieldSetupButton("Field Setup")]
        private void Setup()
        {
            // 비활성화된 객체를 포함하여 모든 자식 가져오기.
            Transform[] childTransform = transform.GetComponentsInChildren<Transform>(false);
            // 첫번째 요소(transform) 제외
            childTransform = childTransform.Skip(1).ToArray();
            
            backGroundList = new BackgroundInfo[childTransform.Length];

            for (int i = 0; i < backGroundList.Length; i++)
            {
                backGroundList[i] = new BackgroundInfo()
                {
                    transform = childTransform[i],
                    spriteRenderer = childTransform[i].GetComponent<SpriteRenderer>()
                };
            }
        }
#endif
    }   
}