using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Nez.Tiled
{
	public class TiledObjectLayer : TiledLayer
    {
        public readonly TiledMap tiledMap;
        public string name;
		public Color color;
		public float opacity;
		public bool visible;
		public Dictionary<string,string> properties = new Dictionary<string,string>();
		public TiledObject[] objects;

        public TiledObjectLayer( TiledMap map, string name, Color color, TiledObject[] objects) : base(name)
		{
		    tiledMap = map;
			this.name = name;
			this.color = color;
            this.objects = objects;
            //this.visible = visible;
            //this.opacity = opacity;
		}


		/// <summary>
		/// gets the first TiledObject with the given name
		/// </summary>
		/// <returns>The with name.</returns>
		/// <param name="name">Name.</param>
		public TiledObject objectWithName( string name )
		{
			for( int i = 0; i < objects.Length; i++ )
			{
				if( objects[i].name == name )
					return objects[i];
			}
			return null;
		}


		/// <summary>
		/// gets all the TiledObjects with the given name
		/// </summary>
		/// <returns>The with name.</returns>
		/// <param name="name">Name.</param>
		public List<TiledObject> objectsWithName( string name )
		{
			var list = new List<TiledObject>();
			for( int i = 0; i < objects.Length; i++ )
			{
				if( objects[i].name == name )
					list.Add( objects[i] );
			}
			return list;
		}

	    public override void draw(Batcher batcher, Vector2 position, float layerDepth, RectangleF cameraClipBounds)
	    {
	        foreach (var tiledObject in objects)
	        {
	            if (tiledObject.tiledObjectType == TiledObject.TiledObjectType.Tile)
	            {
	                var tileRegion = tiledObject.tile.textureRegion;
	                //var tileRegion = layer.tiles[1].textureRegion;
	                var tx = tiledObject.x + (int) position.X;
	                var ty = tiledObject.y + (int) position.Y;
	                var rotation = 0f;

	                var spriteEffects = SpriteEffects.None;
	                if (tiledObject.tile.flippedHorizonally)
	                    spriteEffects |= SpriteEffects.FlipHorizontally;
	                if (tiledObject.tile.flippedVertically)
	                    spriteEffects |= SpriteEffects.FlipVertically;
	                if (tiledObject.tile.flippedDiagonally)
	                {
	                    if (tiledObject.tile.flippedHorizonally && tiledObject.tile.flippedVertically)
	                    {
	                        spriteEffects ^= SpriteEffects.FlipVertically;
	                        rotation = MathHelper.PiOver2;
	                        tx += tiledMap.tileHeight + (tileRegion.sourceRect.Height - tiledMap.tileHeight);
	                        ty -= (tileRegion.sourceRect.Width - tiledMap.tileWidth);
	                    }
	                    else if (tiledObject.tile.flippedHorizonally)
	                    {
	                        spriteEffects ^= SpriteEffects.FlipVertically;
	                        rotation = -MathHelper.PiOver2;
	                        ty += tiledMap.tileHeight;
	                    }
	                    else if (tiledObject.tile.flippedVertically)
	                    {
	                        spriteEffects ^= SpriteEffects.FlipHorizontally;
	                        rotation = MathHelper.PiOver2;
	                        tx += tiledMap.tileWidth + (tileRegion.sourceRect.Height - tiledMap.tileHeight);
	                        ty += (tiledMap.tileWidth - tileRegion.sourceRect.Width);
	                    }
	                    else
	                    {
	                        spriteEffects ^= SpriteEffects.FlipHorizontally;
	                        rotation = -MathHelper.PiOver2;
	                        ty += tiledMap.tileHeight;
	                    }
	                }

	                if (rotation == 0)
	                    ty += (tiledMap.tileHeight - tileRegion.sourceRect.Height);

	                Graphics.instance.batcher.draw(tileRegion, new Vector2(tx, ty), Color.AliceBlue, rotation, Vector2.Zero,
	                    1, spriteEffects, 0.5f);

	            }
	        }

        }
    }
}

