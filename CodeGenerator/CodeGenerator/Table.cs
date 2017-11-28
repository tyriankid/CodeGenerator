using System;
using System.Collections;

namespace CodeGenerator
{
	/// <summary>
	/// Class that stores information for tables in a database.
	/// </summary>
	public class Table {
		string name;
		string programmaticAlias;
		ArrayList columns;
		ArrayList primaryKeys;
		Hashtable foreignKeys;
		
		/// <summary>
		/// Default constructor.  All collections are initialized.
		/// </summary>
		public Table() {
			columns = new ArrayList();
			primaryKeys = new ArrayList();
			foreignKeys = new Hashtable();
		}
		
		/// <summary>
		/// Contains the list of Column instances that define the table.
		/// </summary>
		public ArrayList Columns {
			get { return columns; }
		}

		/// <summary>
		/// Contains the list of Column instances that define the table.  The Hashtable returned 
		/// is keyed on the foreign key name, and the value associated with the key is an 
		/// ArrayList of Column instances that compose the foreign key.
		/// </summary>
		public Hashtable ForeignKeys {
			get { return foreignKeys; }
		}

		/// <summary>
		/// Name of the table.
		/// </summary>
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Contains the list of primary key Column instances that define the table.
		/// </summary>
		public ArrayList PrimaryKeys {
			get { return primaryKeys; }
		}

		/// <summary>
		/// Represents the name that should be used in code to represent the class.
		/// </summary>
		public string ProgrammaticAlias {
			get { return programmaticAlias; }
			set { programmaticAlias = value; }
		}
	}
}
