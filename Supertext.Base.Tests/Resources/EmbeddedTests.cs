using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace Supertext.Base.Tests.Resources
{
    [TestClass]
    public class EmbeddedTests
    {
        private static Assembly _assembly;
        private static DirectoryInfo _diTestDir;
        private static FileInfo _fiTestFile;
        private const string ResourceName = "Supertext.Base.Tests.Resources.TestFiles.lorem_ipsum.txt";

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _assembly = Assembly.GetAssembly(typeof(EmbeddedTests));
            var codebase = _assembly.CodeBase.Replace("file:///", String.Empty);
            var diProject = new FileInfo(codebase).Directory; //.Parent.Parent;
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
            var exceptionThrown = false;

            // Act
            try
            {
                new EmbeddedResource(invalidAssemblyName, ResourceName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected {typeof(FileNotFoundException).Name} but caught {exception.GetType().Name}.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void Constructor_Throws_Expected_Exception_For_Invalid_assemblyName()
        {
            // Arrange
            var invalidAssemblyName = Guid.NewGuid().ToString();
            var exceptionThrown = false;

            // Act
            try
            {
                new EmbeddedResource(invalidAssemblyName, ResourceName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected {typeof(FileNotFoundException).Name} but caught {exception.GetType().Name}.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }

        [TestMethod]
        public void ReadContentsAsByteArray_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var testee = new EmbeddedResource(invalidResourceName);

            // Act
            Action act = () => testee.ReadContentsAsByteArray();

            // Assert
            act.Should().Throw<MissingManifestResourceException>();
        }

        [TestMethod]
        public void ReadContentsAsByteArrayAsync_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var testee = new EmbeddedResource(invalidResourceName);

            // Act
            Func<Task<IEnumerable<byte>>> func = async () => await testee.ReadContentsAsByteArrayAsync();

            // Assert
            func.Should().Throw<MissingManifestResourceException>();
        }

        [TestMethod]
        public void ReadContentsAsByteArray_Returns_Expected_Result()
        {
            // Arrange
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // overload with only resource name

            // Arrange
            var testee = new EmbeddedResource(ResourceName);

            // Act
            var result = testee.ReadContentsAsByteArray();

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);



            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);

            // Act
            result = testee.ReadContentsAsByteArray();

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);


            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);

            // Act
            result = testee.ReadContentsAsByteArray();

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);



            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, ResourceName);

            // Act
            result = testee.ReadContentsAsByteArray();

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

            // overload with only resource name
            // Act
            var result = await testee.ReadContentsAsByteArrayAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);



            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);
            result = await testee.ReadContentsAsByteArrayAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);
            result = await testee.ReadContentsAsByteArrayAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);



            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, ResourceName);
            result = await testee.ReadContentsAsByteArrayAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);
        }

        [TestMethod]
        public void ReadContentsAsStream_Returns_Expected_Result()
        {
            // Arrange
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // overload with only resource name

            // Arrange
            var testee = new EmbeddedResource(ResourceName);

            // Act
            using (var result = testee.ReadContentsAsStream())
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }


            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);

            // Act
            using (var result = testee.ReadContentsAsStream())
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }


            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);

            // Act
            using (var result = testee.ReadContentsAsStream())
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }


            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, ResourceName);

            // Act
            using (var result = testee.ReadContentsAsStream())
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }
        }

        [TestMethod]
        public void ReadContentsAsStream_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();

            // overload with only resource name

            // Arrange
            var testee = new EmbeddedResource(invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsStream()).Should().Throw<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsStream()).Should().Throw<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsStream()).Should().Throw<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsStream()).Should().Throw<MissingManifestResourceException>();
        }

        [TestMethod]
        public void ReadContentsAsString_Returns_Expected_Result()
        {
            // Arrange
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);

            // overload with only resource name

            // Arrange
            var testee = new EmbeddedResource(ResourceName);

            // Act
            var result = testee.ReadContentsAsString();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);



            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);

            // Act
            result = testee.ReadContentsAsString();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);


            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);

            // Act
            result = testee.ReadContentsAsString();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);



            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, ResourceName);

            // Act
            result = testee.ReadContentsAsString();

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

            // overload with only resource name
            // Act
            var result = await testee.ReadContentsAsStringAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);



            // overload with assembly name and resource name

            // Act
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);
            result = await testee.ReadContentsAsStringAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);

            // Act
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);
            result = await testee.ReadContentsAsStringAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);



            // overload with assembly and resource name

            // Act
            testee = new EmbeddedResource(_assembly, ResourceName);
            result = await testee.ReadContentsAsStringAsync().ConfigureAwait(false);

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

            // Arrange
            var testee = new EmbeddedResource(invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.ReadContentsAsString()).Should().Throw<MissingManifestResourceException>();
        }

        [TestMethod]
        public void WriteAsFile_Produces_Expected_File()
        {
            // Arrange
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");
            var fiWriteFileName = new FileInfo(writeFileName);

            // overload with only resource name

            // Arrange
            var testee = new EmbeddedResource(ResourceName);

            // Act
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();


            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), ResourceName);

            // Act
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();


            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, ResourceName);

            // Act
            testee.WriteAsFile(writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();



            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, ResourceName);

            // Act
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
        public void WriteAsFile_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");

            // overload with only resource name

            // Arrange
            var testee = new EmbeddedResource(invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with AssemblyName and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName(), invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with assembly name and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly.GetName().Name, invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();


            // overload with assembly and resource name

            // Arrange
            testee = new EmbeddedResource(_assembly, invalidResourceName);

            // Act & Assert
            testee.Invoking(t => t.WriteAsFile(writeFileName)).Should().Throw<MissingManifestResourceException>();
        }
    }
}