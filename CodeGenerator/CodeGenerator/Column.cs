using System;

namespace CodeGenerator
{
	/// <summary>
	/// Class that stores information for columns in a database table.
	/// </summary>
	public class Column {
		// Private variable used to hold the property values
		private string name;
		private string programmaticAlias;
		private string type;
		private string length;
		private string precision;
		private string scale;
		private bool isRowGuidCol;
		private bool isIdentity;
		private bool isComputed;

		/// <summary>
		/// Name of the column.
		/// </summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Extended name of the column.
		/// </summary>
		public string ProgrammaticAlias {
			get { return programmaticAlias; }
			set { programmaticAlias = value; }
		}

		/// <summary>
		/// Data type of the column.
		/// </summary>
		public string Type {
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Length in bytes of the column.
		/// </summary>
		public string Length {
			get { return length; }
			set { length = value; }
		}
		
		/// <summary>
		/// Precision of the column.  Applicable to decimal, float, and numeric data types only.
		/// </summary>
		public string Precision {
			get { return precision; }
			set { precision = value; }
		}
		
		/// <summary>
		/// Scale of the column.  Applicable to decimal, and numeric data types only.
		/// </summary>
		public string Scale {
			get { return scale; }
			set { scale = value; }
		}
		
		/// <summary>
		/// Flags the column as a uniqueidentifier column.
		/// </summary>
		public bool IsRowGuidCol {
			get { return isRowGuidCol; }
			set { isRowGuidCol = value; }
		}

		/// <summary>
		/// Flags the column as an identity column.
		/// </summary>
		public bool IsIdentity {
			get { return isIdentity; }
			set { isIdentity = value; }
		}

		/// <summary>
		/// Flags the column as being computed.
		/// </summary>
		public bool IsComputed {
			get { return isComputed; }
			set { isComputed = value; }
		}
	}
}
