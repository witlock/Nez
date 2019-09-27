using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace Nez.UI
{
<<<<<<< HEAD
    public class TabPane : Table
    {
        public Tab currentTab;
        public List<Tab> tabs;
        public List<TabButton> tabButtons;
        private TabWindowStyle _style;
        private Table _buttonsTable;
        private Table _tabTable;

        public TabPane( TabWindowStyle style )
        {
            _style = style;
            init();
        }

        private void init()
        {
            setSize( 100, 100 );

            setBackground( _style.background );

            top().left();

            tabs = new List<Tab>();
            tabButtons = new List<TabButton>();

            _buttonsTable = new Table();
            _buttonsTable.setFillParent( true );
            _buttonsTable.top().left();
            _tabTable = new Table();
            _tabTable.top().left();

            row();
            add( _buttonsTable ).fill().setExpandX();
            row();
            add( _tabTable ).fill().setExpandY();
        }

        public void addTab( Tab tab )
        {
            tabs.Add( tab );

            var tabBtn = new TabButton( tab, _style.tabButtonStyle );
            tabBtn.onClick += () =>
            {
                setActiveTab( tabBtn.getTab() );
            };
            tabButtons.Add( tabBtn );
            _buttonsTable.add( tabBtn );

            if( tabs.Count == 1 )
            {
                currentTab = tabs[0];
                _tabTable.add( tab ).left().top().fill().expand();

                tabBtn.toggleOn();
            }

            if( tabs.Count == 1 )
                setActiveTab( 0 );
        }

        public void setActiveTab( int index )
        {
            var tab = tabs[index];
            if( tab != currentTab )
            {
                _tabTable.clear();
                _tabTable.add( tab ).left().top().fill().expand();

                tabButtons[index].toggleOn();

                var i = tabs.IndexOf( currentTab );
                tabButtons[i].toggleOff();

                currentTab = tab;
            }
        }

        protected void setActiveTab( Tab tab )
        {
            var i = tabs.IndexOf( tab );
            setActiveTab( i );
        }
    }

    public class TabWindowStyle
    {
        public IDrawable background;
        public TabButtonStyle tabButtonStyle;
    }

    public class Tab : Table
    {
        private TabStyle _style;
        public string tabName;

        public Tab( string name, TabStyle style )
        {
            tabName = name;
            _style = style;
            setTouchable( Touchable.Enabled );
            setup();
        }

        private void setup()
        {
            setBackground( _style.background );
            setFillParent( true );
            top().left();
        }
    }

    public class TabStyle
    {
        public IDrawable background;
    }

    public class TabButton : Table, IInputListener
    {
        public enum TabButtonState
        {
            Inactive,
            Active,
            Locked
        }

        private TabButtonState state = TabButtonState.Inactive;

        private Label text;
        private TabButtonStyle style;
        private string tabName;
        private Tab tab;

        public Action onClick;

        public Label GetLabel()
        {
            return text;
        }

        public TabButton( Tab tab, TabButtonStyle style )
        {
            this.style = style;
            tabName = tab.tabName;
            this.tab = tab;
            init();
        }

        private void init()
        {
            setTouchable( Touchable.Enabled );
            text = new Label( tabName, style.labelStyle );
            add( text ).setFillX().pad( 8 );
            setBackground( style.inactive );
            padTop( style.paddingTop );
        }

        public string getTabeName()
        {
            return tabName;
        }

        public Tab getTab()
        {
            return tab;
        }

        public bool isSwitchedOn()
        {
            return state == TabButtonState.Active;
        }

        public void toggle()
        {
            if( state != TabButtonState.Locked )
            {
                if( state == TabButtonState.Active )
                {
                    state = TabButtonState.Inactive;
                    setBackground( style.inactive );
                }
                else
                {
                    state = TabButtonState.Active;
                    setBackground( style.active );
                }
            }
        }

        public void toggleOff()
        {
            if( state != TabButtonState.Locked )
            {
                state = TabButtonState.Inactive;

                setBackground( style.inactive );
            }
        }

        public void toggleOn()
        {
            if( state != TabButtonState.Locked )
            {
                state = TabButtonState.Active;

                setBackground( style.active );
            }
        }

        public void toggleLock()
        {
            if( state != TabButtonState.Inactive )
            {
                if( state == TabButtonState.Active )
                {
                    state = TabButtonState.Locked;
                    setBackground( style.locked );
                }
                else
                {
                    state = TabButtonState.Active;
                    setBackground( style.active );
                }
            }
        }

        public void unlock()
        {
            if( state == TabButtonState.Locked )
            {
                state = TabButtonState.Active;
                setBackground( style.active );
            }
        }

        void IInputListener.onMouseEnter()
        {
            if( state == TabButtonState.Inactive )
            {
                setBackground( style.hover );
            }
        }

        void IInputListener.onMouseExit()
        {
            if( state == TabButtonState.Inactive )
            {
                setBackground( style.inactive );
            }
        }

        bool IInputListener.onMousePressed( Vector2 mousePos )
        {
            return true;
        }

        void IInputListener.onMouseMoved( Vector2 mousePos )
        {
        }

        void IInputListener.onMouseUp( Vector2 mousePos )
        {
            onClick?.Invoke();
        }

        bool IInputListener.onMouseScrolled( int mouseWheelDelta )
        {
            return true;
        }
    }

    public class TabButtonStyle
    {
        public IDrawable active;
        public IDrawable inactive;
        public IDrawable locked;
        public IDrawable hover;
        public float paddingTop = 0.0F;
        public LabelStyle labelStyle;
    }
=======
	public class TabPane : Table
	{
		public Tab CurrentTab;
		public List<Tab> Tabs;
		public List<TabButton> TabButtons;
		private TabWindowStyle _style;
		private Table _buttonsTable;
		private Table _tabTable;

		public TabPane(TabWindowStyle style)
		{
			_style = style;
			Init();
		}

		private void Init()
		{
			SetSize(100, 100);

			SetBackground(_style.Background);

			Top().Left();

			Tabs = new List<Tab>();
			TabButtons = new List<TabButton>();

			_buttonsTable = new Table();
			_buttonsTable.SetFillParent(true);
			_buttonsTable.Top().Left();
			_tabTable = new Table();
			_tabTable.Top().Left();

			Row();
			Add(_buttonsTable).Fill().SetExpandX();
			Row();
			Add(_tabTable).Fill().SetExpandY();
		}

		public void AddTab(Tab tab)
		{
			Tabs.Add(tab);

			var tabBtn = new TabButton(tab, _style.TabButtonStyle);
			tabBtn.OnClick += () => { SetActiveTab(tabBtn.GetTab()); };
			TabButtons.Add(tabBtn);
			_buttonsTable.Add(tabBtn);

			if (Tabs.Count == 1)
			{
				CurrentTab = Tabs[0];
				_tabTable.Add(tab).Left().Top().Fill().Expand();

				tabBtn.ToggleOn();
			}

			if (Tabs.Count == 1)
				SetActiveTab(0);
		}

		public void SetActiveTab(int index)
		{
			var tab = Tabs[index];
			if (tab != CurrentTab)
			{
				_tabTable.Clear();
				_tabTable.Add(tab).Left().Top().Fill().Expand();

				TabButtons[index].ToggleOn();

				var i = Tabs.IndexOf(CurrentTab);
				TabButtons[i].ToggleOff();

				CurrentTab = tab;
			}
		}

		protected void SetActiveTab(Tab tab)
		{
			var i = Tabs.IndexOf(tab);
			SetActiveTab(i);
		}
	}

	public class TabWindowStyle
	{
		public IDrawable Background;
		public TabButtonStyle TabButtonStyle;
	}

	public class Tab : Table
	{
		private TabStyle _style;
		public string TabName;

		public Tab(string name, TabStyle style)
		{
			TabName = name;
			_style = style;
			SetTouchable(Touchable.Enabled);
			Setup();
		}

		private void Setup()
		{
			SetBackground(_style.Background);
			SetFillParent(true);
			Top().Left();
		}
	}

	public class TabStyle
	{
		public IDrawable Background;
	}

	public class TabButton : Table, IInputListener
	{
		public enum TabButtonState
		{
			Inactive,
			Active,
			Locked
		}

		private TabButtonState state = TabButtonState.Inactive;

		private Label text;
		private TabButtonStyle style;
		private string tabName;
		private Tab tab;

		public Action OnClick;

		public TabButton(Tab tab, TabButtonStyle style)
		{
			this.style = style;
			tabName = tab.TabName;
			this.tab = tab;
			Init();
		}

		private void Init()
		{
			SetTouchable(Touchable.Enabled);
			text = new Label(tabName, style.LabelStyle);
			Add(text).SetFillX().Pad(8);
			SetBackground(style.Inactive);
			PadTop(style.PaddingTop);
		}

		public string GetTabeName()
		{
			return tabName;
		}

		public Tab GetTab()
		{
			return tab;
		}

		public bool IsSwitchedOn()
		{
			return state == TabButtonState.Active;
		}

		public void Toggle()
		{
			if (state != TabButtonState.Locked)
			{
				if (state == TabButtonState.Active)
				{
					state = TabButtonState.Inactive;
					SetBackground(style.Inactive);
				}
				else
				{
					state = TabButtonState.Active;
					SetBackground(style.Active);
				}
			}
		}

		public void ToggleOff()
		{
			if (state != TabButtonState.Locked)
			{
				state = TabButtonState.Inactive;

				SetBackground(style.Inactive);
			}
		}

		public void ToggleOn()
		{
			if (state != TabButtonState.Locked)
			{
				state = TabButtonState.Active;

				SetBackground(style.Active);
			}
		}

		public void ToggleLock()
		{
			if (state != TabButtonState.Inactive)
			{
				if (state == TabButtonState.Active)
				{
					state = TabButtonState.Locked;
					SetBackground(style.Locked);
				}
				else
				{
					state = TabButtonState.Active;
					SetBackground(style.Active);
				}
			}
		}

		public void Unlock()
		{
			if (state == TabButtonState.Locked)
			{
				state = TabButtonState.Active;
				SetBackground(style.Active);
			}
		}

		void IInputListener.OnMouseEnter()
		{
			if (state == TabButtonState.Inactive)
			{
				SetBackground(style.Hover);
			}
		}

		void IInputListener.OnMouseExit()
		{
			if (state == TabButtonState.Inactive)
			{
				SetBackground(style.Inactive);
			}
		}

		bool IInputListener.OnMousePressed(Vector2 mousePos)
		{
			return true;
		}

		void IInputListener.OnMouseMoved(Vector2 mousePos)
		{
		}

		void IInputListener.OnMouseUp(Vector2 mousePos)
		{
			OnClick?.Invoke();
		}

		bool IInputListener.OnMouseScrolled(int mouseWheelDelta)
		{
			return true;
		}
	}

	public class TabButtonStyle
	{
		public IDrawable Active;
		public IDrawable Inactive;
		public IDrawable Locked;
		public IDrawable Hover;
		public float PaddingTop = 0.0F;
		public LabelStyle LabelStyle;
	}
>>>>>>> 65d2f2cd2bfe95907f48a501bc8573e636285026
}