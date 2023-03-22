using System;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences
{
	/// <summary>
	///		Clase con los datos de una columna de un archivo
	/// </summary>
	internal class FileColumnModel
	{
		/// <summary>
		///		Tipo de columna
		/// </summary>
		internal enum ColumnType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Valor entero</summary>
			Integer,
			/// <summary>Valor decimal</summary>
			Decimal,
			/// <summary>Fecha / hora</summary>
			DateTime,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Valor lógico</summary>
			Boolean
		}

		public FileColumnModel(string name, ColumnType type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		///		Nombre de la columna
		/// </summary>
		internal string Name { get; }

		/// <summary>
		///		Tipo de la columna
		/// </summary>
		internal ColumnType Type { get; }
	}
}
