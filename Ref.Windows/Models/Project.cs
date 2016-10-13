using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Polsys.Ref.Models
{
    /// <summary>
    /// Encapsulates everything present in a saved project file.
    /// Use the static methods for file loading and saving.
    /// </summary>
    [DataContract(Name = "RefProject")]
    internal class Project
    {
        [DataMember]
        private Catalogue Catalogue { get; set; }

        [DataMember]
        private string VersionString { get; set; }

        /// <summary>
        /// Tries to deserialize the project from the stream.
        /// </summary>
        /// <param name="stream">The stream containing the serialized project.</param>
        /// <returns>The catalogue, or null in case of failure.</returns>
        public static Catalogue Deserialize(Stream stream)
        {
            try
            {
                var serializer = GetSerializer();
                var project = (Project)serializer.ReadObject(stream);

                // If the project is newer version than we are, awful things could happen
                // Therefore refuse to load
                var fileVersion = Version.Parse(project.VersionString);
                if (fileVersion > GetCurrentVersion())
                    return null;

                return project.Catalogue;
            }
            catch (Exception)
            {
                // There are just too many things that could go wrong.
                // They *should* not affect global state, however.
                // We don't want to crash just because of a corrupted file.
                return null;
            }
        }

        /// <summary>
        /// Serializes the project to the stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="catalogue">The catalogue to serialize.</param>
        public static void Serialize(Stream stream, Catalogue catalogue)
        {
            var serializer = GetSerializer();
            var project = new Project();
            project.Catalogue = catalogue;
            project.VersionString = GetCurrentVersion().ToString();

            serializer.WriteObject(stream, project);
        }

        private static Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        private static DataContractSerializer _serializer;
        private static DataContractSerializer GetSerializer()
        {
            if (_serializer == null)
            {
                // These types are available in polymorphic contexts
                var knownTypes = new List<Type>();
                knownTypes.Add(typeof(Catalogue));
                knownTypes.Add(typeof(Book));
                knownTypes.Add(typeof(Page));

                _serializer = new DataContractSerializer(typeof(Project), knownTypes);
            }

            return _serializer;
        }
    }
}
