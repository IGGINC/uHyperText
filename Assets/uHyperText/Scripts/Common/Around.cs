using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WXB
{
    public class Around
    {
        List<Rect> m_Rects = new List<Rect>(); // 环绕的区别

        public void Add(Rect rect)
        {
            m_Rects.Add(rect);
        }

        public void Clear()
        {
            m_Rects.Clear();
        }

        public bool isContain(Rect rect, out float ox, bool bRtl = false)
        {
            if (m_Rects.Count == 0)
            {
                ox = 0f;
                return true;
            }

            return isContain(rect.x, rect.y, rect.width, rect.height, out ox, bRtl);
        }

        public bool isContain(float x, float y, float w, float h, out float ox, bool bRtl = false)
        {
            if (m_Rects.Count == 0)
            {
                ox = 0f;
                return true;
            }
            if (bRtl)
            {
                x = x - w;
            }
            Rect r = new Rect(x, y, w, h);
            for (int i = 0; i < m_Rects.Count; ++i)
            {
                if (m_Rects[i].Overlaps(r))
                {
                    ox = (m_Rects[i].xMin - 5f);
                    return false;
                }
            }

            ox = 0f;
            return true;
        }
    }
}