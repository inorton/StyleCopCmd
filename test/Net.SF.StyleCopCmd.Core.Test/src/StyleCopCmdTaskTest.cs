//------------------------------------------------------------------------------
// <copyright 
//  file="StyleCopCmdTaskTest.cs" 
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
namespace Net.SF.StyleCopCmd.Core.Test
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using NAnt.Core;
    using NUnit.Framework;

    /// <summary>
    /// A test fixture for the StyleCopCmdTask class.
    /// </summary>
    [TestFixture]
    public class StyleCopCmdTaskTest
    {
        #region NantSettingsXml
        /// <summary>
        /// The NAnt configuration XML
        /// </summary>
        private const string NantConfigXml =
@"<configuration>
    <nant>
        <frameworks>
            <platform name=""win32"" default=""auto"">
                <task-assemblies>
                        <!-- include NAnt task assemblies -->
                        <include name=""*Tasks.dll"" />
                        <!-- include NAnt test assemblies -->
                        <include name=""*Tests.dll"" />
                        <!-- include framework-neutral assemblies -->
                        <include name=""extensions/common/neutral/**/*.dll"" />
                        <!-- exclude Microsoft.NET specific task assembly -->
                        <exclude name=""NAnt.MSNetTasks.dll"" />
                        <!-- exclude Microsoft.NET specific test assembly -->
                        <exclude name=""NAnt.MSNet.Tests.dll"" />
                </task-assemblies>
                <framework 
                    name=""net-3.5""
                    family=""net""
                    version=""2.0""
                    description=""Microsoft .NET Framework 3.5""
                    sdkdirectory=""${path::combine(sdkInstallRoot, 'bin')}""
                    frameworkdirectory=""${path::combine(installRoot, 'v3.5')}""
                    frameworkassemblydirectory=""${path::combine(installRoot, 'v2.0.50727')}""
                    clrversion=""2.0.50727""
                    >
                    <runtime>
                        <probing-paths>
                            <directory name=""lib/net/2.0"" />
                            <directory name=""lib/net/neutral"" />
                            <directory name=""lib/common/2.0"" />
                            <directory name=""lib/common/neutral"" />
                        </probing-paths>
                        <modes>
                            <strict>
                                <environment>
                                    <variable name=""COMPLUS_VERSION"" value=""v2.0.50727"" />
                                </environment>
                            </strict>
                        </modes>
                    </runtime>
                    <reference-assemblies basedir=""${path::combine(installRoot, 'v2.0.50727')}"">
                        <include name=""Accessibility.dll"" />
                        <include name=""mscorlib.dll"" />
                        <include name=""Microsoft.Build.Utilities.dll"" />
                        <include name=""Microsoft.Vsa.dll"" />
                        <include name=""Microsoft.VisualBasic.dll"" />
                        <include name=""Microsoft.VisualBasic.Compatibility.dll"" />
                        <include name=""Microsoft.VisualBasic.Compatibility.Data.dll"" />
                        <include name=""System.Configuration.dll"" />
                        <include name=""System.Configuration.Install.dll"" />
                        <include name=""System.Data.dll"" />
                        <include name=""System.Data.OracleClient.dll"" />
                        <include name=""System.Data.SqlXml.dll"" />
                        <include name=""System.Deployment.dll"" />
                        <include name=""System.Design.dll"" />
                        <include name=""System.DirectoryServices.dll"" />
                        <include name=""System.dll"" />
                        <include name=""System.Drawing.Design.dll"" />
                        <include name=""System.Drawing.dll"" />
                        <include name=""System.EnterpriseServices.dll"" />
                        <include name=""System.Management.dll"" />
                        <include name=""System.Messaging.dll"" />
                        <include name=""System.Runtime.Remoting.dll"" />
                        <include name=""System.Runtime.Serialization.Formatters.Soap.dll"" />
                        <include name=""System.Security.dll"" />
                        <include name=""System.ServiceProcess.dll"" />
                        <include name=""System.Transactions.dll"" />
                        <include name=""System.Web.dll"" />
                        <include name=""System.Web.Mobile.dll"" />
                        <include name=""System.Web.RegularExpressions.dll"" />
                        <include name=""System.Web.Services.dll"" />
                        <include name=""System.Windows.Forms.dll"" />
                        <include name=""System.Xml.dll"" />
                    </reference-assemblies>
                    <reference-assemblies basedir=""${environment::get-folder-path('ProgramFiles')}/Reference Assemblies/Microsoft/Framework/v3.5"">
                        <include name=""Microsoft.Build.Engine.dll"" />
                        <include name=""Microsoft.Build.Framework.dll"" />
                        <include name=""System.AddIn.Contract.dll"" />
                        <include name=""System.AddIn.dll"" />
                        <include name=""System.Core.dll"" />
                        <include name=""System.Data.DataSetExtensions.dll"" />
                        <include name=""System.Data.Linq.dll"" />
                        <include name=""System.DirectoryServices.AccountManagement.dll"" />
                        <include name=""System.Management.Instrumentation.dll"" />
                        <include name=""System.Net.dll"" />
                        <include name=""System.ServiceModel.Web.dll"" />
                        <include name=""System.Web.Extensions.Design.dll"" />
                        <include name=""System.Web.Extensions.dll"" />
                        <include name=""System.Windows.Presentation.dll"" />
                        <include name=""System.WorkflowServices.dll"" />
                        <include name=""System.Xml.Linq.dll"" />
                    </reference-assemblies>
                    <reference-assemblies basedir=""${environment::get-folder-path('ProgramFiles')}/Reference Assemblies/Microsoft/Framework/v3.0"">
                        <include name=""System.IdentityModel.dll"" />
                        <include name=""System.IdentityModel.Selectors.dll"" />
                        <include name=""System.IO.Log.dll"" />
                        <include name=""System.Printing.dll"" />
                        <include name=""System.Runtime.Serialization.dll"" />
                        <include name=""System.ServiceModel.dll"" />
                        <include name=""System.Speech.dll"" />
                        <include name=""System.Workflow.Activities.dll"" />
                        <include name=""System.Workflow.ComponentModel.dll"" />
                        <include name=""System.Workflow.Runtime.dll"" />
                        <include name=""WindowsBase.dll"" />
                    </reference-assemblies>
                    <task-assemblies>
                        <!-- include MS.NET version-neutral assemblies -->
                        <include name=""extensions/net/neutral/**/*.dll"" />
                        <!-- include MS.NET 2.0 specific assemblies -->
                        <include name=""extensions/net/2.0/**/*.dll"" />
                        <!-- include MS.NET specific task assembly -->
                        <include name=""NAnt.MSNetTasks.dll"" />
                        <!-- include MS.NET specific test assembly -->
                        <include name=""NAnt.MSNet.Tests.dll"" />
                        <!-- include .NET 2.0 specific assemblies -->
                        <include name=""extensions/common/2.0/**/*.dll"" />
                    </task-assemblies>
                    <tool-paths>
                        <directory name=""${path::combine(sdkInstallRoot, 'bin')}"" />
                        <directory name=""${path::combine(installRoot, 'v3.5')}"" />
                        <directory name=""${path::combine(installRoot, 'v2.0.50727')}"" />
                    </tool-paths>
                    <project>
                        <property name=""installRoot""
                                  value=""C:\WINDOWS\Microsoft.NET\Framework\"" />
                        <property name=""sdkInstallRoot""
                                  value=""C:\Program Files\Microsoft SDKs\Windows\v6.1\"" />
                        <!--<readregistry
                            property=""installRoot""
                            key=""SOFTWARE\Microsoft\.NETFramework\InstallRoot""
                            hive=""LocalMachine"" />
                        <readregistry
                            property=""sdkInstallRoot""
                            key=""SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.1\WinSDKNetFxTools\InstallationFolder""
                            hive=""LocalMachine""
                            failonerror=""false"" />-->
                        <property name=""frameworkDirectoryV35"" value=""${path::combine(installRoot, 'v3.5')}"" />
                        <fail if=""${not(directory::exists(frameworkDirectoryV35))}"">The Framework directory for .NET 3.5 does not exist.</fail>
                        <property name=""referenceV35"" value=""${environment::get-folder-path('ProgramFiles')}/Reference Assemblies/Microsoft/Framework/v3.5"" />
                        <fail if=""${not(directory::exists(referenceV35))}"">The Reference Assemblies directory for .NET 3.5 does not exist.</fail>
                    </project>
                    <tasks>
                        <task name=""csc"">
                            <attribute name=""exename"">${path::combine(frameworkDirectoryV35,'csc.exe')}</attribute>
                            <attribute name=""supportsnowarnlist"">true</attribute>
                            <attribute name=""supportswarnaserrorlist"">true</attribute>
                            <attribute name=""supportskeycontainer"">true</attribute>
                            <attribute name=""supportskeyfile"">true</attribute>
                            <attribute name=""supportsdelaysign"">true</attribute>
                            <attribute name=""supportsplatform"">true</attribute>
                            <attribute name=""supportslangversion"">true</attribute>
                        </task>
                        <task name=""vbc"">
                            <attribute name=""exename"">${path::combine(frameworkDirectoryV35,'vbc.exe')}</attribute>
                            <attribute name=""supportsdocgeneration"">true</attribute>
                            <attribute name=""supportsnostdlib"">true</attribute>
                            <attribute name=""supportsnowarnlist"">true</attribute>
                            <attribute name=""supportskeycontainer"">true</attribute>
                            <attribute name=""supportskeyfile"">true</attribute>
                            <attribute name=""supportsdelaysign"">true</attribute>
                            <attribute name=""supportsplatform"">true</attribute>
                            <attribute name=""supportswarnaserrorlist"">true</attribute>
                        </task>
                        <task name=""jsc"">
                            <attribute name=""supportsplatform"">true</attribute>
                        </task>
                        <task name=""vjc"">
                            <attribute name=""supportsnowarnlist"">true</attribute>
                            <attribute name=""supportskeycontainer"">true</attribute>
                            <attribute name=""supportskeyfile"">true</attribute>
                            <attribute name=""supportsdelaysign"">true</attribute>
                        </task>
                        <task name=""resgen"">
                            <attribute name=""supportsassemblyreferences"">true</attribute>
                            <attribute name=""supportsexternalfilereferences"">true</attribute>
                        </task>
                        <task name=""al"">
                            <attribute name=""exename"">${path::combine(sdkInstallRoot, 'bin/al.exe')}</attribute>
                        </task>
                        <task name=""delay-sign"">
                            <attribute name=""exename"">sn</attribute>
                        </task>
                        <task name=""license"">
                            <attribute name=""exename"">lc</attribute>
                            <attribute name=""supportsassemblyreferences"">true</attribute>
                        </task>
                    </tasks>
                </framework>
            </platform>
        </frameworks>
        <properties>
            <!-- properties defined here are accessible to all build files -->
            <!-- <property name=""foo"" value = ""bar"" readonly=""false"" /> -->
        </properties>
    </nant>
</configuration>";
#endregion

        /// <summary>
        /// The NAnt .config file.
        /// </summary>
        private static readonly XmlDocument NAntConfigXmlDoc = 
            new XmlDocument();

        /// <summary>
        /// The path to the root solution.
        /// </summary>
        private string rootSolutionPath = string.Empty;

        /// <summary>
        /// The path to the test solution.
        /// </summary>
        private string testSolutionPath = string.Empty;

        /// <summary>
        /// The path to the output XML file.
        /// </summary>
        private string outputXmlPath = string.Empty;

        /// <summary>
        /// The path to the XSL file.
        /// </summary>
        private string transformFilePath = string.Empty;

        /// <summary>
        /// Sets up this test fixture.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Load the NAnt configuration file.
            NAntConfigXmlDoc.LoadXml(NantConfigXml);

            this.rootSolutionPath = GetRootSolutionPath();
            this.transformFilePath = 
                this.rootSolutionPath + @"\inc\StyleCopReport.xsl";
            this.testSolutionPath = 
                this.rootSolutionPath + 
                @"\test\Net.SF.StyleCopCmd.Core.Test" +
                @"\data\StyleCopTestProject";
            this.outputXmlPath =
                this.rootSolutionPath +
                @"\test\Net.SF.StyleCopCmd.Core.Test" +
                @"\build\StyleCopReport.xml";
        }

        /// <summary>
        /// Tests the Initialize method.
        /// </summary>
        [Test]
        public void InitializeTest()
        {
            var format =
                @"<project name=""StyleCopCmdTest"">
                    <styleCopCmd outputXmlFile=""{0}""
                                 transformFile=""{1}""
                                 ignorePatterns=""AssemblyInfo\.cs,GlobalSuppressions\.cs"">
                        <solutionFiles>
                            <include name=""{2}/*.sln"" />
                        </solutionFiles>
                        <addinDirectories>
                            <include name=""{3}/test/Net.SF.StyleCopCmd.Core.Test/build"" />
                        </addinDirectories>
                    </styleCopCmd>
                </project>";

            // Create the NAnt XML document for the project.
            var xmldoc = this.CreateNAntXmlDocument(format);

            // Create a NAnt project.
            var p = CreateNAntProject(xmldoc);

            // Load the StyleCopCmd.Core assembly into the project.
            LoadStyleCopCmdTask(p);

            // Get the StyleCopCmd task.
            var t = CreateTask(p);

            Assert.AreEqual(
                this.outputXmlPath,
                t.OutputXmlFile);
            Assert.AreEqual(
                @"AssemblyInfo\.cs,GlobalSuppressions\.cs",
                t.IgnorePatterns);
            Assert.AreEqual(
                1,
                t.SolutionFiles.FileNames.Count);
        }

        /// <summary>
        /// Tests the ExecuteTask method.
        /// </summary>
        [Test]
        public void ExecuteTaskTest()
        {
            var format =
                @"<project name=""StyleCopCmdTest"">
                    <styleCopCmd outputXmlFile=""{0}""
                                 transformFile=""{1}""
                                 recursionEnabled=""true""
                                 ignorePatterns=""AssemblyInfo\.cs,GlobalSuppressions\.cs"">
                        <solutionFiles>
                            <include name=""{2}/*.sln"" />
                        </solutionFiles>
                        <addinDirectories>
                            <include name=""{3}/test/Net.SF.StyleCopCmd.Core.Test/build"" />
                        </addinDirectories>
                    </styleCopCmd>
                </project>";

            // Create the NAnt XML document for the project.
            var xmldoc = this.CreateNAntXmlDocument(format);

            // Create a NAnt project.
            var p = CreateNAntProject(xmldoc);

            // Load the StyleCopCmd.Core assembly into the project.
            LoadStyleCopCmdTask(p);

            // Get the StyleCopCmd task.
            var t = CreateTask(p);

            t.Execute();
        }

        /// <summary>Creates a StyleCopCmd task.</summary>
        /// <param name="project">The project to create the task from.</param>
        /// <returns>A StyleCopCmd task.</returns>
        private static StyleCopCmdTask CreateTask(Project project)
        {
            if (project.Document.DocumentElement == null)
            {
                throw new XmlException("DocumentElement is null");
            }

            var el = project.Document.DocumentElement.ChildNodes[0];
            var nt = (StyleCopCmdTask) project.CreateTask(el);
            return nt;
        }

        /// <summary>
        /// Creates a NAnt project.
        /// </summary>
        /// <param name="projectXmlDoc">
        /// The XML document to create the project from.
        /// </param>
        /// <returns>A NAnt Proejct.</returns>
        private static Project CreateNAntProject(XmlDocument projectXmlDoc)
        {
            if (NAntConfigXmlDoc.DocumentElement == null)
            {
                throw new XmlException("DocumentElement is null");
            }

            // Create a NAnt project.
            var p = new Project(
              projectXmlDoc,
              Level.Info,
              0,
              NAntConfigXmlDoc.DocumentElement.ChildNodes[0]);

            return p;
        }

        /// <summary>
        /// Load any custom tasks into the given project from the 
        /// StyleCopCmd.Core assembly.
        /// </summary>
        /// <param name="project">
        /// The project to load the custom tasks into.
        /// </param>
        private static void LoadStyleCopCmdTask(Project project)
        {
            if (project.Document.DocumentElement == null)
            {
                throw new XmlException("DocumentElement is null");
            }

            // Add an echo task to the project's document root to output
            // the result of the task load operation.
            var et = project.Document.CreateElement("echo");
            et.Attributes.Append(project.Document.CreateAttribute("message"));
            project.Document.DocumentElement.AppendChild(et);

            // Load the custom tasks.
            TypeFactory.ScanAssembly(
                Assembly.GetAssembly(typeof(StyleCopCmdTask)),
                project.CreateTask(et));
        }

        /// <summary>
        /// Gets the path to the root solution.
        /// </summary>
        /// <returns>
        /// The path to the root solution.
        /// </returns>
        private static string GetRootSolutionPath()
        {
            var d = new DirectoryInfo(".");

            // Move backwards to the root of the solution.
            while (d != null)
            {
                // Is the solution in this directory?
                if (d.GetFiles().FirstOrDefault(f => f.Extension == ".sln") !=
                    null)
                {
                    break;
                }

                d = d.Parent;
            }

            return d == null ? null : d.FullName;
        }

        /// <summary>
        /// Creates a NAnt XML project document.
        /// </summary>
        /// <param name="format">The XML format pattern to use.</param>
        /// <returns>A new XML document.</returns>
        private XmlDocument CreateNAntXmlDocument(string format)
        {
            var xml = string.Format(
                CultureInfo.CurrentCulture,
                format,
                this.outputXmlPath,
                this.transformFilePath,
                this.testSolutionPath,
                this.rootSolutionPath);

            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);
            return xmldoc;
        }
    }
}