using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Jan18101997.Windows.Installer
{
    public class RegisterUninstaller
    {
        /// <summary>
        /// Creates a new Uninstall Register
        /// </summary>
        public RegisterUninstaller()
        {
            RegisterFor = RegisterFor.AllUser;
            Language = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Creates a new Uninstall Register
        /// </summary>
        /// <param name="applicationIdentifier">Application Name or GUID</param>
        public RegisterUninstaller(string applicationIdentifier)
        {
            ApplicationIdentifier = applicationIdentifier;
            RegisterFor = RegisterFor.AllUser;
            Language = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Creates a new Uninstall Register
        /// </summary>
        /// <param name="applicationIdentifier">Application Name or GUID</param>
        /// <param name="registerFor">location for registratopn</param>
        public RegisterUninstaller(string applicationIdentifier, RegisterFor registerFor)
        {
            ApplicationIdentifier = applicationIdentifier;
            RegisterFor = registerFor;
            Language = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Creates a new Uninstall Register
        /// </summary>
        /// <param name="applicationIdentifier">Application Name or GUID</param>
        /// <param name="registerFor">location for registratopn</param>
        /// <param name="productName">App Product Name</param>
        /// <param name="publisher">Publisher</param>
        /// <param name="uninstallerString">Uninstaller Location</param>
        public RegisterUninstaller(string applicationIdentifier, RegisterFor registerFor, string productName, string publisher, string uninstallerString)
        {
            ApplicationIdentifier = applicationIdentifier;
            RegisterFor = registerFor;
            ProductName = productName;
            publisher = Publisher;
            UninstallString = uninstallerString;
            Language = CultureInfo.CurrentCulture;
        }

        public const string BaseRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";

        /// <summary>
        /// Required Application Name or GUID
        /// </summary>
        public string ApplicationIdentifier { get; set; }

        /// <summary>
        /// Application Comment
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Estimated Size in kb
        /// </summary>
        public int EstimatedSize { get; set; }

        /// <summary>
        /// Help Link
        /// </summary>
        public string HelpLink { get; set; }

        /// <summary>
        /// Help Telephone
        /// </summary>
        public string HelpTelephone { get; set; }

        /// <summary>
        /// Install Date will be a generated Unix-Timestamp
        /// </summary>
        public double InstallDate { get { return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; } }

        /// <summary>
        /// Appfolder
        /// </summary>
        public string InstallLocation { get; set; }

        /// <summary>
        /// Installer Location
        /// </summary>
        public string InstallSource { get; set; }

        /// <summary>
        /// Required Language
        /// </summary>
        public CultureInfo Language { get; set; }

        /// <summary>
        /// A modify App
        /// </summary>
        public string ModifyPath { get; set; }

        /// <summary>
        /// Required Product Name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Product Version
        /// </summary>
        public ApplicationVersion ProductVersion { get; set; }

        /// <summary>
        /// Required Publisher Name
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Readmefile Location
        /// </summary>
        public string Readme { get; set; }

        /// <summary>
        /// Register information for registratopn
        /// </summary>
        public RegisterFor RegisterFor { get; set; }

        /// <summary>
        /// Required Uninstakler App
        /// </summary>
        public string UninstallString { get; set; }

        /// <summary>
        /// Abour this Application Page
        /// </summary>
        public string URLInfoAbout { get; set; }

        /// <summary>
        /// Update Page
        /// </summary>
        public string URLUpdateInfo { get; set; }

        /// <summary>
        /// Reads a specefied key
        /// </summary>
        /// <param name="ApplicationIdentifier">Application Name or GUID</param>
        /// <param name="searchIn">Search in</param>
        /// <returns></returns>
        public static RegisterUninstaller ReadKey(string ApplicationIdentifier, RegisterFor searchIn)
        {
            RegisterUninstaller ru = new RegisterUninstaller();
            ru.ApplicationIdentifier = ApplicationIdentifier;
            ru.RegisterFor = searchIn;

            ru.ReadKey();

            return ru;
        }

        /// <summary>
        /// Checks if all necessary Data exists
        /// </summary>
        /// <returns></returns>
        public bool CheckData()
        {
            return CheckData(true);
        }

        /// <summary>
        /// Checks if all necessary Data exists
        /// </summary>
        /// <param name="throwError">Sets if this will throw errors or rust return false if not complete</param>
        /// <returns></returns>
        public bool CheckData(bool throwError)
        {
            if (string.IsNullOrEmpty(UninstallString) == true)
            {
                if (throwError == true)
                    throw new ArgumentException("Missing UninstallString");
                else
                    return false;
            }
            if (Language == null)
            {
                if (throwError == true)
                    throw new ArgumentException("Missing Language");
                else
                    return false;
            }
            if (string.IsNullOrEmpty(Publisher) == true)
            {
                if (throwError == true)
                    throw new ArgumentException("Missing Publisher");
                else
                    return false;
            }
            if (string.IsNullOrEmpty(ProductName) == true)
            {
                if (throwError == true)
                    throw new ArgumentException("Missing ProductName");
                else
                    return false;
            }
            if (string.IsNullOrEmpty(ApplicationIdentifier) == true)
            {
                if (throwError == true)
                    throw new ArgumentException("Missing ApplicationIdentifier");
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Reads a Specefied key
        /// </summary>
        public void ReadKey()
        {
            RegistryKey UninstallerKey = null;

            switch (RegisterFor)
            {
                case RegisterFor.AllUser:
                    UninstallerKey = Registry.LocalMachine.OpenSubKey(BaseRegPath, true);
                    break;

                case RegisterFor.CurrentUser:
                    UninstallerKey = Registry.CurrentUser.OpenSubKey(BaseRegPath, true);
                    break;
            }

            RegistryKey AppKey = UninstallerKey.OpenSubKey(ApplicationIdentifier);

            if (AppKey == null)
                throw new KeyNotFoundException("The ApplicationIdentifier was not found");

            object tmpValue = null;

            tmpValue = AppKey.GetValue("DisplayName");
            if (tmpValue != null)
                ProductName = (string)tmpValue;

            tmpValue = AppKey.GetValue("DisplayVersion");
            if (tmpValue != null)
                ProductVersion = (string)tmpValue;

            tmpValue = AppKey.GetValue("Publisher");
            if (tmpValue != null)
                Publisher = (string)tmpValue;

            tmpValue = AppKey.GetValue("HelpLink");
            if (tmpValue != null)
                HelpLink = (string)tmpValue;

            tmpValue = AppKey.GetValue("HelpTelephone");
            if (tmpValue != null)
                HelpTelephone = (string)tmpValue;

            tmpValue = AppKey.GetValue("InstallLocation");
            if (tmpValue != null)
                InstallLocation = (string)tmpValue;

            tmpValue = AppKey.GetValue("InstallSource");
            if (tmpValue != null)
                InstallSource = (string)tmpValue;

            tmpValue = AppKey.GetValue("URLInfoAbout");
            if (tmpValue != null)
                URLInfoAbout = (string)tmpValue;

            tmpValue = AppKey.GetValue("URLUpdateInfo");
            if (tmpValue != null)
                URLUpdateInfo = (string)tmpValue;

            tmpValue = AppKey.GetValue("DisplayName");
            if (tmpValue != null)
                ProductName = (string)tmpValue;
            tmpValue = AppKey.GetValue("Comments");
            if (tmpValue != null)
                Comments = (string)tmpValue;

            tmpValue = AppKey.GetValue("Contact");
            if (tmpValue != null)
                Contact = (string)tmpValue;

            tmpValue = AppKey.GetValue("EstimatedSize");
            if (tmpValue != null)
                EstimatedSize = (int)tmpValue;

            tmpValue = AppKey.GetValue("Language");
            if (tmpValue != null)
                Language = new CultureInfo((int)tmpValue);

            tmpValue = AppKey.GetValue("DisplayName");
            if (tmpValue != null)
                ProductName = (string)tmpValue;

            tmpValue = AppKey.GetValue("ModifyPath");
            if (tmpValue != null)
                ModifyPath = (string)tmpValue;

            tmpValue = AppKey.GetValue("Readme");
            if (tmpValue != null)
                Readme = (string)tmpValue;

            tmpValue = AppKey.GetValue("UninstallString");
            if (tmpValue != null)
                UninstallString = (string)tmpValue;
        }

        /// <summary>
        /// Registers all data
        /// </summary>
        public void RegisterApp()
        {
            CheckData();

            RegistryKey UninstallerKey = null;

            switch (RegisterFor)
            {
                case RegisterFor.AllUser:
                    UninstallerKey = Registry.LocalMachine.OpenSubKey(BaseRegPath, true);
                    break;

                case RegisterFor.CurrentUser:
                    UninstallerKey = Registry.CurrentUser.OpenSubKey(BaseRegPath, true);
                    break;
            }

            UninstallerKey.CreateSubKey(ApplicationIdentifier);

            RegistryKey AppKey = UninstallerKey.OpenSubKey(ApplicationIdentifier, true);

            if (string.IsNullOrEmpty(ProductName) == false)
                AppKey.SetValue("DisplayName", ProductName, RegistryValueKind.String);
            if (ProductVersion.Build != 0 && ProductVersion.Major != 0 && ProductVersion.Minor != 0)
                AppKey.SetValue("DisplayVersion", ProductVersion.ToString(), RegistryValueKind.String);
            if (string.IsNullOrEmpty(Publisher) == false)
                AppKey.SetValue("Publisher", Publisher, RegistryValueKind.String);
            if (ProductVersion.Build != 0 && ProductVersion.Major != 0 && ProductVersion.Minor != 0)
                AppKey.SetValue("VersionMinor", ProductVersion.Minor, RegistryValueKind.String);
            if (ProductVersion.Build != 0 && ProductVersion.Major != 0 && ProductVersion.Minor != 0)
                AppKey.SetValue("VersionMajor", ProductVersion.Major, RegistryValueKind.String);
            if (ProductVersion.Build != 0 && ProductVersion.Major != 0 && ProductVersion.Minor != 0)
                AppKey.SetValue("Version", ProductVersion.ToString(), RegistryValueKind.String);
            if (string.IsNullOrEmpty(HelpLink) == false)
                AppKey.SetValue("HelpLink", HelpLink, RegistryValueKind.String);
            if (string.IsNullOrEmpty(HelpTelephone) == false)
                AppKey.SetValue("HelpTelephone", HelpTelephone, RegistryValueKind.String);
            if (InstallDate != 0)
                AppKey.SetValue("InstallDate", InstallDate, RegistryValueKind.String);
            if (string.IsNullOrEmpty(InstallLocation) == false)
                AppKey.SetValue("InstallLocation", InstallLocation, RegistryValueKind.String);
            if (string.IsNullOrEmpty(InstallSource) == false)
                AppKey.SetValue("InstallSource", InstallSource, RegistryValueKind.String);
            if (string.IsNullOrEmpty(URLInfoAbout) == false)
                AppKey.SetValue("URLInfoAbout", URLInfoAbout, RegistryValueKind.String);
            if (string.IsNullOrEmpty(URLUpdateInfo) == false)
                AppKey.SetValue("URLUpdateInfo", URLUpdateInfo, RegistryValueKind.String);
            if (string.IsNullOrEmpty(Comments) == false)
                AppKey.SetValue("Comments", Comments, RegistryValueKind.String);
            if (string.IsNullOrEmpty(Contact) == false)
                AppKey.SetValue("Contact", Contact, RegistryValueKind.String);
            if (EstimatedSize != 0)
                AppKey.SetValue("EstimatedSize", EstimatedSize, RegistryValueKind.DWord);
            if (Language != null)
                AppKey.SetValue("Language", Language.LCID, RegistryValueKind.DWord);
            if (string.IsNullOrEmpty(ModifyPath) == false)
                AppKey.SetValue("ModifyPath", ModifyPath, RegistryValueKind.String);
            if (string.IsNullOrEmpty(Readme) == false)
                AppKey.SetValue("Readme", Readme, RegistryValueKind.String);
            if (string.IsNullOrEmpty(UninstallString) == false)
                AppKey.SetValue("UninstallString", UninstallString, RegistryValueKind.String);

            AppKey.Close();
            UninstallerKey.Close();
        }

        /// <summary>
        /// Unregisters all data
        /// </summary>
        public void UnregisterApp()
        {
            RegistryKey UninstallerKey = null;

            switch (RegisterFor)
            {
                case RegisterFor.AllUser:
                    UninstallerKey = Registry.LocalMachine.OpenSubKey(BaseRegPath, true);
                    break;

                case RegisterFor.CurrentUser:
                    UninstallerKey = Registry.CurrentUser.OpenSubKey(BaseRegPath, true);
                    break;
            }

            UninstallerKey.DeleteSubKeyTree(ApplicationIdentifier);
        }
    }

    public enum RegisterFor
    {
        AllUser,
        CurrentUser
    }

    public struct ApplicationVersion
    {
        public ushort Build;
        public byte Major;
        public byte Minor;

        public static implicit operator ApplicationVersion(string sourceString)
        {
            ApplicationVersion av = new ApplicationVersion();

            av.Major = byte.Parse(sourceString.Split('.')[0]);
            av.Minor = byte.Parse(sourceString.Split('.')[1]);
            av.Build = ushort.Parse(sourceString.Split('.')[2]);

            return av;
        }

        public static implicit operator ApplicationVersion(Version sourceVersion)
        {
            ApplicationVersion av = new ApplicationVersion();

            av.Major = (byte)sourceVersion.Major;
            av.Minor = (byte)sourceVersion.Minor;
            av.Build = (byte)sourceVersion.Build;

            return av;
        }

        public override string ToString()
        {
            return Major + "." + Minor + "." + Build;
        }
    }
}