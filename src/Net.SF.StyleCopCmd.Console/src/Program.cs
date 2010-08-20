//------------------------------------------------------------------------------
// <copyright 
//  file="Program.cs" 
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
namespace Net.SF.StyleCopCmd.Console
{
    using System;
    using System.Reflection;
    using Core;
    using net.sf.dotnetcli;

    /// <summary>
    /// The entry-point class for this application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The command-line options for this application.
        /// </summary>
        private static readonly Options Opts = new Options();

        /// <summary>
        /// The entry-point method for this application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments passed to this method.
        /// </param>
        private static void Main(string[] args)
        {
            // Initialize the command line options.
            InitOptions();

            // Parse the arguments.
            var cl = ProcessArguments(args);
            if (cl == null)
            {
                PrintUsageAndHelp();
                return;
            }

            new StyleCopReport().ReportBuilder()
                .WithStyleCopSettingsFile(cl.GetOptionValue("s"))
                .WithRecursion(cl.HasOption("r"))
                .WithSolutionsFiles(cl.GetOptionValues("sf"))
                .WithProjectFiles(cl.GetOptionValues("pf"))
                .WithDirectories(cl.GetOptionValues("d"))
                .WithFiles(cl.GetOptionValues("f"))
                .WithIgnorePatterns(cl.GetOptionValues("ifp"))
                .WithTransformFile(cl.GetOptionValue("tf"))
                .WithOutputEventHandler(OutputGenerated)
                .Create(cl.GetOptionValue("of"));
        }

        /// <summary>
        /// Prints the output of the StyleCop processor to the console.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The output message to print.</param>
        private static void OutputGenerated(
            object sender, 
            Microsoft.StyleCop.OutputEventArgs e)
        {
            Console.WriteLine(e.Output);
        }

        /// <summary>
        /// Prints this application's usage and help text to StdOut.
        /// </summary>
        private static void PrintUsageAndHelp()
        {
            var hf = new HelpFormatter();
            hf.PrintHelp(
                Assembly.GetExecutingAssembly().GetName().Name,
                Opts,
                true);
        }

        /// <summary>
        /// Initialize the options this program takes.
        /// </summary>
        private static void InitOptions()
        {
            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("solutionFiles")
                    .HasArgs()
                    .WithArgName("filePaths")
                    .WithDescription("Visual Studio solutions files to check")
                    .Create("sf"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("projectFiles")
                    .HasArgs()
                    .WithArgName("filePaths")
                    .WithDescription("Visual Studio project files to check")
                    .Create("pf"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("ignoreFilePattern")
                    .HasArgs()
                    .WithArgName("patterns")
                    .WithDescription(
                    "Regular expression patterns that " +
                    "can be used to ignore files")
                    .Create("ifp"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("directories")
                    .HasArgs()
                    .WithArgName("dirPaths")
                    .WithDescription("Directories to check for CSharp files")
                    .Create("d"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("recurse")
                    .WithDescription("Recursive directory search")
                    .Create("r"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("files")
                    .HasArgs()
                    .WithArgName("filePaths")
                    .WithDescription("Files to check")
                    .Create("f"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("styleCopSettingsFile")
                    .HasArg()
                    .WithArgName("filePath")
                    .WithDescription("Use the given StyleCop settings file")
                    .Create("sc"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("configurationSymbols")
                    .HasArgs()
                    .WithArgName("symbols")
                    .WithDescription(
                    "Configuration symbols to pass to StyleCop " +
                    "(ex. DEBUG, RELEASE)")
                    .Create("cs"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("outputXmlFile")
                    .HasArg()
                    .WithArgName("filePath")
                    .WithDescription("The file the XML output is written to")
                    .Create("of"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("xslFile")
                    .HasArg()
                    .WithArgName("filePath")
                    .WithDescription("The transform file")
                    .Create("tf"));

            Opts.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt("help")
                    .WithDescription("Print this help screen")
                    .Create("?"));
        }

        /// <summary>
        /// Process the command line arguments against the
        /// expected options.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// A CommandLine object if successful, otherwise false.
        /// </returns>
        private static CommandLine ProcessArguments(string[] args)
        {
            var pp = new PosixParser();
            CommandLine cl;

            try
            {
                cl = pp.Parse(
                    Opts,
                    args);
            }
            catch (ParseException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            if (cl.HasOption("?"))
            {
                return null;
            }

            if (!(cl.HasOption("sf") ||
                  cl.HasOption("pf") ||
                  cl.HasOption("d") ||
                  cl.HasOption("f")))
            {
                return null;
            }

            return cl;
        }
    }
}