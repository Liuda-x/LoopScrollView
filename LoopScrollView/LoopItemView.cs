using UnityEngine;

namespace Liudax.LoopScrollView
{
    public interface IElementLife
    {
        public void ItemInit();
        public void ItemDisable();
        public void ItemUpdate();
        public void ItemDestory();
        public void ItemEnable();
    }
    public abstract class LoopItemView<T> : LoopItemView
    {
        public new T Data { get;private set; }
        public override void SetData(object data,int index)
        {
            Data = (T)data;
            this.Index = index;
            this.gameObject.name = index.ToString();
        } 
    }
    public abstract class LoopItemView : MonoBehaviour, IElementLife
    {
        public int Index { get; protected set; }
        public object Data { get; protected set; }
        public virtual void SetData(object data, int index)
        {
            Data = data;
            this.Index = index;
            this.gameObject.name = index.ToString();
        }
        public virtual void ItemInit()
        {
        }
        /// <summary>
        /// 数据更新
        /// </summary>
        public abstract void ItemUpdate();
        public virtual void ItemDisable()
        {
        }
        public virtual void ItemDestory()
        {
        }
        public virtual void ItemEnable()
        {
        }
    }
}
