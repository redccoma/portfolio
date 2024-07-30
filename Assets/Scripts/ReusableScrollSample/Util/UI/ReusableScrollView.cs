/*
 * ScrollRect를 이용하여 아이템재사용이 가능한 스크롤뷰 구현
 * 화면을 넘지 않는 경우 가운데 정렬 및 스크롤 고정 (Vertical, Horizontal 모두 해당)
 * 화면을 넘길 경우 왼쪽 정렬(Horizontal) 혹은 상단 정렬(Vertical) 및 스크롤 가능
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReusableScrollSample.Util.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ReusableScrollView<T> : MonoBehaviour
    {
        private enum ScrollDirection
        {
            /// <summary>
            /// 수직 스크롤.
            /// 자식이 화면을 넘어갈때는 상단정렬 및 스크롤 가능
            /// 그외 가운데 정렬 및 스크롤 정지
            /// </summary>
            Vertical,
            /// <summary>
            /// 수평 스크롤.
            /// 자식이 화면을 넘어갈때는 왼쪽정렬 및 스크롤 가능
            /// 그외 가운데 정렬 및 스크롤 정지
            /// </summary>
            Horizontal
        }

        private class PoolData
        {
            private readonly IScrollViewItem<T> viewItem;
            public readonly GameObject CachedGameObject;
            public readonly RectTransform CachedRectTransform;

            public PoolData(IScrollViewItem<T> v, GameObject g, RectTransform r)
            {
                viewItem = v;
                CachedGameObject = g;
                CachedRectTransform = r;
            }

            public void SetData(T data, Action<T> callback)
            {
                viewItem.SetData(data, callback);
            }
        }
        
        #region 인스펙터 노출 변수
        [SerializeField]    // 스크롤뷰
        private ScrollRect scrollRect;
        
        [SerializeField]    // 아이템프리팹
        private GameObject itemPrefab;
        
        [SerializeField]    // 스크롤뷰 자식 컨텐츠 객체
        private RectTransform content;
        
        [SerializeField]
        private ScrollDirection scrollDirection = ScrollDirection.Vertical;
        
        [SerializeField][Range(0, 100)] // 생성 아이템간의 간격
        private float itemSpacing = 10f;
        #endregion
        
        #region private 변수 및 프로퍼티
        // 풀링 객체리스트
        private List<PoolData> itemPoolList = new List<PoolData>();
        // 스크롤뷰 내부에서 처리할 데이터
        private List<T> dataList = new List<T>();
        // 스크롤할때 자연스러운 스크롤이동처럼 보일수 있도록 버퍼아이템을 추가
        private const int BUFFER_ITEMS = 1;
        // 생성한 스크롤뷰 아이템의 크기
        private Vector2 itemSize;
        // 화면에서 보여질 최대 아이템 갯수
        private int itemsPerScreen;
        // 스크롤링이 가능한가.
        private bool isScrollable;
        // 초기화
        private bool isInitialized = false;

        private Action<T> onClickCallback;

        private float CurrentPosition => scrollDirection == ScrollDirection.Vertical ? Mathf.Abs(content.anchoredPosition.y) : Mathf.Abs(content.anchoredPosition.x);
        private float ItemSize => scrollDirection == ScrollDirection.Vertical ? itemSize.y : itemSize.x;
        private float ItemSizeWithSpacing => ItemSize + itemSpacing;
        private float ViewPortSize => scrollDirection == ScrollDirection.Vertical ? scrollRect.viewport.rect.height : scrollRect.viewport.rect.width;
        #endregion
        
        /// <summary>
        /// 스크롤뷰 아이템 생성
        /// </summary>
        /// <param name="newData"></param>
        public void UpdateData(List<T> newData, Action<T> callback = null)
        {
            dataList = new List<T>(newData);
            
            if (!isInitialized)
            {
                InitializeScrollView();
            }

            onClickCallback = callback;
            
            UpdateContentSize();
            AdjustPoolSize();
            UpdateScrollability();
            ResetScrollPosition();
            UpdateItems();
        }
        
        /// <summary>
        /// 전체 리스트는 동일한 상태에서 특정 아이템의 내용 갱신시
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newData"></param>
        public void UpdateItem(int index, T newData)
        {
            if (index >= 0 && index < dataList.Count)
            {
                dataList[index] = newData;
                UpdateItems();
            }
        }

        #region private 함수
        private void Start()
        {
            InitializeScrollView();
        }

        /// <summary>
        /// 스크롤뷰 초기화
        /// </summary>
        private void InitializeScrollView()
        {
            if (isInitialized) return;

            SetupContent();
            CalculateItemSize();
            CalculateItemsPerScreen();
            SetupScrollRect();

            isInitialized = true;
        }
        
        /// <summary>
        /// 스크롤 방향에 따라 컨텐츠 앵커설정.
        /// </summary>
        private void SetupContent()
        {
            if (scrollDirection == ScrollDirection.Vertical)
            {
                content.anchorMin = new Vector2(0.5f, 1);
                content.anchorMax = new Vector2(0.5f, 1);
                content.pivot = new Vector2(0.5f, 1);
            }
            else
            {
                content.anchorMin = new Vector2(0, 0.5f);
                content.anchorMax = new Vector2(0, 0.5f);
                content.pivot = new Vector2(0, 0.5f);
            }
        }

        /// <summary>
        /// 스크롤뷰 아이템 크기 계산
        /// </summary>
        private void CalculateItemSize()
        {
            RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();
            itemSize = new Vector2(itemRect.rect.width, itemRect.rect.height);
        }

        /// <summary>
        /// 스크롤 방향에 따라 화면에 보여질 최대 아이템 갯수 계산
        /// 이 함수는 초기화시 1회만 호출하므로, 함수 내부에서 프로퍼티로 대체할수 있는 부분은 수정없이 그대로 사용합니다.
        /// </summary>
        private void CalculateItemsPerScreen()
        {
            itemsPerScreen = Mathf.CeilToInt(ViewPortSize / ItemSizeWithSpacing);
        }

        /// <summary>
        /// 지정된 스크롤 방향에 따른 ScrollRect 설정 및 이벤트 리스너 등록
        /// </summary>
        private void SetupScrollRect()
        {
            scrollRect.vertical = scrollDirection == ScrollDirection.Vertical;
            scrollRect.horizontal = scrollDirection == ScrollDirection.Horizontal;
            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        /// <summary>
        /// 스크롤 움직임 발생시 콜백함수
        /// </summary>
        /// <param name="position">스크롤뷰 현재 위치</param>
        private void OnScroll(Vector2 position)
        {
            if (isScrollable)
            {
                // 매개변수인 position은 0~1 값인데, 아이템의 이동은 content의 현재 위치를 기반으로 동작하도록 되어있으므로,
                // 이 값은 사용하지 않고 UpdateItems안에서 직접 content의 anchoredPosition을 사용하여 계산합니다.
                UpdateItems();
            }
        }

        /// <summary>
        /// ScrollRect의 뷰포트 사이즈와 content.anchoredPosition 기반으로 아이템의 위치 갱신 및 데이터 갱신을 처리합니다.
        /// </summary>
        private void UpdateItems()
        {
            if (!isScrollable)
            {
                // 한 화면 안에 모든 아이템이 보이는 경우 (스크롤될때는 UpdateItems 함수 미호출. 즉 처음만 계산한다.)
                // 전체 길이 계산
                float totalSize = (dataList.Count * ItemSize) + ((dataList.Count - 1) * itemSpacing);
                // 시작 위치 계산
                float startPosition = (ViewPortSize - totalSize) / 2;

                for (int i = 0; i < itemPoolList.Count; i++)
                {
                    if (i < dataList.Count)
                    {
                        itemPoolList[i].CachedGameObject.SetActive(true);
                        itemPoolList[i].SetData(dataList[i], onClickCallback);
                        itemPoolList[i].CachedRectTransform.anchoredPosition = scrollDirection == ScrollDirection.Vertical
                            ? new Vector2(0, -startPosition - i * ItemSizeWithSpacing)
                            : new Vector2(startPosition + i * ItemSizeWithSpacing, 0);
                    }
                    else
                    {
                        itemPoolList[i].CachedGameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // 화면바깥에도 아이템이 있는 경우.
                // 현재 위치대비 계산할 아이템의 시작 인덱스
                int startIndex = Mathf.FloorToInt(CurrentPosition / ItemSizeWithSpacing);
                // 시작 인덱스 보정
                startIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, dataList.Count - itemsPerScreen));

                for (int i = 0; i < itemPoolList.Count; i++)
                {
                    int dataIndex = startIndex + i;
                    if (dataIndex < dataList.Count)
                    {
                        itemPoolList[i].CachedGameObject.SetActive(true);
                        itemPoolList[i].SetData(dataList[dataIndex], onClickCallback);
                        itemPoolList[i].CachedRectTransform.anchoredPosition = scrollDirection == ScrollDirection.Vertical
                            ? new Vector2(0, -dataIndex * ItemSizeWithSpacing)
                            : new Vector2(dataIndex * ItemSizeWithSpacing, 0);
                    }
                    else
                    {
                        itemPoolList[i].CachedGameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 전체 스크롤뷰 아이템 갯수 기반한 content 크기 변경
        /// </summary>
        private void UpdateContentSize()
        {
            float contentSize = (dataList.Count * ItemSize) + ((dataList.Count - 1) * itemSpacing);

            if (scrollDirection == ScrollDirection.Vertical)
                content.sizeDelta = new Vector2(content.sizeDelta.x, contentSize);
            else
                content.sizeDelta = new Vector2(contentSize, content.sizeDelta.y);
        }

        /// <summary>
        /// 풀 리스트 갯수 조정.
        /// </summary>
        private void AdjustPoolSize()
        {
            // dataList의 갯수가 화면에 보여질 아이템의 갯수보다 작다면, itemsPerScreen만큼
            // dataList의 갯수가 화면크기를 넘어갈 경우 itemsPerScreen + BUFFER_ITEMS만큼 풀링 갯수를 지정
            int requiredPoolSize = Mathf.Min(dataList.Count, itemsPerScreen + BUFFER_ITEMS);
            requiredPoolSize = Mathf.Max(requiredPoolSize, itemsPerScreen);

            while (itemPoolList.Count < requiredPoolSize)
            {
                CreatePoolItem();
            }

            // 풀링 아이템의 삭제는 반드시 필요한 것이 아니므로, 성능상 이슈가 있을 경우 주석처리 가능합니다.
            while (itemPoolList.Count > requiredPoolSize)
            {
                int lastIndex = itemPoolList.Count - 1;
                Destroy(itemPoolList[lastIndex].CachedGameObject);
                itemPoolList.RemoveAt(lastIndex);
            }
        }

        /// <summary>
        /// 풀 아이템 생성.
        /// 풀 아이템 관리 리스트인 itemPoolList에 추가하는 것까지 포함됨. 
        /// </summary>
        private void CreatePoolItem()
        {
            GameObject item = Instantiate(itemPrefab, content);
            IScrollViewItem<T> scrollViewItem = item.GetComponent<IScrollViewItem<T>>();
            if (scrollViewItem == null)
            {
                Debug.LogError("ReusableScrollView 아이템은 IScrollViewItem을 상속받아야 합니다.");
                return;
            }
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
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
            }

            PoolData poolData = new PoolData(scrollViewItem, item, rectTransform);
            itemPoolList.Add(poolData);
        }

        /// <summary>
        /// 뷰포트와 컨텐츠 크기를 기반으로 스크롤뷰 이동 여부 처리.
        /// </summary>
        private void UpdateScrollability()
        {
            float contentSize = scrollDirection == ScrollDirection.Vertical ? content.sizeDelta.y : content.sizeDelta.x;

            isScrollable = contentSize > ViewPortSize;

            scrollRect.vertical = scrollDirection == ScrollDirection.Vertical && isScrollable;
            scrollRect.horizontal = scrollDirection == ScrollDirection.Horizontal && isScrollable;
        }

        /// <summary>
        /// 스크롤뷰의 위치를 초기화
        /// </summary>
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
        #endregion
    }
    
    public interface IScrollViewItem<T>
    {
        void SetData(T data, Action<T> callback);
    }
}

