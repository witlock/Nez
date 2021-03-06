﻿using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;


namespace Nez.TiledMaps
{
	[ContentImporter( ".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer" )]
	public class TiledMapImporter : ContentImporter<TmxMap>
	{
		public override TmxMap Import( string filename, ContentImporterContext context )
		{
			if( filename == null )
				throw new ArgumentNullException( nameof( filename ) );

			using( var reader = new StreamReader( filename ) )
			{
				context.Logger.LogMessage( "Deserializing filename: {0}", filename );

				var serializer = new XmlSerializer( typeof( TmxMap ) );
				var map = (TmxMap)serializer.Deserialize( reader );
				var xmlSerializer = new XmlSerializer( typeof( TmxTileset ) );

				foreach( var l in map.layers )
					context.Logger.LogMessage( "Deserialized Layer: {0}", l );

				context.Logger.LogMessage( "" );

				for( var i = 0; i < map.tilesets.Count; i++ )
				{
					var tileset = map.tilesets[i];
					if( !string.IsNullOrWhiteSpace( tileset.source ) )
					{
						var directoryName = Path.GetDirectoryName( filename );
						var tilesetLocation = tileset.source.Replace( '/', Path.DirectorySeparatorChar );
						var filePath = Path.Combine( directoryName, tilesetLocation );

						var normExtTilesetPath = new DirectoryInfo( filePath ).FullName;
						context.Logger.LogMessage( "Reading External Tileset File: " + normExtTilesetPath );
						using( var file = new StreamReader( filePath ) )
						{
							map.tilesets[i] = (TmxTileset)xmlSerializer.Deserialize( file );
							map.tilesets[i].fixImagePath( filename, tileset.source );
							map.tilesets[i].firstGid = tileset.firstGid;
						    // fixes the issue of missing tilesets on terrains
                            map.tilesets[i].mapFolder = Path.GetDirectoryName(Path.GetFullPath(filename));
                        }
                        
					    
                    }
					else
					{
						tileset.mapFolder = Path.GetDirectoryName( Path.GetFullPath( filename ) );
					}
				}

				return map;
			}
		}
	}
}
