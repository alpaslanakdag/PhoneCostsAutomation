using PhoneCostApp;

var processExcel = new ExcelProcess();

string file = "c://sql/NFink.xls";

await processExcel.ReadFromSharePoint();
//processExcel.AccessFolder();

Dictionary<string, List<string>> rowsOfExcelFile = new Dictionary<string, List<string>>();
rowsOfExcelFile = processExcel.ReadExcelFile(file);

await processExcel.saveExcelData(rowsOfExcelFile,file); 
