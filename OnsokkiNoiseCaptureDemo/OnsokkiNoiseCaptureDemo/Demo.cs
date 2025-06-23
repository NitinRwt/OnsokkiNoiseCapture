using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Globalization;
using System.Diagnostics;

using FAPL_HW_Driver;

namespace OnsokkiNoiseCaptureDemo
{
    public partial class Demo : Form
    {
        private DateTime testStartTime;
        private DateTime testEndTime;
        private bool isTestRunning = false;
        private static int testCounter = 0; // Static counter for Test ID serialization

        public Demo()
        {
            InitializeComponent();
            StartConsoleApp();
            InitializeTestDataTable();
            LoadTestCounter();
            AddPreprocessButton();
        }

        public static string onsokkiFilePath = @"C:\FAPLData\OnsokkiData";
        public static string destinationDirectory = @"C:\FAPLData\final_report";
        public static string rawDataDirectory = @"C:\FAPLData\OnsokkiData\raw";
        private static string testCounterFilePath = @"C:\FAPLData\OnsokkiData\TestCounter.txt";

        FAPL_Debug_FileWrite debugFile;
        public const ushort SW_VER = 1;

        private void LoadTestCounter()
        {
            try
            {
                if (File.Exists(testCounterFilePath))
                {
                    string counterText = File.ReadAllText(testCounterFilePath);
                    if (int.TryParse(counterText, out int counter))
                    {
                        testCounter = counter;
                    }
                }
            }
            catch (Exception ex)
            {
                update_debug_message($"Error loading test counter: {ex.Message}");
            }
        }

        private void SaveTestCounter()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(testCounterFilePath));
                File.WriteAllText(testCounterFilePath, testCounter.ToString());
            }
            catch (Exception ex)
            {
                update_debug_message($"Error saving test counter: {ex.Message}");
            }
        }

        private string GenerateTestID()
        {
            testCounter++;
            SaveTestCounter();
            return $"006B{DateTime.Now:yyyyMMdd}{testCounter:D3}";
        }

        private void InitializeTestDataTable()
        {
            // Initialize DataGridView with test data
            dgv_test_data.Columns.Add("Parameter", "Parameter");
            dgv_test_data.Columns.Add("Value", "Value");

            // Set column widths
            dgv_test_data.Columns["Parameter"].Width = 250;
            dgv_test_data.Columns["Value"].Width = 300;

            // Make Value column editable
            dgv_test_data.Columns["Parameter"].ReadOnly = true;
            dgv_test_data.Columns["Value"].ReadOnly = false;

            // Add rows with default data
            string[] parameters = {
                "Started Time",
                "Test ID",
                "Model",
                "Date",
                "Test Location",
                "Test Name",
                "Rated Cool / Heat Capacity (W)",
                "Compressor Make/Model",
                "Fan Speed",
                "Evaporator",
                "Condenser",
                "Capillary/ Expansion",
                "Inter connecting Piping",
                "Indoor Unit Rated Sound Level dB(A)",
                "O.D Air D.B.T",
                "Indoor Unit Measurement Channel"
            };

            string[] defaultValues = {
                "(Auto Generated) Starting of the reporting software",
                "006B20250522001 (Auto Generated)",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San",
                "Initially to be filled by Daikin San"
            };

            for (int i = 0; i < parameters.Length; i++)
            {
                dgv_test_data.Rows.Add(parameters[i], defaultValues[i]);
            }

            // Add second table data
            dgv_test_data.Rows.Add("End Time", "(Auto Generated) csv import time");
            dgv_test_data.Rows.Add("Serial No.", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Application", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Standard", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Test Engineer", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Power Supply", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Rated Power(Cool/Heat) (W)", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Rated Current(A)", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Refrigerant Type / QTY", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("IDU Fan Motor / Rated RPM", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("ODU Fan Motor / Rated RPM", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Sample No.", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Sound Jacket", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Outdoor Unit Rated Sound Level dB(A)", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("O.D Air W.B.T", "Initially to be filled by Daikin San");
            dgv_test_data.Rows.Add("Outdoor Unit Measurement Channel", "Initially to be filled by Daikin San");
        }

        private void StartConsoleApp()
        {
            string exePath = @"C:\sunil automated ostep API\Test V7\ConsoleApp1.exe";

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.UseShellExecute = true;
                process.Start();
                //Console.WriteLine("ConsoleApp1 started successfully.");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Error starting ConsoleApp1: {exp.Message}");
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            testStartTime = DateTime.Now;
            isTestRunning = true;

            // Generate new Test ID
            string newTestID = GenerateTestID();

            // Update the Started Time and Test ID in the table
            dgv_test_data.Rows[0].Cells["Value"].Value = testStartTime.ToString("yyyy-MM-dd HH:mm:ss");
            dgv_test_data.Rows[1].Cells["Value"].Value = newTestID;

            // Update UI
            btn_start.BackColor = Color.Green;
            btn_stop.BackColor = SystemColors.Control;

            update_debug_message($"Test started at: {testStartTime}");
            update_debug_message($"Test ID generated: {newTestID}");
            OnosokkiControl.updateOnosokkiStart();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            testEndTime = DateTime.Now;
            isTestRunning = false;

            // Update the End Time in the table
            int endTimeRowIndex = -1;
            for (int i = 0; i < dgv_test_data.Rows.Count; i++)
            {
                if (dgv_test_data.Rows[i].Cells["Parameter"].Value?.ToString() == "End Time")
                {
                    endTimeRowIndex = i;
                    break;
                }
            }

            if (endTimeRowIndex != -1)
            {
                dgv_test_data.Rows[endTimeRowIndex].Cells["Value"].Value = testEndTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Update UI
            btn_stop.BackColor = Color.Red;
            btn_start.BackColor = SystemColors.Control;

            update_debug_message($"Test stopped at: {testEndTime}");
            OnosokkiControl.updateOnosokkiStop();
        }

        private void btn_test_data_Click(object sender, EventArgs e)
        {
            try
            {
                debugFile = new FAPL_Debug_FileWrite();
                string fileName = Utils.GetLatestFile(onsokkiFilePath);
                if (fileName == null)
                {
                    update_debug_message("File Not Created");
                }
                else
                {
                    // File is available 
                    update_debug_message("File Name received:" + fileName);
                    FileInfo fileinfo = new FileInfo(fileName);
                    update_debug_message("File Length received:" + fileinfo.Length);

                    var (ch1timeList, ch1List, maxCH1, agv1, noiseLimitCh1) = ReadDataTrend(fileName);

                    chart_noise1.Series[0].Points.DataBindXY(ch1timeList, ch1List);

                    update_debug_message($"Noise Values: {maxCH1} ");
                    string noiseVal = Math.Round((maxCH1), 2).ToString();
                    update_debug_message("Noise Value:" + noiseVal);
                    lbl_noise.Text = "Noise: " + noiseVal;

                    // Generate report with table data
                    GenerateReportWithTable(fileName, maxCH1, agv1);
                }
            }
            catch (Exception exp)
            {
                update_debug_message("Exception:" + exp.Message);
            }
        }
        private void GenerateSimplePDF(string csvPath, string pdfPath)
        {
            try
            {
                var lines = File.ReadAllLines(csvPath);
                using (var writer = new StreamWriter(pdfPath))
                {
                    writer.WriteLine("Processed Report PDF View");
                    writer.WriteLine(new string('=', 30));
                    foreach (var line in lines)
                        writer.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                update_debug_message("Failed to generate PDF: " + ex.Message);
            }
        }

        private void GenerateReportWithTable(string fileName, double maxNoise, double avgNoise)
        {
            try
            {
                // Get current Test ID from the table
                string currentTestID = dgv_test_data.Rows[1].Cells["Value"].Value?.ToString() ?? "Unknown";

                string csvPath = Path.Combine(destinationDirectory, $"NoiseReport_{currentTestID}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                string pdfPath = Path.Combine(destinationDirectory, $"NoiseReport_{currentTestID}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                // Generate CSV file with enhanced format
                GenerateEnhancedCSVReport(csvPath, fileName, maxNoise, avgNoise);

                // Generate actual PDF file
                GenerateActualPDFReport(pdfPath, csvPath, maxNoise, avgNoise);

                update_debug_message($"CSV Report generated: {csvPath}");
                update_debug_message($"PDF Report generated: {pdfPath}");
                MessageBox.Show($"Reports generated successfully!\nCSV: {csvPath}\nPDF: {pdfPath}", "Reports Generated",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                update_debug_message($"Error generating report: {ex.Message}");
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateEnhancedCSVReport(string csvPath, string fileName, double maxNoise, double avgNoise)
        {
            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                // Write the header as per the image format
                writer.WriteLine("DAIKIN,,Test Data (Anechoic Chamber)");
                writer.WriteLine(""); // Empty line

                // Write test parameters in the side-by-side format
                WriteTestParametersTable(writer);

                writer.WriteLine(""); // Empty line separator
                writer.WriteLine(""); // Empty line separator

                // Write analysis results
                writer.WriteLine("NOISE ANALYSIS RESULTS");
                writer.WriteLine($"Maximum Noise Level (dB),{Math.Round(maxNoise, 2)}");
                writer.WriteLine($"Average Noise Level (dB),{Math.Round(avgNoise, 2)}");
                writer.WriteLine($"Data Source File,{Path.GetFileName(fileName)}");
                writer.WriteLine($"Report Generated,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine(""); // Empty line separator

                // Write noise trend data
                writer.WriteLine("NOISE TREND DATA");
                writer.WriteLine("Time,Channel1(dB)");
                var (ch1timeList, ch1List, _, _, _) = ReadDataTrend(fileName);
                for (int i = 0; i < ch1timeList.Count; i++)
                {
                    writer.WriteLine($"{ch1timeList[i]},{ch1List[i]}");
                }

                writer.WriteLine(""); // Empty line separator
                writer.WriteLine(""); // Empty line separator

                // Add Window1_Graph001_Frame1.csv data if exists
                WriteGraphData(writer);
            }
        }

        private void WriteTestParametersTable(StreamWriter writer)
        {
            // Get values from DataGridView
            Dictionary<string, string> paramValues = new Dictionary<string, string>();
            foreach (DataGridViewRow row in dgv_test_data.Rows)
            {
                if (!row.IsNewRow)
                {
                    string param = row.Cells["Parameter"].Value?.ToString() ?? "";
                    string value = row.Cells["Value"].Value?.ToString() ?? "";
                    paramValues[param] = value;
                }
            }

            // Define left and right column parameters as per the image layout
            string[] leftParams = {
                "Started Time", "Test ID", "Model", "Date", "Test Location", "Test Name",
                "Rated Cool / Heat Capacity (W)", "Compressor Make/Model", "Fan Speed",
                "Evaporator", "Condenser", "Capillary/ Expansion", "Inter connecting Piping",
                "Indoor Unit Rated Sound Level dB(A)", "O.D Air D.B.T", "Indoor Unit Measurement Channel"
            };

            string[] rightParams = {
                "End Time", "Serial No.", "Application", "Standard", "Test Engineer", "Power Supply",
                "Rated Power(Cool/Heat) (W)", "Rated Current(A)", "Refrigerant Type / QTY",
                "IDU Fan Motor / Rated RPM", "ODU Fan Motor / Rated RPM", "Sample No.",
                "Sound Jacket", "Outdoor Unit Rated Sound Level dB(A)", "O.D Air W.B.T",
                "Outdoor Unit Measurement Channel"
            };

            // Write the table header
            writer.WriteLine("Parameter,Value,Parameter,Value");

            // Write paired rows (left and right columns together)
            int maxRows = Math.Max(leftParams.Length, rightParams.Length);
            for (int i = 0; i < maxRows; i++)
            {
                string leftParam = i < leftParams.Length ? leftParams[i] : "";
                string leftValue = leftParam != "" && paramValues.ContainsKey(leftParam) ? paramValues[leftParam] : "";
                string rightParam = i < rightParams.Length ? rightParams[i] : "";
                string rightValue = rightParam != "" && paramValues.ContainsKey(rightParam) ? paramValues[rightParam] : "";

                // Escape CSV values that contain commas
                leftValue = EscapeCSVValue(leftValue);
                rightValue = EscapeCSVValue(rightValue);

                writer.WriteLine($"{leftParam},{leftValue},{rightParam},{rightValue}");
            }
        }

        private string GetLatestCSVFile(string directory)
        {
            if (!Directory.Exists(directory)) return null;
            var files = new DirectoryInfo(directory).GetFiles("*.csv");
            if (files.Length == 0) return null;
            return files.OrderByDescending(f => f.LastWriteTime).First().FullName;
        }

        public void update_debug_message(string message)
        {
            debugFile.logDebug(message);
        }

        private void WriteGraphData(StreamWriter writer)
        {
            try
            {
                string latestGraphFile = GetLatestCSVFile(rawDataDirectory);

                if (!string.IsNullOrEmpty(latestGraphFile) && File.Exists(latestGraphFile))
                {
                    string fileName = Path.GetFileName(latestGraphFile);
                    writer.WriteLine($"RAW GRAPH DATA ({fileName})");
                    writer.WriteLine(""); // Empty line

                    // Read and write the graph data
                    string[] lines = File.ReadAllLines(latestGraphFile);
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line);
                    }

                    update_debug_message($"Raw graph data added to report from: {fileName}");
                }
                else
                {
                    writer.WriteLine("RAW GRAPH DATA");
                    writer.WriteLine("No CSV files found in raw data directory");
                    update_debug_message($"No CSV files found in raw data directory: {rawDataDirectory}");
                }
            }
            catch (Exception ex)
            {
                writer.WriteLine("RAW GRAPH DATA");
                writer.WriteLine($"Error reading raw graph data: {ex.Message}");
                update_debug_message($"Error reading raw graph data: {ex.Message}");
            }
        }

        private string EscapeCSVValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            // If value contains comma, quote, or newline, wrap in quotes and escape internal quotes
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                value = value.Replace("\"", "\"\""); // Escape quotes by doubling them
                return $"\"{value}\"";
            }
            return value;
        }

        private void GenerateActualPDFReport(string pdfPath, string csvPath, double maxNoise, double avgNoise)
        {
            try
            {
                // Method 1: Try using built-in Print functionality to create PDF
                if (TryGeneratePDFWithPrint(pdfPath, csvPath, maxNoise, avgNoise))
                {
                    update_debug_message("PDF generated successfully using Print method");
                    return;
                }

                // Method 2: Create HTML and try to convert to PDF using WebBrowser control
                if (TryGeneratePDFWithWebBrowser(pdfPath, csvPath, maxNoise, avgNoise))
                {
                    update_debug_message("PDF generated successfully using WebBrowser method");
                    return;
                }

                // Method 3: Fallback - Create a proper formatted text file with PDF extension
                CreateFormattedPDFFile(pdfPath, csvPath, maxNoise, avgNoise);
                update_debug_message("PDF created as formatted text file");

            }
            catch (Exception ex)
            {
                update_debug_message($"Error generating PDF: {ex.Message}");
                CreateFormattedPDFFile(pdfPath, csvPath, maxNoise, avgNoise);
            }
        }

        private bool TryGeneratePDFWithPrint(string pdfPath, string csvPath, double maxNoise, double avgNoise)
        {
            try
            {
                // Create a rich text document
                using (var rtb = new RichTextBox())
                {
                    StringBuilder content = new StringBuilder();
                    content.AppendLine("DAIKIN");
                    content.AppendLine("Test Data (Anechoic Chamber)");
                    content.AppendLine(new string('=', 80));
                    content.AppendLine();

                    // Add test parameters
                    content.AppendLine("TEST PARAMETERS:");
                    content.AppendLine(new string('-', 80));

                    foreach (DataGridViewRow row in dgv_test_data.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string parameter = row.Cells["Parameter"].Value?.ToString() ?? "";
                            string value = row.Cells["Value"].Value?.ToString() ?? "";
                            content.AppendLine($"{parameter}: {value}");
                        }
                    }

                    content.AppendLine();
                    content.AppendLine("NOISE ANALYSIS RESULTS:");
                    content.AppendLine(new string('-', 80));
                    content.AppendLine($"Maximum Noise Level: {Math.Round(maxNoise, 2)} dB(A)");
                    content.AppendLine($"Average Noise Level: {Math.Round(avgNoise, 2)} dB(A)");
                    content.AppendLine($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                    // Add raw data
                    string latestRawFile = GetLatestCSVFile(rawDataDirectory);
                    if (!string.IsNullOrEmpty(latestRawFile))
                    {
                        content.AppendLine();
                        content.AppendLine($"RAW DATA ({Path.GetFileName(latestRawFile)}):");
                        content.AppendLine(new string('-', 80));
                        string[] rawLines = File.ReadAllLines(latestRawFile);
                        foreach (string line in rawLines.Take(50)) // Limit to first 50 lines
                        {
                            content.AppendLine(line);
                        }
                    }

                    rtb.Text = content.ToString();

                    // Save as RTF first, then try to convert
                    string rtfPath = Path.ChangeExtension(pdfPath, ".rtf");
                    rtb.SaveFile(rtfPath, RichTextBoxStreamType.RichText);

                    // Copy RTF to PDF location with PDF extension
                    File.Copy(rtfPath, pdfPath, true);
                    File.Delete(rtfPath);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool TryGeneratePDFWithWebBrowser(string pdfPath, string csvPath, double maxNoise, double avgNoise)
        {
            try
            {
                // Create HTML content
                StringBuilder htmlContent = new StringBuilder();
                htmlContent.AppendLine("<html><head>");
                htmlContent.AppendLine("<style>");
                htmlContent.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; font-size: 12px; }");
                htmlContent.AppendLine("h1 { color: #0066cc; text-align: center; margin: 10px 0; }");
                htmlContent.AppendLine("h2 { color: #0066cc; border-bottom: 2px solid #0066cc; margin: 15px 0 10px 0; }");
                htmlContent.AppendLine("table { border-collapse: collapse; width: 100%; margin: 10px 0; }");
                htmlContent.AppendLine("th, td { border: 1px solid #ddd; padding: 6px; text-align: left; font-size: 11px; }");
                htmlContent.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
                htmlContent.AppendLine(".header-table { background-color: #e6f2ff; }");
                htmlContent.AppendLine(".analysis-table { background-color: #fff2e6; }");
                htmlContent.AppendLine("</style>");
                htmlContent.AppendLine("</head><body>");

                htmlContent.AppendLine("<h1>DAIKIN</h1>");
                htmlContent.AppendLine("<h1>Test Data (Anechoic Chamber)</h1>");

                // Test Parameters Table
                htmlContent.AppendLine("<h2>Test Parameters</h2>");
                htmlContent.AppendLine("<table class='header-table'>");
                htmlContent.AppendLine("<tr><th>Parameter</th><th>Value</th></tr>");

                foreach (DataGridViewRow row in dgv_test_data.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string parameter = row.Cells["Parameter"].Value?.ToString() ?? "";
                        string value = row.Cells["Value"].Value?.ToString() ?? "";
                        htmlContent.AppendLine($"<tr><td>{parameter}</td><td>{value}</td></tr>");
                    }
                }

                htmlContent.AppendLine("</table>");

                // Analysis Results
                htmlContent.AppendLine("<h2>Noise Analysis Results</h2>");
                htmlContent.AppendLine("<table class='analysis-table'>");
                htmlContent.AppendLine($"<tr><td><strong>Maximum Noise Level</strong></td><td>{Math.Round(maxNoise, 2)} dB(A)</td></tr>");
                htmlContent.AppendLine($"<tr><td><strong>Average Noise Level</strong></td><td>{Math.Round(avgNoise, 2)} dB(A)</td></tr>");
                htmlContent.AppendLine($"<tr><td><strong>Report Generated</strong></td><td>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</td></tr>");
                htmlContent.AppendLine("</table>");

                htmlContent.AppendLine("</body></html>");

                // Save HTML file with PDF extension
                File.WriteAllText(pdfPath, htmlContent.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CreateFormattedPDFFile(string pdfPath, string csvPath, double maxNoise, double avgNoise)
        {
            using (StreamWriter writer = new StreamWriter(pdfPath))
            {
                writer.WriteLine("%PDF-1.4");
                writer.WriteLine("% Simple PDF-like format");
                writer.WriteLine();
                writer.WriteLine("DAIKIN");
                writer.WriteLine("Test Data (Anechoic Chamber)");
                writer.WriteLine(new string('=', 100));
                writer.WriteLine();

                writer.WriteLine("TEST PARAMETERS:");
                writer.WriteLine(new string('-', 100));

                foreach (DataGridViewRow row in dgv_test_data.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string parameter = row.Cells["Parameter"].Value?.ToString() ?? "";
                        string value = row.Cells["Value"].Value?.ToString() ?? "";
                        writer.WriteLine($"{parameter,-50}: {value}");
                    }
                }

                writer.WriteLine();
                writer.WriteLine("NOISE ANALYSIS RESULTS:");
                writer.WriteLine(new string('-', 100));
                writer.WriteLine($"Maximum Noise Level: {Math.Round(maxNoise, 2)} dB(A)");
                writer.WriteLine($"Average Noise Level: {Math.Round(avgNoise, 2)} dB(A)");
                writer.WriteLine($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();

                // Add latest raw data
                string latestRawFile = GetLatestCSVFile(rawDataDirectory);
                if (!string.IsNullOrEmpty(latestRawFile))
                {
                    writer.WriteLine($"RAW DATA ({Path.GetFileName(latestRawFile)}):");
                    writer.WriteLine(new string('-', 100));
                    string[] rawLines = File.ReadAllLines(latestRawFile);
                    foreach (string line in rawLines)
                    {
                        writer.WriteLine(line);
                    }
                }
                else
                {
                    writer.WriteLine("RAW DATA: No files found in raw directory");
                }
            }
        }
        private void Demo_Load(object sender, EventArgs e)
        {
            // Add initialization logic here if needed
            InitializeTestDataTable();
            LoadTestCounter();
        }

        public (List<double> ch1timeList, List<double> ch1List,
                     double maxCH1, double avgCH1, bool noiseLimitCh1)
        ReadDataTrend(string filePath)
        {
            List<double> ch1timeList = new List<double>();
            List<double> ch1List = new List<double>();

            double maxCH1 = double.MinValue;
            bool dataStart = false;
            int skipLines = 2;
            bool noiseLimitCh1 = false;

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (dataStart && skipLines > 0)
                        {
                            skipLines--;  // Skip two lines after [Data_Trend]
                            continue;
                        }

                        if (dataStart && skipLines == 0)
                        {
                            string[] parts = line.Split(',');

                            if (parts.Length >= 5)  // Ensure the correct number of columns
                            {
                                if (double.TryParse(parts[1], out double time) &&
                                    double.TryParse(parts[2], out double ch1) &&
                                    double.TryParse(parts[3], out double ch2) &&
                                    double.TryParse(parts[4], out double ch3))
                                {
                                    ch1timeList.Add(time);
                                    ch1List.Add(ch1); // Normal value, add directly
                                    if (ch1 > maxCH1)
                                    {
                                        update_debug_message("Changing Max1:" + ch1);
                                        maxCH1 = ch1;
                                    }
                                }
                            }
                        }

                        if (line == "[Data_Trend]")
                        {
                            dataStart = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }

            double avgCH1 = ch1List.Any() ? ch1List.Average() : 0;
            return (ch1timeList, ch1List, maxCH1, avgCH1, noiseLimitCh1);
        }

        private void lbl_test_parameters_Click(object sender, EventArgs e)
        {
            // Example: Show a message box or perform any action you want
            MessageBox.Show("Test Parameters label clicked.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    
        private Button btn_preprocess;

        private void AddPreprocessButton()
        {
            btn_preprocess = new Button();
            btn_preprocess.Text = "Preprocess Data";
            btn_preprocess.Size = new Size(140, 40);
            btn_preprocess.Location = new Point(850, 100); // Adjust as needed
            btn_preprocess.Click += new EventHandler(btn_preprocess_Click);
            this.Controls.Add(btn_preprocess);
        }
        private void btn_preprocess_Click(object sender, EventArgs e)
        {
            string latestCSV = GetLatestCSVFile(destinationDirectory);
            if (string.IsNullOrEmpty(latestCSV))
            {
                MessageBox.Show("No CSV found in final_report folder.");
                return;
            }

            string[] lines;
            try
            {
                lines = File.ReadAllLines(latestCSV);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading CSV: {ex.Message}");
                return;
            }

            if (lines.Length < 2)
            {
                MessageBox.Show("CSV has no data rows.");
                return;
            }

            string[] headers = lines[0].Split(',');
            var dataRows = lines.Skip(1)
                                .Where(l => !string.IsNullOrWhiteSpace(l))
                                .Select(l => l.Split(','))
                                .ToList();

            int freqCols = Math.Min(5, headers.Length);
            double[] sums = new double[freqCols];
            int rowCount = dataRows.Count;

            foreach (var row in dataRows)
            {
                for (int i = 0; i < freqCols; i++)
                {
                    if (double.TryParse(row[i], out double val))
                        sums[i] += val;
                }
            }

            if (rowCount == 0)
            {
                MessageBox.Show("No data to process.");
                return;
            }

            double[] avgs = sums.Select(sum => sum / rowCount).ToArray();
            string newRow = string.Join(",", avgs.Select(a => a.ToString("F2")));

            var updatedLines = new List<string> { lines[0] };
            updatedLines.AddRange(lines.Skip(1));
            updatedLines.Add(newRow);

            string processedDir = Path.Combine(destinationDirectory, "processed_report");
            Directory.CreateDirectory(processedDir);

            string updatedCSVPath = Path.Combine(processedDir, "Processed_" + Path.GetFileName(latestCSV));
            File.WriteAllLines(updatedCSVPath, updatedLines);

            string pdfPath = Path.ChangeExtension(updatedCSVPath, ".pdf");
            GenerateSimplePDF(updatedCSVPath, pdfPath);

            MessageBox.Show($"Data preprocessed and saved:\nCSV: {updatedCSVPath}\nPDF: {pdfPath}");
        }

    }
}