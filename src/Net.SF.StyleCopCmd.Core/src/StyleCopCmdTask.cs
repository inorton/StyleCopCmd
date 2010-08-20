//------------------------------------------------------------------------------
// <copyright 
//  file="StyleCopCmdTask.cs" 
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
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.StyleCop;
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Types;

    /// <summary>
    /// This class provides a NAnt task that calls StyleCopCmd.
    /// </summary>
    [TaskName("styleCopCmd")]
    public class StyleCopCmdTask : Task
    {
        /// <summary>
        /// Gets or sets a list of directories used by the SyleCop processor
        /// to load its own add-ins.
        /// </summary>
        [BuildElement("addinDirectories")]
        public DirSet AddInDirectories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of Visual Studio Solution files to check.
        /// Visual Studio 2008 is supported.
        /// </summary>
        [BuildElement("solutionFiles")]
        public FileSet SolutionFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of Visual Studio Project files to check.
        /// Visual Studio 2008 is supported.
        /// </summary>
        [BuildElement("projectFiles")]
        public FileSet ProjectFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of directories to check.
        /// </summary>
        [BuildElement("directories")]
        public DirSet Directories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of files to check.
        /// </summary>
        [BuildElement("files")]
        public FileSet Files
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of regular expression patterns used
        /// to ignore files (if a file name matches any of the patterns, the
        /// file is not checked).
        /// </summary>
        [TaskAttribute("ignorePatterns")]
        [StringValidator]
        public string IgnorePatterns
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not directories are 
        /// recursed.
        /// </summary>
        [TaskAttribute("recursionEnabled")]
        [BooleanValidator]
        public bool RecursionEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of processor symbols (ex. DEBUG, CODE_ANALYSIS)
        /// to be used by StyleCop.
        /// </summary>
        [TaskAttribute("processorSymbols")]
        [StringValidator]
        public string ProcessorSymbols
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to the StyleCop setting file to use.
        /// </summary>
        [TaskAttribute("styleCopSettingsFile")]
        [StringValidator]
        public string StyleCopSettingsFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to the XSL file used to transform the 
        /// outputted XML file to an HTML report.
        /// </summary>
        [TaskAttribute("transformFile")]
        [StringValidator]
        public string TransformFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to the output file.
        /// </summary>
        [TaskAttribute("outputXmlFile")]
        [StringValidator]
        public string OutputXmlFile
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        protected override void ExecuteTask()
        {
            new StyleCopReport().ReportBuilder()
                .WithAddInDirectories(GetDirectories(this.AddInDirectories))
                .WithSolutionsFiles(GetFiles(this.SolutionFiles))
                .WithProjectFiles(GetFiles(this.ProjectFiles))
                .WithDirectories(GetDirectories(this.Directories))
                .WithFiles(GetFiles(this.Files))
                .WithProcessorSymbols(Split(this.ProcessorSymbols))
                .WithIgnorePatterns(Split(this.IgnorePatterns))
                .WithTransformFile(this.TransformFile)
                .WithOutputEventHandler(this.LogOutput)
                .WithRecursion(this.RecursionEnabled)
                .WithStyleCopSettingsFile(this.StyleCopSettingsFile)
                .Create(this.OutputXmlFile);
        }

        /// <summary>
        /// Splits a string into an array of strings at commas.
        /// </summary>
        /// <param name="s">The string to split.</param>
        /// <returns>
        /// An array of strings. If the input is null a null value is returned.
        /// </returns>
        private static IList<string> Split(string s)
        {
            if (s == null)
            {
                return null;
            }

            return s.Split(',');
        }

        /// <summary>
        /// Gets a generic list from a non-generic collection.
        /// </summary>
        /// <typeparam name="T">The type of list to get.</typeparam>
        /// <param name="collection">
        /// The collection to get the list from.
        /// </param>
        /// <returns>The generic list.</returns>
        private static IList<T> GetIList<T>(ICollection collection)
        {
            if (collection == null)
            {
                return null;
            }

            var arr = new T[collection.Count];
            collection.CopyTo(
                arr,
                0);
            return arr;
        }

        /// <summary>
        /// Gets a list of file names.
        /// </summary>
        /// <param name="fileSet">The fileset to get the names from.</param>
        /// <returns>The list of file names; otherwise null.</returns>
        private static IList<string> GetFiles(FileSet fileSet)
        {
            return fileSet == null ? null : GetIList<string>(fileSet.FileNames);
        }

        /// <summary>
        /// Gets a list of directory names.
        /// </summary>
        /// <param name="dirSet">The dirset to get the names from.</param>
        /// <returns>The list of directory names; otherwise null.</returns>
        private static IList<string> GetDirectories(FileSet dirSet)
        {
            return dirSet == null ? null : GetIList<string>(dirSet.DirectoryNames);
        }

        /// <summary>
        /// Logs StyleCop output to the NAnt logger.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The OutputEventArgs object.</param>
        private void LogOutput(object sender, OutputEventArgs e)
        {
            this.Log(
                Level.Info,
                e.Output);
        }
    }
}