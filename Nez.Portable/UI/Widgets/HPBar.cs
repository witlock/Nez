using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nez.UI
{
    public class HPBar : Element
    {
        #region properties and fields

        public event Action<float> onChanged;

        public bool disabled;

        public override float preferredWidth
        {
            get
            {
                if (_vertical)
                    return Math.Max(style.knob == null ? 0 : style.knob.minWidth, style.background != null ? style.background.minWidth : 0);
                else
                    return width;
            }
        }

        public override float preferredHeight
        {
            get
            {
                if (_vertical)
                    return height;
                else
                    return Math.Max(style.knob == null ? 0 : style.knob.minHeight, style.background != null ? style.background.minHeight : 0);
            }
        }

        public float min { get; protected set; }
        public float max { get; protected set; }

        public float stepSize
        {
            get { return _stepSize; }
            set { setStepSize(value); }
        }

        public float value
        {
            get { return _value; }
            set { setValue(value); }
        }

        public float[] snapValues;
        public float snapThreshold;
        public bool shiftIgnoresSnap;

        protected float _stepSize, _value;
        protected bool _vertical;
        protected float position;
        HPBarStyle style;

        #endregion


        public HPBar(float min, float max, float stepSize, bool vertical, HPBarStyle style)
        {
            Insist.isTrue(min < max, "min must be less than max");
            Insist.isTrue(stepSize > 0, "stepSize must be greater than 0");

            setStyle(style);
            this.min = min;
            this.max = max;
            this.stepSize = stepSize;
            _vertical = vertical;
            _value = this.min;

            setSize(preferredWidth, preferredHeight);
        }

        public HPBar(float min, float max, float stepSize, bool vertical, Skin skin, string styleName = null) : this(min, max, stepSize, vertical, skin.get<HPBarStyle>(styleName))
        { }

        public HPBar(Skin skin, string styleName = null) : this(0, 1, 0.01f, false, skin)
        { }


        public virtual void setStyle(HPBarStyle style)
        {
            this.style = style;
            invalidateHierarchy();
        }


        /// <summary>
        /// Returns the progress bar's style. Modifying the returned style may not have an effect until
        /// {@link #setStyle(HPBarStyle)} is called.
        /// </summary>
        /// <returns>The style.</returns>
        public HPBarStyle getStyle()
        {
            return style;
        }


        /// <summary>
        /// Sets the progress bar position, rounded to the nearest step size and clamped to the minimum and maximum values.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSnap">If set to <c>true</c> we ignore value snapping.</param>
        public HPBar setValue(float value, bool ignoreSnap = false)
        {
            if ((shiftIgnoresSnap && InputUtils.isShiftDown()) || ignoreSnap)
            {
                value = Mathf.clamp(value, min, max);
            }
            else
            {
                // if value is lower/higher than min/max then we're not rounding to avoid situation where we can't achieve those values
                if (value >= max)
                    value = max;
                else if (value <= min)
                    value = min;
                else
                    value = Mathf.clamp(Mathf.round(value / stepSize) * stepSize, min, max);

                value = snap(value);
            }

            if (value == _value)
                return this;

            _value = value;

            // fire changed event
            if (onChanged != null)
                onChanged(_value);

            return this;
        }


        public HPBar setStepSize(float stepSize)
        {
            _stepSize = stepSize;
            return this;
        }


        public HPBar setMinMax(float min, float max)
        {
            Insist.isTrue(min < max, "min must be less than max");
            this.min = min;
            this.max = max;
            _value = Mathf.clamp(_value, this.min, this.max);

            return this;
        }


        /// <summary>
        /// Sets stepSize to a value that will evenly divide this progress bar into specified amount of steps.
        /// </summary>
        /// <param name="totalSteps">Total amount of steps.</param>
        public HPBar setTotalSteps(int totalSteps)
        {
            Insist.isTrue(totalSteps != 0, "totalSteps cannot be equal to 0");
            _stepSize = Math.Abs((min - max) / totalSteps);
            return this;
        }


        protected virtual Nez.UI.IDrawable getKnobDrawable()
        {
            return (disabled && style.disabledKnob != null) ? style.disabledKnob : style.knob;
        }


        public override void draw(Graphics graphics, float parentAlpha)
        {
            var knob = getKnobDrawable();
            var bg = (disabled && style.disabledBackground != null) ? style.disabledBackground : style.background;
            var knobBefore = (disabled && style.disabledKnobBefore != null) ? style.disabledKnobBefore : style.knobBefore;
            var knobAfter = (disabled && style.disabledKnobAfter != null) ? style.disabledKnobAfter : style.knobAfter;

            var x = this.x;
            var y = this.y;
            var width = this.width;
            var height = this.height;
            var knobHeight = knob == null ? 0 : knob.minHeight;
            var knobWidth = knob == null ? 0 : knob.minWidth;
            var percent = getVisualPercent();
            var color = new Color(this.color, (int)(this.color.A * parentAlpha));

            if (_vertical)
            {
                var positionHeight = height;

                float bgTopHeight = 0;
                if (bg != null)
                {
                    bg.draw(graphics, x + (int)((width - bg.minWidth) * 0.5f), y, bg.minWidth, height, color);
                    bgTopHeight = bg.topHeight;
                    positionHeight -= bgTopHeight + bg.bottomHeight;
                }

                float knobHeightHalf = 0;
                if (min != max)
                {
                    if (knob == null)
                    {
                        knobHeightHalf = knobBefore == null ? 0 : knobBefore.minHeight * 0.5f;
                        position = (positionHeight - knobHeightHalf) * percent;
                        position = Math.Min(positionHeight - knobHeightHalf, position);
                    }
                    else
                    {
                        var bgBottomHeight = bg != null ? bg.bottomHeight : 0;
                        knobHeightHalf = knobHeight * 0.5f;
                        position = (positionHeight - knobHeight) * percent;
                        position = Math.Min(positionHeight - knobHeight, position) + bgBottomHeight;
                    }
                    position = Math.Max(0, position);
                }

                if (knobBefore != null)
                {
                    float offset = 0;
                    if (bg != null)
                        offset = bgTopHeight;
                    knobBefore.draw(graphics, x + ((width - knobBefore.minWidth) * 0.5f), y + offset, knobBefore.minWidth,
                        (int)(position + knobHeightHalf), color);
                }

                if (knobAfter != null)
                {
                    knobAfter.draw(graphics, x + ((width - knobAfter.minWidth) * 0.5f), y + position + knobHeightHalf,
                        knobAfter.minWidth, height - position - knobHeightHalf, color);
                }

                if (knob != null)
                    knob.draw(graphics, x + (int)((width - knobWidth) * 0.5f), (int)(y + position), knobWidth, knobHeight, color);
            }
            else
            {
                float positionWidth = width;

                float bgLeftWidth = 0;
                if (bg != null)
                {
                    bg.draw(graphics, x, y + (int)((height - bg.minHeight) * 0.5f), width, bg.minHeight, color);
                    //bgLeftWidth = bg.leftWidth;
                    //positionWidth -= bgLeftWidth + bg.rightWidth;
                }

                float knobWidthHalf = 0;
                if (min != max)
                {
                    if (knob == null)
                    {
                        knobWidthHalf = knobBefore == null ? 0 : knobBefore.minWidth * 0.5f;
                        position = (positionWidth - knobWidthHalf) * percent;
                        position = Math.Min(positionWidth - knobWidthHalf, position);
                    }
                    else
                    {
                        knobWidthHalf = knobWidth * 0.5f;
                        position = (positionWidth - knobWidth) * percent;
                        position = Math.Min(positionWidth - knobWidth, position) + bgLeftWidth;
                    }
                    position = Math.Max(0, position);
                }

                if (knobBefore != null)
                {
                    float offset = 0;
                    if (position != 0f)
                    {

                        knobBefore.draw(graphics, x + offset, y + (int)((height - knobBefore.minHeight) * 0.5f),
                            (int)(position + knobWidthHalf), knobBefore.minHeight, color);
                    }

                }

                //if (knobAfter != null)
                //{
                //    knobAfter.draw(graphics, x + (int)(position + knobWidthHalf), y + (int)((height - knobAfter.minHeight) * 0.5f),
                //        width - (int)(position + knobWidthHalf), knobAfter.minHeight, color);
                //}

                //if (knob != null)
                //    knob.draw(graphics, (int)(x + position), (int)(y + (height - knobHeight) * 0.5f), knobWidth, knobHeight, color);
            }
        }


        public float getVisualPercent()
        {
            return (_value - min) / (max - min);
        }


        /// <summary>
        /// Returns a snapped value
        /// </summary>
        /// <param name="value">Value.</param>
        float snap(float value)
        {
            if (snapValues == null)
                return value;

            for (var i = 0; i < snapValues.Length; i++)
            {
                if (Math.Abs(value - snapValues[i]) <= snapThreshold)
                    return snapValues[i];
            }
            return value;
        }

    }


    /// <summary>
    /// The style for a hp bar
    /// </summary>
    public class HPBarStyle
    {
        /// <summary>
        /// The progress bar background, stretched only in one direction. Optional.
        /// </summary>
        public Nez.UI.IDrawable background;

        /// <summary>
        /// Optional
        /// </summary>
        public Nez.UI.IDrawable disabledBackground;

        /// <summary>
        /// Optional, centered on the background.
        /// </summary>
        public Nez.UI.IDrawable knob, disabledKnob;

        /// <summary>
        /// Optional
        /// </summary>
        public Nez.UI.IDrawable knobBefore, knobAfter, disabledKnobBefore, disabledKnobAfter;


        public HPBarStyle()
        {
        }


        public HPBarStyle(Nez.UI.IDrawable background, Nez.UI.IDrawable knob)
        {
            this.background = background;
            this.knob = knob;
        }


        public static HPBarStyle create(Color knobBeforeColor, Color knobAfterColor)
        {
            var knobBefore = new PrimitiveDrawable(knobBeforeColor);
            knobBefore.minHeight = 10;

            var knobAfter = new PrimitiveDrawable(knobAfterColor);
            knobAfter.minWidth = knobAfter.minHeight = 10;

            return new HPBarStyle
            {
                knobBefore = knobBefore,
                knobAfter = knobAfter
            };
        }


        public static HPBarStyle createWithKnob(Color backgroundColor, Color knobColor)
        {
            var background = new PrimitiveDrawable(backgroundColor);
            background.minWidth = background.minHeight = 10;

            var knob = new PrimitiveDrawable(knobColor);
            knob.minWidth = knob.minHeight = 20;

            return new HPBarStyle
            {
                background = background,
                knob = knob
            };
        }


        public HPBarStyle clone()
        {
            return new HPBarStyle
            {
                background = background,
                disabledBackground = disabledBackground,
                knob = knob,
                disabledKnob = disabledKnob,
                knobBefore = knobBefore,
                knobAfter = knobAfter,
                disabledKnobBefore = disabledKnobBefore,
                disabledKnobAfter = disabledKnobAfter
            };
        }
    }
}
