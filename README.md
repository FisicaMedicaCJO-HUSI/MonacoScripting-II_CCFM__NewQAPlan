# MonacoScripting-II_CCFM__NewQAPlan
This script NewQAPlan creates QA plans for all approved plans of a patient, in the Centro Javeriano de Oncología. 
 * 1.This code imports the necessary libraries and creates a class called Program. 
 * The class begins by defining some script variables, which are read from the App.config file and from a pop-up dialog box.
 * The variables used in this script are given via App.config as well as customizable pop-up dialog.
 * 2. First creates a new Monaco application instance and launches Monaco.
 * 3. Then, it loads the patient data and gets the plan list for that patient. 
 * 4. Next, it iterates over the plan list and creates a QA plan for each approved plan. 
 * 5. Once the QA plan has been created, the script calculates the plan and saves the patient. 
 * 6. Then, it exports the plan report and prints customized reports.
 * 7. Finally, it unloads the activated plan and closes the patient, and exit Monaco.
