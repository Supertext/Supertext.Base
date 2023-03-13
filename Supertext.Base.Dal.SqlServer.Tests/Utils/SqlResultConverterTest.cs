using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.SqlServer.Tests.Utils;

[TestClass]
public class SqlResultConverterTest
{
    [TestMethod]
    public void Level1DateToUtc()
    {
        var testData = new List<Dictionary<string, object>>() {
                new () {
                    {"a", new DateTime()},
                    {"b", DateTime.UtcNow}
                }
            };

        var converter = new SqlResultConverter();
        converter.InterpretUtcDates(testData);

        Assert.AreEqual(DateTimeKind.Utc, ((DateTime)testData[0]["a"]).Kind);
        Assert.AreEqual(DateTimeKind.Utc, ((DateTime)testData[0]["b"]).Kind);
    }

    [TestMethod]
    public void Level2Keys()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b1", 1},
                {"a.b2", 2},
            }
        };
        var converter = new SqlResultConverter();
        converter.DecodeStructure(testData);

        Assert.IsTrue(testData[0]["a"] is Dictionary<string, object>);
        Assert.AreEqual(1, testData[0].Keys.Count);
        var a = (Dictionary<string, object>)testData[0]["a"];
        Assert.AreEqual(2, a.Keys.Count);
        Assert.AreEqual(1, a["b1"]);
        Assert.AreEqual(2, a["b2"]);
    }

    [TestMethod]
    public void Level3Keys()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b.c1", 1},
                {"a.b.c2", 2},
            }
        };
        var converter = new SqlResultConverter();
        converter.DecodeStructure(testData);

        Assert.IsTrue(testData[0]["a"] is Dictionary<string, object>);
        Assert.AreEqual(1, testData[0].Keys.Count);
        var a = (Dictionary<string, object>)testData[0]["a"];
        Assert.AreEqual(1, a.Keys.Count);
        var b = (Dictionary<string, object>)a["b"];
        Assert.AreEqual(2, b.Keys.Count);
        Assert.AreEqual(1, b["c1"]);
        Assert.AreEqual(2, b["c2"]);
    }

    [TestMethod]
    public void Level2DateToUtc()
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
        Assert.AreEqual(DateTimeKind.Utc, ((DateTime)a["b"]).Kind);
    }

    [TestMethod]
    public void Level3DateToUtc()
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
        Assert.AreEqual(DateTimeKind.Utc, ((DateTime)b["c"]).Kind);
    }

    [TestMethod]
    public void Level1Json()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a_json_", "{\"b1\":1,\"b2\":2}"}
            }
        };

        var converter = new SqlResultConverter();
        converter.DecodeStructure(testData);

        Assert.IsTrue(testData[0]["a"] is JsonElement);
        Assert.AreEqual(1, testData[0].Keys.Count);
        var a = (JsonElement)testData[0]["a"];
        Assert.AreEqual(JsonValueKind.Object, a.ValueKind);
        Assert.AreEqual(1, a.GetProperty("b1").GetInt32());
        Assert.AreEqual(2, a.GetProperty("b2").GetInt32());
    }

    [TestMethod]
    public void Level1JsonNull()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a_json_", null},
                {"b", null}
            }
        };

        var converter = new SqlResultConverter();
        converter.DecodeStructure(testData);

        Assert.AreEqual(0, testData[0].Keys.Count);
    }

    [TestMethod]
    public void Level2Json()
    {
        var testData = new List<Dictionary<string, object>>() {
            new () {
                {"a.b_json_", "{\"c1\":1,\"c2\":2}"}
            }
        };

        var converter = new SqlResultConverter();
        converter.DecodeStructure(testData);

        Assert.IsTrue(testData[0]["a"] is Dictionary<string, object>);
        Assert.AreEqual(1, testData[0].Keys.Count);
        var a = (Dictionary<string, object>)testData[0]["a"];
        var b = (JsonElement)a["b"];
        Assert.AreEqual(JsonValueKind.Object, b.ValueKind);
        Assert.AreEqual(1, b.GetProperty("c1").GetInt32());
        Assert.AreEqual(2, b.GetProperty("c2").GetInt32());
    }

}