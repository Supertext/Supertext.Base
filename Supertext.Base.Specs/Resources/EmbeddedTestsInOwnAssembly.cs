using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Resources;
using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace Supertext.Base.Specs.Resources
{
    [TestClass]
    public class EmbeddedTestsInOwnAssembly
    {
        private static Assembly _assembly;
        private static DirectoryInfo _diTestDir;
        private static FileInfo _fiTestFile;
        private const string ResourceName = "Supertext.Base.Specs.Resources.TestFiles.lorem_ipsum.txt";

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _assembly = Assembly.GetAssembly(typeof(EmbeddedTests));
            var codebase = _assembly.CodeBase.Replace("file:///", String.Empty);
            var diProject = new FileInfo(codebase).Directory;//.Parent.Parent;
            _diTestDir = new DirectoryInfo(Path.Combine(diProject.FullName,
                                                        "Resources",
                                                        "TestFiles",
                                                        "EmbeddedTestWrites"));
            if (!_diTestDir.Exists)
            {
                _diTestDir.Create();
            }
            _fiTestFile = new FileInfo(Path.Combine(diProject.FullName,
                                                    "Resources",
                                                    "TestFiles",
                                                    "lorem_ipsum.txt"));

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (_diTestDir.Exists)
            {
                _diTestDir.Delete(true);
            }
        }

        [TestMethod]
        public void Constructor_Throws_Expected_Exception_For_Invalid_Assembly_Name()
        {
            // Arrange
            var invalidAssemblyName = Guid.NewGuid().ToString();
            const string resourceName = "whatever";
            var exceptionThrown = false;

            // overload with only resource name
            // Act
            try
            {
                new EmbeddedResource(invalidAssemblyName, resourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(FileNotFoundException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void ReadContentsAsByteArray_Returns_Expected_Result()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // Act
            var result = testee.ReadContentsAsByteArray();

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);
        }

        [TestMethod]
        public async Task ReadContentsAsByteArrayAsync_Returns_Expected_Result()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // Act
            var result = await testee.ReadContentsAsByteArrayAsync();

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);
        }

        [TestMethod]
        public void ReadContentsAsStream_Returns_Expected_Result()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // Act
            using (var result = testee.ReadContentsAsStream())
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }
        }

        [TestMethod]
        public void ReadContentsAsString_Returns_Expected_Result()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);

            // Act
            var result = testee.ReadContentsAsString();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);
        }

        [TestMethod]
        public async Task ReadContentsAsStringAsync_Returns_Expected_Result()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);

            // Act
            var result = await testee.ReadContentsAsStringAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);
        }

        [TestMethod]
        public void ReadContentsAsString_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();

            // overload with only resource name
            // Act
            var testee = new EmbeddedResource(invalidResourceName);

            // Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), invalidResourceName);

            // Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, invalidResourceName);

            // Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, invalidResourceName);

            // Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();
        }

        [TestMethod]
        public async Task ReadContentsAsStringAsync_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();

            // overload with only resource name
            // Act
            var testee = new EmbeddedResource(invalidResourceName);

            // Assert
            Func<Task> act = async () => { await testee.ReadContentsAsStringAsync(); };
            await act.Should().ThrowAsync<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), invalidResourceName);

            // Assert
            act = async () => { await testee.ReadContentsAsStringAsync(); };
            await act.Should().ThrowAsync<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, invalidResourceName);

            // Assert
            act = async () => { await testee.ReadContentsAsStringAsync(); };
            await act.Should().ThrowAsync<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, invalidResourceName);

            // Assert
            act = async () => { await testee.ReadContentsAsStringAsync(); };
            await act.Should().ThrowAsync<MissingManifestResourceException>();
        }

        [TestMethod]
        public void WriteAsFile_Produces_Expected_File()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");
            var fiWriteFileName = new FileInfo(writeFileName);

            // overload with only resource name
            // Act
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();


            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();



            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, ResourceName);
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();
        }

        [TestMethod]
        public async Task WriteAsFileAsync_Produces_Expected_File()
        {
            // Arrange
            var testee = new EmbeddedResource(ResourceName);
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");
            var fiWriteFileName = new FileInfo(writeFileName);

            // overload with only resource name
            // Act
            await testee.WriteAsFileAsync(writeFileName).ConfigureAwait(false);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();


            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);
            await testee.WriteAsFileAsync(writeFileName).ConfigureAwait(false);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);
            await testee.WriteAsFileAsync(writeFileName).ConfigureAwait(false);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();



            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, ResourceName);
            await testee.WriteAsFileAsync(writeFileName).ConfigureAwait(false);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();
        }

        [TestMethod]
        public void WriteAsFile_Throws_Expected_Exception_For_Invalid_assemblyName()
        {
            // Arrange
            var invalidAssemblyName = Guid.NewGuid().ToString();
            var testee = new EmbeddedResource();
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");


            // overload with AssemblyName and resource name

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(new AssemblyName(invalidAssemblyName),
                                               ResourceName,
                                               writeFileName)).Should().Throw<FileNotFoundException>();


            // overload with assembly name and resource name

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(invalidAssemblyName,
                                               ResourceName,
                                               writeFileName)).Should().Throw<FileNotFoundException>();
        }

        [TestMethod]
        public void WriteAsFile_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var testee = new EmbeddedResource();
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");

            // overload with only resource name
            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(invalidResourceName, writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(_assembly.GetName(), invalidResourceName, writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(_assembly.GetName().Name, invalidResourceName, writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(_assembly, invalidResourceName, writeFileName)).Should().Throw<MissingManifestResourceException>();
        }

        [TestMethod]
        public void WriteAsFile_Throws_Expected_Exception_For_Invalid_resourceName_NEW()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");

            // overload with only resource name
            // Act
            var testee = new EmbeddedResource(invalidResourceName);

            // Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), invalidResourceName);

            // Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, invalidResourceName);

            // Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, invalidResourceName);

            // Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();
        }
    }
}