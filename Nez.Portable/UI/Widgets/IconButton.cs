using System;
using Microsoft.Xna.Framework;


namespace Nez.UI
{
    /// <summary>
    /// Best button
    /// </summary>
    public class IconButton : Button
    {
        Image image;
        Image iconImage;
        IconButtonStyle style;


        public IconButton(IconButtonStyle style) : base(style)
        {
            image = new Image();
            iconImage = new Image();
            var stck = new Stack();
            image.setScaling(Scaling.Fit);
            stck.add(image);
            stck.add(iconImage);
            add(stck);
            setStyle(style);
            padLeft(style.PadLeft).padRight(style.PadRight).padTop(style.PadTop).padBottom(style.PadBottom);
            setSize(preferredWidth, preferredHeight);
        }

        public IconButton(Skin skin, string styleName = null) : this(skin.get<IconButtonStyle>(styleName))
        { }


        public IconButton(IDrawable icon, IDrawable imageUp) : this(new IconButtonStyle(icon, null, null, null, imageUp, null, null))
        {
        }


        public IconButton(IDrawable icon, IDrawable imageUp, IDrawable imageDown) : this(new IconButtonStyle(icon, null, null, null, imageUp, imageDown, null))
        {
        }


        public IconButton(IDrawable icon, IDrawable imageUp, IDrawable imageDown, IDrawable imageOver) : this(new IconButtonStyle(icon, null, null, null, imageUp, imageDown, imageOver))
        {
        }


        public override void setStyle(ButtonStyle style)
        {
            Insist.isTrue(style is IconButtonStyle, "style must be a ImageButtonStyle");

            base.setStyle(style);
            this.style = (IconButtonStyle)style;
            if (image != null)
                updateImage();
        }


        public new IconButtonStyle getStyle()
        {
            return style;
        }


        public Image getImage()
        {
            return image;
        }


        public Cell getImageCell()
        {
            return getCell(image);
        }


        private void updateImage()
        {
            IDrawable drawable = null;
            if (_isDisabled && style.imageDisabled != null)
                drawable = style.imageDisabled;
            else if (_mouseDown && style.imageDown != null)
                drawable = style.imageDown;
            else if (isChecked && style.imageChecked != null)
                drawable = (style.imageCheckedOver != null && _mouseOver) ? style.imageCheckedOver : style.imageChecked;
            else if (_mouseOver && style.imageOver != null)
                drawable = style.imageOver;
            else if (style.imageUp != null) //
                drawable = style.imageUp;

            image.setDrawable(drawable);
            iconImage.setDrawable(style.Icon);
        }


        public override void draw(Graphics graphics, float parentAlpha)
        {
            updateImage();
            base.draw(graphics, parentAlpha);
        }

    }


    public class IconButtonStyle : ImageButtonStyle
    {
        public IDrawable Icon;
        public int PadLeft, PadRight, PadTop, PadBottom;

        public IconButtonStyle()
        { }


        public IconButtonStyle(IDrawable icon, IDrawable up, IDrawable down, IDrawable checkked, IDrawable imageUp, IDrawable imageDown, IDrawable imageChecked) : base(up, down, checkked, imageUp, imageDown, imageChecked)
        {
            this.Icon = icon;
        }


        public new IconButtonStyle clone()
        {
            return new IconButtonStyle
            {
                Icon = Icon,
                up = up,
                down = down,
                over = over,
                checkked = checkked,
                checkedOver = checkedOver,
                disabled = disabled,

                imageUp = imageUp,
                imageDown = imageDown,
                imageOver = imageOver,
                imageChecked = imageChecked,
                imageCheckedOver = imageCheckedOver,
                imageDisabled = imageDisabled
            };
        }
    }
}

