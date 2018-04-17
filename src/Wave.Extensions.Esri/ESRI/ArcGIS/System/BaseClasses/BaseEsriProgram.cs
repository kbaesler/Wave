using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using ILog = System.Diagnostics.ILog;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     Provides the necessary methods for a command line interface.
    /// </summary>
    /// <typeparam name="TProgramArguments">The type of the program arguments.</typeparam>
    [ComVisible(false)]
    public abstract class BaseEsriProgram<TProgramArguments>
    {
        #region Protected Properties

        /// <summary>
        ///     The log for the program.
        /// </summary>
        protected virtual ILog Log { get; set; } = LogProvider.For<BaseEsriProgram<TProgramArguments>>(new ApacheLogProvider(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts the array of arguments to the strongly type object and executes the program
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing 0 for success and failure any other value.
        /// </returns>
        public virtual int Run(string[] args)
        {
            return this.Run(args, esriLicenseProductCode.esriLicenseProductCodeStandard);
        }

        /// <summary>
        ///     Converts the array of arguments to the strongly type object and executes the program while checking
        ///     out the ArcFM and ESRI licenses
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="esriLicenseProduct">The esri license product.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing 0 for success and failure any other value.
        /// </returns>
        public virtual int Run(string[] args, esriLicenseProductCode esriLicenseProduct)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                Console.Title = Application.ProductName;

                Log.Info("===============================");
                Log.Info(Application.ProductName + " Started.");
                Log.Info("Version: " + Application.ProductVersion);
                Log.Info("User: " + Environment.UserName);
                Log.Info("===============================");
                Log.Info("");

                if (args == null || args.Length == 0)
                {
                    args = this.CreateArguments();
                }

                Log.Info($"Arguments: {string.Join(" ", args)}");
                Log.Info("");

                TProgramArguments o = this.ParseArguments(args);
                if (this.Initialize(o))
                {
                    using (EsriRuntimeAuthorization lic = new EsriRuntimeAuthorization())
                    {
                        if (!lic.Initialize(esriLicenseProduct))
                        {
                            Log.Error(lic.GetInitializationStatus());
                            return -1;
                        }
                        else
                        {
                            Log.Info(lic.GetInitializationStatus());
                        }

                        return (this.Execute(o)) ? 1 : 0;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return -1;
            }
            finally
            {
                Log.Info("");
                Log.Info("Elapsed time was: {0}:{1}:{2}.{3}", stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Chops the file into smaller files based on the line number limit specified.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="limit">The number of lines per file.</param>
        /// <param name="directory">The directory that will contain the chopped files.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{String}" /> representing the chopped files.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">func</exception>
        protected IEnumerable<string> ChopFile(string fileName, int limit, string directory)
        {
            return this.ChopFile(fileName, limit, directory, (s, i) => string.Format("{0}_{1}", s, i));
        }

        /// <summary>
        ///     Chops the file into smaller files based on the line number limit specified.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="limit">The number of lines per file.</param>
        /// <param name="directory">The directory that will contain the chopped files.</param>
        /// <param name="func">The function delegate used to name the next file (without the extension).</param>
        /// <returns>Returns a <see cref="IEnumerable{String}" /> representing the chopped files.</returns>
        /// <exception cref="System.ArgumentNullException">func</exception>
        protected IEnumerable<string> ChopFile(string fileName, int limit, string directory, Func<string, int, string> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            List<string> list = new List<string>();
            int count = 0;

            int lineCount = File.ReadAllLines(fileName).Length;
            if (lineCount > limit)
            {
                string extension = Path.GetExtension(fileName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                StreamWriter sw = null;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (count % limit == 0 || sw == null)
                        {
                            if (sw != null)
                            {
                                sw.Close();
                                sw.Dispose();
                            }

                            string nextFileName = func(fileNameWithoutExtension, count + limit);
                            string path = Path.Combine(directory, nextFileName + extension);
                            sw = new StreamWriter(File.Create(path));
                            list.Add(path);
                        }

                        sw.WriteLine(line);
                        count++;
                    }
                }

                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }

            return list;
        }

        /// <summary>
        ///     Gets the program arguments from the application configuration file.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="string" /> array reperesenting the arguments from the configuration file.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">chars</exception>
        protected virtual string[] CreateArguments()
        {
            var list = new List<string>();

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                var value = ConfigurationManager.AppSettings.Get(key).Trim();
                list.Add(string.Format("{0}{1}", key, value));
            }

            return list.ToArray();
        }


        /// <summary>
        ///     Creates the scratch workspace.
        /// </summary>
        /// <param name="scratchConnectionFile">The scratch connection file.</param>
        /// <param name="fallbackScratchName">Name of the fallback scratch.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the temporary workspace that is either local or in-memory.
        /// </returns>
        protected IWorkspace CreateScratchWorkspace(string scratchConnectionFile, string fallbackScratchName)
        {
            if (string.IsNullOrEmpty(scratchConnectionFile))
            {
                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                return factory.DefaultScratchWorkspace;
            }

            if (!scratchConnectionFile.EndsWith(".gdb", StringComparison.InvariantCultureIgnoreCase))
                scratchConnectionFile = Path.Combine(scratchConnectionFile, fallbackScratchName);

            Log.Info("");
            Log.Info("Connecting to the geodatabase specified by the {0} file.", Path.GetFileName(scratchConnectionFile));

            var fgdb = new FileGDBWorkspaceFactoryClass();

            if (!fgdb.IsWorkspace(scratchConnectionFile) && Directory.Exists(scratchConnectionFile))
                Directory.Delete(scratchConnectionFile, true);

            if (!Directory.Exists(scratchConnectionFile))
            {
                var name = fgdb.Create(Path.GetDirectoryName(scratchConnectionFile), Path.GetFileName(scratchConnectionFile), null, 0);
                return ((IName) name).Open() as IWorkspace;
            }

            var workspace = fgdb.OpenFromFile(scratchConnectionFile, 0);
            return workspace;
        }

        /// <summary>
        ///     Executes the program with the strongly typed command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the program was successful; otherwise <c>false</c>
        /// </returns>
        protected abstract bool Execute(TProgramArguments args);

        /// <summary>
        ///     Gets the characters that are used to split the configuration file argument into an array of arguments,
        ///     that have been decorated with the ArgumentType.Multiple enumeration value.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="char" /> representing an array of characters.
        /// </returns>
        protected virtual char[] GetArgumentTypeMultipleCharacters()
        {
            return new[] {','};
        }


        /// <summary>
        ///     Gets the name of the valid file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns a <see cref="string" /> representing a valid file name.</returns>
        protected string GetValidFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, invalid) => current.Replace(invalid, '-'));
        }


        /// <summary>
        ///     Initializes the program with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual bool Initialize(TProgramArguments args)
        {
            return args != null;
        }


        /// <summary>
        ///     Connects to the geodatabase given the specified parameters.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///     Returns the <see cref="IWorkspace" /> representing the connection to the geodatabase; otherwise <c>null</c>.
        /// </returns>
        protected IWorkspace Open(string fileName)
        {
            Log.Info("");
            Log.Info("Connecting to the geodatabase specified by the {0} file.", Path.GetFileName(fileName));

            IWorkspaceFactory factory = WorkspaceFactories.GetFactory(fileName);
            return factory.OpenFromFile(fileName, 0);
        }

        /// <summary>
        ///     Creates the typed arguments from the specified string array otherwise, the arguments
        ///     are derived from the configuration file.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///     Returns a <see cref="T:TProgramArguments" /> representing the typed arguments.
        /// </returns>
        protected abstract TProgramArguments ParseArguments(string[] args);

        /// <summary>
        ///     Creates a queue using the files that match the specified <paramref name="searchPattern" /> in the specified
        ///     <paramref name="path" />. Each file will be passed to the <paramref name="func" /> delegate for execution,
        ///     afterwards the results will be passed to the <paramref name="action" /> delegate for post processing.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="path">The path to the directory.</param>
        /// <param name="searchPattern">The search pattern for the files.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="func">The function delegate that will be executed for each file.</param>
        /// <param name="action">The action delegate that will be executed after function delegate completes.</param>
        /// <exception cref="System.ArgumentException"></exception>
        protected virtual void Queue<TResult>(string path, string searchPattern, SearchOption searchOption, Func<string, string, TResult> func, Action<TResult> action)
        {
            this.Queue(path, searchPattern, searchOption, s => s, func, action, (s, e) => { });
        }

        /// <summary>
        ///     Creates a queue using the files that match the specified <paramref name="searchPattern" /> in the specified
        ///     <paramref name="path" />. Each file will be passed to the <paramref name="func" /> delegate for execution,
        ///     afterwards the results will be passed to the <paramref name="action" /> delegate for post processing.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="path">The path to the directory.</param>
        /// <param name="searchPattern">The search pattern for the files.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="enqueue">The function delegate that is executed before the file is processed.</param>
        /// <param name="func">The function delegate that will be executed for each file.</param>
        /// <param name="dequeue">The function delegate that is executed after the file has been processed.</param>
        /// <param name="action">The action delegate that will be executed after function delegate completes.</param>
        /// <exception cref="System.ArgumentException"></exception>
        protected void Queue<TResult>(string path, string searchPattern, SearchOption searchOption, Func<string, string> enqueue, Func<string, string, TResult> func, Action<TResult> action, Action<string, Exception> dequeue)
        {
            string directoryName = Path.GetFileNameWithoutExtension(path);
            if (directoryName == null) return;

            foreach (var dataFile in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
            {
                if (!File.Exists(dataFile))
                    continue;

                Exception error = null;
                var queuedFile = enqueue(dataFile);

                try
                {
                    var result = func(directoryName, queuedFile);
                    action(result);
                }
                catch (Exception e)
                {
                    error = e;
                    throw;
                }
                finally
                {
                    dequeue(queuedFile, error);
                }
            }
        }

        /// <summary>
        ///     Roots the file name and ensures that target folder exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns a <see cref="string" /> representing the name of the file.</returns>
        protected string RootFileNameAndEnsureTargetFolderExists(string fileName)
        {
            string rootedFileName = fileName;
            if (!Path.IsPathRooted(rootedFileName))
            {
                rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedFileName);
            }

            string directory = Path.GetDirectoryName(rootedFileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(rootedFileName))
                File.Delete(rootedFileName);

            return rootedFileName;
        }

        #endregion
    }
}