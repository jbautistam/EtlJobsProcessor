using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage
{
    /// <summary>
    ///		Controlador de archivos en local
    /// </summary>
    internal class LocalFileStorage : BaseFileStorage
    {
        internal LocalFileStorage(DbScriptInterpreter processor) : base(processor)
        {
        }

        /// <summary>
        ///		Abre la conexión
        /// </summary>
        internal override void Open()
        {
            // ... en este caso no hace nada simplemente implementa la interface
        }

        /// <summary>
        ///		Crea un directorio
        /// </summary>
        internal override async Task CreatePathAsync(string path)
        {
            // Evita los warnings
            await Task.Delay(1);
            // Crea el directorio si no existía en local
            LibHelper.Files.HelperFiles.MakePath(path);
        }

        /// <summary>
        ///		Obtiene los archivos del directorio
        /// </summary>
        internal override async IAsyncEnumerable<string> GetFilesAsync(string folder, bool importFolder, string extension)
        {
            // Evita las advertencias
            await Task.Delay(1);
            // Obtiene el nombre de los archivos
            if (!importFolder)
                yield return folder;
            else
                foreach (string file in Directory.GetFiles(folder, extension))
                    yield return file;
        }

        /// <summary>
        ///		Obtiene un stream sobre un archivo
        /// </summary>
        internal override async Task<Stream> GetStreamAsync(string fileName, OpenFileMode mode)
        {
            // Evita los warnings
            await Task.Delay(1);
            // Devuelve el stream
            if (mode == OpenFileMode.Read)
                return new StreamReader(fileName).BaseStream;
            else
                return new StreamWriter(fileName).BaseStream;
        }

        /// <summary>
        ///		Libera los datos
        /// </summary>
        protected override void DisposeData()
        {
            // ... en archivos locales no hace nada
        }
    }
}
