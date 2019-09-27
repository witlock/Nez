﻿using System.Text;


namespace Nez
{
<<<<<<< HEAD
    /// <summary>
    /// helper that wraps either a string or StringBuilder and provides a common API to read them for measuring/drawing
    /// </summary>
    public struct FontCharacterSource
    {
        readonly string _string;
        readonly StringBuilder _builder;
        public readonly int Length;

        public string value
        {
            get { return _string; }
        }

        public FontCharacterSource(string s)
        {
            _string = s;
            _builder = null;
            Length = s.Length;
        }


        public FontCharacterSource(StringBuilder builder)
        {
            _builder = builder;
            _string = null;
            Length = _builder.Length;
        }


        public char this[int index]
        {
            get
            {
                if (_string != null)
                    return _string[index];
                return _builder[index];
            }
        }
    }
}
=======
	/// <summary>
	/// helper that wraps either a string or StringBuilder and provides a common API to read them for measuring/drawing
	/// </summary>
	public struct FontCharacterSource
	{
		readonly string _string;
		readonly StringBuilder _builder;
		public readonly int Length;


		public FontCharacterSource(string s)
		{
			_string = s;
			_builder = null;
			Length = s.Length;
		}


		public FontCharacterSource(StringBuilder builder)
		{
			_builder = builder;
			_string = null;
			Length = _builder.Length;
		}


		public char this[int index]
		{
			get
			{
				if (_string != null)
					return _string[index];

				return _builder[index];
			}
		}
	}
}
>>>>>>> 65d2f2cd2bfe95907f48a501bc8573e636285026
