using UnityEngine;
using UnityEngine.UI;
namespace Liudax.LoopScrollView
{
    public static class LoopUtils
    {
        /// <summary>
        /// 获取最大显示数量
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="bounds"></param>
        public static int GetMaxShowCount(MoveType moveType, LoopScrollView view)
        {
            switch (moveType)
            {
                case MoveType.Bottom2Top:
                case MoveType.Top2Bottom:
                    return Mathf.CeilToInt(view.Bounds.height  / (view.cellHeight + view.cellSpacing.y) +1) * view.perLineCount ;
                case MoveType.Left2Right:
                case MoveType.Right2Left:
                    return Mathf.CeilToInt(view.Bounds.width / (view.cellWidth + view.cellSpacing.x) + 1) * view.perLineCount;
            }
            return 0;
        }
        /// <summary>
        /// 获取Content范围
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        internal static Vector2 GetContentSize(MoveType moveType, LoopScrollView view)
        {
            switch (moveType)
            {
                case MoveType.Bottom2Top:
                case MoveType.Top2Bottom:
                    return new Vector2((view.cellWidth+view.cellSpacing.x)*view.perLineCount-view.cellSpacing.x,
                        Mathf.Max( view.LineCount * view.cellHeight + view.cellSpacing.y * (view.LineCount - 1),view.alignmentType!=AlignmentType.Center? view.Bounds.height:0));
                case MoveType.Right2Left:
                case MoveType.Left2Right:
                    return new Vector2(Mathf.Max(view.LineCount * view.cellWidth + view.cellSpacing.x * (view.LineCount - 1), view.alignmentType != AlignmentType.Center ? view.Bounds.width : 0),
                        (view.cellHeight + view.cellSpacing.y) * view.perLineCount - view.cellSpacing.y);
            }
            return Vector2.zero;
        }
        /// <summary>
        /// 设置视口的锚点
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="view"></param>
        internal static void SetViewPort(MoveType moveType,LoopScrollView view)
        {
            view.viewport.pivot = new Vector2(0.5f, 0.5f);
        }
        internal static void SetContentPivot(LoopScrollView view)
        {
            view.content.pivot = new Vector2(0.5f, 0.5f);
        }
        /// <summary>
        /// 获取Item位置
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="alignmentType"></param>
        /// <param name="view"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static Vector3 GetItemPositionByIndex(MoveType moveType, AlignmentType alignmentType, LoopScrollView view, int index)
        {
            Vector3 offset = GetOffset( view);
            int lineIndex = Mathf.FloorToInt(index / view.perLineCount);
            int chilrenIndex = index % view.perLineCount;
            switch (moveType)
            {
                case MoveType.Top2Bottom:
                    return new Vector3((-view.content.sizeDelta.x / 2 + view.cellWidth / 2 + (view.cellWidth + view.cellSpacing.x) * (chilrenIndex)),
                            -lineIndex * (view.cellHeight + view.cellSpacing.y) - view.cellHeight / 2, 0)
                            + new Vector3(0, offset.y, 0);
                case MoveType.Bottom2Top:
                    return new Vector3((-view.content.sizeDelta.x / 2 + view.cellWidth / 2 + (view.cellWidth + view.cellSpacing.x) * (chilrenIndex)),
                            lineIndex * (view.cellHeight + view.cellSpacing.y) + view.cellHeight / 2, 0)
                            - new Vector3(0, offset.y, 0);
                case MoveType.Left2Right:
                    return new Vector3(lineIndex * (view.cellWidth + view.cellSpacing.x) + view.cellWidth / 2
                        , (view.content.sizeDelta.y / 2 - view.cellHeight / 2 - (view.cellHeight + view.cellSpacing.y) * (chilrenIndex)), 0)
                        - new Vector3(offset.x, 0, 0);
                case MoveType.Right2Left:
                    return new Vector3(-lineIndex * (view.cellWidth + view.cellSpacing.x) - view.cellWidth / 2
                        , (view.content.sizeDelta.y / 2 - view.cellHeight / 2 - (view.cellHeight + view.cellSpacing.y) * (chilrenIndex)), 0)
                        + new Vector3(offset.x, 0, 0);
            }
            return Vector3.zero;
        }
        internal static Vector3 InitContentPosition(LoopScrollView view)
        {
            if (view.moveType == MoveType.Left2Right||view.moveType==MoveType.Right2Left)
            {
                switch (view.alignmentType)
                {
                    case AlignmentType.Top:
                        return new Vector3(0, view.Bounds.height / 2 - view.content.sizeDelta.y / 2, 0);
                    case AlignmentType.Bottom:
                        return -new Vector3(0, view.Bounds.height / 2 - view.content.sizeDelta.y / 2, 0);
                }
            }
            else
            {
                switch (view.alignmentType)
                {
                    case AlignmentType.Left:
                        return -new Vector3(view.Bounds.width / 2 - view.content.sizeDelta.x / 2, 0, 0);
                    case AlignmentType.Right:
                        return new Vector3(view.Bounds.width / 2 - view.content.sizeDelta.x / 2, 0, 0);
                }
            }
            return Vector3.zero;
        }
        private static Vector3 GetOffset( LoopScrollView view)
        {
            return view.content.sizeDelta / 2;
        }
        /// <summary>
        /// 获取当前首个Item索引
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        internal static int GetCurFirstIndex(MoveType moveType, LoopScrollView view)
        {
            Vector3 offset = GetOffset(view);
            switch (moveType)
            {
                case MoveType.Top2Bottom:
                    return (int)((view.content.localPosition.y+offset.y+view.Bounds.y) / (view.cellHeight + view.cellSpacing.y))*(view.perLineCount);
                case MoveType.Bottom2Top:
                    return (int)((-view.content.localPosition.y + offset.y + view.Bounds.y) / (view.cellHeight + view.cellSpacing.y)) * (view.perLineCount);
                case MoveType.Left2Right:
                    return (int)((-view.content.localPosition.x + offset.x + view.Bounds.x) / (view.cellWidth + view.cellSpacing.x)) * (view.perLineCount);
                case MoveType.Right2Left:
                    return (int)((view.content.localPosition.x + offset.x + view.Bounds.x) / (view.cellWidth + view.cellSpacing.x)) * (view.perLineCount);
            }
            return 0;
        }
        /// <summary>
        /// 获取Content的位置
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="alignmentType"></param>
        /// <param name="view"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static Vector3 GetContentPositionByIndex(MoveType moveType, AlignmentType alignmentType, LoopScrollView view,int index)
        {
            if (view.LineCount < (int)view.Bounds.y/(view.cellHeight+view.cellHeight)||alignmentType==AlignmentType.Center) return Vector3.zero;//实际的content范围小于视口范围
            Vector3 itemPos = GetItemPositionByIndex(moveType, alignmentType, view, index);
            switch (moveType)
            {
                case MoveType.Top2Bottom:
                case MoveType.Bottom2Top:
                    return -((itemPos.y + view.cellHeight / 2) * Vector3.up) + InitContentPosition(view);
                case MoveType .Left2Right:
                case MoveType.Right2Left:
                    return (-itemPos.x + view.cellWidth / 2) * Vector3.right + InitContentPosition(view);
            }
            return Vector3.zero;
        }
        /// <summary>
        /// 获取视口区域
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="scrollRect"></param>
        /// <returns></returns>
        internal static Rect GetViewBounds(RectTransform viewport, ScrollRect scrollRect)
        {
            Vector3[] cornerPos = new Vector3[4];
            viewport.GetWorldCorners(cornerPos);
            Vector3 Pos_LB = viewport.parent.InverseTransformPoint(cornerPos[0]);
            Vector3 Pos_RT = viewport.parent.InverseTransformPoint(cornerPos[2]);
            Vector3 offset = Pos_RT - Pos_LB;
            return new Rect(Pos_LB.x, Pos_LB.y, offset.x, offset.y);
        }
    }
}

