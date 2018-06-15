using System.Collections.Generic;
using System.Xml.Serialization;


namespace Nez.TiledMaps
{
	public class TmxObjectLayer : TmxLayer
	{

		[XmlAttribute( AttributeName = "color" )]
		public string color;

		[XmlElement( ElementName = "object" )]
		public List<TmxObject> objects = new List<TmxObject>();


		public override string ToString()
		{
			return string.Format( "[TmxObjectLayer] name: {0}, offsetx: {1}, offsety: {2}", name, offsetx, offsety );
		}

	}
}