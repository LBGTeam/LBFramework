using System;
using UnityEditor;
using UnityEngine;

namespace LBFramework.PackageKit
{
    /// <summary>
    /// 切分方式
    /// </summary>
    public enum SplitType
    {
        /// <summary>
        /// 纵向
        /// </summary>
        Vertical,
        /// <summary>
        /// 横向
        /// </summary>
        Horizontal
    }
    public class VerticalSplitView
    {
        private SplitType _splitType = SplitType.Vertical;
        private float _split = 200;
        public Action<Rect> fistPan, secondPan;
        public event Action onBeginResize;
        public event Action onEndResize;

        public bool dragging
        {
            get { return _resizing; }
            private set
            {
                if (_resizing != value)
                {
                    _resizing = value;
                    if (value)
                    {
                        if (onBeginResize != null)
                        {
                            onBeginResize();
                        }
                    }
                    else
                    {
                        if (onEndResize != null)
                        {
                            onEndResize();
                        }
                    }
                }
            }
        }

        private bool _resizing;

        public void OnGUI(Rect position)
        {
            var rs = position.Split(_splitType, _split, 4);
            var mid = position.SplitRect(_splitType, _split, 4);
            if (fistPan != null)
            {
                fistPan(rs[0]);
            }

            if (secondPan != null)
            {
                secondPan(rs[1]);
            }

            GUI.Box(mid, "");
            Event e = Event.current;
            if (mid.Contains(e.mousePosition))
            {
                if (_splitType == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeVertical);
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (mid.Contains(Event.current.mousePosition))
                    {
                        dragging = true;
                    }

                    break;
                case EventType.MouseDrag:
                    if (dragging)
                    {
                        switch (_splitType)
                        {
                            case SplitType.Vertical:
                                _split += Event.current.delta.x;
                                break;
                            case SplitType.Horizontal:
                                _split += Event.current.delta.y;
                                break;
                        }

                        _split = Mathf.Clamp(_split, 100, position.width - 100);
                    }

                    break;
                case EventType.MouseUp:
                    if (dragging)
                    {
                        dragging = false;
                    }

                    break;
            }
        }
    }
}