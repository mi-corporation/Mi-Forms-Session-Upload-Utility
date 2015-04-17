// Map - Customize the form field data mappings here
// Kevin Burgess - 04/07/2015
// Mi-Corporation

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace MiCo.MiForms.Uploader
{
    public static class Map
    {
        /// <summary>
        /// This method handles form field mapping from a file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="xForm"></param>
        /// <returns></returns>
        internal static Form Files(String file, Form xForm)
        {
            // First we need to get properties from the file
            // The naming of the files expected: FILENAME_DATE_CUSTOMER_FACILITY.EXTENSION
            // We will split it into an array and store the values as variables
            var fileName = Path.GetFileNameWithoutExtension(file).Split('_');
            var name = fileName[0];
            var date = fileName[1];
            var customer = fileName[2];
            var facility = fileName[3];
            var extension = Path.GetExtension(file);

            // To get more information you can use QueryDatabase() here 
            // Pass in a table name, column name, and a customer
            // using a call like the following
            // var dt = GetData.QueryDatabase(name, "Customer", customer);

            // Or you can use a stored procedure that returns here as well
            // using a call like the following
            //var dt = GetData.QueryStoredProcedure("sp_your_stored_precedure_name", "@" + customer, "@" + facility);
            
            // set the form fields
            // customize this information to use form field names from your form template.
            xForm.GetFormControlByName("HIDDEN_FIELD_FILE_NAME").Value = name;
            xForm.GetFormControlByName("CTEXT_FIELD_DATE").Value = date;
            xForm.GetFormControlByName("CTEXT_FIELD_CUSTOMER").Value = customer;
            xForm.GetFormControlByName("PICKLIST_FIELD_FACILITY").Value = facility;
            xForm.GetFormControlByName("HIDDEN_FIELD_FILE_EXTENSION").Value = extension;

            // attach any files you want to the form
            // in this case we are attaching the file used to get values
            var att = xForm.CreateAttachmentData();
            att.Name = Path.GetFileName(file);
            att.SetBinaryDataFromFile(file);
            xForm.AddAttachment(att);

            return xForm;
        }

        /// <summary>
        /// This method handles mapping from a database query
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xForm"></param>
        /// <returns></returns>
        internal static Form Sql(DataRow data, Form xForm)
        {
            // the dataset can be any number of values. 
            // map them here
            var name = data[data.Table.Columns.IndexOf("name")].ToString();
            var date = data[data.Table.Columns.IndexOf("date")].ToString();
            var customer = data[data.Table.Columns.IndexOf("customer")].ToString();
            var facility = data[data.Table.Columns.IndexOf("facility")].ToString(); 

            // set the form fields
            // customize this information to use form field names from your form template.
            xForm.GetFormControlByName("HIDDEN_FIELD_FILE_NAME").Value = name;
            xForm.GetFormControlByName("CTEXT_FIELD_DATE").Value = date;
            xForm.GetFormControlByName("CTEXT_FIELD_CUSTOMER").Value = customer;
            xForm.GetFormControlByName("PICKLIST_FIELD_FACILITY").Value = facility;

            return xForm;
        }

    }
}
