using UnityEditor;
using UnityEngine;
namespace Liudax.LoopScrollView
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LoopScrollView))]
    public class LoopScrollViewEditor : UnityEditor.Editor
    {
        public LoopScrollView LoopScrollView => target as LoopScrollView;
        private string indexDesc;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (LoopScrollView.scrollRect == null) return;
            EditorGUILayout.HelpBox($"Bounds：{LoopScrollView.Bounds}\n" +
                $"FirstIndex：{LoopScrollView.GetFirstIndex()}\n" +
                $"MaxShowCount：{LoopScrollView.MaxShowCount}", MessageType.Info);
            using (new EditorGUILayout.HorizontalScope())
            {
                indexDesc = GUILayout.TextField(indexDesc);
                if (GUILayout.Button("跳转"))
                {
                    LoopScrollView.MoveToIndex(int.Parse(indexDesc));
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("重置"))
                {
                    LoopScrollView.ResetBounds();
                }
            }
        }
    }
#endif
}

