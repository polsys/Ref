using System;
using System.IO;
using Polsys.Ref.Models;

namespace Polsys.Ref.Export
{
    /// <summary>
    /// Base class for exporters.
    /// </summary>
    internal abstract class CatalogueExporter
    {
        /// <summary>
        /// The default file extension used by this exporter.
        /// </summary>
        public virtual string FileExtension
        {
            get;
        }

        /// <summary>
        /// The name of this exporter.
        /// </summary>
        public virtual string Name
        {
            get;
        }

        /// <summary>
        /// Exports the catalogue to the specified file.
        /// </summary>
        /// <param name="filename">The file to write the catalogue to.</param>
        /// <param name="catalogue">The catalogue to export.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Export(string filename, Catalogue catalogue)
        {
            try
            {
                using (var stream = File.Open(filename, FileMode.Create))
                {
                    Export(stream, catalogue);
                    return true;
                }
            }
            catch (ArgumentException) { return false; }
            catch (IOException) { return false; }
        }

        /// <summary>
        /// Exports the catalogue to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the catalogue to.</param>
        /// <param name="catalogue">The catalogue to export.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public virtual bool Export(Stream stream, Catalogue catalogue)
        {
            throw new NotImplementedException();
        }
    }
}
