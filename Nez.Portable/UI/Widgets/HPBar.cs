﻿using Microsoft.Xna.Framework;
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

        public override float PreferredWidth
        {
            get
            {
                if (_vertical)
                    return Math.Max(style.Knob == null ? 0 : style.Knob.MinWidth, style.Background != null ? style.Background.MinWidth : 0);
                else
                    return width;
            }
        }

        public override float PreferredHeight
        {
            get
            {
                if (_vertical)
                    return height;
                else
                    return Math.Max(style.Knob == null ? 0 : style.Knob.MinHeight, style.Background != null ? style.Background.MinHeight : 0);
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
            Insist.IsTrue(min < max, "min must be less than max");
            Insist.IsTrue(stepSize > 0, "stepSize must be greater than 0");

            setStyle(style);
            this.min = min;
            this.max = max;
            this.stepSize = stepSize;
            _vertical = vertical;
            _value = this.min;

            SetSize(PreferredWidth, PreferredHeight);
        }

        public HPBar(float min, float max, float stepSize, bool vertical, Skin skin, string styleName = null) : this(min, max, stepSize, vertical, skin.Get<HPBarStyle>(styleName))
        { }

        public HPBar(Skin skin, string styleName = null) : this(0, 1, 0.01f, false, skin)
        { }


        public virtual void setStyle(HPBarStyle style)
        {
            this.style = style;
            InvalidateHierarchy();
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
        /// Sets the progress bar position, rounded to the nearest step size and Clamped to the minimum and maximum values.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="ignoreSnap">If set to <c>true</c> we ignore value snapping.</param>
        public HPBar setValue(float value, bool ignoreSnap = false)
        {
            if ((shiftIgnoresSnap && InputUtils.IsShiftDown()) || ignoreSnap)
            {
                value = Mathf.Clamp(value, min, max);
            }
            else
            {
                // if value is lower/higher than min/max then we're not rounding to avoid situation where we can't achieve those values
                if (value >= max)
                    value = max;
                else if (value <= min)
                    value = min;
                else
                    value = Mathf.Clamp(Mathf.Round(value / stepSize) * stepSize, min, max);

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
            Insist.IsTrue(min < max, "min must be less than max");
            this.min = min;
            this.max = max;
            _value = Mathf.Clamp(_value, this.min, this.max);

            return this;
        }


        /// <summary>
        /// Sets stepSize to a value that will evenly divide this progress bar into specified amount of steps.
        /// </summary>
        /// <param name="totalSteps">Total amount of steps.</param>
        public HPBar setTotalSteps(int totalSteps)
        {
            Insist.IsTrue(totalSteps != 0, "totalSteps cannot be equal to 0");
            _stepSize = Math.Abs((min - max) / totalSteps);
            return this;
        }


        protected virtual Nez.UI.IDrawable getKnobDrawable()
        {
            return (disabled && style.DisabledKnob != null) ? style.DisabledKnob : style.Knob;
        }


        public override void Draw(Batcher batcher, float parentAlpha)
        {
            var knob = getKnobDrawable();
            var bg = (disabled && style.DisabledBackground != null) ? style.DisabledBackground : style.Background;
            var knobBefore = (disabled && style.DisabledKnobBefore != null) ? style.DisabledKnobBefore : style.KnobBefore;
            var knobAfter = (disabled && style.DisabledKnobAfter != null) ? style.DisabledKnobAfter : style.KnobAfter;

            var x = this.x;
            var y = this.y;
            var width = this.width;
            var height = this.height;
            var knobHeight = knob == null ? 0 : knob.MinHeight;
            var knobWidth = knob == null ? 0 : knob.MinWidth;
            var percent = getVisualPercent();
            var color = new Color(this.color, (int)(this.color.A * parentAlpha));

            if (_vertical)
            {
                var positionHeight = height;

                float bgTopHeight = 0;
                if (bg != null)
                {
                    bg.Draw(batcher, x + (int)((width - bg.MinWidth) * 0.5f), y, bg.MinWidth, height, color);
                    bgTopHeight = bg.TopHeight;
                    positionHeight -= bgTopHeight + bg.BottomHeight;
                }

                float knobHeightHalf = 0;
                if (min != max)
                {
                    if (knob == null)
                    {
                        knobHeightHalf = knobBefore == null ? 0 : knobBefore.MinHeight * 0.5f;
                        position = (positionHeight - knobHeightHalf) * percent;
                        position = Math.Min(positionHeight - knobHeightHalf, position);
                    }
                    else
                    {
                        var bgBottomHeight = bg != null ? bg.BottomHeight : 0;
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
                    knobBefore.Draw(batcher, x + ((width - knobBefore.MinWidth) * 0.5f), y + offset, knobBefore.MinWidth,
                        (int)(position + knobHeightHalf), color);
                }

                if (knobAfter != null)
                {
                    knobAfter.Draw(batcher, x + ((width - knobAfter.MinWidth) * 0.5f), y + position + knobHeightHalf,
                        knobAfter.MinWidth, height - position - knobHeightHalf, color);
                }

                if (knob != null)
                    knob.Draw(batcher, x + (int)((width - knobWidth) * 0.5f), (int)(y + position), knobWidth, knobHeight, color);
            }
            else
            {
                float positionWidth = width;

                float bgLeftWidth = 0;
                if (bg != null)
                {
                    bg.Draw(batcher, x, y + (int)((height - bg.MinHeight) * 0.5f), width, bg.MinHeight, color);
                    //bgLeftWidth = bg.leftWidth;
                    //positionWidth -= bgLeftWidth + bg.rightWidth;
                }

                float knobWidthHalf = 0;
                if (min != max)
                {
                    if (knob == null)
                    {
                        knobWidthHalf = knobBefore == null ? 0 : knobBefore.MinWidth * 0.5f;
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

                        knobBefore.Draw(batcher, x + offset, y + (int)((height - knobBefore.MinHeight) * 0.5f),
                            (int)(position + knobWidthHalf), knobBefore.MinHeight, color);
                    }

                }

                //if (knobAfter != null)
                //{
                //    knobAfter.draw(graphics, x + (int)(position + knobWidthHalf), y + (int)((height - knobAfter.MinHeight) * 0.5f),
                //        width - (int)(position + knobWidthHalf), knobAfter.MinHeight, color);
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
        public Nez.UI.IDrawable Background;

        /// <summary>
        /// Optional
        /// </summary>
        public Nez.UI.IDrawable DisabledBackground;

        /// <summary>
        /// Optional, centered on the background.
        /// </summary>
        public Nez.UI.IDrawable Knob, DisabledKnob;

        /// <summary>
        /// Optional
        /// </summary>
        public Nez.UI.IDrawable KnobBefore, KnobAfter, DisabledKnobBefore, DisabledKnobAfter;


        public HPBarStyle()
        {
        }


        public HPBarStyle(Nez.UI.IDrawable background, Nez.UI.IDrawable knob)
        {
            this.Background = background;
            this.Knob = knob;
        }


        public static HPBarStyle create(Color knobBeforeColor, Color knobAfterColor)
        {
            var knobBefore = new PrimitiveDrawable(knobBeforeColor);
            knobBefore.MinHeight = 10;

            var knobAfter = new PrimitiveDrawable(knobAfterColor);
            knobAfter.MinWidth = knobAfter.MinHeight = 10;

            return new HPBarStyle
            {
                KnobBefore = knobBefore,
                KnobAfter = knobAfter
            };
        }


        public static HPBarStyle createWithKnob(Color backgroundColor, Color knobColor)
        {
            var background = new PrimitiveDrawable(backgroundColor);
            background.MinWidth = background.MinHeight = 10;

            var knob = new PrimitiveDrawable(knobColor);
            knob.MinWidth = knob.MinHeight = 20;

            return new HPBarStyle
            {
                Background = background,
                Knob = knob
            };
        }


        public HPBarStyle Clone()
        {
            return new HPBarStyle
            {
                Background = Background,
                DisabledBackground = DisabledBackground,
                Knob = Knob,
                DisabledKnob = DisabledKnob,
                KnobBefore = KnobBefore,
                KnobAfter = KnobAfter,
                DisabledKnobBefore = DisabledKnobBefore,
                DisabledKnobAfter = DisabledKnobAfter
            };
        }
    }
}
