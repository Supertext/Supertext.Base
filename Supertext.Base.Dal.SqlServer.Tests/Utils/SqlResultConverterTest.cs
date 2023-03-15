using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Utils;
using FluentAssertions;

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
        converter.InterpretUtcDates(testData);

        (((DateTime)testData[0]["a"]).Kind).Should().Be(DateTimeKind.Utc);
        (((DateTime)testData[0]["b"]).Kind).Should().Be(DateTimeKind.Utc);
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
        converter.DecodeStructure(testData);

        (testData[0]["a"] is Dictionary<string, object>).Should().BeTrue();
        (testData[0].Keys.Count).Should().Be(1);
        var a = (Dictionary<string, object>)testData[0]["a"];
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
        converter.DecodeStructure(testData);

        (testData[0]["a"] is Dictionary<string, object>).Should().BeTrue();
        (testData[0].Keys.Count).Should().Be(1);
        var a = (Dictionary<string, object>)testData[0]["a"];
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
        converter.InterpretUtcDates(testData);
        converter.DecodeStructure(testData);

        var a = (Dictionary<string, object>)testData[0]["a"];
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
        converter.InterpretUtcDates(testData);
        converter.DecodeStructure(testData);

        var a = (Dictionary<string, object>)testData[0]["a"];
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
        converter.DecodeStructure(testData);

        (testData[0]["a"] is JsonElement).Should().BeTrue();
        (testData[0].Keys.Count).Should().Be(1);
        var a = (JsonElement)testData[0]["a"];
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
        converter.DecodeStructure(testData);

        (testData[0].Keys.Count).Should().Be(0);
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
        converter.DecodeStructure(testData);

        (testData[0]["a"] is Dictionary<string, object>).Should().BeTrue();
        (testData[0].Keys.Count).Should().Be(1);
        var a = (Dictionary<string, object>)testData[0]["a"];
        var b = (JsonElement)a["b"];
        (b.ValueKind).Should().Be(JsonValueKind.Object);
        (b.GetProperty("c1").GetInt32()).Should().Be(1);
        (b.GetProperty("c2").GetInt32()).Should().Be(2);
    }
}