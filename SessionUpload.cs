// SessionUpload - Manages form template download, session creation, and upload 
// Kevin Burgess - 04/07/2015
// Mi-Corporation

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiCo.MiForms;
using MiCo.MiForms.Server;
using System.Configuration;
using System.IO;
using log4net;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using PdfSharp.Pdf.IO;

namespace MiCo.MiForms.Uploader
{
    public class SessionUpload
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private WebserviceInterfaceEx _wsi = new WebserviceInterfaceEx();

        public SessionUpload()
        {            
            ProcessForms();        
        }

        /// <summary>
        /// Logic to process the forms using the data collected from the files in a folder
        /// </summary>
        /// <param name="collectedFiles">List of files collected from a folder</param>
        private void ProcessForms()
        {
            // authenticate with Mi-Forms server
            if (AuthenticateUser())
            {
                _log.Info("Authenticated " + _wsi.Credentials.User);
            }
            else
            {
                _log.Error("Failed to authenticate " + _wsi.Credentials.User);
                return;
            }

            // look for files in a folder
            if (Config.IsFileCollectionUsed)
            {
                var files = GetData.CollectFiles();
                foreach (var f in files)
                {
                    var xForm = DownloadTemplate();
                    xForm = Map.Files(f, xForm);
                    UploadForm(xForm);
                }
            }

            // look for data from the database
            if (Config.IsDbCollectionUsed)
            {
                var table = GetData.QueryDatabase(Config.Table, Config.Column, Config.Where);
                foreach (DataRow r in table.Rows)
                {
                    var xForm = DownloadTemplate();
                    xForm = Map.Sql(r, xForm);
                    UploadForm(xForm);
                }
            }

            // use a stored procedure to collect data
            if (Config.IsStoredProcedureCollectionUsed)
            {
                var table = GetData.QueryStoredProcedure(Config.StoredProcedureName, Config.StoredProcedureParameters, Config.StoredProcedureValues);
                foreach (DataRow r in table.Rows)
                {
                    var xForm = DownloadTemplate();
                    xForm = Map.Sql(r, xForm);
                    UploadForm(xForm);
                }
            }
        }

        /// <summary>
        /// Authenticates the user and server defined in the WebServiceInterfaceEx
        /// </summary>
        /// <returns>Returns a bool indicating whether the authentication was successful or not.</returns>
        private bool AuthenticateUser()
        {
            var err = "";
            ServerResponse sr;
            _log.Info("Attempting to authenticate server and user " + _wsi.Credentials.User);

            // add credentials and server settings
            _wsi.NetworkSettings.Server = Config.Server;
            _wsi.NetworkSettings.Port = Config.Port;
            _wsi.NetworkSettings.URLPrefix = Config.Prefix;
            _wsi.Credentials.CustomerName = Config.Customer;
            _wsi.Credentials.User = Config.Username;
            _wsi.Credentials.Password = Config.Password;

            // verify server
            sr = _wsi.VerifyServer();
            if (!sr.Success || !sr.BoolData)
            {
                if (sr.Error != null)
                {
                    if (sr.Error.GetType() == typeof(Server.Error.AuthenticationError))
                    {
                        var authErr = (Server.Error.AuthenticationError)sr.Error;
                        err = authErr.Exception.Message;
                    }
                    else
                    {
                        err = sr.Error.Exception.Message;
                    }
                }
                _log.Error("Could not verify the server. Check the server configuration. " + err);
                return false;
            }
            else
            {
                _log.Info("Server verified.");
            }

            // verify login
            sr = _wsi.VerifyLogin();
            if (sr.Success && sr.BoolData)
            {
                _log.Info("Successful login");
                return true;
            }
            if (sr.Error != null)
            {
                err = sr.Error.Exception.Message;
            }
            _log.Error("Could not verify the user's login credentials. " + err);
            return false;
        }

        /// <summary>
        /// Downloads the form template identified in the app config file.
        /// </summary>
        /// <returns>Returns the downloaded form.</returns>
        private Form DownloadTemplate()
        {
            ServerResponse sr;
            string err = string.Empty;
            // get a list of possible form templates
            sr = _wsi.GetFormTemplatesForUser();
            if (!sr.Success)
            {
                if (sr.Error != null)
                {
                    err = sr.Error.Exception.Message;
                }
                _log.Error("Error: could not get form templates. " + err);
                return null;
            }

            // find the correct form template based on app.config
            FormTemplateDescription[] formTemplates = sr.FormTemplates;
            FormTemplateDescription ccForm = null;
            string formId = Config.FormId;
            
            foreach (FormTemplateDescription form in formTemplates)
            {
                if (form.FormID.Equals(formId))
                {
                    ccForm = form;
                    _log.Info("Located Template: " + formId);
                }
            }

            if (ccForm == null)
            {
                _log.Error("Could not find template: " + formId);
                return null;
            }

            // get the actual form template XML
            Form xform = new Form();
            
            if (ccForm.FormID.Equals(formId))
            {
                sr = _wsi.GetFormTemplateRevision(ccForm.FormID, ccForm.Revision);
                if (!sr.Success)
                {
                    if (sr.Error != null)
                    {
                        err = sr.Error.Exception.Message;
                    }
                    _log.Error("Could not download the form. " + err);
                    return null;
                }
                string formXML = sr.StringData;

                // load XML into the form object
                Credentials xcreds = new Credentials(_wsi.Credentials.CustomerName, _wsi.Credentials.User, _wsi.Credentials.Password, "");
                xform.StartSession(xcreds);
                xform.Load(formXML);

                return xform;
            }
            _log.Error("Error: could not find form template with the Id " + formId);
            return null;
        }

        /// <summary>
        /// Uploads a form to the Mio-Forms server
        /// </summary>
        /// <param name="xForm">Form xForm</param>
        /// <returns>bool</returns>
        private bool UploadForm(Form xForm)
        {
            var strXML = string.Empty;
            var err = string.Empty;

            if (!xForm.BuildXMLString(ref strXML))
            {
                return false;
            }

            // attempt to upload session
            ServerResponse sr;
            sr = _wsi.UploadSession(strXML);
            if (!sr.Success)
            {
                if (sr.Error != null)
                {
                    err = sr.Error.Exception.Message;
                }
                _log.Error("Could not upload form to server. " + err);
                return false;
            }
            else
            {
                _log.Info("Uploaded session to server.");
            }

            string sessionID = sr.StringData;
            _log.Info("Form uploaded to server with session ID " + sessionID);

            // confirm uploaded session
            sr = _wsi.ConfirmUploadedSession(sessionID);
            if (!sr.Success || !sr.BoolData)
            {
                if (sr.Error != null)
                {
                    err = sr.Error.Exception.Message;
                }
                _log.Error("Could not confirm uploaded session. " + err);
                return false;
            }
            else
            {
                _log.Info("Confirmed uploaded session " + sessionID + ".");
            }

            return true;
        }        

        
    }
}
