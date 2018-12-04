using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Resources;
using Exception = System.Exception;


namespace Supertext.Base.Test.Resources
{
    [TestClass]
    public class EmbeddedTests
    {
        private static Assembly _assembly;
        private static DirectoryInfo _diTestDir;
        private static FileInfo _fiTestFile;
        private const string ResourceName = "Supertext.Base.Test.Resources.TestFiles.lorem_ipsum.txt";


        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _assembly = Assembly.GetAssembly(typeof(EmbeddedTests));
            var codebase = _assembly.CodeBase.Replace("file:///", String.Empty);
            var diProject = new FileInfo(codebase).Directory.Parent.Parent;
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
        public void ReadContentsAsByteArray_Returns_Expected_Result()
        {
            // Arrange
            var testee = new Base.Resources.EmbeddedResource();
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // overload with only resource name
            // Act
            var result = testee.ReadContentsAsByteArray(ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);



            // overload with assembly name and resource name

            // Act
            result = testee.ReadContentsAsByteArray(_assembly.GetName(), ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);

            // Act
            result = testee.ReadContentsAsByteArray(_assembly.GetName().Name, ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);



            // overload with assembly and resource name

            // Act
            result = testee.ReadContentsAsByteArray(_assembly, ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(resourceFileContents.Length);
        }


        [TestMethod]
        public void ReadContentsAsByteArray_Throws_Expected_Exception_For_Invalid_assemblyName()
        {
            // Arrange
            var invalidAssemblyName = Guid.NewGuid().ToString();
            var testee = new Base.Resources.EmbeddedResource();
            var exceptionThrown = false;


            // overload with assembly name and resource name

            // Act
            try
            {
                testee.ReadContentsAsByteArray(new AssemblyName(invalidAssemblyName), ResourceName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                testee.ReadContentsAsByteArray(invalidAssemblyName, ResourceName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
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
            var testee = new Base.Resources.EmbeddedResource();
            var exceptionThrown = false;

            // overload with only resource name
            // Act
            try
            {
                testee.ReadContentsAsByteArray(invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly name and resource name

            // Act
            try
            {
                testee.ReadContentsAsByteArray(_assembly.GetName(), invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                testee.ReadContentsAsByteArray(_assembly.GetName().Name, invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly and resource name

            // Act
            try
            {
                testee.ReadContentsAsByteArray(_assembly, invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
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
        public void ReadContentsAsStream_Returns_Expected_Result()
        {
            // Arrange
            var testee = new Base.Resources.EmbeddedResource();
            var resourceFileContents = File.ReadAllBytes(_fiTestFile.FullName);

            // overload with only resource name
            // Act
            using (var result = testee.ReadContentsAsStream(ResourceName))
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }


            // overload with assembly name and resource name

            // Act
            using (var result = testee.ReadContentsAsStream(_assembly.GetName(), ResourceName))
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }

            // Act
            using (var result = testee.ReadContentsAsStream(_assembly.GetName().Name, ResourceName))
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }


            // overload with assembly and resource name

            // Act
            using (var result = testee.ReadContentsAsStream(_assembly, ResourceName))
            {
                // Assert
                result.Should().NotBeNull();
                result.Length.Should().Be(resourceFileContents.Length);
            }
        }


        [TestMethod]
        public void ReadContentsAsStream_Throws_Expected_Exception_For_Invalid_assemblyName()
        {
            // Arrange
            var invalidAssemblyName = Guid.NewGuid().ToString();
            var testee = new Base.Resources.EmbeddedResource();
            var exceptionThrown = false;


            // overload with assembly name and resource name

            // Act
            try
            {
                using (testee.ReadContentsAsStream(new AssemblyName(invalidAssemblyName), ResourceName))
                { }
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                using (testee.ReadContentsAsStream(invalidAssemblyName, ResourceName))
                { }
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }


        [TestMethod]
        public void ReadContentsAsStream_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var testee = new Base.Resources.EmbeddedResource();
            var exceptionThrown = false;

            // overload with only resource name
            // Act
            try
            {
                using (testee.ReadContentsAsStream(invalidResourceName))
                { }
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly name and resource name

            // Act
            try
            {
                using (testee.ReadContentsAsStream(_assembly.GetName(), invalidResourceName))
                { }
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                using (testee.ReadContentsAsStream(_assembly.GetName().Name, invalidResourceName))
                { }
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly and resource name

            // Act
            try
            {
                using (testee.ReadContentsAsStream(_assembly, invalidResourceName))
                { }
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
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
        public void ReadContentsAsString_Returns_Expected_Result()
        {
            // Arrange
            var testee = new Base.Resources.EmbeddedResource();
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);

            // overload with only resource name
            // Act
            var result = testee.ReadContentsAsString(ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);



            // overload with assembly name and resource name

            // Act
            result = testee.ReadContentsAsString(_assembly.GetName(), ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);

            // Act
            result = testee.ReadContentsAsString(_assembly.GetName().Name, ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);



            // overload with assembly and resource name

            // Act
            result = testee.ReadContentsAsString(_assembly, ResourceName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resourceFileContents);
        }


        [TestMethod]
        public void ReadContentsAsString_Throws_Expected_Exception_For_Invalid_assemblyName()
        {
            // Arrange
            var invalidAssemblyName = Guid.NewGuid().ToString();
            var testee = new Base.Resources.EmbeddedResource();
            var exceptionThrown = false;


            // overload with assembly name and resource name

            // Act
            try
            {
                testee.ReadContentsAsString(new AssemblyName(invalidAssemblyName), ResourceName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                testee.ReadContentsAsString(invalidAssemblyName, ResourceName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }


        [TestMethod]
        public void ReadContentsAsString_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var testee = new Base.Resources.EmbeddedResource();
            var exceptionThrown = false;

            // overload with only resource name
            // Act
            try
            {
                testee.ReadContentsAsString(invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly name and resource name

            // Act
            try
            {
                testee.ReadContentsAsString(_assembly.GetName(), invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                testee.ReadContentsAsString(_assembly.GetName().Name, invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly and resource name

            // Act
            try
            {
                testee.ReadContentsAsString(_assembly, invalidResourceName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
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
        public void WriteAsFile_Produces_Expected_File()
        {
            // Arrange
            var testee = new Base.Resources.EmbeddedResource();
            var resourceFileContents = File.ReadAllText(_fiTestFile.FullName);
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");
            var fiWriteFileName = new FileInfo(writeFileName);

            // overload with only resource name
            // Act
            testee.WriteAsFile(ResourceName, writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();


            // overload with assembly name and resource name

            // Act
            testee.WriteAsFile(_assembly.GetName(),
                               ResourceName,
                               writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();

            // Act
            testee.WriteAsFile(_assembly.GetName().Name,
                               ResourceName,
                               writeFileName);

            // Assert
            fiWriteFileName.Exists.Should().BeTrue();
            File.ReadAllText(writeFileName).Should().BeEquivalentTo(resourceFileContents);

            fiWriteFileName.Delete();



            // overload with assembly and resource name

            // Act
            testee.WriteAsFile(_assembly,
                               ResourceName,
                               writeFileName);

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
            var testee = new Base.Resources.EmbeddedResource();
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");
            var exceptionThrown = false;

            // overload with assembly name and resource name

            // Act
            try
            {
                testee.WriteAsFile(new AssemblyName(invalidAssemblyName),
                                   ResourceName,
                                   writeFileName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                testee.WriteAsFile(invalidAssemblyName,
                                   ResourceName,
                                   writeFileName);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }


        [TestMethod]
        public void WriteAsFile_Throws_Expected_Exception_For_Invalid_resourceName()
        {
            // Arrange
            var invalidResourceName = Guid.NewGuid().ToString();
            var testee = new Base.Resources.EmbeddedResource();
            var writeFileName = Path.Combine(_diTestDir.FullName, "embedded_write.txt");
            var exceptionThrown = false;

            // overload with only resource name
            // Act
            try
            {
                testee.WriteAsFile(invalidResourceName, writeFileName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly name and resource name

            // Act
            try
            {
                testee.WriteAsFile(_assembly.GetName(),
                                   invalidResourceName,
                                   writeFileName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;


            // Act
            try
            {
                testee.WriteAsFile(_assembly.GetName().Name,
                                   invalidResourceName,
                                   writeFileName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }

            // reset
            exceptionThrown = false;



            // overload with assembly and resource name

            // Act
            try
            {
                testee.WriteAsFile(_assembly,
                                   invalidResourceName,
                                   writeFileName);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;
                if (exception.GetType() != typeof(MissingManifestResourceException))
                {
                    Assert.Fail("The exception was not of the expected type.");
                }
            }

            if (!exceptionThrown)
            {
                Assert.Fail("The expected exception was not thrown.");
            }
        }
    }
}