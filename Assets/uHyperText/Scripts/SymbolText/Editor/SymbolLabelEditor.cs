using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WXB
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SymbolLabel), true)]
    public class SymbolLabelEditor : SymbolTextEditor
    {
        protected SerializedProperty m_MaxElement;

        protected override void OnGUIFontData()
        {
            EditorGUILayout.PropertyField(m_FontData);
        }
        protected override void OnGUIOther()
        {
            SymbolText st = target as SymbolText;
            // modify by Johance 此处需要检测值是否修改了。 主要是内部isArabic没有做数值变化的检测。
            if (EditorGUILayout.Toggle("Arabic Mode", st.isArabic) != st.isArabic)
            {
                st.isArabic = !st.isArabic;
            }

            if (m_MaxElement == null)
                m_MaxElement = serializedObject.FindProperty("m_MaxElement");

            EditorGUILayout.PropertyField(m_MaxElement);
        }
    }
}