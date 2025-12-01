using FAPL_HW_Driver;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ITextFont = iTextSharp.text.Font;
using ITextBaseColor = iTextSharp.text.BaseColor;

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

        }

        public static string onsokkiFilePath = @"C:\FAPLData\OnsokkiData";
        public static string destinationDirectory = @"C:\FAPLData\final_report";
        public static string rawDataDirectory = @"C:\FAPLData\OnsokkiData\raw";
        private static string testCounterFilePath = @"C:\FAPLData\OnsokkiData\TestCounter.txt";
        FAPL_Debug_FileWrite debugFile;
        public const ushort SW_VER = 1;
        public static string processedDataDirectory = @"C:\FAPLData\processed_data";
        private void btn_preprocess_Click(object sender, EventArgs e)
        {
            try
            {
                string latestCsv = GetLatestCSVFile(destinationDirectory);
                if (latestCsv == null)
                {
                    update_debug_message("No CSV file found for preprocessing.");
                    MessageBox.Show("No CSV files found in destination directory.", "Preprocessing Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Read CSV content
                var lines = File.ReadAllLines(latestCsv).ToList();

                // Find the frequency data section
                int frequencyHeaderIndex = lines.FindIndex(line => line.Contains("Frequency") && line.Contains("Ave."));

                if (frequencyHeaderIndex < 0)
                {
                    update_debug_message("Could not find frequency header in CSV.");
                    MessageBox.Show("CSV file doesn't contain frequency data.", "Preprocessing Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lists for batch averages
                List<double> ch1BatchAverages = new List<double>();
                List<double> ch2BatchAverages = new List<double>();
                List<int> batchSizes = new List<int>();

                // Process frequency values in batches of 5
                List<double> currentCh1Batch = new List<double>();
                List<double> currentCh2Batch = new List<double>();

                // Start processing from the row after the header
                for (int i = frequencyHeaderIndex + 1; i < lines.Count; i++)
                {
                    string[] parts = lines[i].Split(',');

                    // Check if this line contains valid frequency data
                    if (parts.Length >= 3 &&
                        double.TryParse(parts[1], out double ch1Value) &&
                        double.TryParse(parts[2], out double ch2Value))
                    {
                        // Add values to current batch
                        currentCh1Batch.Add(ch1Value);
                        currentCh2Batch.Add(ch2Value);
                        update_debug_message($"Frequency: {parts[0]}, CH1: {ch1Value}, CH2: {ch2Value}");

                        // When batch is complete (5 values), calculate average and prepare for next batch
                        if (currentCh1Batch.Count == 5)
                        {
                            double ch1Average = Math.Round(currentCh1Batch.Average(), 2);
                            double ch2Average = Math.Round(currentCh2Batch.Average(), 2);

                            ch1BatchAverages.Add(ch1Average);
                            ch2BatchAverages.Add(ch2Average);
                            batchSizes.Add(currentCh1Batch.Count);

                            update_debug_message($"Batch {ch1BatchAverages.Count} - CH1 Avg: {ch1Average}, CH2 Avg: {ch2Average}");

                            // Clear for next batch
                            currentCh1Batch.Clear();
                            currentCh2Batch.Clear();
                        }
                    }
                }

                // Process any remaining values (if not a multiple of 5)
                if (currentCh1Batch.Count > 0)
                {
                    double ch1Average = Math.Round(currentCh1Batch.Average(), 2);
                    double ch2Average = Math.Round(currentCh2Batch.Average(), 2);

                    ch1BatchAverages.Add(ch1Average);
                    ch2BatchAverages.Add(ch2Average);
                    batchSizes.Add(currentCh1Batch.Count);

                    update_debug_message($"Final batch ({currentCh1Batch.Count} values) - CH1 Avg: {ch1Average}, CH2 Avg: {ch2Average}");
                }

                if (ch1BatchAverages.Count == 0)
                {
                    update_debug_message("No valid frequency data found.");
                    MessageBox.Show("No valid frequency data found in CSV.", "Preprocessing Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calculate overall averages
                double overallCh1Average = Math.Round(ch1BatchAverages.Average(), 2);
                double overallCh2Average = Math.Round(ch2BatchAverages.Average(), 2);

                // Add results to the CSV
                lines.Add("");
                lines.Add("Preprocessing Results");
                lines.Add($"Overall CH_001-P. Ave. Average,{overallCh1Average}");
                lines.Add($"Overall CH_002-P. Ave. Average,{overallCh2Average}");
                lines.Add("");
                lines.Add("Batch Processing Results (5 values per batch)");

                for (int i = 0; i < ch1BatchAverages.Count; i++)
                {
                    lines.Add($"Batch {i + 1} ({batchSizes[i]} values) - CH_001-P. Ave.,{ch1BatchAverages[i]}");
                    lines.Add($"Batch {i + 1} ({batchSizes[i]} values) - CH_002-P. Ave.,{ch2BatchAverages[i]}");
                }

                // Ensure processed data directory exists
                Directory.CreateDirectory(processedDataDirectory);
                string processedCsv = Path.Combine(
                    processedDataDirectory,
                    Path.GetFileNameWithoutExtension(latestCsv) + "_processed.csv"
                );
                File.WriteAllLines(processedCsv, lines);

                // Pass all batch data to PDF generation
                GenerateDetailedPDF(processedCsv, ch1BatchAverages, ch2BatchAverages, batchSizes, overallCh1Average, overallCh2Average);

                update_debug_message($"Preprocessing complete. Overall CH1 Avg: {overallCh1Average}, CH2 Avg: {overallCh2Average}");
                update_debug_message($"Processed {ch1BatchAverages.Count} batches of frequency data");
                update_debug_message($"Processed files saved to: {processedDataDirectory}");

                MessageBox.Show($"Preprocessing complete!\n\n" +
                               $"Overall CH_001-P. Ave. Average: {overallCh1Average}\n" +
                               $"Overall CH_002-P. Ave. Average: {overallCh2Average}\n" +
                               $"Processed {ch1BatchAverages.Count} batches of data\n\n" +
                               $"Files saved to: {processedDataDirectory}",
                               "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                update_debug_message($"Preprocessing error: {ex.Message}");
                MessageBox.Show($"Error during preprocessing: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GenerateDetailedPDF(string csvPath, List<double> ch1Averages, List<double> ch2Averages,
                                       List<int> batchSizes, double overallCh1Average, double overallCh2Average)
        {
            string pdfPath = Path.ChangeExtension(csvPath, ".pdf");

            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));

                using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                {
                    Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Add title
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                    Paragraph title = new Paragraph("Frequency Data Analysis", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20f;
                    doc.Add(title);

                    // Add overall analysis results
                    Paragraph results = new Paragraph("Overall Analysis Results:", headerFont);
                    results.SpacingAfter = 10f;
                    doc.Add(results);

                    PdfPTable overallTable = new PdfPTable(2);
                    overallTable.WidthPercentage = 80;
                    overallTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    overallTable.SpacingAfter = 20f;

                    // Add table headers
                    overallTable.AddCell(new Phrase("Channel", headerFont));
                    overallTable.AddCell(new Phrase("Overall Average Value", headerFont));

                    // Add data
                    overallTable.AddCell(new Phrase("CH_001-P. Ave.", normalFont));
                    overallTable.AddCell(new Phrase(overallCh1Average.ToString("F2"), normalFont));

                    overallTable.AddCell(new Phrase("CH_002-P. Ave.", normalFont));
                    overallTable.AddCell(new Phrase(overallCh2Average.ToString("F2"), normalFont));

                    doc.Add(overallTable);

                    // Add batch analysis results
                    Paragraph batchResults = new Paragraph("Batch Analysis Results:", headerFont);
                    batchResults.SpacingAfter = 10f;
                    batchResults.SpacingBefore = 10f;
                    doc.Add(batchResults);

                    PdfPTable batchTable = new PdfPTable(3);
                    batchTable.WidthPercentage = 90;
                    batchTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    batchTable.SpacingAfter = 20f;

                    // Add table headers
                    batchTable.AddCell(new Phrase("Batch", headerFont));
                    batchTable.AddCell(new Phrase("CH_001-P. Ave. Average", headerFont));
                    batchTable.AddCell(new Phrase("CH_002-P. Ave. Average", headerFont));

                    // Add batch data
                    for (int i = 0; i < ch1Averages.Count; i++)
                    {
                        batchTable.AddCell(new Phrase($"Batch {i + 1} ({batchSizes[i]} values)", normalFont));
                        batchTable.AddCell(new Phrase(ch1Averages[i].ToString("F2"), normalFont));
                        batchTable.AddCell(new Phrase(ch2Averages[i].ToString("F2"), normalFont));
                    }

                    doc.Add(batchTable);

                    // Add CSV data header
                    Paragraph csvHeader = new Paragraph("CSV Data (First 10 Rows):", headerFont);
                    csvHeader.SpacingBefore = 10f;
                    csvHeader.SpacingAfter = 10f;
                    doc.Add(csvHeader);

                    // Add sample of CSV data
                    string[] csvLines = File.ReadAllLines(csvPath);
                    int lineCount = Math.Min(csvLines.Length, 11); // Header + 10 rows

                    for (int i = 0; i < lineCount; i++)
                    {
                        Paragraph line = new Paragraph(csvLines[i], normalFont);
                        doc.Add(line);
                    }

                    doc.Close();
                    update_debug_message($"Generated detailed PDF: {pdfPath}");
                }
            }
            catch (Exception ex)
            {
                update_debug_message($"Error generating detailed PDF: {ex.Message}");
            }
        }
        
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
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin ",
                "Initially to be filled by Daikin "
            };

            for (int i = 0; i < parameters.Length; i++)
            {
                dgv_test_data.Rows.Add(parameters[i], defaultValues[i]);
            }

            // Add second table data
            dgv_test_data.Rows.Add("End Time", "(Auto Generated) csv import time");
            dgv_test_data.Rows.Add("Serial No.", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Application", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Standard", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Test Engineer", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Power Supply", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Rated Power(Cool/Heat) (W)", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Rated Current(A)", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Refrigerant Type / QTY", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("IDU Fan Motor / Rated RPM", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("ODU Fan Motor / Rated RPM", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Sample No.", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Sound Jacket", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Outdoor Unit Rated Sound Level dB(A)", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("O.D Air W.B.T", "Initially to be filled by Daikin ");
            dgv_test_data.Rows.Add("Outdoor Unit Measurement Channel", "Initially to be filled by Daikin ");
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
        public static class TestIdGenerator
        {
            private static readonly string filePath = "last_test_id.txt";

            public static string Generate(string labCode = "006")
            {
                string today = DateTime.Now.ToString("yyyyMMdd");
                int serial = 1;

                if (File.Exists(filePath))
                {
                    string[] parts = File.ReadAllText(filePath).Split(',');
                    string lastDate = parts[0];
                    int lastSerial = int.Parse(parts[1]);

                    if (lastDate == today)
                        serial = lastSerial + 1;
                }

                // Pad serial to 3 digits: 001, 002...
                string serialStr = serial.ToString("D3");

                // Save updated serial
                File.WriteAllText(filePath, $"{today},{serialStr}");

                // Final format
                return $"{labCode}{today}{serialStr}";
            }
        }

        private async void btn_start_Click(object sender, EventArgs e)
        {
            testStartTime = DateTime.Now;
            isTestRunning = true;

            string newTestID = TestIdGenerator.Generate();

            // Update the Started Time and Test ID in the table
            dgv_test_data.Rows[0].Cells["Value"].Value = testStartTime.ToString("yyyy-MM-dd HH:mm:ss");
            dgv_test_data.Rows[1].Cells["Value"].Value = newTestID;

            // Update UI
            btn_start.BackColor = Color.Green;
            btn_stop.BackColor = SystemColors.Control;

            update_debug_message($"Test started at: {testStartTime}");
            update_debug_message($"Test ID generated: {newTestID}");
            OnosokkiControl.updateOnosokkiStart();

            // 🔻 NEW: run full auto sequence
            await RunAutoTestSequenceAsync();
        }
        private async Task RunAutoTestSequenceAsync()
        {
            try
            {
                // Disable buttons so user doesn't spam things during the auto run
                btn_start.Enabled = false;
                btn_stop.Enabled = false;
                btn_test_data.Enabled = false;

                update_debug_message("Auto test sequence started: waiting 10 seconds before stopping.");

                // 1) Wait 10 seconds for the measurement
                await Task.Delay(TimeSpan.FromSeconds(10));

                btn_stop_Click(null, EventArgs.Empty);

                await Task.Delay(TimeSpan.FromSeconds(2));
                update_debug_message("Attempting to read latest file and generate reports.");

                btn_test_data_Click(null, EventArgs.Empty);

                update_debug_message("Auto test sequence completed successfully.");
            }
            catch (Exception ex)
            {
                update_debug_message("Auto test sequence error: " + ex.Message);
                MessageBox.Show($"Auto test failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable buttons
                btn_start.Enabled = true;
                btn_stop.Enabled = true;
                btn_test_data.Enabled = true;

                // Reset button colors if you want
                btn_start.BackColor = SystemColors.Control;
                btn_stop.BackColor = SystemColors.Control;
            }
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
                
                writer.WriteLine("DAIKIN,,Test Data (Anechoic Chamber)");
                writer.WriteLine(""); 

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
            try
            {
                if (!Directory.Exists(directory))
                {
                    update_debug_message($"Directory does not exist: {directory}");
                    return null;
                }

                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                FileInfo[] csvFiles = dirInfo.GetFiles("*.csv");

                if (csvFiles.Length == 0)
                {
                    update_debug_message($"No CSV files found in directory: {directory}");
                    return null;
                }

                // Sort by last write time (most recent first)
                FileInfo latestFile = csvFiles.OrderByDescending(f => f.LastWriteTime).First();

                update_debug_message($"Latest CSV file found: {latestFile.Name} - {latestFile.LastWriteTime}");
                return latestFile.FullName;
            }
            catch (Exception ex)
            {
                update_debug_message($"Error getting latest CSV file: {ex.Message}");
                return null;
            }
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
                Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));
                using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                using (Document doc = new Document(PageSize.A4, 50, 50, 50, 50)) 
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // Title
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                    var normal = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                    doc.Add(new Paragraph("DAIKIN", titleFont));
                    doc.Add(new Paragraph("Test Data (Anechoic Chamber)", titleFont));
                    doc.Add(new Paragraph("\n"));

                    // Test parameters (from DataGridView)
                    doc.Add(new Paragraph("TEST PARAMETERS:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
                    foreach (DataGridViewRow row in dgv_test_data.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string p = row.Cells["Parameter"].Value?.ToString() ?? "";
                            string v = row.Cells["Value"].Value?.ToString() ?? "";
                            doc.Add(new Paragraph($"{p}: {v}", normal));
                        }
                    }
                    doc.Add(new Paragraph("\n"));

                    // Analysis
                    doc.Add(new Paragraph("NOISE ANALYSIS RESULTS:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
                    doc.Add(new Paragraph($"Maximum Noise Level: {Math.Round(maxNoise, 2)} dB(A)", normal));
                    doc.Add(new Paragraph($"Average Noise Level: {Math.Round(avgNoise, 2)} dB(A)", normal));
                    doc.Add(new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normal));

                    // doc.Close() is handled by the using statement
                }
                update_debug_message("PDF generated successfully: " + pdfPath);
            }
            catch (Exception ex)
            {
                update_debug_message("PDF generation error: " + ex.Message);
            }
        }
        public void update_debug_message(string message)
        {
            debugFile.logDebug(message);
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            debugFile = new FAPL_Debug_FileWrite();
            update_debug_message("SW version:" + SW_VER);
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

        private void panel_table_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}