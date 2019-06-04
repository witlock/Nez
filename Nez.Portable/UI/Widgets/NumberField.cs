using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nez.BitmapFonts;

namespace Nez.UI
{
    public class NumberField : Table, IInputListener
    {
        public event Action<NumberField, float> onNumberChanged = delegate { };
        TextButton decrease;
        TextButton increase;
        TextField field;
        NumberFieldStyle style;
        private float minimum;
        private float maximum;
        private float number;
        private float step = 0.1f;

        public NumberField(float initial, float min, float max, float step, bool showButtons, NumberFieldStyle style)
        {
            this.style = style;
            defaults().space(0);
            SetMin(min);
            SetMax(max);
            SetStep(step);

            field = new TextField(initial.ToString(), this.style);
            field.setAlignment(Align.center);
            setNumber(initial);
            if (showButtons)
            {
                decrease = new TextButton("", style.DecreaseButtonStyle);
                
                increase = new TextButton("", style.IncreaseButtonStyle);
                increase.onClicked += button =>
                {
                    increaseNumber();
                };

                decrease.onClicked += button =>
                {
                    decreaseNumber();
                };

            }

            field.onTextChanged += (field, s) =>
            {
                if (float.TryParse(s, out float n))
                {
                    setNumber(n >= maximum ? maximum : n);
                }
                else
                {
                    setNumber(minimum);
                }

            };

            if (showButtons)
                add(decrease);

            add(field).fill().expand();

            if (showButtons)
                add(increase);

            //setSize(preferredWidth, preferredHeight);
        }

        private void increaseNumber()
        {
            if (number + step > maximum)
            {
                setNumber(maximum);
            }
            else
            {
                setNumber(Mathf.roundToNearest(number + step, step));
            }
        }

        private void decreaseNumber()
        {
            if (number - step < minimum)
            {
                setNumber(minimum);
            }
            else
            {
                setNumber(Mathf.roundToNearest(number - step, step));
            }
        }

        public TextButton getDecreaseButton()
        {
            return decrease;
        }

        public TextButton getIncreaseButton()
        {
            return increase;
        }

        public TextField getTextField()
        {
            return field;
        }

        public Cell getDecreaseButtonCell()
        {
            return getCell(decrease);
        }

        public Cell getIncreaseButtonCell()
        {
            return getCell(increase);
        }

        public Cell getNumberFieldCell()
        {
            return getCell(field);
        }

        public void setNumber(float value)
        {
            field.setTextForced(value.ToString());
            number = value;

            onNumberChanged(this, value);
        }

        public float getNumber()
        {
            return number;
        }

        public NumberField(float initial, float min, float max, float step, bool showButtons, Skin skin, string styleName = null) : this(initial, min, max, step, showButtons, skin.get<NumberFieldStyle>(styleName))
        {
        }

        public void SetMax(float max)
        {
            maximum = max;
        }
        public void SetStep(float value)
        {
            step = value;
        }
        public void SetMin(float min)
        {
            minimum = min;
        }

        public void onMouseEnter()
        {
            
        }

        public void onMouseExit()
        {
            
        }

        public bool onMousePressed(Vector2 mousePos)
        {
            return false;
        }

        public void onMouseMoved(Vector2 mousePos)
        {
            
        }

        public void onMouseUp(Vector2 mousePos)
        {
            
        }

        public bool onMouseScrolled(int mouseWheelDelta)
        {

            if(mouseWheelDelta > 0)
                increaseNumber();
            else
                decreaseNumber();

            return true;
        }
    }

    public class NumberFieldStyle : TextFieldStyle
    {
        /** Optional. */
        public IDrawable imageUp, imageDown, imageOver, imageChecked, imageCheckedOver, imageDisabled;

        public TextButtonStyle DecreaseButtonStyle;
        public TextButtonStyle IncreaseButtonStyle;

        public NumberFieldStyle()
        {
            font = Graphics.instance.bitmapFont;
        }


        public NumberFieldStyle(BitmapFont font, Color fontColor, IDrawable cursor, IDrawable selection, IDrawable background, TextButtonStyle decreaseButtonStyle, TextButtonStyle increaseButtonStyle) : base(font, fontColor, cursor, selection, background)
        {
            this.DecreaseButtonStyle = decreaseButtonStyle;
            this.IncreaseButtonStyle = increaseButtonStyle;
        }


        public TextFieldStyle clone()
        {
            return new TextFieldStyle
            {
                font = font,
                fontColor = fontColor,
                focusedFontColor = focusedFontColor,
                disabledFontColor = disabledFontColor,
                background = background,
                focusedBackground = focusedBackground,
                disabledBackground = disabledBackground,
                cursor = cursor,
                selection = selection,
                messageFont = messageFont,
                messageFontColor = messageFontColor
            };
        }
    }
}
