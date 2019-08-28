using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Supertext.Base.Resources
{
    // This class contains all the deprecated methods.
    public partial class EmbeddedResource
    {
        /// <summary>
        /// Default constructor for <see cref="EmbeddedResource"/>. This overload requires passing the arguments to the method rather than the constructor.
        /// </summary>
        [Obsolete("Instantiate this class using a parameterised constructor, then call a parameterless method.")]
        public EmbeddedResource()
        {
        }

        #region ReadContentsAsByteArray

        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the calling assembly.</param>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the calling assembly using the specified <see cref="resourceName"/> argument.</exception>
        [Obsolete("Instantiate the class using EmbeddedResource(string resourceName) and call ReadContentsAsByteArray().")]
        public byte[] ReadContentsAsByteArray(string resourceName)
        {
            _assembly = Assembly.GetCallingAssembly();
            _resourceName = resourceName;

            return ReadContentsAsByteArray();
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
        [Obsolete("Instantiate the class using EmbeddedResource(string assemblyName, string resourceName) and call ReadContentsAsByteArray().")]
        public byte[] ReadContentsAsByteArray(string assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            return ReadContentsAsByteArray();
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
        [Obsolete("Instantiate the class using EmbeddedResource(AssemblyName assemblyName, string resourceName) and call ReadContentsAsByteArray().")]
        public byte[] ReadContentsAsByteArray(AssemblyName assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            return ReadContentsAsByteArray();
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
        [Obsolete("Instantiate the class using EmbeddedResource(Assembly assembly, string resourceName) and call ReadContentsAsByteArray().")]
        public byte[] ReadContentsAsByteArray(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;

            return ReadContentsAsByteArray();
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
        [Obsolete("Instantiate the class using EmbeddedResource(string resourceName) and call ReadContentsAsStream().")]
        public Stream ReadContentsAsStream(string resourceName)
        {
            _assembly = Assembly.GetCallingAssembly();
            _resourceName = resourceName;

            return ReadContentsAsStream();
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
        [Obsolete("Instantiate the class using EmbeddedResource(string assemblyName, string resourceName) and call ReadContentsAsStream().")]
        public Stream ReadContentsAsStream(string assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            return ReadContentsAsStream();
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
        [Obsolete("Instantiate the class using EmbeddedResource(AssemblyName assemblyName, string resourceName) and call ReadContentsAsStream().")]
        public Stream ReadContentsAsStream(AssemblyName assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            return ReadContentsAsStream();
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
        [Obsolete("Instantiate the class using EmbeddedResource(Assembly assembly, string resourceName) and call ReadContentsAsStream().")]
        public Stream ReadContentsAsStream(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;

            return ReadContentsAsStream();
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
        [Obsolete("Instantiate the class using EmbeddedResource(string resourceName) and call ReadContentsAsString().")]
        public string ReadContentsAsString(string resourceName)
        {
            _assembly = Assembly.GetCallingAssembly();
            _resourceName = resourceName;

            return ReadContentsAsString();
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
        [Obsolete("Instantiate the class using EmbeddedResource(string assemblyName, string resourceName) and call ReadContentsAsString().")]
        public string ReadContentsAsString(string assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            return ReadContentsAsString();
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
        [Obsolete("Instantiate the class using EmbeddedResource(AssemblyName assemblyName, string resourceName) and call ReadContentsAsString().")]
        public string ReadContentsAsString(AssemblyName assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            return ReadContentsAsString();
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
        [Obsolete("Instantiate the class using EmbeddedResource(Assembly assembly, string resourceName) and call ReadContentsAsString().")]
        public string ReadContentsAsString(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;

            return ReadContentsAsString();
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
        [Obsolete("Instantiate the class using EmbeddedResource(string resourceName) and call WriteAsFile(string fullPath).")]
        public void WriteAsFile(string resourceName, string fullPath)
        {
            _assembly = Assembly.GetCallingAssembly();
            _resourceName = resourceName;

            WriteAsFile(fullPath);
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
        [Obsolete("Instantiate the class using EmbeddedResource(string assemblyName, string resourceName) and call WriteAsFile(string fullPath).")]
        public void WriteAsFile(string assemblyName,
                                string resourceName,
                                string fullPath)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            WriteAsFile(fullPath);
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
        [Obsolete("Instantiate the class using EmbeddedResource(AssemblyName assemblyName, string resourceName) and call WriteAsFile(string fullPath).")]
        public void WriteAsFile(AssemblyName assemblyName,
                                string resourceName,
                                string fullPath)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;

            WriteAsFile(fullPath);
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
        [Obsolete("Instantiate the class using EmbeddedResource(Assembly assembly, string resourceName) and call WriteAsFile(string fullPath).")]
        public void WriteAsFile(Assembly assembly,
                                string resourceName,
                                string fullPath)
        {
            _assembly = assembly;
            _resourceName = resourceName;

            WriteAsFile(fullPath);
        }

        #endregion
    }

    // This class contains all the current methods
    public partial class EmbeddedResource
    {
        private Assembly _assembly;
        private string _resourceName;

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="EmbeddedResource"/> providing access to the specified resource in the calling assembly.
        /// </summary>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the calling assembly.</param>
        public EmbeddedResource(string resourceName) : this(Assembly.GetCallingAssembly(), resourceName)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="EmbeddedResource"/> providing access to the specified resource in the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        public EmbeddedResource(string assemblyName, string resourceName) : this(new AssemblyName(assemblyName), resourceName)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="EmbeddedResource"/> providing access to the specified resource in the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        public EmbeddedResource(AssemblyName assemblyName, string resourceName)
        {
            try
            {
                _assembly = Assembly.Load(assemblyName);
            }
            catch (Exception exception)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".", exception);
            }

            if (_assembly == null)
            {
                throw new FileNotFoundException($"Unable to load the assembly \"{assemblyName}\".");
            }

            _resourceName = resourceName;
        }

        /// <summary>
        /// Creates an instance of <see cref="EmbeddedResource"/> providing access to the specified resource in the specified assembly.
        /// </summary>
        /// <param name="assembly">The <c>Assembly</c> in which the resource has been embedded.</param>
        /// <param name="resourceName">The fully-qualified name of an embedded resource in the specified assembly.</param>
        public EmbeddedResource(Assembly assembly, string resourceName)
        {
            _assembly = assembly;
            _resourceName = resourceName;
        }

        #endregion

        #region ReadContentsAsByteArray

        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public byte[] ReadContentsAsByteArray()
        {
            using (var stream = _assembly.GetManifestResourceStream(_resourceName))
            {
                if (stream == null)
                {
                    throw new MissingManifestResourceException($"Unable to read the resource \"{_resourceName}\" from the assembly \"{_assembly.GetName().Name}\".");
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes,
                            0,
                            bytes.Length);

                return bytes;
            }
        }

        /// <summary>
        /// Reads the contents of the specified embedded resource as a byte array.
        /// </summary>
        /// <returns>
        /// A <c>byte[]</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public async Task<byte[]> ReadContentsAsByteArrayAsync()
        {
            using (var stream = _assembly.GetManifestResourceStream(_resourceName))
            {
                if (stream == null)
                {
                    throw new MissingManifestResourceException($"Unable to read the resource \"{_resourceName}\" from the assembly \"{_assembly.GetName().Name}\".");
                }

                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes,
                                       0,
                                       bytes.Length).ConfigureAwait(false);

                return bytes;
            }
        }

        #endregion

        #region ReadContentsAsStream

        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>Stream</c>.
        /// </summary>
        /// <returns>
        /// A <c>Stream</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public Stream ReadContentsAsStream()
        {
            var stream = _assembly.GetManifestResourceStream(_resourceName);

            if (stream == null)
            {
                throw new MissingManifestResourceException($"Unable to read the resource \"{_resourceName}\" from the assembly \"{_assembly.GetName().Name}\".");
            }

            return stream;
        }

        #endregion

        #region ReadContentsAsString

        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>string</c>.
        /// </summary>
        /// <returns>
        /// A <c>string</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public string ReadContentsAsString()
        {
            using (var stream = ReadContentsAsStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the contents of the specified embedded resource as a <c>string</c>.
        /// </summary>
        /// <returns>
        /// A <c>string</c> containing the contents of the specified embedded resource.
        /// </returns>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public async Task<string> ReadContentsAsStringAsync()
        {
            using (var stream = ReadContentsAsStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region WriteAsFile

        /// <summary>
        /// Writes an embedded resource to the specified path.
        /// </summary>
        /// <param name="fullPath">The full path of the file to be created with the embedded resource's contents.</param>
        /// <remarks>
        /// Probably only used for integration testing.
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified <see cref="assembly"/> argument could not be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public void WriteAsFile(string fullPath)
        {
            var contents = ReadContentsAsString();

            File.WriteAllText(fullPath, contents);
        }

        /// <summary>
        /// Writes an embedded resource to the specified path.
        /// </summary>
        /// <param name="fullPath">The full path of the file to be created with the embedded resource's contents.</param>
        /// <remarks>
        /// Probably only used for integration testing.
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified <see cref="assembly"/> argument could not be loaded.</exception>
        /// <exception cref="MissingManifestResourceException">No resource could be found in the specified assembly using the specified <see cref="resourceName"/> argument.</exception>
        public async Task WriteAsFileAsync(string fullPath)
        {
            var contents = await ReadContentsAsStringAsync().ConfigureAwait(false);

            File.WriteAllText(fullPath, contents);
        }

        #endregion
    }
}