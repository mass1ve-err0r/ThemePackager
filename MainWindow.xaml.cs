using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinDeb
{
    public partial class MainWindow : Window
    {

        ///////////////////////////////////////////////////////////////////////
        //                             GLOBAL VARS                           //
        ///////////////////////////////////////////////////////////////////////

        // <-- m4gic -->
        enum Type : uint {Tweak=1, Theme=2};
        public string extensionT = ".theme";
        private System.Windows.Forms.FolderBrowserDialog FolderDialogIB;
        private System.Windows.Forms.FolderBrowserDialog FolderDialogB;
        private System.Windows.Forms.OpenFileDialog FileOpenerDialogI;


        // <-- PATHS -->
        // base_files
        public readonly string controlFilePath = Directory.GetCurrentDirectory() + "\\_resources\\control-base";
        public readonly string debianbinaryFilePath = Directory.GetCurrentDirectory() + "\\_resources\\debian-binary";
        // executables (ar & 7zip)
        public string UNIXarchiverEXEPath = Directory.GetCurrentDirectory() + "\\_resources\\archiver.exe";
        public string SevenZipEXEPath = Directory.GetCurrentDirectory() + "\\_resources\\7za.zip";
        // pre-made directories to (re-)use
        public string basic_struct = Directory.GetCurrentDirectory() + "\\_resources\\Library\\Themes";
        // Info.plist path
        public string plistPath;
        // base directories
        public string outputPath = Directory.GetCurrentDirectory() + "\\_output";
        public string resourcesPath = Directory.GetCurrentDirectory() + "\\_resources";


        // <-- STRINGS -->
        // path-savers
        public string IconBundlesPath, BundlesPath;
        // specific strings for control
        public string packageBID, packageName, packageVersion, packageDescShort, packageAuthor;
        // deb-string
        public string debfile;


        // <-- DATA STORAGE -->


        // <-- FLAGS / BOOLS -->
        public Boolean Flag_IconBundlesPathSet = false;
        public Boolean Flag_BundlesPathSet = false;
        public Boolean Flag_controlPropertiesSet = false;
        public Boolean Flag_InfoPLISTSet = false;

        // <!--ENTRY--!>
        public MainWindow()
        {
            // inits
            InitializeComponent();
            IBButton.Click += IBHandler;
            BButton.Click += BHandler;
            CButton.Click += CHandler;
            IPButton.Click += IPHandler;
            ResetButton.Click += ResetHandler;
            StartButton.Click += StartHandler;
        }


        ///////////////////////////////////////////////////////////////////////
        //                           MAIN FUNCTIONS                          //
        ///////////////////////////////////////////////////////////////////////

        // IconBundles Handler
        void IBHandler(object sender, RoutedEventArgs e)
        {
            // set it
            this.FolderDialogIB = new System.Windows.Forms.FolderBrowserDialog();
            FolderDialogIB.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            System.Windows.Forms.DialogResult result = FolderDialogIB.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // set TBText
                StatusIB.Text = "SET";
                StatusIB.Foreground = Brushes.Green;
                // set global path
                IconBundlesPath = FolderDialogIB.SelectedPath;
                Environment.SpecialFolder root = FolderDialogIB.RootFolder;
                // set flag for IB
                Flag_IconBundlesPathSet = true;
            }
        }


        // Bundles Handler
        void BHandler(object sender, RoutedEventArgs e)
        {
            // set it
            this.FolderDialogB = new System.Windows.Forms.FolderBrowserDialog();
            FolderDialogB.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            System.Windows.Forms.DialogResult result = FolderDialogB.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // set TBText
                StatusB.Text = "SET";
                StatusB.Foreground = Brushes.Green;
                // set global path
                BundlesPath = FolderDialogB.SelectedPath;
                Environment.SpecialFolder root = FolderDialogB.RootFolder;
                // set flag for IB
                Flag_BundlesPathSet = true;
            }
        }


        // Control-File Handler
        void CHandler(object sender, RoutedEventArgs e)
        {
            // spawn the new Window & read values.
            Window1 controlWindow = new Window1();
            controlWindow.ShowDialog();
            // rip properties from TextBoxes
            packageBID = controlWindow.controlWindow_packageBID.Text;
            packageName = controlWindow.controlWindow_packageName.Text;
            packageVersion = controlWindow.controlWindow_packageVersion.Text;
            packageDescShort = controlWindow.controlWindow_packageDescShort.Text;
            packageAuthor = controlWindow.controlWindow_packageAuthor.Text;
            // set TBText
            StatusCP.Text = "SET";
            StatusCP.Foreground = Brushes.Green;
            // Set Flag for CP
            Flag_controlPropertiesSet = true;
        }


        // Icon.PLIST File Handler
        void IPHandler(object sender, RoutedEventArgs e)
        {
            // grab the file lul
            this.FileOpenerDialogI = new OpenFileDialog();
            System.Windows.Forms.DialogResult result = FileOpenerDialogI.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // set text
                StatusIP.Text = "SET";
                StatusIP.Foreground = Brushes.Green;
                // save full filepath
                var filePath = FileOpenerDialogI.FileName;
                plistPath = filePath;
                // set Flag
                Flag_InfoPLISTSet = true;
            }
        }


        // Start Handler
        async void StartHandler(object sender, RoutedEventArgs e)
        {
            // check for the Flags 
            if (Flag_IconBundlesPathSet && Flag_BundlesPathSet && Flag_controlPropertiesSet && Flag_InfoPLISTSet)
            {
                // structs
                string FullTheme = basic_struct + "\\" + packageName + extensionT;
                DirectoryInfo baseDir = new DirectoryInfo(basic_struct);
                // create DI for given paths
                DirectoryInfo BundlesDir = new DirectoryInfo(BundlesPath);
                DirectoryInfo IconBundlesDir = new DirectoryInfo(IconBundlesPath);
                // create theme directory & DI
                baseDir.CreateSubdirectory(packageName + extensionT);
                DirectoryInfo themeDir = new DirectoryInfo(FullTheme);
                // create respective subdirs & DIs
                themeDir.CreateSubdirectory("IconBundles");
                themeDir.CreateSubdirectory("Bundles");
                DirectoryInfo IBDirDestination = new DirectoryInfo(FullTheme + "\\IconBundles");
                DirectoryInfo BDirDestination = new DirectoryInfo(FullTheme + "\\Bundles");
                // lauch copy-agent for Bundles & IconBundles
                copyAgent(BundlesDir, BDirDestination);
                copyAgent(IconBundlesDir, IBDirDestination);
                // once copied, lauch property injector to create finished control file
                injectProperties();
                // copy the Info.plist into it, call wait after | ASYNC
                System.IO.File.Copy(plistPath, FullTheme + "\\Info.plist");
                await Task.Delay(3000);
                // call our compressing algos
                compressFiles();
                createDebianPackage();
                // lastly, copy the deb to the output folder so we can clean the resource folder lmao | ASYNC
                System.IO.File.Move(resourcesPath + "\\" + debfile, outputPath + "\\" + debfile);
                await Task.Delay(3000);
                // fire cleanup-agent after a lil wait (need to move the shit out)
                cleanupAgent();
                // final message
                System.Windows.MessageBox.Show("debian package created & saved in the output folder!", "ThemePackager - Success");
            }
            else
            {
                // throw error message
                System.Windows.MessageBox.Show("One or more Fields have NOT been set. Please check again!", "ThemePackager - Error");
            }
        }


        // Reset Handler
        void ResetHandler(object sender, RoutedEventArgs e)
        {
            // Flags
            Flag_IconBundlesPathSet = false;
            Flag_BundlesPathSet = false;
            Flag_controlPropertiesSet = false;
            Flag_InfoPLISTSet = false;
            // IconBundlesButton
            StatusIB.Text = "";
            // BundlesButton
            StatusB.Text = "";
            // ControlFileButton
            StatusCP.Text = "";
            // InfoPLISTFileButton
            StatusIP.Text = "";
        }


        // PT.1: compress
        void compressFiles()
        {
            // construct local cmd-string
            string sz_cmd_data = "cd _resources & 7za.exe a -ttar data.tar .\\Library";
            string sz_cmd_control = "cd _resources & 7za.exe a -ttar control.tar control";
            // spawn processes
            spawnCMDProcess(sz_cmd_control);
            spawnCMDProcess(sz_cmd_data);
        }


        // PT.2: create the deb
        void createDebianPackage()
        {
            // construct local cmd-string | style: packageBID_packageVersion.deb
            debfile = packageBID + "_" + packageVersion + ".deb";
            string ar_cmd = "cd _resources & archiver.exe -rcs " + debfile + " debian-binary control.tar data.tar";
            // spawn process
            spawnCMDProcess(ar_cmd);
        }


        // PT.3: clean up afterwards
        void cleanupAgent()
        {
            // structs
            string FullThemePath = basic_struct + "\\" + packageName + extensionT;
            // delete the temp theme
            Directory.Delete(FullThemePath, true);
            // delete the temp files !--hard-offsets--!
            File.Delete(Directory.GetCurrentDirectory() + "\\_resources\\control");
            File.Delete(Directory.GetCurrentDirectory() + "\\_resources\\control.tar");
            File.Delete(Directory.GetCurrentDirectory() + "\\_resources\\data.tar");
            //File.Delete(Directory.GetCurrentDirectory() + "\\_resources\\" + debfile);
        }


        ///////////////////////////////////////////////////////////////////////
        //                           HELPER FUNCTIONS                        //
        ///////////////////////////////////////////////////////////////////////
        
        // cmd-process spawner
        void spawnCMDProcess(string args)
        {
            // spawn process
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            // "/C" makes it silent, no extra cmd window
            startInfo.Arguments = "/C " + args;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }


        // copy agent -thanks MSDN
        void copyAgent(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
            }
            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                copyAgent(diSourceSubDir, nextTargetSubDir);
            }
        }


        // property-injector | sed-i, just cheaper lmao
        void injectProperties()
        {
            // actual logic: stream File with newlines (UNIX -LF), replace and re-create
            string text = File.ReadAllText(controlFilePath);
            // replacements
            text = text.Replace("$packageBID", packageBID);
            text = text.Replace("$packageName", packageName);
            text = text.Replace("$packageVersion", packageVersion);
            text = text.Replace("$packageDescShort", packageDescShort);
            text = text.Replace("$packageAuthor", packageAuthor);
            // write-back
            File.WriteAllText(resourcesPath + "\\control", text);
        }

        //
    }
    // <--FIN-->
}
