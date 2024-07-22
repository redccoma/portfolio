using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EcoHero.UI
{
    public class ReusableScrollView : MonoBehaviour
    {
        public class PoolData
        {
            private readonly ScrollViewItem viewItem;
            public readonly GameObject CachedGameObject;
            public readonly RectTransform CachedRectTransform;

            public PoolData(ScrollViewItem v, GameObject g, RectTransform r)
            {
                viewItem = v;
                CachedGameObject = g;
                CachedRectTransform = r;
            }
            
            public void SetData(string data)
            {
                viewItem.SetData(data);
            }
        }
        
        public enum ScrollDirection
        {
            Vertical,
            Horizontal
        }
        
        // 버퍼 아이템 수
        private const int BUFFER_ITEMS = 1;

        public ScrollRect scrollRect;
        public GameObject itemPrefab;
        public RectTransform content;
        public ScrollDirection scrollDirection = ScrollDirection.Vertical;

        // 오브젝트 풀 리스트
        private List<PoolData> itemPoolList = new List<PoolData>();
        // 보여질 스크롤뷰 아이템들의 데이터
        private List<string> dataList = new List<string>();
        // Prefab의 크기
        private Vector2 itemSize;
        // 화면에 보여져야할 아이템의 갯수
        private int itemsPerScreen;
        // 스크롤이 가능한가
        private bool isScrollable;
        // 스크롤뷰 초기화 완료 플래그
        private bool isInitialized = false;

        private void Start()
        {
            InitializeScrollView();
        }
        
        private void InitializeScrollView()
        {
            if (isInitialized)
                return;
            
            // 아이템의 크기 계산
            RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();
            itemSize = new Vector2(itemRect.rect.width, itemRect.rect.height);
            
            // 화면에 표시될 수 있는 아이템 수 계산 (버퍼 아이템갯수+1)
            if (scrollDirection == ScrollDirection.Vertical)
                itemsPerScreen = Mathf.CeilToInt(scrollRect.viewport.rect.height / itemSize.y);
            else
                itemsPerScreen = Mathf.CeilToInt(scrollRect.viewport.rect.width / itemSize.x);

            // 스크롤 방향 설정
            scrollRect.vertical = scrollDirection == ScrollDirection.Vertical;
            scrollRect.horizontal = scrollDirection == ScrollDirection.Horizontal;

            // 스크롤 이벤트 리스너 추가
            scrollRect.onValueChanged.AddListener(OnScroll);

            isInitialized = true;
        }

        private void UpdateContentSize()
        {
            float contentSize = dataList.Count * (scrollDirection == ScrollDirection.Vertical ? itemSize.y : itemSize.x);

            if (scrollDirection == ScrollDirection.Vertical)
            {
                content.sizeDelta = new Vector2(content.sizeDelta.x, contentSize);
            }
            else
            {
                content.sizeDelta = new Vector2(contentSize, content.sizeDelta.y);
            }
        }

        private PoolData CreatePoolItem()
        {
            GameObject item = Instantiate(itemPrefab, content);
            ScrollViewItem scrollViewItem = item.GetComponent<ScrollViewItem>();
            RectTransform rectTransform = item.GetComponent<RectTransform>();
    
            if (scrollDirection == ScrollDirection.Vertical)
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 0.5f);
            }

            PoolData poolData = new PoolData(scrollViewItem, item, rectTransform);
            itemPoolList.Add(poolData);
            return poolData;
        }

        private void UpdateItems()
        {
            if (!isScrollable)
            {
                // 스크롤이 불가능한 경우 중앙 정렬
                float startPosition = scrollDirection == ScrollDirection.Vertical
                    ? (scrollRect.viewport.rect.height - dataList.Count * itemSize.y) / 2
                    : (scrollRect.viewport.rect.width - dataList.Count * itemSize.x) / 2;

                for (int i = 0; i < itemPoolList.Count; i++)
                {
                    if (i < dataList.Count)
                    {
                        itemPoolList[i].CachedGameObject.SetActive(true);
                        itemPoolList[i].SetData(dataList[i]);
                        
                        itemPoolList[i].CachedRectTransform.anchoredPosition = scrollDirection == ScrollDirection.Vertical
                            ? new Vector2(0, -startPosition - i * itemSize.y)
                            : new Vector2(startPosition + i * itemSize.x, 0);
                    }
                    else
                    {
                        itemPoolList[i].CachedGameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // 스크롤이 가능한 경우 기존 로직 사용
                float contentPosition = scrollDirection == ScrollDirection.Vertical 
                    ? Mathf.Abs(content.anchoredPosition.y)
                    : Mathf.Abs(content.anchoredPosition.x);
                
                int startIndex = Mathf.FloorToInt(contentPosition / (scrollDirection == ScrollDirection.Vertical ? itemSize.y : itemSize.x));
                startIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, dataList.Count - itemsPerScreen));

                for (int i = 0; i < itemPoolList.Count; i++)
                {
                    int dataIndex = startIndex + i;
                    if (dataIndex < dataList.Count)
                    {
                        itemPoolList[i].CachedGameObject.SetActive(true);
                        itemPoolList[i].SetData(dataList[dataIndex]);
                
                        itemPoolList[i].CachedRectTransform.anchoredPosition = scrollDirection == ScrollDirection.Vertical
                            ? new Vector2(0, -dataIndex * itemSize.y)
                            : new Vector2(dataIndex * itemSize.x, 0);
                    }
                    else
                    {
                        itemPoolList[i].CachedGameObject.SetActive(false);
                    }
                }
            }
        }
        
        private void OnScroll(Vector2 position)
        {
            if (isScrollable)
            {
                UpdateItems();
            }
        }

        // 데이터 업데이트 메서드 (외부에서 호출)
        public void UpdateData(List<string> newData)
        {
            if (!isInitialized)
            {
                InitializeScrollView();
            }
            
            dataList = newData;
            
            UpdateContentSize();
            AdjustPoolSize();
            UpdateScrollability();
            ResetScrollPosition();
            UpdateItems();
        }
        
        private void ResetScrollPosition()
        {
            if (isScrollable)
            {
                if (scrollDirection == ScrollDirection.Horizontal)
                {
                    scrollRect.horizontalNormalizedPosition = 0;
                }
                else
                {
                    scrollRect.verticalNormalizedPosition = 1;
                }
            }
            else
            {
                content.anchoredPosition = Vector2.zero;
            }
        }
        
        private void AdjustPoolSize()
        {
            int requiredPoolSize = Mathf.Min(dataList.Count, itemsPerScreen + BUFFER_ITEMS);

            // 풀 크기가 부족한 경우 아이템 추가
            while (itemPoolList.Count < requiredPoolSize)
            {
                CreatePoolItem();
            }

            // 풀 크기가 과도한 경우 매번 Destroy하기엔 비용이 오히려 커질수 있으므로 적정선에서 풀 생성만 하고 지우진 않습니다.
        }
        
        private void UpdateScrollability()
        {
            float viewportSize = scrollDirection == ScrollDirection.Vertical
                ? scrollRect.viewport.rect.height
                : scrollRect.viewport.rect.width;
    
            float contentSize = scrollDirection == ScrollDirection.Vertical
                ? content.sizeDelta.y
                : content.sizeDelta.x;

            isScrollable = contentSize > viewportSize;
    
            scrollRect.vertical = scrollDirection == ScrollDirection.Vertical && isScrollable;
            scrollRect.horizontal = scrollDirection == ScrollDirection.Horizontal && isScrollable;
        }
    }
}