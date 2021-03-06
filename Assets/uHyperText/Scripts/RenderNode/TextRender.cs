﻿using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace WXB
{
    public partial class TextNode : NodeBase
    {
        struct Helper
        {
            public RenderCache cache;
            public float x;
            public uint yline;
            public List<Line> lines;
            public Anchor xFormatting;
            public float offsetX;
            public float offsetY;
            public StringBuilder sb;
            
            public Helper(float maxWidth, RenderCache cache, float x, uint yline, List<Line> lines, Anchor xFormatting, float offsetX, float offsetY, StringBuilder sb)
            {
                this.maxWidth = maxWidth;
                this.cache = cache;
                this.x = x;
                this.yline = yline;
                this.lines = lines;
                this.xFormatting = xFormatting;
                this.offsetX = offsetX;
                this.offsetY = offsetY;

                pixelsPerUnit = 1f;
                alignedX = 0;
                pt = Vector2.zero;
                    node = null;
                fHeight = 0f;

                this.sb = sb;
            }

            public float pixelsPerUnit;

            Vector2 pt;
            float alignedX;

            TextNode node;

            float maxWidth;

            float fHeight;

            void DrawCurrent(bool isnewLine, Around around)
            {
                if(node.owner.isArabic)
                {
                    if (sb.Length != 0)
                    {
                        float width = pt.x - x - offsetX;
                        Rect area_rect = new Rect(x - alignedX, pt.y, width, node.getHeight());

                        cache.cacheText(lines[(int)yline], node, sb.ToString(), area_rect);

                        sb.Remove(0, sb.Length);
                    }

                    if (isnewLine)
                    {
                        // 再换行
                        yline++;
                        x = maxWidth;

                        pt.x = x - offsetX;
                        pt.y = offsetY;
                        for (int n = 0; n < yline; ++n)
                            pt.y += lines[n].y;

                        if (yline >= lines.Count)
                        {
                            --yline;
                            //Debug.LogError("yline >= vLineSize.Count!yline:" + yline + " vLineSize:" + lines.Count);
                        }

                        float curentWidth = lines[(int)(yline)].x;
                        alignedX = AlignedFormatting(node.owner, xFormatting, maxWidth, curentWidth);
                        alignedX = maxWidth - (alignedX + curentWidth);

                        float newx;
                        if (!around.isContain(pt.x + alignedX, pt.y, 1, node.getHeight(), out newx, true))
                        {
                            pt.x = newx - alignedX;
                            x = pt.x;
                        }
                    }
                }
                else
                {
                    if (sb.Length != 0)
                    {
                        Rect area_rect = new Rect(pt.x + alignedX, pt.y, x - pt.x + offsetX, node.getHeight());

                        cache.cacheText(lines[(int)yline], node, sb.ToString(), area_rect);

                        sb.Remove(0, sb.Length);
                    }

                    if (isnewLine)
                    {
                        // 再换行
                        yline++;
                        x = 0.0f;

                        pt.x = offsetX;
                        pt.y = offsetY;
                        for (int n = 0; n < yline; ++n)
                            pt.y += lines[n].y;

                        if (yline >= lines.Count)
                        {
                            --yline;
                            //Debug.LogError("yline >= vLineSize.Count!yline:" + yline + " vLineSize:" + lines.Count);
                        }

                        alignedX = AlignedFormatting(node.owner, xFormatting, maxWidth, lines[(int)(yline)].x);

                        float newx;
                        if (!around.isContain(pt.x + alignedX, pt.y, 1, node.getHeight(), out newx))
                        {
                            pt.x = newx - alignedX;
                            x = pt.x;
                        }
                    }

                }
            }

            public void Draw(TextNode n)
            {
                node = n;
                if (n.owner.isArabic)
                {
                    pt = new Vector2(x - offsetX, offsetY);
                    for (int i = 0; i < yline; ++i)
                        pt.y += lines[i].y;

                    if (maxWidth == 0)
                        return;

                    float curentWidth = lines[(int)(yline)].x;
                    alignedX = AlignedFormatting(n.owner, xFormatting, maxWidth, curentWidth);
                    alignedX = maxWidth - (alignedX + curentWidth);
                    fHeight = node.getHeight();

                    sb.Remove(0, sb.Length);

                    Around around = n.owner.around;

                    int textindex = node.d_text.Length-1;
                    float newx = maxWidth;
                    for (int k = node.d_widthList.Count-1; k >= 0 ; --k)
                    {
                        Element e = node.d_widthList[k];
                        float totalwidth = e.totalwidth;
                        if ((x - totalwidth) < 0)
                        {
                            if (x != maxWidth)
                            {
                                DrawCurrent(true, around);
                            }

                            if (e.widths == null)
                            {
                                if ((x - e.totalwidth < 0))
                                {
                                    DrawCurrent(true, around);
                                }
                                else
                                {
                                    x -= e.totalwidth;
                                    sb.Insert(0, node.d_text[textindex--]);
                                }
                            }
                            else
                            {
                                for (int m = 0; m < e.widths.Count;)
                                {
                                    if (x != maxWidth && x - e.widths[m] < 0)
                                    {
                                        DrawCurrent(true, around);
                                    }
                                    else
                                    {
                                        x -= e.widths[m];
                                        sb.Insert(0, node.d_text[textindex--]);
                                        ++m;
                                    }
                                }
                            }
                        }
                        else if (!around.isContain(x, pt.y, totalwidth, fHeight, out newx, true))
                        {
                            DrawCurrent(false, around);

                            x = newx;
                            pt.x = newx;
                            ++k;
                        }
                        else
                        {
                            int ec = e.count;
                            sb.Insert(0, node.d_text.Substring(textindex+1-ec, ec));
                            textindex -= ec;
                            x -= totalwidth;
                        }
                    }

                    if (sb.Length != 0)
                    {
                        DrawCurrent(false, around);
                    }

                    if (node.d_bNewLine == true)
                    {
                        yline++;
                        x = maxWidth;
                    }
                }
                else
                {
                    pt = new Vector2(x + offsetX, offsetY);
                    for (int i = 0; i < yline; ++i)
                        pt.y += lines[i].y;

                    if (maxWidth == 0)
                        return;

                    alignedX = AlignedFormatting(n.owner, xFormatting, maxWidth, lines[(int)(yline)].x);
                    fHeight = node.getHeight();

                    sb.Remove(0, sb.Length);

                    Around around = n.owner.around;

                    int textindex = 0;
                    float newx = 0f;
                    for (int k = 0; k < node.d_widthList.Count; ++k)
                    {
                        Element e = node.d_widthList[k];
                        float totalwidth = e.totalwidth;
                        if ((x + totalwidth) > maxWidth)
                        {
                            if (x != 0f)
                            {
                                DrawCurrent(true, around);
                            }

                            if (e.widths == null)
                            {
                                if ((x + e.totalwidth > maxWidth))
                                {
                                    DrawCurrent(true, around);
                                }
                                else
                                {
                                    x += e.totalwidth;
                                    sb.Append(node.d_text[textindex++]);
                                }
                            }
                            else
                            {
                                for (int m = 0; m < e.widths.Count;)
                                {
                                    if (x != 0 && x + e.widths[m] > maxWidth)
                                    {
                                        DrawCurrent(true, around);
                                    }
                                    else
                                    {
                                        x += e.widths[m];
                                        sb.Append(node.d_text[textindex++]);
                                        ++m;
                                    }
                                }
                            }
                        }
                        else if (!around.isContain(x, pt.y, totalwidth, fHeight, out newx))
                        {
                            DrawCurrent(false, around);

                            x = newx;
                            pt.x = newx;
                            --k;
                        }
                        else
                        {
                            int ec = e.count;
                            sb.Append(node.d_text.Substring(textindex, ec));
                            textindex += ec;
                            x += totalwidth;
                        }
                    }

                    if (sb.Length != 0)
                    {
                        DrawCurrent(false, around);
                    }

                    if (node.d_bNewLine == true)
                    {
                        yline++;
                        x = 0;
                    }
                }
            }
        }
    }
}
