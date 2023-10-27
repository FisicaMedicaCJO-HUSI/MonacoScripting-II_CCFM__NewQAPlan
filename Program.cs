/*************************************************************************************************************************************
 * This script NewQAPlan creates QA plans for all approved plans of a patient, in the Centro Javeriano de Oncología. 
 * 1.This code imports the necessary libraries and creates a class called Program. 
 * The class begins by defining some script variables, which are read from the App.config file and from a pop-up dialog box.
 * The variables used in this script are given via App.config as well as customizable pop-up dialog.
 * 2. First creates a new Monaco application instance and launches Monaco.
 * 3. Then, it loads the patient data and gets the plan list for that patient. 
 * 4. Next, it iterates over the plan list and creates a QA plan for each approved plan. 
 * 5. Once the QA plan has been created, the script calculates the plan and saves the patient. 
 * 6. Then, it exports the plan report and prints customized reports.
 * 7. Finally, it unloads the activated plan and closes the patient, and exit Monaco.
*
 *************************************************************************************************************************************/
// Imports the following libraries
using Elekta.MonacoScripting.API;
using Elekta.MonacoScripting.DataType;
using Elekta.MonacoScripting.API.DICOMExport;
using Elekta.MonacoScripting.Log;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace NewQAPlan
{
    class Program
    {
        private static string Installation = ConfigurationManager.AppSettings["Installation"];
        private static string Clinic = ConfigurationManager.AppSettings["Clinic"];
        private static string PatientID = ConfigurationManager.AppSettings["PatientID"];
        private static string PhantomName = ConfigurationManager.AppSettings["Phantom"];
        private static string PhantomImageName = ConfigurationManager.AppSettings["PhantomImageName"];
        private static string Iso = ConfigurationManager.AppSettings["QAPlanIso"];


        static void Main(string[] args)
        {

            try
            {
                //Step 1: Create a new Monaco application instance.
                MonacoApplication app = MonacoApplication.Instance; 
                //Step 2: launch Monaco
                app.LaunchMonaco(); 
                //Step 3: Open patient in Monaco
                app.GetPatientSelection().LoadPatient(Installation, Clinic, PatientID); 

                //Step 4: Get plan list of this Patient
                //This function returns a list of all the plans for the current patient.
                var PlanList = app.GetPlanList(); 
                //Step 5: Create QA plan for all the approved plans of this patient one by one
                foreach (var plan in PlanList)
                {
                    if (plan.Status == PlanStatus.Unapproved)
                        continue;
                    // Step 6: New QA plan on Phantom
                    // To create a QA plan without using phantom, app.GetNewQAPlanCreator(Plan).CreateQAPlan() can be used
                    // This method creates a new QA plan on the specified phantom with the specified ISO dose.
                    app.GetNewQAPlanCreator(plan.Name).CreateQAPlanOnPhantom(PhantomName, PhantomImageName, Iso);
                    // Step 7: Calculate QA plan
                    app.ExecuteCalculationForActiveRX(); 
                    // Step 8: Save patient
                    app.SavePatient(); 

                    // Step 9: export Plan report
                    app.ExportReport(ReportType.Plan); 
                    // Step 10: Print customized reports via printer xxx 
                    app.PrintCustomizedReports("QA Reports Template", "Printer 1"); 
                    #region Export the QA plan
                    // Step 11: Open Dicom Export dialog
                    DicomExport DCMexport = app.GetDicomExport(); 
                    // Step 12: Select modality to export
                    DCMexport.SelectDICOMExportModalities(ExportModality.TotalPlan); 
                    DCMexport.SelectDICOMExportModalities(ExportModality.TotalPlanDose);
                    // Step 13: Select export desination
                    DCMexport.ToggleDestination("Mosaiq", true); 

                    // Step 14: Click Export button. Log a warning if dicom export failed.
                    if (!DCMexport.ClickExport())
                    {
                        Logger.Instance.Warn("Dicom Export failed. Check if machine is mapped.");
                        DCMexport.ClickCancel();
                    } 

                    #endregion

                    //Step 15: Unload activated plan
                    app.UnLoadAll(MonacoApplication.SaveDlgOptions.Save);
                }
                // Step 16:Close Patient
                app.ClosePatient(); 
                // Step 17:Exit Monaco
                app.ExitMonaco(); 
            }
            //Step 16: Exception handling
            //This will display a message box with the exception message.
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }


    }
}
