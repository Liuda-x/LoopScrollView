using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Liudax.LoopScrollView
{
    /// <summary>
    /// 滑动方向
    /// </summary>
    public enum MoveType
    {
        Top2Bottom = 0,
        Left2Right = 1,
        Right2Left = 2,
        Bottom2Top = 3,
    }
    /// <summary>
    /// 对齐方向
    /// </summary>
    public enum AlignmentType
    {
        Top,
        Left,
        Center,
        Bottom,
        Right,
    }
    public class LoopScrollView : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public MoveType moveType;
        public AlignmentType alignmentType;
        public float cellHeight;
        public float cellWidth;
        public Vector2 cellSpacing;
        public GameObject itemPrefab;
        [Range(1, 10)]
        public int perLineCount=1;
        public bool backgroundCanDrag = false;
        public RectTransform content => scrollRect.content;
        public RectTransform viewport => scrollRect.viewport;
        private IList itemDataList ;
        public Rect Bounds { get; private set; }
        public int MaxShowCount { get; private set; }
        public int Count => itemDataList == null ? 0 : itemDataList.Count;
        public int LineCount =>Mathf.CeilToInt(1.0f*Count / perLineCount);
        private LinkedList<LoopItemView> showItems;                                 //所有显示的Items
        private LinkedList<LoopItemView> recyclePool;                               //回收池
        public bool IsInit { get;private set; }
        private void OnDisable()
        {
            if (!IsInit) return;
            SetAllShowItemDisable();
        }
        private void OnEnable()
        {
            if (!IsInit) return;
            SetAllShowItemEnable();
            RefreshAllShowItem();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            LoopUtils.SetViewPort(moveType,this);
            LoopUtils.SetContentPivot(this);
            InitBoundsInfo();
            showItems = new LinkedList<LoopItemView>();
            recyclePool = new LinkedList<LoopItemView>();
            IsInit = true;
        }
        public void ResetScrollView()
        {
            Init();
            SetData(itemDataList);
        }
        public void Dispse()
        {
            while (showItems.Count > 0 && showItems != null)
            {
                LoopItemView view = showItems.First.Value;
                showItems.RemoveFirst();
                view.ItemDisable();
                view.ItemDestory();
                Destroy(view.gameObject);
            }
            itemDataList = null;
            IsInit = false;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        public void SetData(IList dataList)
        {
            SetData(dataList,0);
        }
        public void SetData(IList dataList,int moveIndex)
        {
            itemDataList = dataList;
            InitItemInfo();
            InitContentSize();
            MoveToIndex(moveIndex);
        }
        public void ResetBounds()
        {
            LoopUtils.SetViewPort(moveType, this);
            LoopUtils.SetContentPivot(this);
            InitBoundsInfo();
            InitItemInfo();
            SetData(itemDataList, 0);
        }
        /// <summary>
        /// 初始化Content
        /// </summary>
        private void InitContentSize()
        {
            content.sizeDelta = LoopUtils.GetContentSize(moveType, this);
            if (backgroundCanDrag)
            {
                if (viewport.TryGetComponent<RawImage>(out RawImage image))
                {
                    image.raycastTarget = true;
                }
                else
                {
                    image = viewport.gameObject.AddComponent<RawImage>();
                    image.raycastTarget = true;
                    image.color = new Color(0, 0, 0, 0);
                }
            }
        }
        /// <summary>
        /// 移动到对应index
        /// </summary>
        /// <param name="index"></param>
        public void MoveToIndex(int index)
        {
            int realIndex = CampMoveIndex(index);
            this.content.transform.localPosition = LoopUtils.GetContentPositionByIndex(moveType, alignmentType, this, index);
            RecycleHideItem(realIndex);
            SetShowItem(realIndex);
        }
        /// <summary>
        /// 初始化范围信息
        /// </summary>
        private void InitBoundsInfo()
        {
            Bounds = LoopUtils.GetViewBounds(viewport, scrollRect); //
        }
        /// <summary>
        /// 初始化item信息
        /// </summary>
        private void InitItemInfo()
        {
            MaxShowCount = LoopUtils.GetMaxShowCount(moveType, this);
            MaxShowCount = (int)Mathf.Min(MaxShowCount, Count);
        }
        /// <summary>
        /// 获取生成位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 GetItemPosition(int index)
        {
            return LoopUtils.GetItemPositionByIndex(moveType, alignmentType, this, index);
        }
        /// <summary>
        /// 刷新所有显示的Item
        /// </summary>
        private void RefreshAllShowItem()
        {
            foreach (LoopItemView item in showItems)
            {
                item.ItemUpdate();
            }
        }
        private void Update()
        {
            if (!IsInit || showItems == null || itemDataList == null || itemDataList.Count == 0) return;
            int curIndex = GetFirstIndex();
            RecycleHideItem(curIndex);
            SetShowItem(curIndex);
        }
        /// <summary>
        /// 激活item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="itemView"></param>
        private void EnableItem(int index, LoopItemView itemView)
        {
            itemView.SetData(itemDataList[index], index);
            itemView.gameObject.SetActive(true);
            itemView.transform.localPosition = GetItemPosition(index);
            itemView.ItemEnable();
            itemView.ItemUpdate();
        }
        /// <summary>
        /// 隐藏item
        /// </summary>
        /// <param name="itemView"></param>
        private void DisableItem(LoopItemView itemView)
        {
            itemView.gameObject.SetActive(false);
            itemView.ItemDisable();
        }
        int time = 0;
        private bool CheckTimeOver()
        {
            time++;
            if (time > 10000)
            {
                Debug.LogError("TimeOver");
                return true;
            }
            return false;
        }
        private LoopItemView GetLoopItemView()
        {
            LoopItemView itemView;
            if (recyclePool.Count > 0)
            {
                itemView = recyclePool.First.Value;
                recyclePool.RemoveFirst();
            }
            else
            {
                GameObject obj = Instantiate(itemPrefab, content);
                itemView = obj.GetComponent<LoopItemView>();
                itemView.ItemInit();
            }
            return itemView;
        }
        internal void SetShowItem(int curIndex)
        {
            int curFirst = showItems.Count == 0 ? 0 : showItems.First.Value.Index;
            for (int i = curFirst - 1; i >= curIndex; i--)
            {
                LoopItemView itemView = GetLoopItemView();
                itemView.transform.SetAsFirstSibling();
                EnableItem(CampListIndex(i), itemView);
                showItems.AddFirst(itemView);
            }
            int lastIndex = CampListIndex(curIndex + MaxShowCount - 1);
            int curLast = showItems.Count == 0 ? curIndex - 1 : showItems.Last.Value.Index;
            for (int i = curLast + 1; i <= lastIndex; i++)
            {
                LoopItemView itemView = GetLoopItemView();
                itemView.transform.SetAsLastSibling();
                EnableItem(CampListIndex(i), itemView);
                showItems.AddLast(itemView);
            }
        }
        private int CampMoveIndex(int curIndex)
        {
            return Clamp(curIndex, 0, Count - MaxShowCount);
        }
        private int CampListIndex(int curIndex)
        {
            return Clamp(curIndex, 0, Count - 1);
        }
        private int Clamp(int value,int min,int max)
        {
            if (max < min) (min, max) = (max, min);
            if (value < min) return min;
            if(value> max) return max;
            return value;
        }
        internal void RecycleHideItem(int curIndex)
        {
            time = 0;
            while (showItems.Count > 0 && showItems.First.Value.Index < curIndex && !CheckTimeOver())
            {
                LoopItemView itemView = showItems.First.Value;
                showItems.RemoveFirst();
                DisableItem(itemView);
                recyclePool.AddLast(itemView);
            }
            int lastIndex = CampListIndex(curIndex + MaxShowCount);
            while (showItems.Count > 0 && showItems.Last.Value.Index > lastIndex && !CheckTimeOver())
            {
                LoopItemView itemView = showItems.Last.Value;
                showItems.RemoveLast();
                DisableItem(itemView);
                recyclePool.AddLast(itemView);
            }
        }
        internal int GetFirstIndex()
        {
            if (itemDataList == null) return 0;
            return CampMoveIndex(LoopUtils.GetCurFirstIndex(moveType, this));
        }
        private void SetAllShowItemEnable()
        {
            foreach (LoopItemView item in showItems)
            {
                item.ItemEnable();
            }
        }
        private void SetAllShowItemDisable()
        {
            foreach (LoopItemView item in showItems)
            {
                item.ItemDisable();
            }
        }
        #region 暂不处理
        //（1）RectMask2D
        //private RectMask2D rectMask;
        //rectMask = viewport.GetComponent<RectMask2D>();
        //+new Vector3(rectMask.padding.x, rectMask.padding.y)
        //- new Vector3(rectMask.padding.z, rectMask.padding.w)
        #endregion
        #region 绘制辅助线
#if UNITY_EDITOR
        Vector3[] cornerPos = new Vector3[4];
        void OnDrawGizmosSelected()
        {
            if (IsInit == false) return;
            Gizmos.color = Color.blue;
            content.GetWorldCorners(cornerPos);
            Vector2 pos = new Vector2(Bounds.x, Bounds.y) + Bounds.size / 2;
            Gizmos.DrawWireCube(new Vector3(pos.x+scrollRect.transform.position.x,pos.y+scrollRect.transform.position.y,0), Bounds.size);
            Gizmos.DrawWireCube((cornerPos[2] + cornerPos[0])/2, content.rect.size);
            Gizmos.color = Color.yellow;
            foreach (LoopItemView item in showItems)
            {
                Gizmos.DrawWireCube(item.transform.position, new Vector3(cellWidth,cellHeight));
            }
        }
#endif
        #endregion
    }
}

