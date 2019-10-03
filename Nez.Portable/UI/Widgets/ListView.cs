using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez.BitmapFonts;

namespace Nez.UI
{
    public interface IListViewFilter
    {
        bool isValid(string searchFilter, object userData);
    }

    public class ListView<T> : Element, IInputListener where T : Element
    {
        public event Action<T> onChanged;

        ListViewStyle _style;
        List<T> _Items = new List<T>();
        List<T> _filteredItems = new List<T>();
        ArraySelection<T> _selection;
        float _prefWidth, _prefHeight;
        float _itemHeight;
        float _offsetX, _offsetY;
        Rectangle _cullingArea;
        int _hoveredItemIndex = -1;
        bool _isMouseOverList;

        public IListViewFilter filter;

        public ListView(Skin skin) : this(skin.Get<ListViewStyle>())
        {
        }

        public ListView(Skin skin, string styleName) : this(skin.Get<ListViewStyle>(styleName))
        {
        }

        public ListView(ListViewStyle style)
        {
            _selection = new ArraySelection<T>(_Items);
            _selection.SetElement(this);
            _selection.SetRequired(true);

            setStyle(style);
            SetSize(PreferredWidth, PreferredHeight);
        }


        #region ILayout

        public override float PreferredWidth
        {
            get
            {
                Validate();
                return _prefWidth;
            }
        }

        public override float PreferredHeight
        {
            get
            {
                Validate();
                return _prefHeight;
            }
        }

        #endregion


        #region IInputListener

        void IInputListener.OnMouseEnter()
        {
            _isMouseOverList = true;
        }


        void IInputListener.OnMouseExit()
        {
            _isMouseOverList = false;
            _hoveredItemIndex = -1;
        }


        bool IInputListener.OnMousePressed(Vector2 mousePos)
        {
            if (_selection.IsDisabled() || _filteredItems.Count == 0)
                return false;

            var lastSelectedItem = _selection.GetLastSelected();
            var index = getItemIndexUnderMousePosition(mousePos);
            index = System.Math.Max(0, index);
            index = System.Math.Min(_filteredItems.Count - 1, index);
            _selection.Choose(_filteredItems[index]);

            if (lastSelectedItem != _filteredItems[index] && onChanged != null)
                onChanged(_filteredItems[index]);

            return true;
        }

        void IInputListener.OnMouseMoved(Vector2 mousePos)
        {
        }


        void IInputListener.OnMouseUp(Vector2 mousePos)
        {
        }


        bool IInputListener.OnMouseScrolled(int mouseWheelDelta)
        {
            return false;
        }


        int getItemIndexUnderMousePosition(Vector2 mousePos)
        {
            if (_selection.IsDisabled() || _filteredItems.Count == 0)
                return -1;

            var top = 0f;
            if (_style.background != null)
            {
                top += _style.background.TopHeight + _style.background.BottomHeight;
                mousePos.Y += _style.background.BottomHeight;
            }

            var index = (int) ((top + mousePos.Y) / _itemHeight);
            if (index < 0 || index > _filteredItems.Count - 1)
                return -1;

            return index;
        }

        #endregion


        public override void Layout()
        {
            IDrawable selectedDrawable = _style.selection;

            _prefWidth = 0;
            for (var i = 0; i < _Items.Count; i++)
            {
                var e = _Items[i] as Element;
                if (e == null) continue;
                _prefWidth = System.Math.Max(e.PreferredWidth, _prefWidth);
                _itemHeight = System.Math.Max(e.PreferredHeight, _itemHeight);
            }

            _prefWidth += selectedDrawable.LeftWidth + selectedDrawable.RightWidth;
            _prefHeight = _Items.Count * _itemHeight;

            var background = _style.background;
            if (background != null)
            {
                _prefWidth += background.LeftWidth + background.RightWidth;
                _prefHeight += background.TopHeight + background.BottomHeight;
            }

            for (int i = 0, n = _Items.Count; i < n; i++)
            {
                var child = _Items[i];
                if (child is ILayout)
                    ((ILayout)child).Validate();
            }
        }


        public void filterItems(string filterText)
        {
            _filteredItems.Clear();

            if (filterText.Length == 0)
            {
                _filteredItems.AddRange(_Items);
                return;
            }
                

            for (int i = 0; i < _Items.Count; i++)
            {
                var item = _Items[i] as Element;

                if (filter.isValid(filterText, item.UserData))
                    _filteredItems.Add(item as T);
            }
        }

        public override void Draw(Batcher batcher, float parentAlpha)
        {
            // update our hoved item if the mouse is over the list
            if (_isMouseOverList)
            {
                var mousePos = ScreenToLocalCoordinates(_stage.GetMousePosition());
                _hoveredItemIndex = getItemIndexUnderMousePosition(mousePos);
            }

            Validate();

            var selectedDrawable = _style.selection;

            var color = GetColor();
            color = new Color(color, (int) (color.A * parentAlpha));

            float x = GetX(), y = GetY(), width = GetWidth(), height = GetHeight();
            var itemY = 0f;

            var background = _style.background;
            if (background != null)
            {
                background.Draw(batcher, x, y, width, height, color);
                var LeftWidth = background.LeftWidth;
                x += LeftWidth;
                itemY += background.TopHeight;
                width -= LeftWidth + background.RightWidth;
            }

            _cullingArea = new RectangleF(0, 0, parent.GetWidth(), parent.GetHeight());
            for (var i = 0; i < _filteredItems.Count; i++)
            {
                var item = _filteredItems[i];
                var e = item as Element;

                var pos = new Vector2(x + _offsetX, y + itemY + _offsetY + _itemHeight / 2);

                var selected = _selection.Contains(item);
                if (selected)
                {
                    selectedDrawable.Draw(batcher, x, y + itemY, width, _itemHeight, color);
                }
                else if (i == _hoveredItemIndex && _style.hoverSelection != null)
                {
                    _style.hoverSelection.Draw(batcher, x, y + itemY, width, _itemHeight, color);
                }

                //if (_cullingArea.Contains(pos))
                //{
                    e.SetPosition(pos.X, pos.Y- _itemHeight/2);
                    e.Draw(batcher, parentAlpha);
                //}

                itemY += _itemHeight;
            }
        }


        #region config

        public ListView<T> setStyle(ListViewStyle style)
        {
            Insist.IsNotNull(style, "style cannot be null");
            _style = style;
            InvalidateHierarchy();
            return this;
        }

        public ListViewStyle getStyle()
        {
            return _style;
        }


        public ArraySelection<T> getSelection()
        {
            return _selection;
        }

        public T getSelected()
        {
            return _selection.First();
        }

        public ListView<T> setSelected(T item)
        {
            if (_Items.Contains(item))
                _selection.Set(item);
            else if (_selection.GetRequired() && _Items.Count > 0)
                _selection.Set(_Items[0]);
            else
                _selection.Clear();

            return this;
        }

        public int getSelectedIndex()
        {
            var selected = _selection.Items();
            return selected.Count == 0 ? -1 : _Items.IndexOf(selected[0]);
        }

        public ListView<T> setSelectedIndex(int index)
        {
            Insist.IsFalse(index < -1 || index >= _Items.Count,
                "index must be >= -1 and < " + _Items.Count + ": " + index);

            if (index == -1)
                _selection.Clear();
            else
                _selection.Set(_Items[index]);

            return this;
        }


        public ListView<T> setItems(params T[] newItems)
        {
            setItems(new List<T>(newItems));
            return this;
        }

        public ListView<T> setItems(IList<T> newItems)
        {
            Insist.IsNotNull(newItems, "newItems cannot be null");
            float oldPrefWidth = _prefWidth, oldPrefHeight = _prefHeight;

            _Items.Clear();
            _Items.AddRange(newItems);
            _selection.Validate();

            Invalidate();
            Validate();
            if (oldPrefWidth != _prefWidth || oldPrefHeight != _prefHeight)
            {
                InvalidateHierarchy();
                SetSize(_prefWidth, _prefHeight);
            }
            _filteredItems.AddRange(_Items);
            return this;
        }


        public void clearItems()
        {
            if (_Items.Count == 0)
                return;

            _Items.Clear();
            _filteredItems.Clear();
            _selection.Clear();
            InvalidateHierarchy();
        }

        public List<T> getItems()
        {
            return _Items;
        }

        public void AddItem(T item)
        {
            float oldPrefWidth = _prefWidth, oldPrefHeight = _prefHeight;
            _Items.Add(item);
            _filteredItems.Clear();
            Invalidate();
            Validate();
            if (oldPrefWidth != _prefWidth || oldPrefHeight != _prefHeight)
            {
                InvalidateHierarchy();
                SetSize(_prefWidth, _prefHeight);
            }
            _filteredItems.AddRange(_Items);
        }


        public float getItemHeight()
        {
            return _itemHeight;
        }


        public ListView<T> setCullingArea(Rectangle cullingArea)
        {
            _cullingArea = cullingArea;
            return this;
        }

        #endregion
    }

    public class ListViewStyle
    {
        public IDrawable selection;

        /** Optional */
        public IDrawable hoverSelection;

        /** Optional */
        public IDrawable background;

        public ListViewStyle()
        {
        }

        public ListViewStyle(BitmapFont font, Color fontColorSelected, Color fontColorUnselected, IDrawable selection)
        {
            this.selection = selection;
        }

        public ListViewStyle clone()
        {
            return new ListViewStyle
            {
                selection = selection,
                hoverSelection = hoverSelection,
                background = background
            };
        }
    }
}