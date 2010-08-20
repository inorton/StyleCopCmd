//------------------------------------------------------------------------------
// <copyright 
//  file="ReportBuilder.cs" 
//  company="Schley Andrew Kutz">
//  Copyright (c) Schley Andrew Kutz. All rights reserved.
// </copyright>
// <authors>
//   <author>Schley Andrew Kutz</author>
// </authors>
//------------------------------------------------------------------------------
/*******************************************************************************
 * Copyright (c) 2008, Schley Andrew Kutz <sakutz@gmail.com>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Schley Andrew Kutz nor the names of its 
 *   contributors may be used to endorse or promote products derived from this 
 *   software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
namespace Net.SF.StyleCopCmd.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using System.Xml.Xsl;
    using Microsoft.StyleCop;

    /// <summary>
    /// This class assists in building a StyleCop report.
    /// </summary>
    public class ReportBuilder
    {
        /// <summary>
        /// Initializes a new instance of the ReportBuilder class.
        /// </summary>
        /// <param name="report">
        /// The StyleCopReport to build.
        /// </param>
        internal ReportBuilder(StyleCopReport report)
        {
            this.Report = report;
        }

        // ReSharper disable UnusedPrivateMember

        /// <summary>
        /// Prevents a default instance of the ReportBuilder class from
        /// being created.
        /// </summary>
        private ReportBuilder()
        {
            // Do nothing
        }

        // ReSharper restore UnusedPrivateMember

        /// <summary>
        /// Occurs when the stle processor outputs a message.
        /// </summary>
        public event EventHandler<OutputEventArgs> OutputGenerated;

        /// <summary>
        /// Gets or sets the StyleCopReport data set that is used to store the 
        /// style cop results and write the results to an XML file.
        /// </summary>
        private StyleCopReport Report
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of Visual Studio Solution files to check.
        /// Visual Studio 2008 is supported.
        /// </summary>
        private IList<string> SolutionFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of Visual Studio Project files to check.
        /// Visual Studio 2008 is supported.
        /// </summary>
        private IList<string> ProjectFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of directories to check.
        /// </summary>
        private IList<string> Directories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of files to check.
        /// </summary>
        private IList<string> Files
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of regular expression patterns used
        /// to ignore files (if a file name matches any of the patterns, the
        /// file is not checked).
        /// </summary>
        private IList<string> IgnorePatterns
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not directories are 
        /// recursed.
        /// </summary>
        private bool RecursionEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of processor symbols (ex. DEBUG, CODE_ANALYSIS)
        /// to be used by StyleCop.
        /// </summary>
        private IList<string> ProcessorSymbols
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to the StyleCop setting file to use.
        /// </summary>
        private string StyleCopSettingsFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to the XSL file used to transform the 
        /// outputted XML file to an HTML report.
        /// </summary>
        private string TransformFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of directories used by StyleCop to search for 
        /// add-ins.
        /// </summary>
        private IList<string> AddInDirectories
        {
            get;
            set;
        }

        /// <summary>
        /// Includes the following directories in the StyleCop processor's
        /// search path for add-ins.
        /// </summary>
        /// <param name="addinDirectories">The directories to include.</param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithAddInDirectories(
            IList<string> addinDirectories)
        {
            this.AddInDirectories = addinDirectories;
            return this;
        }

        /// <summary>
        /// Adds Visual Studio Solution files to check. Visual Studio 2008
        /// is supported.
        /// </summary>
        /// <param name="solutionsFiles">
        /// A list of fully-qualified paths to Visual Studio solutions files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithSolutionsFiles(
            IList<string> solutionsFiles)
        {
            this.SolutionFiles = solutionsFiles;
            return this;
        }

        /// <summary>
        /// Adds Visual Studio Project files to check. Visual Studio 2008
        /// is supported.
        /// </summary>
        /// <param name="projectFiles">
        /// A list of fully-qualified paths to Visual Studio project files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithProjectFiles(
            IList<string> projectFiles)
        {
            this.ProjectFiles = projectFiles;
            return this;
        }

        /// <summary>
        /// Adds directories to check.
        /// </summary>
        /// <param name="directories">
        /// A list of fully-qualifieid paths to directories.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithDirectories(
            IList<string> directories)
        {
            this.Directories = directories;
            return this;
        }

        /// <summary>
        /// Adds files to check.
        /// </summary>
        /// <param name="files">
        /// A list of fully-qualified paths to files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithFiles(
            IList<string> files)
        {
            this.Files = files;
            return this;
        }

        /// <summary>
        /// Adds a list of patterns to ignore when checking files.
        /// </summary>
        /// <param name="ignorePatterns">
        /// A list of regular expression patterns to ignore when checking
        /// files.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithIgnorePatterns(
            IList<string> ignorePatterns)
        {
            this.IgnorePatterns = ignorePatterns;
            return this;
        }

        /// <summary>
        /// Specifies to recurse directories.
        /// </summary>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithRecursion()
        {
            return this.WithRecursion(true);
        }

        /// <summary>
        /// Specifies to recurse directories.
        /// </summary>
        /// <param name="withRecursion">
        /// True to recurse; otherwise false.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithRecursion(bool withRecursion)
        {
            this.RecursionEnabled = withRecursion;
            return this;
        }

        /// <summary>
        /// Adds a list of processor symbols to use when performing the check.
        /// </summary>
        /// <param name="processorSymbols">
        /// A list of processor symboles to use when performing the check.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithProcessorSymbols(
            IList<string> processorSymbols)
        {
            this.ProcessorSymbols = processorSymbols;
            return this;
        }

        /// <summary>
        /// Adds a StyleCop settings file to use when performing the check.
        /// </summary>
        /// <param name="styleCopSettingsFile">
        /// A fully-qualified path to a StyleCop settings file to use.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithStyleCopSettingsFile(
            string styleCopSettingsFile)
        {
            this.StyleCopSettingsFile = styleCopSettingsFile;
            return this;
        }

        /// <summary>
        /// Adds an XSL transform file to use to create an HTML report
        /// from the results of the check.
        /// </summary>
        /// <param name="transformFile">
        /// An XSL transform file to use to create an HTML report from the 
        /// results of the check.
        /// </param>
        /// <returns>This ReportBuilder.</returns>
        public ReportBuilder WithTransformFile(
            string transformFile)
        {
            this.TransformFile = transformFile;
            return this;
        }

        /// <summary>
        /// Adds an event handler for when output is generated by the 
        /// StyleCop processor.
        /// </summary>
        /// <param name="outputEventHandler">
        /// The event handler to add.
        /// </param>
        /// <returns>
        /// This ReportBuilder.
        /// </returns>
        public ReportBuilder WithOutputEventHandler(
            EventHandler<OutputEventArgs> outputEventHandler)
        {
            this.OutputGenerated += outputEventHandler;
            return this;
        }

        /// <summary>
        /// Creates a StyleCop report.
        /// </summary>
        /// <param name="outputXmlFile">
        /// The fully-qualified path to write the output of the report to.
        /// </param>
        public void Create(string outputXmlFile)
        {
            // Create a StyleCop configuration specifying the configuration
            // symbols to use for this report.
            var cfg = new Configuration(
                this.ProcessorSymbols != null
                    ?
                        this.ProcessorSymbols.ToArray()
                    :
                        null);

            // Create a new StyleCop console used to do the check.
            var scc = new StyleCopConsole(
                this.StyleCopSettingsFile,
                true,
                GetViolationsFile(outputXmlFile),
                this.AddInDirectories,
                true);

            // Process solution files
            if (this.SolutionFiles != null)
            {
                foreach (var i in this.SolutionFiles)
                {
                    this.AddSolutionFile(i);
                }
            }

            // Process project files
            if (this.ProjectFiles != null)
            {
                foreach (var i in this.ProjectFiles)
                {
                    this.AddProjectFile(
                        i,
                        null);
                }
            }

            // Process directories
            if (this.Directories != null)
            {
                foreach (var i in this.Directories)
                {
                    this.AddDirectory(i);
                }
            }

            // Process files
            if (this.Files != null)
            {
                foreach (var i in this.Files)
                {
                    this.AddFile(
                        i,
                        null,
                        null);
                }
            }

            // Create a list of code projects from the data set.
            var cps = this.Report.Projects.Select(
                r => new CodeProject(
                         r.ID,
                         r.Location,
                         cfg)).ToList();

            // Add the source code files to the style cop checker
            foreach (var f in this.Report.SourceCodeFiles)
            {
                // ReSharper disable AccessToModifiedClosure
                var cp = cps.SingleOrDefault(i => i.Key == f.CodeProjectID);
                scc.Core.Environment.AddSourceCode(
                    cp,
                    f.Path,
                    null);

                // ReSharper restore AccessToModifiedClosure
            }

            if (this.OutputGenerated != null)
            {
                scc.OutputGenerated += this.OutputGenerated;
            }

            scc.ViolationEncountered += this.ViolationEncountered;

            scc.Start(
                cps,
                true);

            if (this.OutputGenerated != null)
            {
                scc.OutputGenerated -= this.OutputGenerated;
            }

            scc.ViolationEncountered -= this.ViolationEncountered;
            scc.Dispose();

            // Write the report to the output XML file.
            this.Report.WriteXml(outputXmlFile);

            if (!string.IsNullOrEmpty(this.TransformFile))
            {
                this.Transform(outputXmlFile);
            }
        }

        /// <summary>
        /// Gets the path of the violations file to use.
        /// </summary>
        /// <param name="outputXmlFile">
        /// The output XML file.
        /// </param>
        /// <returns>The path of the violations file.</returns>
        private static string GetViolationsFile(string outputXmlFile)
        {
            var offp = Path.GetFullPath(outputXmlFile);
            var f = string.Format(
                CultureInfo.CurrentCulture,
                "{0}\\{1}.violations.xml",
                Path.GetFullPath(Path.GetDirectoryName(offp)),
                Path.GetFileNameWithoutExtension(outputXmlFile));
            return f;
        }

        /// <summary>
        /// Transforms the outputted report using an XSL transform file.
        /// </summary>
        /// <param name="outputXmlFile">
        /// The fully-qualified path of the report to transform.
        /// </param>
        private void Transform(string outputXmlFile)
        {
            var xt = new XslCompiledTransform();
            var offp = Path.GetFullPath(outputXmlFile);
            xt.Load(this.TransformFile);
            var htmlout = string.Format(
                CultureInfo.CurrentCulture,
                "{0}\\{1}.html",
                Path.GetFullPath(Path.GetDirectoryName(offp)),
                Path.GetFileNameWithoutExtension(outputXmlFile));
            xt.Transform(
                outputXmlFile,
                htmlout);
        }

        /// <summary>
        /// Callback method for when a violation is encountered.
        /// </summary>
        /// <param name="sender">
        /// The object that caused this event.
        /// </param>
        /// <param name="e">
        /// The ViolationEventArgs object that describes this violation.
        /// </param>
        private void ViolationEncountered(
            object sender,
            ViolationEventArgs e)
        {
            // Add the violation.
            var sfiles =
                this.Report.SourceCodeFiles.Where(
                    r => r.Path == e.SourceCode.Path);

            foreach (var sf in sfiles)
            {
                var lines = File.ReadAllLines(sf.Path);

                lock (this.Report)
                {
                    var violation = this.Report.Violations.AddViolationsRow(
                        e.LineNumber,
                        e.Message,
                        sf,
                        lines[e.LineNumber - 1]);

                    // Add the rule.
                    this.Report.Rules.AddRulesRow(
                        e.Violation.Rule.CheckId,
                        e.Violation.Rule.Description,
                        e.Violation.Rule.EnabledByDefault,
                        e.Violation.Rule.Name,
                        e.Violation.Rule.Namespace,
                        e.Violation.Rule.RuleGroup,
                        e.Violation.Rule.Warning,
                        violation);
                }
            }
        }

        /// <summary>
        /// Add the directory's source files to the list of
        /// files that StyleCop checks.
        /// </summary>
        /// <param name="path">
        /// The fully-qualified path to the directory to add.
        /// </param>
        private void AddDirectory(string path)
        {
            var recurse = this.RecursionEnabled
                              ?
                                  SearchOption.AllDirectories
                              :
                                  SearchOption.TopDirectoryOnly;

            var files = Directory.GetFiles(
                path,
                "*.cs",
                recurse);
        
            var sr = this.Report.Solutions.AddSolutionsRow(
                path,
                "Directory");

            var pr = this.Report.Projects.AddProjectsRow(
                path,
                "Directory",
                sr);

            // Add the source files.
            Array.ForEach(
                files,
                f => this.AddFile(
                         f,
                         sr,
                         pr));
        }

        /// <summary>
        /// Add the given file to the list of files to
        /// be checked by StyleCop.
        /// </summary>
        /// <param name="filePath">
        /// The fully-qualified path of the file to add.
        /// </param>
        /// <param name="solutionsRow">
        /// The solutions row for this file.
        /// </param>
        /// <param name="projectsRow">
        /// The projects row for this file.
        /// </param>
        private void AddFile(
            string filePath,
            StyleCopReport.SolutionsRow solutionsRow,
            StyleCopReport.ProjectsRow projectsRow)
        {
            if (this.IgnorePatterns != null)
            {
                // Check to see if this file should be ignored.
                if (this.IgnorePatterns.FirstOrDefault(
                        fp => Regex.IsMatch(
                                  Path.GetFileName(filePath),
                                  fp)) != null)
                {
                    return;
                }
            }

            var sr = solutionsRow ??
                     this.Report.Solutions.SingleOrDefault(
                         r => r.Name == "Files") ??
                     this.Report.Solutions.AddSolutionsRow(
                         "Files",
                         "Files");

            var pr = projectsRow ??
                     this.Report.Projects.SingleOrDefault(
                         r => r.Name == "Files") ??
                     this.Report.Projects.AddProjectsRow(
                         "Files",
                         "Files",
                         sr);

            var ext = Path.GetExtension(
                filePath).Replace(
                ".",
                string.Empty)
                .ToUpper(CultureInfo.CurrentCulture);

            this.Report.SourceCodeFiles.AddSourceCodeFilesRow(
                filePath,
                File.GetLastWriteTimeUtc(filePath),
                ext,
                Path.GetFileName(filePath),
                pr);
        }

        /// <summary>
        /// Adds a Visual Studio solution file and add its
        /// CSharp projects to the list of projects to be checked
        /// by StyleCop.
        /// </summary>
        /// <param name="solutionFilePath">
        /// The fully-qualified path to a Visual Studio solution file.
        /// </param>
        private void AddSolutionFile(string solutionFilePath)
        {
            // Add a solutions row
            var sr = this.Report.Solutions.AddSolutionsRow(
                solutionFilePath,
                Path.GetFileNameWithoutExtension(solutionFilePath));

            // Get a list of the CSharp projects in the solutions file
            // and parse the project files.
            var sfin = File.ReadAllText(solutionFilePath);
            var smatches = Regex.Matches(
                sfin,
                @"^Project\(.*?\) = "".*?"", ""(?<ppath>.*?.csproj)""",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match sm in smatches)
            {
                var ppath =
                    Path.GetFullPath(
                        Path.GetDirectoryName(
                            Path.GetFullPath(solutionFilePath)))
                    + "\\" + sm.Groups["ppath"].Value;
                this.AddProjectFile(
                    ppath,
                    sr);
            }
        }

        /// <summary>
        /// Adds a Visual Studio CSharp project file and
        /// adds its source files to the list of files to be 
        /// checked by StyleCop.
        /// </summary>
        /// <param name="projectFilePath">
        /// The fully-qualified path to the project file.
        /// </param>
        /// <param name="solutionsRow">
        /// The solutions row this project belongs to. Specify
        /// null if this project is not a member of a solution.
        /// </param>
        private void AddProjectFile(
            string projectFilePath,
            StyleCopReport.SolutionsRow solutionsRow)
        {
            // Get the project's name.
            var pname = Path.GetFileNameWithoutExtension(projectFilePath);

            if (solutionsRow == null)
            {
                solutionsRow = this.Report.Solutions.AddSolutionsRow(
                    projectFilePath,
                    "Project - " + pname);
            }

            // Add a new project row.
            var pr = this.Report.Projects.AddProjectsRow(
                projectFilePath,
                pname,
                solutionsRow);
            
            var pf = XDocument.Load(projectFilePath);

            // ReSharper disable PossibleNullReferenceException

            // Get the source files that are not auto-generated.
            var cnodes =
                pf.Root.Descendants().Where(
                    d => d.Name.LocalName == "Compile" &&
                         d.Attribute(XName.Get("Include")).Value.EndsWith(
                             "cs",
                             StringComparison.CurrentCultureIgnoreCase) &&
                         d.Elements().SingleOrDefault(
                             c =>
                             c.Name.LocalName == "AutoGen" &&
                             c.Value == "True") ==
                         null);

            // Add the source files.
            foreach (var n in cnodes)
            {
                var fpath =
                    Path.GetFullPath(Path.GetDirectoryName(projectFilePath))
                    + "\\" + n.Attribute(XName.Get("Include")).Value;

                this.AddFile(
                    fpath,
                    solutionsRow,
                    pr);
            }

            // ReSharper restore PossibleNullReferenceException
        }
    }
}