////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationConfigConverterTests.cs
//
// Unit tests for AnimationConfigConverter and AnimationConfigJson.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool;
using ACAT.ConfigMigrationTool.Configuration;
using System.Text.Json;
using System.Xml;

namespace ACAT.ConfigMigrationTool.Tests
{
    [TestClass]
    public class AnimationConfigConverterTests
    {
        private string _tempDir = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "acat-anim-test-" + Guid.NewGuid());
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
            {
                try { Directory.Delete(_tempDir, true); }
                catch { /* best-effort */ }
            }
        }

        // ----------------------------------------------------------------
        // ConvertFile — null / skip when no <Animations> present
        // ----------------------------------------------------------------

        [TestMethod]
        public void ConvertFile_NoAnimationsElement_ReturnsNull()
        {
            string xml = @"<?xml version=""1.0""?>
<ACAT>
  <WidgetAttributes />
  <Layout />
</ACAT>";
            string path = WriteTempXml("NoAnim.xml", xml);

            var converter = new AnimationConfigConverter();
            var result = converter.ConvertFile(path);

            Assert.IsNull(result, "Should return null when no <Animations> element present");
        }

        [TestMethod]
        public void ConvertFile_ThrowsWhenFileNotFound()
        {
            var converter = new AnimationConfigConverter();
            bool threw = false;
            try
            {
                converter.ConvertFile(@"C:\does\not\exist.xml");
            }
            catch (FileNotFoundException)
            {
                threw = true;
            }
            Assert.IsTrue(threw, "Expected FileNotFoundException for missing file");
        }

        // ----------------------------------------------------------------
        // ConvertNode — basic round-trip
        // ----------------------------------------------------------------

        [TestMethod]
        public void ConvertNode_SingleSequence_MapsAllAttributes()
        {
            var doc = new XmlDocument();
            doc.LoadXml(@"
<Animations>
  <Animation name=""Row1"" start=""true"" autoStart=""true""
             scanTime=""600"" iterations=""3"" firstPauseTime=""200""
             onEnter=""enter()"" onEnd=""end()"">
    <Widget name=""Btn1"" onSelect=""actuate(Btn1)"" />
    <Widget name=""Btn2"" onSelect=""actuate(Btn2)"" playBeep=""true"" />
  </Animation>
</Animations>");

            var converter = new AnimationConfigConverter();
            AnimationConfigJson config = converter.ConvertNode("TestPanel", doc.DocumentElement!);

            Assert.AreEqual("TestPanel", config.PanelName);
            Assert.AreEqual("auto", config.ScanStrategy);
            Assert.AreEqual(1, config.Sequences.Count);

            AnimationSequenceConfigJson seq = config.Sequences[0];
            Assert.AreEqual("Row1", seq.Name);
            Assert.IsTrue(seq.IsFirst);
            Assert.IsTrue(seq.AutoStart);
            Assert.AreEqual("600", seq.ScanTime);
            Assert.AreEqual("3", seq.Iterations);
            Assert.AreEqual("200", seq.FirstPauseTime);
            Assert.AreEqual("enter()", seq.OnEnter);
            Assert.AreEqual("end()", seq.OnEnd);

            Assert.AreEqual(2, seq.Widgets.Count);
            Assert.AreEqual("Btn1", seq.Widgets[0].Name);
            Assert.AreEqual("actuate(Btn1)", seq.Widgets[0].OnSelected);
            Assert.IsFalse(seq.Widgets[0].PlayBeep);

            Assert.AreEqual("Btn2", seq.Widgets[1].Name);
            Assert.IsTrue(seq.Widgets[1].PlayBeep);
        }

        [TestMethod]
        public void ConvertNode_MultipleSequences_FirstFlagSetCorrectly()
        {
            var doc = new XmlDocument();
            doc.LoadXml(@"
<Animations>
  <Animation name=""SeqA"" start=""true"" scanTime=""200"" iterations=""1"">
    <Widget name=""W1"" />
  </Animation>
  <Animation name=""SeqB"" start=""false"" scanTime=""400"" iterations=""2"">
    <Widget name=""W2"" />
    <Widget name=""W3"" />
  </Animation>
</Animations>");

            var converter = new AnimationConfigConverter();
            AnimationConfigJson config = converter.ConvertNode("MultiPanel", doc.DocumentElement!);

            Assert.AreEqual(2, config.Sequences.Count);
            Assert.IsTrue(config.Sequences[0].IsFirst);
            Assert.IsFalse(config.Sequences[1].IsFirst);
            Assert.AreEqual(1, config.Sequences[0].Widgets.Count);
            Assert.AreEqual(2, config.Sequences[1].Widgets.Count);
        }

        [TestMethod]
        public void ConvertNode_EmptyAnimations_ReturnsConfigWithNoSequences()
        {
            var doc = new XmlDocument();
            doc.LoadXml("<Animations />");

            var converter = new AnimationConfigConverter();
            AnimationConfigJson config = converter.ConvertNode("EmptyPanel", doc.DocumentElement!);

            Assert.AreEqual("EmptyPanel", config.PanelName);
            Assert.AreEqual(0, config.Sequences.Count);
        }

        [TestMethod]
        public void ConvertNode_VariableReferences_PreservedAsStrings()
        {
            // C1/C2 constraint: @VarName references must survive as strings
            var doc = new XmlDocument();
            doc.LoadXml(@"
<Animations>
  <Animation name=""Seq"" start=""true"" scanTime=""@ScanTime""
             iterations=""@GridScanIterations"" firstPauseTime=""@FirstPauseTime"">
    <Widget name=""W1"" />
  </Animation>
</Animations>");

            var converter = new AnimationConfigConverter();
            AnimationConfigJson config = converter.ConvertNode("VarPanel", doc.DocumentElement!);

            AnimationSequenceConfigJson seq = config.Sequences[0];
            Assert.AreEqual("@ScanTime", seq.ScanTime);
            Assert.AreEqual("@GridScanIterations", seq.Iterations);
            Assert.AreEqual("@FirstPauseTime", seq.FirstPauseTime);
        }

        [TestMethod]
        public void ConvertNode_PCodeOnSelected_Preserved()
        {
            // C4 constraint: per-widget onSelect PCode must survive
            var doc = new XmlDocument();
            doc.LoadXml(@"
<Animations>
  <Animation name=""Seq"" start=""true"" scanTime=""300"" iterations=""1"">
    <Widget name=""B1"" onSelect=""actuate(B1);transition(Row)"" />
  </Animation>
</Animations>");

            var converter = new AnimationConfigConverter();
            AnimationConfigJson config = converter.ConvertNode("PCodePanel", doc.DocumentElement!);

            Assert.AreEqual("actuate(B1);transition(Row)", config.Sequences[0].Widgets[0].OnSelected);
        }

        // ----------------------------------------------------------------
        // ConvertFile — real XML file with DTD
        // ----------------------------------------------------------------

        [TestMethod]
        public void ConvertFile_WithDtdAndAnimations_Succeeds()
        {
            string xml = @"<?xml version=""1.0"" ?>
<!DOCTYPE ACAT [
  <!ENTITY usebold ""false"">
]>
<ACAT>
  <WidgetAttributes>
    <WidgetAttribute name=""B1"" label=""OK"" />
  </WidgetAttributes>
  <Layout />
  <Animations>
    <Animation name=""Scan"" start=""true"" autoStart=""true"" scanTime=""@ScanTime"" iterations=""1"">
      <Widget name=""B1"" onSelect=""actuate(B1)"" />
    </Animation>
  </Animations>
</ACAT>";
            string path = WriteTempXml("DtdPanel.xml", xml);

            var converter = new AnimationConfigConverter();
            AnimationConfigJson? config = converter.ConvertFile(path);

            Assert.IsNotNull(config);
            Assert.AreEqual("DtdPanel", config!.PanelName);
            Assert.AreEqual(1, config.Sequences.Count);
            Assert.AreEqual("B1", config.Sequences[0].Widgets[0].Name);
        }

        // ----------------------------------------------------------------
        // WriteAsync — JSON round-trip
        // ----------------------------------------------------------------

        [TestMethod]
        public async Task WriteAsync_ProducesReadableJson()
        {
            var config = new AnimationConfigJson
            {
                PanelName = "MyPanel",
                ScanStrategy = "auto",
                Sequences = new List<AnimationSequenceConfigJson>
                {
                    new AnimationSequenceConfigJson
                    {
                        Name = "Seq1",
                        IsFirst = true,
                        AutoStart = true,
                        Iterations = "2",
                        ScanTime = "400",
                        Widgets = new List<AnimationWidgetConfigJson>
                        {
                            new AnimationWidgetConfigJson { Name = "Btn1", OnSelected = "actuate(Btn1)" }
                        }
                    }
                }
            };

            var converter = new AnimationConfigConverter();
            string outputPath = await converter.WriteAsync(config, _tempDir);

            Assert.IsTrue(File.Exists(outputPath), "Output file should be created");
            Assert.AreEqual("MyPanel.animation.json", Path.GetFileName(outputPath));

            string json = File.ReadAllText(outputPath);
            Assert.IsTrue(json.Contains("\"panelName\""), "JSON should use camelCase key 'panelName'");
            Assert.IsTrue(json.Contains("\"sequences\""), "JSON should contain sequences array");

            // Verify it's valid JSON
            using var doc = JsonDocument.Parse(json);
            Assert.AreEqual("MyPanel", doc.RootElement.GetProperty("panelName").GetString());
        }

        [TestMethod]
        public async Task WriteAsync_NullOnSelectedOmittedFromJson()
        {
            var config = new AnimationConfigJson
            {
                PanelName = "CleanPanel",
                Sequences = new List<AnimationSequenceConfigJson>
                {
                    new AnimationSequenceConfigJson
                    {
                        Name = "Seq",
                        IsFirst = true,
                        Widgets = new List<AnimationWidgetConfigJson>
                        {
                            new AnimationWidgetConfigJson { Name = "W1", OnSelected = null }
                        }
                    }
                }
            };

            var converter = new AnimationConfigConverter();
            string outputPath = await converter.WriteAsync(config, _tempDir);
            string json = File.ReadAllText(outputPath);

            // WhenWritingNull means the property should be absent
            Assert.IsFalse(json.Contains("\"onSelected\""), "Null onSelected should be omitted from JSON");
        }

        // ----------------------------------------------------------------
        // ConvertDirectoryAsync — batch conversion
        // ----------------------------------------------------------------

        [TestMethod]
        public async Task ConvertDirectoryAsync_MultipleXmlFiles_ConvertsAllWithAnimations()
        {
            string inputDir = Path.Combine(_tempDir, "input");
            string outputDir = Path.Combine(_tempDir, "output");
            Directory.CreateDirectory(inputDir);

            // File 1: has animations
            WriteTempXml(Path.Combine(inputDir, "Panel1.xml"), @"<?xml version=""1.0""?>
<ACAT>
  <Animations>
    <Animation name=""Seq"" start=""true"" scanTime=""300"" iterations=""1"">
      <Widget name=""W1"" />
    </Animation>
  </Animations>
</ACAT>");

            // File 2: has animations
            WriteTempXml(Path.Combine(inputDir, "Panel2.xml"), @"<?xml version=""1.0""?>
<ACAT>
  <Animations>
    <Animation name=""SeqA"" start=""true"" scanTime=""400"" iterations=""2"">
      <Widget name=""WA"" />
    </Animation>
    <Animation name=""SeqB"" scanTime=""500"" iterations=""1"">
      <Widget name=""WB"" />
    </Animation>
  </Animations>
</ACAT>");

            // File 3: no animations (should be skipped)
            WriteTempXml(Path.Combine(inputDir, "PanelNoAnim.xml"), @"<?xml version=""1.0""?>
<ACAT>
  <WidgetAttributes />
  <Layout />
</ACAT>");

            var converter = new AnimationConfigConverter();
            AnimationConversionResult result = await converter.ConvertDirectoryAsync(inputDir, outputDir);

            Assert.AreEqual(3, result.TotalFiles);
            Assert.AreEqual(2, result.SuccessCount, "2 files with animations should succeed");
            Assert.AreEqual(1, result.SkippedCount, "1 file without animations should be skipped");
            Assert.AreEqual(0, result.FailureCount);

            Assert.IsTrue(File.Exists(Path.Combine(outputDir, "Panel1.animation.json")));
            Assert.IsTrue(File.Exists(Path.Combine(outputDir, "Panel2.animation.json")));
            Assert.IsFalse(File.Exists(Path.Combine(outputDir, "PanelNoAnim.animation.json")));
        }

        [TestMethod]
        public async Task ConvertDirectoryAsync_DryRun_NoFilesWritten()
        {
            string inputDir = Path.Combine(_tempDir, "input");
            string outputDir = Path.Combine(_tempDir, "output");
            Directory.CreateDirectory(inputDir);

            WriteTempXml(Path.Combine(inputDir, "Panel.xml"), @"<?xml version=""1.0""?>
<ACAT>
  <Animations>
    <Animation name=""Seq"" start=""true"" scanTime=""300"" iterations=""1"">
      <Widget name=""W1"" />
    </Animation>
  </Animations>
</ACAT>");

            var converter = new AnimationConfigConverter();
            AnimationConversionResult result = await converter.ConvertDirectoryAsync(inputDir, outputDir, dryRun: true);

            Assert.AreEqual(1, result.SuccessCount);
            Assert.IsFalse(Directory.Exists(outputDir), "No output dir should be created in dry-run");
        }

        [TestMethod]
        public async Task ConvertDirectoryAsync_PreservesSubdirectoryStructure()
        {
            string inputDir = Path.Combine(_tempDir, "input");
            string subDir = Path.Combine(inputDir, "common");
            string outputDir = Path.Combine(_tempDir, "output");
            Directory.CreateDirectory(subDir);

            WriteTempXml(Path.Combine(subDir, "PanelInSub.xml"), @"<?xml version=""1.0""?>
<ACAT>
  <Animations>
    <Animation name=""Seq"" start=""true"" scanTime=""300"" iterations=""1"">
      <Widget name=""W1"" />
    </Animation>
  </Animations>
</ACAT>");

            var converter = new AnimationConfigConverter();
            await converter.ConvertDirectoryAsync(inputDir, outputDir);

            string expected = Path.Combine(outputDir, "common", "PanelInSub.animation.json");
            Assert.IsTrue(File.Exists(expected), "Sub-directory structure should be preserved in output");
        }

        [TestMethod]
        public async Task ConvertDirectoryAsync_EmptyDir_ReturnsZeroFiles()
        {
            string inputDir = Path.Combine(_tempDir, "empty");
            Directory.CreateDirectory(inputDir);

            var converter = new AnimationConfigConverter();
            AnimationConversionResult result = await converter.ConvertDirectoryAsync(inputDir, _tempDir);

            Assert.AreEqual(0, result.TotalFiles);
            Assert.AreEqual(0, result.SuccessCount);
        }

        [TestMethod]
        public async Task ConvertDirectoryAsync_MissingInputDir_Throws()
        {
            var converter = new AnimationConfigConverter();
            bool threw = false;
            try
            {
                await converter.ConvertDirectoryAsync(@"/nonexistent/path", _tempDir);
            }
            catch (DirectoryNotFoundException)
            {
                threw = true;
            }
            Assert.IsTrue(threw, "Expected DirectoryNotFoundException for missing input directory");
        }

        // ----------------------------------------------------------------
        // AnimationConversionResult.GenerateReport
        // ----------------------------------------------------------------

        [TestMethod]
        public void GenerateReport_ContainsKeyCounts()
        {
            var result = new AnimationConversionResult
            {
                TotalFiles = 10,
                SuccessCount = 7,
                SkippedCount = 2,
                FailureCount = 1,
            };
            result.Errors.Add(("file.xml", "Parse error"));

            string report = result.GenerateReport();

            StringAssert.Contains(report, "10", "Report should contain total count");
            StringAssert.Contains(report, "7", "Report should contain success count");
            StringAssert.Contains(report, "2", "Report should contain skipped count");
            StringAssert.Contains(report, "1", "Report should contain failure count");
            StringAssert.Contains(report, "file.xml", "Report should list errored file");
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private string WriteTempXml(string fileName, string content)
        {
            string path = Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(_tempDir, fileName);
            File.WriteAllText(path, content);
            return path;
        }
    }
}
