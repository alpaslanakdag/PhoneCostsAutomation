using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using PhoneCostApp.Model;
using Aspose.Cells;
using Microsoft.SharePoint.Client;


using System.Net;
using System.Web;
using System.Security;

namespace PhoneCostApp
{
     class ExcelProcess
    {
        private  readonly PhoneCostContext db = new PhoneCostContext();

        //public static List<string> readFileName()
        //{
        //    string name = "N.Fink - Phone costs.xlsx";


        //    var listOfFiles = db.ExcelFiles.ToList();

        //    List<string> filePaths = new List<string>();

        //    foreach (ExcelFile item in listOfFiles)
        //    {
        //        try
        //        {
        //            filePaths.Add(item.FilePath);

        //        }
        //        catch (Exception e)
        //        {

                    
        //        }
        //    }

        //    return filePaths;
        //}

        public   Dictionary<string, List<string>> ReadExcelFile(string file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string convertedExcelFile = "";
            Dictionary<string, List<string>> rowsOfExcelFile = new Dictionary<string, List<string>>();
            
                    //string path = item; 
                    FileInfo fileInfo = new FileInfo(file);
            try
            {
                


                    if (fileInfo.Extension == ".xls")
                    {
                        convertedExcelFile = ConvertXLS_XLSX(file);

                    }
                    else
                    {
                        convertedExcelFile = file;
                    }
                    ExcelPackage package = new ExcelPackage(new FileInfo(convertedExcelFile));
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rows = worksheet.Dimension.Rows; // 20
                    int columns = worksheet.Dimension.Columns; // 7

                    for (int i = 2; i <= rows; i++)
                    {
                        List<string> valuesOfRow = new List<string>();
                        for (int j = 1; j <= columns; j++)
                        {

                            string content = (worksheet.Cells[i, j].Value ?? string.Empty).ToString();
                            /* Do something ...*/
                            valuesOfRow.Add(content);

                        }
                        rowsOfExcelFile.Add("Row" + (i).ToString(), valuesOfRow);
                    }

                    //listOfFilesData.Add(item, rowsOfExcelFile);
                    //Console.ReadLine();

                
                
                    return rowsOfExcelFile;
            }

            catch (Exception e)
            {
                LogEntry log = new LogEntry();
                log.FileName = file;
                log.Comment = ExceptionMessages(e);
                log.CreatedDate = DateTime.Now;
                db.LogEntries.Add(log);
                db.SaveChanges();

                throw;

            }
            
            
           
        }

        public  string ConvertXLS_XLSX(string file)
        {
            
            var book = new Aspose.Cells.Workbook(file);
            // save XLS as XLSX
            book.Save(file+"x", Aspose.Cells.SaveFormat.Auto);
            var xlsxFile = book.FileName;
            return xlsxFile;
        }

        public  async Task saveExcelData( Dictionary<string, List<string>> excelData, string fileName)
        {
            
            
            PhoneCost phoneCost = new PhoneCost();
            Company company = new Company();
            Employee employee = new Employee();
            Department department = new Department();
            var existingEmployees = db.Employees.ToList();
            var existingDepartments = db.Departments.ToList();
            var existingCompanies = db.Companies.ToList();
            var existingEntries = db.PhoneCosts.ToList();
            bool isNumeric;

            foreach (var data in excelData)
            {
                try
                {
                    phoneCost = existingEntries.Where(u => u.ReferencePeriod == data.Value[21] && u.Employee.EmployeeName == data.Value[1] && u.Total == Convert.ToDecimal(data.Value[6])).SingleOrDefault();
                    if (phoneCost == null)
                    {

                    
                        employee = existingEmployees.Where(u => u.EmployeeName == data.Value[1] && u.PhoneNumber == Convert.ToInt32(data.Value[2])).SingleOrDefault();
                        if (employee == null)
                        {
                            employee = new Employee();
                            employee.EmployeeName = data.Value[1];
                            isNumeric = int.TryParse(data.Value[2], out int n);
                            if (isNumeric)
                            {
                            employee.PhoneNumber = Convert.ToInt32(data.Value[2]);

                            }
                            else
                            {
                                employee.PhoneNumber = 0;   
                            }
                            db.Employees.Add(employee);
                            existingEmployees.Add(employee);
                        }


                        department = existingDepartments.Where(u => u.Org1 == data.Value[4]).SingleOrDefault();
                        if (department == null)
                        {
                            department = new Department();
                            department.Org1 = data.Value[4];
                            department.ParentId = 0;
                            db.Departments.Add(department);
                            db.SaveChanges();
                            existingDepartments.Add(department);

                            //db.Departments.Add(new Department { 
                            //    Org1= data.Value[5],
                            //    ParentId = db.Departments.Where(u=> u.Org1==data.Value[4]).Single().Id 
                            //});
                        }
                        else
                        {
                            department = null;
                        }

                        department = existingDepartments.Where(u => u.Org1 == data.Value[5]).SingleOrDefault();
                        if (department == null)
                        {
                            department = new Department();
                            department.Org1 = data.Value[5];
                            department.ParentId = existingDepartments.Where(u => u.Org1 == data.Value[4]).FirstOrDefault().Id;
                            existingDepartments.Add(department);
                        }

                        company = existingCompanies.Where(u => u.Name == data.Value[3]).SingleOrDefault();
                        if (company == null)
                        {
                            company = new Company();
                            company.Name = data.Value[3];
                            db.Companies.Add(company);
                            existingCompanies.Add(company);
                        }

                        phoneCost = new PhoneCost();
                        phoneCost.CustomerCostCenter = data.Value[0];
                        phoneCost.Total = Convert.ToDecimal(data.Value[6]);
                        phoneCost.MobileConnection = Convert.ToDecimal(data.Value[14]);

                        isNumeric = int.TryParse(data.Value[14], out int a);
                        if (isNumeric)
                        {
                             phoneCost.MobileCalls = Convert.ToDecimal(data.Value[15]);


                        }
                        else
                        {
                            phoneCost.MobileCalls = 0;
                        }

                        phoneCost.Debtor = data.Value[19];
                        phoneCost.Date = DateTime.ParseExact(data.Value[20], "yyyyMMdd", null);
                        phoneCost.ReferencePeriod = data.Value[21];
                        phoneCost.CreatedDate = DateTime.Now.Date;
                        phoneCost.Employee = employee;
                        phoneCost.Company = company;
                        phoneCost.Department = department;

                        existingEntries.Add(phoneCost);


                        db.PhoneCosts.Add(phoneCost);


                        //model.CustomerCostCenter = data.Value[0];
                        //model.Name = data.Value[1];
                        //model.PhoneNumber = data.Value[2];
                        //model.Company = data.Value[3];
                        //model.Org1 = data.Value[4];
                        //model.Org2 = data.Value[5];
                        //model.Total = Convert.ToDecimal(data.Value[6]);
                        //model.FixConnection = Convert.ToDecimal(data.Value[7]);
                        //model.Facilities = Convert.ToDecimal(data.Value[8]);
                        //model.Exchange = Convert.ToDecimal(data.Value[9]);
                        //model.MaintenanceDevice = Convert.ToDecimal(data.Value[10]);
                        //model.FixedCallCarrier1 = Convert.ToDecimal(data.Value[11]);
                        //model.FixedCallCarrier2 = Convert.ToDecimal(data.Value[12]);
                        //model.PrivateCallsFixedNetwork = Convert.ToDecimal(data.Value[13]);
                        //model.MobileConnection = Convert.ToDecimal(data.Value[14]);
                        //model.MobileCalls = Convert.ToDecimal(data.Value[15]);
                        //model.PhoneBook = Convert.ToDecimal(data.Value[16]);
                        //model.Flexnet = Convert.ToDecimal(data.Value[17]);
                        //model.CellPhoneHardware = Convert.ToDecimal(data.Value[18]);
                        //model.Debtor = data.Value[19];
                        //model.Date = Convert.ToDateTime(data.Value[20]);
                        //model.ReferencePeriod = data.Value[21];
                        //model.CreatedDate = DateTime.Now.Date;

                        //db.LocalApplicationsNinaFinks.Add(model);


                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    LogEntry logEntry = new LogEntry();
                    logEntry.FileName = fileName;
                    logEntry.Comment = data.Key +" -> " + ExceptionMessages(e);
                    logEntry.CreatedDate = DateTime.Now;
                    db.LogEntries.Add(logEntry);
                     
                }

            }

            Console.WriteLine("Process done succesfully");

        }

        public  string ExceptionMessages(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }

            return ex.Message + " -> " + ExceptionMessages(ex.InnerException);

        }

        public async Task  AccessFolder()
        {
            Uri path = new Uri( "https://abb.sharepoint.com/:f:/r/teams/FS-ISLocalApplications-PBIReports/Shared%20Documents/Data%20Source/Phone%20Costs?csf=1&web=1&e=HzuWU7");
            //string relativePath = "/:f:/r/teams/FS-ISLocalApplications-PBIReports/Shared Documents/Data Source/Phone Costs";


            string password = "Kalkmak04;";
            string userName = "alpaslan.akdag@ch.abb.com";
           
            ClientContext ctx = new ClientContext(path);
            SecureString securePassword = new SecureString();

            foreach (var item in password.ToCharArray())
            {
                securePassword.AppendChar(item);
            }

            using (var authenticationManager = new AuthenticationManager())
            using (var context = authenticationManager.GetContext(path, userName, securePassword))
            {
                context.Load(context.Web, p => p.Title);
                await context.ExecuteQueryAsync();
                Console.WriteLine($"Title: {context.Web.Title}");
            }
            //  ctx.Credentials = new NetworkCredential("alpaslan.akdag@ch.abb.com","Kalkmak04;");

            // var credentials = new SharePointOnlineCredentials(userName,securePassword);


            //FileCollection file = ctx.Web.GetFolderByServerRelativeUrl(path).Files;
            //ctx.Credentials = new SharePointOnlineCredentials(userName, password);

            Web web = ctx.Web;
            //ctx.Load(file);
            ctx.ExecuteQuery();

            //foreach (var item in file)
            //{
            //    Console.WriteLine(item.Title);
            //}
        }

        public  async Task ReadFromSharePoint()
        {
            Uri site = new Uri("https://abb.sharepoint.com");
            string user = "alpaslan.akdag@ch.abb.com";
            string pas = "Kalkmak04;";
            SecureString password = new SecureString();
            foreach (var item in pas.ToCharArray())
            {
                password.AppendChar(item);
            }

            // Note: The PnP Sites Core AuthenticationManager class also supports this
            using (var authenticationManager = new AuthenticationManager())
            using (var context = authenticationManager.GetContext(site, user, password))
            {
                context.Load(context.Web, p => p.Title);
                await context.ExecuteQueryAsync();
                Console.WriteLine($"Title: {context.Web.Title}");
            }
        }

        public ClientContext GetContext(Uri web, string userPrincipalName, SecureString userPassword)
        {
            AuthenticationManager authenticationManager = new AuthenticationManager();
            var context = new ClientContext(web);
            context.ExecutingWebRequest += (sender, e) =>
            {
                // Get an access token using your preferred approach
                string accessToken = authenticationManager.EnsureAccessTokenAsync(new Uri($"{web.Scheme}://{web.DnsSafeHost}"), userPrincipalName, new System.Net.NetworkCredential(string.Empty, userPassword).Password).GetAwaiter().GetResult();
                // Insert the access token in the request
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken;
            };

            return context;
        }


    }

    }

