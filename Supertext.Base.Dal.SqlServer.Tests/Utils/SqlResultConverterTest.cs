using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Utils;
using FluentAssertions;
using System.Linq;

namespace Supertext.Base.Dal.SqlServer.Tests.Utils;

[TestClass]
public class SqlResultConverterTest
{
    [TestMethod]
    public void InterpretUtcDates_DatesInUtcFormatOnLevel1_ConvertedInUtc()
    {
        var testData = new List<Dictionary<string, object>>() {
                new () {
                    {"a", new DateTime()},
                    {"b", DateTime.UtcNow}
                }
            };

        var converter = new SqlResultConverter();
        var results = testData.Select(converter.InterpretUtcDates).ToList();

        (((DateTime)results[0]["a"]).Kind).Should().Be(DateTimeKind.Utc);
        (((DateTime)results[0]["b"]).Kind).Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void DecodeStructure_LevelEncodedFieldNames_Level2Keys()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b1", 1},
                {"a.b2", 2},
            }
        };
        var converter = new SqlResultConverter();
        var results = testData.Select(converter.DecodeStructure).ToList();

        (results[0]["a"] is Dictionary<string, object>).Should().BeTrue();
        (results[0].Keys.Count).Should().Be(1);
        var a = (Dictionary<string, object>)results[0]["a"];
        (a.Keys.Count).Should().Be(2);
        (a["b1"]).Should().Be(1);
        (a["b2"]).Should().Be(2);
    }

    [TestMethod]
    public void DecodeStructure_LevelEncodedFieldNames_Level3Keys()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b.c1", 1},
                {"a.b.c2", 2},
            }
        };
        var converter = new SqlResultConverter();
        var results = testData.Select(converter.DecodeStructure).ToList();

        (results[0]["a"] is Dictionary<string, object>).Should().BeTrue();
        (results[0].Keys.Count).Should().Be(1);
        var a = (Dictionary<string, object>)results[0]["a"];
        (a.Keys.Count).Should().Be(1);
        var b = (Dictionary<string, object>)a["b"];
        (b.Keys.Count).Should().Be(2);
        (b["c1"]).Should().Be(1);
        (b["c2"]).Should().Be(2);
    }

    [TestMethod]
    public void InterpretUtcDates_DatesInUtcFormatOnLevel2_ConvertedInUtc()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b", new DateTime()},
            }
        };

        var converter = new SqlResultConverter();
        var results = testData
            .Select(converter.InterpretUtcDates)
            .Select(converter.DecodeStructure)
            .ToList();

        var a = (Dictionary<string, object>)results[0]["a"];
        (((DateTime)a["b"]).Kind).Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void InterpretUtcDates_DatesInUtcFormatOnLevel3_ConvertedInUtc()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b.c", new DateTime()},
            }
        };

        var converter = new SqlResultConverter();
        var results = testData
            .Select(converter.InterpretUtcDates)
            .Select(converter.DecodeStructure)
            .ToList();

        var a = (Dictionary<string, object>)results[0]["a"];
        var b = (Dictionary<string, object>)a["b"];
        (((DateTime)b["c"]).Kind).Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void DecodeStructure_JsonEncodedFieldNames_Level1Json()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a_json_", "{\"b1\":1,\"b2\":2}"}
            }
        };

        var converter = new SqlResultConverter();
        var results = testData.Select(converter.DecodeStructure).ToList();

        (results[0]["a"] is JsonElement).Should().BeTrue();
        (results[0].Keys.Count).Should().Be(1);
        var a = (JsonElement)results[0]["a"];
        (a.ValueKind).Should().Be(JsonValueKind.Object);
        (a.GetProperty("b1").GetInt32()).Should().Be(1);
        (a.GetProperty("b2").GetInt32()).Should().Be(2);
    }

    [TestMethod]
    public void DecodeStructure_JsonEncodedFieldNamesNull_FieldsRemoved()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a_json_", null},
                {"b", null}
            }
        };

        var converter = new SqlResultConverter();
        var results = testData.Select(converter.DecodeStructure).ToList();

        (results[0].Keys.Count).Should().Be(0);
    }

    [TestMethod]
    public void DecodeStructure_JsonEncodedFieldNames_Level2Json()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b_json_", "{\"c1\":1,\"c2\":2}"}
            }
        };

        var converter = new SqlResultConverter();
        var results = testData.Select(converter.DecodeStructure).ToList();

        (results[0]["a"] is Dictionary<string, object>).Should().BeTrue();
        (results[0].Keys.Count).Should().Be(1);
        var a = (Dictionary<string, object>)results[0]["a"];
        var b = (JsonElement)a["b"];
        (b.ValueKind).Should().Be(JsonValueKind.Object);
        (b.GetProperty("c1").GetInt32()).Should().Be(1);
        (b.GetProperty("c2").GetInt32()).Should().Be(2);
    }

    [TestMethod]
    public void InterpretUtcDates_ObjectWithNonNullableDateTime_ConvertedInUtc()
    {
        var testData = new TestEntity(Guid.NewGuid(),
                                      "Test",
                                      DateTime.Now,
                                      null);

        testData.CreatedOn.Kind.Should().Be(DateTimeKind.Local);

        var converter = new SqlResultConverter();
        var results = converter.InterpretUtcDates(testData);

        results.Id.Should().Be(testData.Id);
        results.Name.Should().Be(testData.Name);
        results.CreatedOn.Kind.Should().Be(DateTimeKind.Utc);
        results.UpdatedOn.Should().BeNull();
    }

    [TestMethod]
    public void InterpretUtcDates_ObjectWithNullableDateTime_ConvertedInUtc()
    {
        var testData = new TestEntity(Guid.NewGuid(),
                                      "Test",
                                      DateTime.Now.AddDays(-2),
                                      DateTime.Now);

        testData.UpdatedOn!.Value.Kind.Should().Be(DateTimeKind.Local);

        var converter = new SqlResultConverter();
        var results = converter.InterpretUtcDates(testData);

        results.UpdatedOn!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void InterpretUtcDates_ObjectWithNestedObject_ConvertedInUtc()
    {
        var nestedObject = new TestEntity(Guid.NewGuid(),
                                          "Nested",
                                          DateTime.Now,
                                          null);

        var testData = new TestEntity(Guid.NewGuid(),
                                      "Test",
                                      DateTime.Now,
                                      null,
                                      nestedObject);

        testData.NestedObject.CreatedOn.Kind.Should().Be(DateTimeKind.Local);

        var converter = new SqlResultConverter();
        var results = converter.InterpretUtcDates(testData);

        results.NestedObject!.Id.Should().Be(nestedObject.Id);
        results.NestedObject.Name.Should().Be(nestedObject.Name);
        results.NestedObject.CreatedOn.Kind.Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void InterpretUtcDates_ObjectWithMultipleNestedObject_ConvertedInUtc()
    {
        var nestedObject1 = new TestEntity(Guid.NewGuid(),
                                          "Nested 1",
                                          DateTime.Now,
                                          null);

        var nestedObject2 = new TestEntity(Guid.NewGuid(),
                                           "Nested 2",
                                           DateTime.Now.AddDays(-5),
                                           DateTime.Now,
                                           nestedObject1);

        var testData = new TestEntity(Guid.NewGuid(),
                                      "Test",
                                      DateTime.Now.AddDays(-5),
                                      null,
                                      nestedObject2);

        testData.NestedObject.NestedObject.CreatedOn.Kind.Should().Be(DateTimeKind.Local);

        var converter = new SqlResultConverter();
        var results = converter.InterpretUtcDates(testData);

        results.NestedObject!.NestedObject!.Id.Should().Be(nestedObject1.Id);
        results.NestedObject.NestedObject.Name.Should().Be(nestedObject1.Name);
        results.NestedObject.NestedObject.CreatedOn.Kind.Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void InterpretUtcDates_ObjectWithCollection_ConvertedInUtc()
    {
        var item1 = new TestEntity(Guid.NewGuid(),
                                           "Item 1",
                                           DateTime.Now,
                                           null);

        var item2 = new TestEntity(Guid.NewGuid(),
                                           "Item 2",
                                           DateTime.Now.AddDays(-5),
                                           DateTime.Now);

        var testData = new TestEntity(Guid.NewGuid(),
                                      "Test",
                                      DateTime.Now.AddDays(-5),
                                      null,
                                      NestedCollection: [item1, item2]);

        testData.NestedCollection[0].CreatedOn.Kind.Should().Be(DateTimeKind.Local);
        testData.NestedCollection[1].CreatedOn.Kind.Should().Be(DateTimeKind.Local);
        testData.NestedCollection[1].UpdatedOn!.Value.Kind.Should().Be(DateTimeKind.Local);

        var converter = new SqlResultConverter();
        var results = converter.InterpretUtcDates(testData);

        results.NestedCollection!.Count.Should().Be(2);
        results.NestedCollection[0].Id.Should().Be(item1.Id);
        results.NestedCollection[0].Name.Should().Be(item1.Name);
        results.NestedCollection[0].CreatedOn.Kind.Should().Be(DateTimeKind.Utc);
        results.NestedCollection[1].Id.Should().Be(item2.Id);
        results.NestedCollection[1].Name.Should().Be(item2.Name);
        results.NestedCollection[1].CreatedOn.Kind.Should().Be(DateTimeKind.Utc);
        results.NestedCollection[1].UpdatedOn!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [TestMethod]
    public void InterpretUtcDates_ObjectWithCollectionThatHasNestedObject_ConvertedInUtc()
    {
        var nestedObject = new TestEntity(Guid.NewGuid(),
                                           "Nested",
                                           DateTime.Now,
                                           null);

        var item = new TestEntity(Guid.NewGuid(),
                                           "Item",
                                           DateTime.Now.AddDays(-5),
                                           null,
                                           nestedObject);

        var testData = new TestEntity(Guid.NewGuid(),
                                      "Test",
                                      DateTime.Now.AddDays(-5),
                                      null,
                                      NestedCollection: [item]);

        testData.NestedCollection[0].NestedObject.CreatedOn.Kind.Should().Be(DateTimeKind.Local);

        var converter = new SqlResultConverter();
        var results = converter.InterpretUtcDates(testData);

        results.NestedCollection!.Count.Should().Be(1);
        results.NestedCollection[0].NestedObject!.Id.Should().Be(nestedObject.Id);
        results.NestedCollection[0].NestedObject!.Name.Should().Be(nestedObject.Name);
        results.NestedCollection[0].NestedObject!.CreatedOn.Kind.Should().Be(DateTimeKind.Utc);
    }

    private record TestEntity(Guid Id,
                              string Name,
                              DateTime CreatedOn,
                              DateTime? UpdatedOn,
                              TestEntity NestedObject = null,
                              IList<TestEntity> NestedCollection = null);
}