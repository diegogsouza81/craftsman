﻿namespace Craftsman.Builders.Tests.UnitTests
{
    using Craftsman.Exceptions;
    using Craftsman.Helpers;
    using Craftsman.Models;
    using System.IO;
    using System.IO.Abstractions;
    using System.Text;
    using Enums;

    public class UpdateEntityUnitTestBuilder
    {
        public static void CreateTests(string solutionDirectory, string testDirectory, string srcDirectory, string entityName, string entityPlural, string projectBaseName, IFileSystem fileSystem)
        {
            var classPath = ClassPathHelper.UnitTestEntityTestsClassPath(testDirectory, $"{Utilities.UpdateEntityUnitTestName(entityName)}.cs", entityPlural, projectBaseName);
            var fileText = WriteTestFileText(solutionDirectory, srcDirectory, classPath, entityName, entityPlural, projectBaseName);
            Utilities.CreateFile(classPath, fileText, fileSystem);
        }

        private static string WriteTestFileText(string solutionDirectory, string srcDirectory, ClassPath classPath, string entityName, string entityPlural, string projectBaseName)
        {
            var entityClassPath = ClassPathHelper.EntityClassPath(srcDirectory, "", entityPlural, projectBaseName);
            var fakerClassPath = ClassPathHelper.TestFakesClassPath(solutionDirectory, "", entityName, projectBaseName);
            var updateDto = Utilities.GetDtoName(entityName, Dto.Update);
            var fakeEntityForUpdate = $"Fake{updateDto}";

            return @$"namespace {classPath.ClassNamespace};

using {fakerClassPath.ClassNamespace};
using {entityClassPath.ClassNamespace};
using Bogus;
using FluentAssertions;
using NUnit.Framework;

public class {Path.GetFileNameWithoutExtension(classPath.FullClassPath)}
{{
    private readonly Faker _faker;

    public {Path.GetFileNameWithoutExtension(classPath.FullClassPath)}()
    {{
        _faker = new Faker();
    }}
    
    [Test]
    public void can_update_{entityName.LowercaseFirstLetter()}()
    {{
        // Arrange
        var fake{entityName} = Fake{entityName}.Generate();
        var updated{entityName} = new {fakeEntityForUpdate}().Generate();
        
        // Act
        fake{entityName}.Update(updated{entityName});

        // Assert
        fake{entityName}.Should().BeEquivalentTo(updated{entityName}, options =>
            options.ExcludingMissingMembers());
    }}
}}";
        }
    }
}