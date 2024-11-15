using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using QlmLicenseLib;

namespace LicenseDemoProject.ViewModels
{
    public class LicenseValidator
    {
        #region Data Members

        protected QlmLicense license;
        protected string activationKey = string.Empty;
        protected string computerKey = string.Empty;

        protected bool hasExpiryDate = false;
        protected bool licenseExpired = false;
        protected int licenseRemainingDays = -1;
        protected bool wrongProductVersion = false;

        protected string customData1 = string.Empty;
        protected string customData2 = string.Empty;
        protected string customData3 = string.Empty;

        protected string serverMessage = string.Empty;
        protected EServerErrorCode serverErrorCode = EServerErrorCode.NoError;
        protected ILicenseInfo serverLicenseInfo = new LicenseInfo();

        // You should customize the Product Properties filename as well as the folder where this file is stored.
        // Check the ReadProductPropertiesFile and WriteProductPropertiesFile methods in this class
        protected string productPropertiesFileName = "QlmProductProperties.xml";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor initializes the license product definition
        /// </summary>
        public LicenseValidator()
        {
            license = new QlmLicense();

            // Always obfuscate your code. In particular, you should always obfuscate all arguments
            // of DefineProduct and the Public Key (i.e. encrypt all the string arguments).

            // If you are using the QLM License Wizard, you can load the product definition from the settings.xml file generated
            // by the Protect Your App Wizard.
            // To load the settings from the XML file, call the license.LoadSettings function.

            license.DefineProduct (2, "Demo Enterprise", 1, 0, "DemoKey", "{7E3C0CFD-8495-462F-9C1F-D7EDB4355448}");
			license.LicenseEngineLibrary = ELicenseEngineLibrary.DotNet;
			license.PublicKey = "F+FViE9WEKoLDA==";
			license.RsaPublicKey = "<RSAKeyValue><Modulus>4DK1SdhuO1waywt6D7sP6wTH4ih7+6In5idGfFBDMj6O+7wtUFkM6lqFn1D//HT0tb090XG34hlGUyAO5sCZd2z5s54T70Z/DdFG8ef/61NFPxbKFYfUmD87swgRdDLQPzPCsIqECV0aIAXhldrLBY3JxGobsPlldiPo3mir6pk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			license.CommunicationEncryptionKey = "{B6163D99-F46A-4580-BB42-BF276A507A14}";
			license.DefaultWebServiceUrl = "https://qlm3.net/qlmdemo/QlmLicenseServer/qlmservice.asmx";
			license.StoreKeysLocation = EStoreKeysTo.EFileCommonData;
			license.StoreKeysOptions = EStoreKeysOptions.EStoreKeysPerUser;
			license.ValidateOnServer = false;
			license.PublishAnalytics = true;
			license.EvaluationPerUser = true;
			license.EnableMultibyte = true;
			license.ExpiryDateRoundHoursUp = true;
			license.EnableSoapExtension = true;
			license.EnableClientLanguageDetection = true;
			license.LimitTerminalServerInstances = false;
			license.AllowGenericKeys = false;
			license.DownloadLicenseFile = false;
			license.DownloadProductProperties = true;
			license.Version = "5.0.00";

            // If you are using QLM Professional, you should also set the communicationEncryptionKey property
            // The CommunicationEncryptionKey must match the value specified in the web.config file of the QLM License Server

            // Make sure that the StoreKeysLocation specified here is consistent with the one specified in the QLM .NET Control
            

            // To ignore server certificate issues, uncomment this line
            //ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidator;
        }

        /// <summary>
        /// Constructor initializes the license product definition
        /// </summary>
        public LicenseValidator(string settingsFile)
        {
            license = new QlmLicense();
            license.LoadSettings(settingsFile);
            
			license.RsaPublicKey = "<RSAKeyValue><Modulus>4DK1SdhuO1waywt6D7sP6wTH4ih7+6In5idGfFBDMj6O+7wtUFkM6lqFn1D//HT0tb090XG34hlGUyAO5sCZd2z5s54T70Z/DdFG8ef/61NFPxbKFYfUmD87swgRdDLQPzPCsIqECV0aIAXhldrLBY3JxGobsPlldiPo3mir6pk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            string errorMessage;
            if (license.ValidateSettingsFile(settingsFile, out errorMessage) == false)
            {
                throw new Exception(errorMessage);
            }

        }

        /// <summary>
        /// Initializes the LicenseValidator class from the XML Settings file or content
        /// </summary>
        /// <param name="settingsFile">XML Settings file generated by the Protect Your App wizard</param>
        /// <param name="settingsContent">XML Settings content. If set, the settingsFile is ignored</param>
        /// <exception cref="Exception"></exception>
        public LicenseValidator(string settingsFile, string settingsContent)
        {
            license = new QlmLicense();

            if (String.IsNullOrEmpty(settingsContent) && !String.IsNullOrEmpty(settingsFile) && File.Exists(settingsFile))
            {
                try
                {
                    settingsContent = File.ReadAllText(settingsFile, Encoding.UTF8);
                }
                catch { }
            }

            if (!String.IsNullOrEmpty(settingsContent))
            {
                license.LoadSettingsXml(settingsContent);

                
			license.RsaPublicKey = "<RSAKeyValue><Modulus>4DK1SdhuO1waywt6D7sP6wTH4ih7+6In5idGfFBDMj6O+7wtUFkM6lqFn1D//HT0tb090XG34hlGUyAO5sCZd2z5s54T70Z/DdFG8ef/61NFPxbKFYfUmD87swgRdDLQPzPCsIqECV0aIAXhldrLBY3JxGobsPlldiPo3mir6pk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

                string errorMessage;
                if (license.ValidateSettingsContent(settingsContent, out errorMessage) == false)
                {
                    throw new Exception(errorMessage);
                }
            }
            else
            {
                throw new Exception(String.Format("The XML settings content is empty."));
            }

        }

        #endregion

        #region Public Methods

        public virtual bool ValidateLicenseAtStartup(string computerID, ref bool needsActivation, ref string returnMsg)
        {
            return ValidateLicenseAtStartup(computerID, ELicenseBinding.UserDefined, ref needsActivation, ref returnMsg);
        }

        public virtual bool ValidateLicenseAtStartup(ELicenseBinding licenseBinding, ref bool needsActivation, ref string returnMsg)
        {
            return ValidateLicenseAtStartup(string.Empty, licenseBinding, ref needsActivation, ref returnMsg);
        }


        /// <remarks>Call ValidateLicenseAtStartup when your application is launched. 
        /// If this function returns false, exit your application.
        /// </remarks>
        /// 
        /// <summary>
        /// Validates the license when the application starts up. 
        /// The first time a license key is validated successfully,
        /// it is stored in a hidden file on the system. 
        /// When the application is restarted, this code will load the license
        /// key from the hidden file and attempt to validate it again. 
        /// If it validates succesfully, the function returns true.
        /// If the license key is invalid, expired, etc, the function returns false.
        /// </summary>
        /// <param name="computerID">Unique Computer identifier</param>
        /// <param name="returnMsg">Error message returned, in case of an error</param>
        /// <returns>true if the license is OK.</returns>
        public virtual bool ValidateLicenseAtStartup(string computerID, ELicenseBinding licenseBinding, ref bool needsActivation, ref string returnMsg)
        {
            returnMsg = string.Empty;
            needsActivation = false;

            bool serverUpdateRequiresReactivation = false;

            license.ReadKeys(ref activationKey, ref computerKey);

            if (String.IsNullOrEmpty(activationKey) && String.IsNullOrEmpty(computerKey))
            {
                if (String.IsNullOrEmpty(license.EvaluationLicenseKey))
                {
                    returnMsg = "No license key was found.";
                    return false;
                }
                else
                {
                    activationKey = license.EvaluationLicenseKey;
                }
            }

            bool ret = ValidateLicense(activationKey, computerKey, ref computerID, licenseBinding, ref needsActivation, ref returnMsg);

            if ((ret == true) && license.ValidateOnServer)
            {
                // If the local license is valid, check on the server if it's valid as well.

                // When ValidateLicenseOnServer is not able to contact the License Server:
                //  If MaxDaysOffline is set to -1, ValidateLicenseOnServer will aways return true, connectionSuccessfull will be false.
                //  If MaxDaysOffline is set to a specific value, say 5 days, ValidateLicenseOnServer will return true if no connection was establihed for <= 5 days
                //  otherwise it will return false.              

                // Renitialize the serverLicenseInfo object
                serverLicenseInfo = new LicenseInfo();

                if (license.ValidateLicenseOnServerEx2(string.Empty, activationKey, computerKey, computerID, Environment.MachineName, false, ref serverLicenseInfo, out serverErrorCode, out serverMessage) == false)
                {
                    if (serverErrorCode == EServerErrorCode.License_ComputerKeyMismatch)
                    {
                        if (ReactivateKey(computerID))
                        {
                            return true;
                        }
                    }

                    returnMsg = serverMessage;
                    return false;
                }

                if (serverErrorCode == EServerErrorCode.NoError)
                {
                    serverUpdateRequiresReactivation = (serverLicenseInfo.NewExpiryDate != DateTime.MinValue) || !String.IsNullOrEmpty(serverLicenseInfo.NewFeatures) || (serverLicenseInfo.NewFloatingSeats != -1);
                }
            }

            if (wrongProductVersion || LicenseExpired || serverUpdateRequiresReactivation)
            {
                if (String.IsNullOrEmpty(computerKey) && license.IsLicenseKeyACloudLicense(activationKey))
                {
                    ret = false;
                    needsActivation = true;
                }
                else if (license.ValidateOnServer)
                {
                    //
                    // If a license has expired but then renewed on the server, reactivating the key will extend the client
                    // with the new subscription period.
                    //
                    ret = ReactivateKey(computerID);
                }
            }

            if (ret && this.QlmLicenseObject.DownloadLicenseFile)
            {
                ret = VerifyLicenseFile(computerID, out returnMsg);                
            }

            return ret;

        }

        /// <remarks>Call this function in the dialog where the user enters the license key to validate the license.</remarks>
        /// <summary>
        /// Validates a license key. If you provide a computer key, the computer key is validated. 
        /// Otherwise, the activation key is validated. 
        /// If you are using machine bound keys (UserDefined), you can provide the computer identifier, 
        /// otherwise set the computerID to an empty string.
        /// </summary>
        /// <param name="activationKey">Activation Key</param>
        /// <param name="computerKey">Computer Key</param>
        /// <param name="computerID">Unique Computer identifier</param>
        /// <returns>true if the license is OK.</returns>
        public virtual bool ValidateLicense(string activationKey, string computerKey, ref string computerID, ELicenseBinding licenseBinding, ref bool needsActivation, ref string returnMsg)
        {
            bool ret = false;

            needsActivation = false;
            hasExpiryDate = false;
            licenseExpired = false;
            licenseRemainingDays = -1;
            wrongProductVersion = false;

            string licenseKey = computerKey;

            if (String.IsNullOrEmpty(licenseKey))
            {
                licenseKey = activationKey;

                if (String.IsNullOrEmpty(licenseKey))
                {
                    return false;
                }
            }

            if (licenseBinding == ELicenseBinding.UserDefined)
            {
                returnMsg = license.ValidateLicenseEx(licenseKey, computerID);
            }
            else
            {
                returnMsg = license.ValidateLicenseEx3(licenseKey, licenseBinding, false, false);
                computerID = license.GetComputerIDEx1(licenseBinding, licenseKey);
            }

            int nStatus = (int)license.GetStatus();

            if (IsTrue(nStatus, (int)ELicenseStatus.EKeyInvalid) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyProductInvalid) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyMachineInvalid) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyExceededAllowedInstances) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyTampered))
            {
                // the key is invalid
                ret = false;
            }
            else if (IsTrue(nStatus, (int)ELicenseStatus.EKeyVersionInvalid))
            {
                wrongProductVersion = true;
                ret = false;
            }
            else if (IsTrue(nStatus, (int)ELicenseStatus.EKeyDemo))
            {
                hasExpiryDate = true;

                if (IsTrue(nStatus, (int)ELicenseStatus.EKeyExpired))
                {
                    // the key has expired
                    ret = false;
                    licenseExpired = true;
                }
                else
                {
                    // the demo key is still valid
                    ret = true;
                    licenseRemainingDays = license.DaysLeft;
                }
            }
            else if (IsTrue(nStatus, (int)ELicenseStatus.EKeyPermanent))
            {
                // the key is OK                
                ret = true;
            }

            if (ret == true)
            {

                if (license.IsActivationLicense (license.LicenseType))
                {
                    needsActivation = true;
                    ret = false;
                }                
            }

            return ret;

        }

        /// <remarks>Call this function in the dialog where the user enters the license key to validate the license.</remarks>
        /// <summary>
        /// Validates a license key. If you provide a computer key, the computer key is validated. 
        /// Otherwise, the activation key is validated. 
        /// If you are using machine bound keys (UserDefined), you can provide the computer identifier, 
        /// otherwise set the computerID to an empty string.
        /// </summary>
        /// <param name="activationKey">Activation Key</param>
        /// <param name="computerKey">Computer Key</param>
        /// <param name="computerID">Unique Computer identifier</param>
        /// <returns>true if the license is OK.</returns>
        public virtual bool ValidateLicense(string activationKey, string computerKey, string computerID, ref bool needsActivation, ref string returnMsg)
        {
            bool ret = false;

            needsActivation = false;
            hasExpiryDate = false;
            licenseExpired = false;
            licenseRemainingDays = -1;
            wrongProductVersion = false;

            string licenseKey = computerKey;

            if (String.IsNullOrEmpty(licenseKey))
            {
                licenseKey = activationKey;

                if (String.IsNullOrEmpty(licenseKey))
                {
                    return false;
                }
            }

            returnMsg = license.ValidateLicenseEx(licenseKey, computerID);

            int nStatus = (int)license.GetStatus();

            if (IsTrue(nStatus, (int)ELicenseStatus.EKeyInvalid) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyProductInvalid) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyMachineInvalid) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyExceededAllowedInstances) ||
                IsTrue(nStatus, (int)ELicenseStatus.EKeyTampered))
            {
                // the key is invalid
                ret = false;
            }
            else if (IsTrue(nStatus, (int)ELicenseStatus.EKeyVersionInvalid))
            {
                wrongProductVersion = true;
                ret = false;
            }
            else if (IsTrue(nStatus, (int)ELicenseStatus.EKeyDemo))
            {
                hasExpiryDate = true;

                if (IsTrue(nStatus, (int)ELicenseStatus.EKeyExpired))
                {
                    // the key has expired
                    ret = false;
                    licenseExpired = true;
                }
                else
                {
                    // the demo key is still valid
                    ret = true;
                    licenseRemainingDays = license.DaysLeft;
                }
            }
            else if (IsTrue(nStatus, (int)ELicenseStatus.EKeyPermanent))
            {
                // the key is OK                
                ret = true;
            }

            if (ret == true)
            {

                if (license.IsActivationLicense(license.LicenseType))
                {
                    needsActivation = true;
                    ret = false;
                }
            }

            return ret;

        }

        /// <summary>
        /// Delete all license keys stored in the registry or on the file system
        /// </summary>
        public virtual void DeleteAllKeys()
        {
            // the license was revoked, we need to remove the keys on this system.
            EStoreKeysTo saveLocation = license.StoreKeysLocation;

            try
            {
                // Remove keys stored on the file system
                license.StoreKeysLocation = EStoreKeysTo.EFile;
                license.DeleteKeys();

                // Remove keys stored in the registry
                license.StoreKeysLocation = EStoreKeysTo.ERegistry;
                license.DeleteKeys();

                // Remove keys stored in the common data
                license.StoreKeysLocation = EStoreKeysTo.EFileCommonData;
                license.DeleteKeys();
            }
            catch
            { }
            finally
            {
                computerKey = string.Empty;
                // Restore the previous setting
                license.StoreKeysLocation = saveLocation;
            }
        }

        /// <summary>
        /// Reactivates a key - this is typically used to automatically get a subscription extension from the server
        /// </summary>
        /// <param name="computerID"></param>
        /// <param name="newComputerKey"></param>
        /// <returns></returns>
        protected virtual bool ReactivateKey(string computerID)
        {
            bool ret = false;
            string newComputerKey = string.Empty;

            DateTime serverDate;
            string response = string.Empty;
            if (license.PingEx (string.Empty, out response, out serverDate) == false)
            {
                // we cannot connect to the server so we cannot do any validation with the server
                return false;
            }
            

            // try to reactivate the license and see if it still expired
            response = string.Empty;
            license.ReactivateLicense(license.DefaultWebServiceUrl, ActivationKey, computerID, out response);

            // Renitialize the serverLicenseInfo object
            serverLicenseInfo = new LicenseInfo();
            string message = string.Empty;
            if (license.ParseResults(response, ref serverLicenseInfo, ref message))
            {
                newComputerKey = serverLicenseInfo.ComputerKey;
                serverErrorCode = serverLicenseInfo.ServerErrorCode;
                serverMessage = String.IsNullOrEmpty(serverLicenseInfo.ErrorMessage) ? serverLicenseInfo.InfoMessage : serverLicenseInfo.ErrorMessage;


                bool needsActivation = false;
                string returnMsg = string.Empty;

                ret = ValidateLicense(activationKey, newComputerKey, ref computerID, ELicenseBinding.UserDefined, ref needsActivation, ref returnMsg);

                if (ret == true)
                {
                    // The Computer Key has changed, update the local one
                    license.StoreKeys(activationKey, newComputerKey);
                }
            }

            return ret;
        }

        /// <summary>
        /// Deletes the license keys stored on the computer. 
        /// </summary>
        public virtual void DeleteKeys()
        {
            license.DeleteKeys();
            this.computerKey = string.Empty;
        }

        protected virtual bool VerifyLicenseFile(string computerID, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool ret = false;

            
            ILicenseInfo li = license.VerifyActivatedLicenseFile(this.ActivationKey, this.ComputerKey, computerID, out errorMessage);

            if (li != null)
            {
                if (li.Status == ReturnStatus.Error)
                {
                    errorMessage = li.ErrorMessage;
                }
                else
                {
                    ret = true;
                }
            }


            return ret;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the registered activation key
        /// </summary>
        public string ActivationKey
        {
            get
            {
                return activationKey;
            }
        }

        /// <summary>
        /// Returns the registered computer key
        /// </summary>
        public string ComputerKey
        {
            get
            {
                return computerKey;
            }            
        }

        public bool IsEvaluation
        {
            get
            {
                return hasExpiryDate;
            }
        }

        public bool LicenseExpired
        {
            get
            {
                return licenseExpired;
            }
        }

        public int EvaluationRemainingDays
        {
            get
            {
                return licenseRemainingDays;
            }
        }

        /// <summary>
        /// Returns the underlying license object
        /// </summary>
        public QlmLicense QlmLicenseObject
        {
            get
            {
                return license;
            }
        }

        public bool WrongProductVersion
        {
            get
            {
                return wrongProductVersion;
            }

            set
            {
                wrongProductVersion = value;
            }
        }

        public string CustomData1
        {
            get
            {
                return customData1;
            }

            set
            {
                customData1 = value;
            }
        }

        public string CustomData2
        {
            get
            {
                return customData2;
            }

            set
            {
                customData2 = value;
            }
        }

        public string CustomData3
        {
            get
            {
                return customData3;
            }

            set
            {
                customData3 = value;
            }
        }

        public EServerErrorCode ServerErrorCode
        {
            get { return serverErrorCode; }
        }
        public ILicenseInfo ServerLicenseInfo
        {
            get { return serverLicenseInfo; }
        }
        public string ServerMessage
        {
            get { return serverMessage; }
        }
        #endregion

        #region Product Properties
        /// <summary>
        /// every time we activate a license, get the product properties from the server
        /// and write them to a local file
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public virtual bool WriteProductPropertiesFile(out string errorMessage)
        {
            bool ret = false;

            errorMessage = string.Empty;

            try
            {
                // store the license file - you may want to customize the destination folder
                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string licenseFile = Path.Combine(docsFolder, productPropertiesFileName);

                // WriteProductPropertiesFile contacts the server, gets the product properties
                // and writes them to a digitally signed xml file
                license.WriteProductPropertiesFile(this.ActivationKey, licenseFile, out errorMessage);
                
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return ret;

        }
        public virtual IQlmProductProperties ReadProductPropertiesFile(out string errorMessage)
        {
            errorMessage = string.Empty;

            IQlmProductProperties pps = null;

            try
            {
                // store the license file - you may want to customize the destination folder
                string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string licenseFile = Path.Combine(docsFolder, productPropertiesFileName);

                pps = license.ReadProductPropertiesFile(licenseFile, out errorMessage);

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return pps;
        }

        public virtual void PublishAnalyticsToServer(string computerID, string customData1, string customData2, string customData3)
        {
            if (license.PublishAnalytics)
            {
                try
                {
                    QlmAnalytics analytics = new QlmAnalytics(license);

                    string errorMessage;

                    string installID = analytics.ReadInstallID(out errorMessage);
                    

                    if (String.IsNullOrEmpty(installID))
                    {
                        bool ret = analytics.AddInstallEx(license.ApplicationVersion, analytics.GetOperatingSystem(),
                                                   Environment.MachineName, computerID,
                                                   activationKey, this.computerKey, license.IsEvaluation(),
                                                   license.ProductName, license.MajorVersion, license.MinorVersion,
                                                   customData1, customData2, customData3,
                                                   ref installID);

                        if (ret == true)
                        {
                            analytics.WriteInstallID(installID, out errorMessage);
                        }
                    }
                    else
                    {
                        analytics.UpdateInstallEx(installID,
                                                    license.ApplicationVersion, analytics.GetOperatingSystem(),
                                                    Environment.MachineName, computerID,
                                                    activationKey, this.computerKey, license.IsEvaluation(),
                                                    license.ProductName, license.MajorVersion, license.MinorVersion,
                                                    customData1, customData2, customData3
                                                    );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public virtual void UnpublishAnalyticsFromServer()
        {
            if (license.PublishAnalytics)
            {
                try
                {
                    QlmAnalytics analytics = new QlmAnalytics(license);

                    string errorMessage;

                    string installID = analytics.ReadInstallID(out errorMessage);

                    if (!String.IsNullOrEmpty(installID))
                    {
                        if (analytics.RemoveInstall(installID, out errorMessage) == true)
                        {
                        }
                    }
                }
                catch { }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Compares flags
        /// </summary>
        /// <param name="nVal1">Value 1</param>
        /// <param name="nVal2">Value 2</param>
        /// <returns></returns>
        private bool IsTrue(int nVal1, int nVal2)
        {
            if (((nVal1 & nVal2) == nVal1) || ((nVal1 & nVal2) == nVal2))
            {
                return true;
            }
            return false;
        }

        public static bool ServerCertificateValidator(object sender, X509Certificate certificate, X509Chain chain,
                                     SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion

    }
}
