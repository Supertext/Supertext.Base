using System;
using System.IO;
using System.Reflection;
using System.Resources;


namespace Supertext.Base.Resources
{
    public class EmbeddedResource
    {
        #region ReadContentsAsByteArray

        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the calling assembly.</param>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the calling assembly using the specified <see cref="resourceName"/> argument.</exception>
        public byte[] ReadContentsAsByteArray(string resourceName)
        {
            var an = Assembly.GetCallingAssembly().GetName();

            return ReadContentsAsByteArray(an, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public byte[] ReadContentsAsByteArray(string assemblyName, string resourceName)
        {
            var an = new AssemblyName(assemblyName);

            return ReadContentsAsByteArray(an, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public byte[] ReadContentsAsByteArray(AssemblyName assemblyName, string resourceName)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            return ReadContentsAsByteArray(assembly, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <param name="assembly">The <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public byte[] ReadContentsAsByteArray(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new MissingManifestResourceException($"Unable to read the resource \"{resourceName}\" from the assembly \"{assembly.GetName().Name}\".");
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes,
                            0,
                            bytes.Length);

                return bytes;
            }
        }

        #endregion


        #region ReadContentsAsStream

        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>Stream</c>.
        /// </summary>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the calling assembly.</param>
        /// <returns>
        /// A <c>Stream</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the calling assembly using the specified <see cref="resourceName"/> argument.</exception>
        public Stream ReadContentsAsStream(string resourceName)
        {
            var an = Assembly.GetCallingAssembly().GetName();

            return ReadContentsAsStream(an, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>Stream</c>.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>Stream</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public Stream ReadContentsAsStream(string assemblyName, string resourceName)
        {
            var an = new AssemblyName(assemblyName);

            return ReadContentsAsStream(an, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>Stream</c>.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>Stream</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public Stream ReadContentsAsStream(AssemblyName assemblyName, string resourceName)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            return ReadContentsAsStream(assembly, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>Stream</c>.
        /// </summary>
        /// <param name="assembly">The <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>Stream</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public Stream ReadContentsAsStream(Assembly assembly, string resourceName)
        {
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new MissingManifestResourceException($"Unable to read the resource \"{resourceName}\" from the assembly \"{assembly.GetName().Name}\".");
            }

            return stream;
        }

        #endregion


        #region ReadContentsAsString

        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>string</c>.
        /// </summary>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the calling assembly.</param>
        /// <returns>
        /// A <c>string</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the calling assembly using the specified <see cref="resourceName"/> argument.</exception>
        public string ReadContentsAsString(string resourceName)
        {
            var an = Assembly.GetCallingAssembly().GetName();

            return ReadContentsAsString(an, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>string</c>.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>string</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public string ReadContentsAsString(string assemblyName, string resourceName)
        {
            var an = new AssemblyName(assemblyName);

            return ReadContentsAsString(an, resourceName);
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>string</c>.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>string</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public string ReadContentsAsString(AssemblyName assemblyName, string resourceName)
        {
            using (var stream = ReadContentsAsStream(assemblyName, resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>string</c>.
        /// </summary>
        /// <param name="assembly">The <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <returns>
        /// A <c>string</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public string ReadContentsAsString(Assembly assembly, string resourceName)
        {
            using (var stream = ReadContentsAsStream(assembly, resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion


        #region WriteAsFile

        /// <summary>
        /// Writes the contents of the embedded resource to the specified path.
        /// </summary>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <param name="fullPath">The full path of the file to be created with the embedded resource's contents.</param>
        /// <remarks>
        /// Probably only used for integration testing.
        /// </remarks>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public void WriteAsFile(string resourceName, string fullPath)
        {
            var an = Assembly.GetCallingAssembly().GetName();

            WriteAsFile(an,
                        resourceName,
                        fullPath);
        }


        /// <summary>
        /// Writes an embedded resource to the specified path.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <param name="fullPath">The full path of the file to be created with the embedded resource's contents.</param>
        /// <remarks>
        /// Probably only used for integration testing.
        /// </remarks>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public void WriteAsFile(string assemblyName,
                                string resourceName,
                                string fullPath)
        {
            var an = new AssemblyName(assemblyName);

            WriteAsFile(an,
                        resourceName,
                        fullPath);
        }


        /// <summary>
        /// Writes an embedded resource to the specified path.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <param name="fullPath">The full path of the file to be created with the embedded resource's contents.</param>
        /// <remarks>
        /// Probably only used for integration testing.
        /// </remarks>
        /// <exception cref="FileNotFoundException">No assembly with the specified <see cref="assemblyName"/> argument could be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public void WriteAsFile(AssemblyName assemblyName,
                                string resourceName,
                                string fullPath)
        {
            var contents = ReadContentsAsString(assemblyName, resourceName);

            File.WriteAllText(fullPath, contents);
        }


        /// <summary>
        /// Writes an embedded resource to the specified path.
        /// </summary>
        /// <param name="assembly">The <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        /// <param name="fullPath">The full path of the file to be created with the embedded resource's contents.</param>
        /// <remarks>
        /// Probably only used for integration testing.
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified <see cref="assembly"/> argument could not be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public void WriteAsFile(Assembly assembly,
                                string resourceName,
                                string fullPath)
        {
            var contents = ReadContentsAsString(assembly, resourceName);

            File.WriteAllText(fullPath, contents);
        }

        #endregion
    }
}