using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace UEP
{
    public partial class Form1 : Form
    {
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private ImageList imageList1;
        private TextBox txtProcessName;
        private Button btnSavePath;
        private Button btnRunPath;
        private Button btnProcessRefresh;
        private ComboBox comboBox;

        private TabControl tabControl1;
        private TabPage tabPage1;

        private TrackBar trackBarMouseSpeed;
        private TextBox txtSpeedValue;

        //private int rowIndexFromMouseDown;
        //private int rowIndexOfItemUnderMouseToDrop;

        private string selectFile;

        public Form1()
        {
            InitializeComponent();
            InitializeComponents();
            this.Size = new Size(1200, 1200);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.AutoScaleMode = AutoScaleMode.Font; // ��Ʈũ�� �ڵ������̶�µ� �Ǵ°��� �𸣰ڴ�
            this.Resize += new EventHandler(Form1_Resize); // ��Ʈ ũ�� ������
        }

        // ������ �ڵ�
        private void InitializeComponents()
        {
            imageList1 = new ImageList();
            imageList1.ImageSize = new Size(100, 100);  // ������ ũ�� ����

            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();

            // �� ��Ʈ�� ����
            tabControl1 = new TabControl();

            // �� ��Ʈ���� ��ġ �� ũ�� ����
            tabControl1.Location = new Point(30, 500); // ��ġ
            tabControl1.Size = new Size(1100, 300); // ũ��

            // �� ��Ʈ���� ���� �߰�
            this.Controls.Add(tabControl1);

            // �� ������ �߰�
            TabPage tabPage1 = new TabPage("SoftWare");
            TabPage tabPage2 = new TabPage("Mouse");
            TabPage tabPage3 = new TabPage("Display");
            TabPage tabPage4 = new TabPage("Sound");

            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);

            // �׸����2�� ù ��° �� �������� �߰�
            tabPage1.Controls.Add(dataGridView2);

            // DataGridView ����
            this.dataGridView1.Location = new Point(30, 30);
            this.dataGridView1.Size = new Size(1100, 300);
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = true;
            this.dataGridView1.RowHeadersVisible = false;

            this.dataGridView2.Size = new Size(1100, 300);
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = true;
            this.dataGridView2.RowHeadersVisible = false;


            // �÷� �߰� �� �ʱ� �ʺ� ����
            AddImageColumn(".", 60, 1); // �̹��� �÷� �߰�
            AddColumn("App Name", 300, 1);
            AddColumn("Process Name", 200, 1);
            AddColumn("X", 100, 1);
            AddColumn("Y", 100, 1);
            AddColumn("Width", 100, 1);
            AddColumn("Height", 100, 1);
            AddColumn("State", 100, 1);

            // �÷� �߰� �� �ʱ� �ʺ� ����
            AddImageColumn(".", 60, 2); // �̹��� �÷� �߰�
            AddColumn("App Name", 300, 2);
            AddColumn("Process Name", 200, 2);
            AddColumn("X", 100, 2);
            AddColumn("Y", 100, 2);
            AddColumn("Width", 100, 2);
            AddColumn("Height", 100, 2);
            AddColumn("State", 100, 2);

            // ���� DataGridView �߰�
            this.Controls.Add(this.dataGridView1);



            // ���μ��� �ε� (����: �����͸� ���� �߰�)
            LoadProcesses();

            dataGridView1.CellDoubleClick += new DataGridViewCellEventHandler(gridView1_CellClick); // �̰� ��ġ �Űܵ� �Ǵ��� Ȯ���غ���

            // �ؽ�Ʈ �ڽ� ����
            txtProcessName = new TextBox();
            txtProcessName.Location = new Point(30, 350); // ��ġ ����
            txtProcessName.Size = new Size(200, 20); // ũ�� ����

            // ��ư ����
            btnSavePath = new Button();
            btnSavePath.Location = new Point(240, 350); // ��ġ ����
            btnSavePath.Size = new Size(80, 40); // ũ�� ����
            btnSavePath.Text = "Save"; // ��ư �ؽ�Ʈ ����
            btnSavePath.Click += new EventHandler(btnSavePath_Click); // Ŭ�� �̺�Ʈ �ڵ鷯 ����

            // ���μ�������� ���ΰ�ħ�ϴ� ��ư
            btnProcessRefresh = new Button();
            btnProcessRefresh.Location = new Point(800, 350); // ��ġ ����
            btnProcessRefresh.Size = new Size(80, 40); // ũ�� ����
            btnProcessRefresh.Text = "���ε�"; // ��ư �ؽ�Ʈ ����
            btnProcessRefresh.Click += new EventHandler(btnProcessRefresh_Click); // Ŭ�� �̺�Ʈ �ڵ鷯 ����

            // ��ư ����            
            btnRunPath = new Button();
            btnRunPath.Location = new Point(240, 450); // ��ġ ����
            btnRunPath.Size = new Size(80, 40); // ũ�� ����
            btnRunPath.Text = "����"; // ��ư �ؽ�Ʈ ����
            btnRunPath.Click += new EventHandler(btnRunPath_Click); // Ŭ�� �̺�Ʈ �ڵ鷯 ����

            // ������ ��� �����ֱ�
            comboBox = new ComboBox();
            comboBox.Location = new Point(30, 450);
            comboBox.Size = new Size(200, 20);
            comboBox.Text = "������ ���";
            comboBox.Click += new EventHandler(LoadTextFilesToComboBox);
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);


            // �ؽ�Ʈ �ڽ��� ��ư�� ���� �߰�
            this.Controls.Add(txtProcessName);
            this.Controls.Add(btnSavePath);
            this.Controls.Add(btnRunPath);
            this.Controls.Add(comboBox);
            this.Controls.Add(btnProcessRefresh);

            // Mouse tab UI ��� �߰�
            InitializeMouseTabComponents(tabPage2);
        }


        // �׸���信 ���ڿ� �߰�
        private void AddColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.MinimumWidth = 50; // �ּ� �ʺ� ����
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // �ڵ� ũ�� ���� ��Ȱ��ȭ
            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }

        }


        // �׸���信 �̹��� �߰�
        private void AddImageColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewImageColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.ImageLayout = DataGridViewImageCellLayout.Zoom; // �̹��� ���̾ƿ� ����
            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }
        }


        // ��Ʈ ���� �Լ�
        private void Form1_Resize(object sender, EventArgs e)
        {
            float scaleFactor = Math.Min((float)this.ClientSize.Width / 1800, (float)this.ClientSize.Height / 1600);
            AdjustControlFonts(this, scaleFactor);
        }


        // ��Ʈ ���� �Լ�
        private void AdjustControlFonts(Control ctrl, float scaleFactor)
        {
            foreach (Control c in ctrl.Controls)
            {
                c.Font = new System.Drawing.Font(c.Font.FontFamily, 10 * scaleFactor, c.Font.Style);
                if (c.Controls.Count > 0)
                {
                    AdjustControlFonts(c, scaleFactor);
                }
            }
        }


        // ���μ����� ���ΰ�ħ �ϴ� ��ư�� �̺�Ʈ
        private void btnProcessRefresh_Click(object sender, EventArgs e)
        {
            LoadProcesses();
        }


        // �޺��ڽ��� �޴��� �������� ���, ��θ� ã�� �ؽ�Ʈ������ �о ���μ��� ���� ������ �����´�
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex != -1) // ���õ� �׸��� �ִ��� Ȯ���մϴ�.
            {
                string selectedFile = comboBox.SelectedItem.ToString();
                Console.WriteLine(selectedFile);


                string modifiedFile = selectedFile.Substring(0, selectedFile.Length - 4);

                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exeLocation);
                string filePath = Path.Combine(exeDirectory, modifiedFile + "\\" + selectedFile);
                Console.WriteLine(filePath);

                selectFile = filePath;

                List<string> processInfoLines = new List<string>(); // ó���� ������ ������ ����Ʈ�Դϴ�.
                int blankLineCount = 0; // ���� ���� ī��Ʈ�� �����մϴ�.

                foreach (var line in File.ReadLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        blankLineCount++; // ���� ���̸� ī��Ʈ�� ������ŵ�ϴ�.
                    }
                    else if (blankLineCount >= 3)
                    {
                        processInfoLines.Add(line); // 3���� ���� �� ������ �����͸� ����Ʈ�� �߰��մϴ�.
                    }

                    if (!string.IsNullOrWhiteSpace(line) && blankLineCount < 3)
                    {
                        blankLineCount = 0; // ������ �ƴ� ���� ������ 3�� �̸��� ���� ���̾����� ī��Ʈ�� �����մϴ�.
                    }
                }

                DisplayProcessInfoInGridView(processInfoLines, modifiedFile); // �����͸� �׸��� �信 ǥ���մϴ�.
            }
        }


        // �޺��ڽ��� �޴��� �������� ���, �׸����2�� ���μ��� ������ ���ε��Ѵ�
        private void DisplayProcessInfoInGridView(List<string> processInfoLines, string modifiedFile)
        {
            dataGridView2.Rows.Clear(); // ������ ������ ����� �� ������ ǥ���մϴ�.
            int imageNum = 0;
            foreach (string line in processInfoLines)
            {
                // line ������ ����� �ؽ�Ʈ�� "//" �������� ����
                string[] data = line.Split(new string[] { "//" }, StringSplitOptions.None);

                // DataGridViewRow ��ü ����
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView2);


                // ���� ���� ���� ������� ��ġ�� ����
                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // ������� ���͸��� ����
                string exeDirectory = Path.GetDirectoryName(exeLocation);
                // �̹��� ������ ��� ��θ� �߰��Ͽ� ��ü ��θ� ����
                string imagePath = Path.Combine(exeDirectory, modifiedFile, imageNum.ToString() + ".png");
                imageNum++;

                // �̹��� ������ �ε�
                System.Drawing.Image icon = System.Drawing.Image.FromFile(imagePath);

                // �ε��� �̹����� DataGridView�� ���� �Ҵ�
                row.Cells[0].Value = icon;


                // ������ ���� �ؽ�Ʈ ������ �Ҵ�
                for (int i = 1; i < row.Cells.Count; i++)
                {
                    // data �迭�� ���̸� �ʰ����� �ʵ��� ���� Ȯ��
                    if (i < data.Length)
                    {
                        row.Cells[i].Value = data[i];
                    }
                }

                // ������ ���� DataGridView�� �߰�
                dataGridView2.Rows.Add(row);
            }
        }


        // �޺��ڽ��� ���� ���丮�� �ִ� ��� �ؽ�Ʈ ������ �������� �Լ�
        private void LoadTextFilesToComboBox(object sender, EventArgs e)
        {
            comboBox.Items.Clear(); // ���� ����� ���ϴ�.

            // ���� ���� ���� ������� ��ġ�� ����
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // ������� ���͸��� ����
            string exeDirectory = Path.GetDirectoryName(exeLocation);
            // �̹��� ������ ��� ��θ� �߰��Ͽ� ��ü ��θ� ����
            string directoryPath = Path.Combine(exeDirectory);


            // "[FILE]"�� �����ϴ� ��� ������ �����ɴϴ�.
            string[] directories = Directory.GetDirectories(directoryPath, "[FILE]*");

            foreach (string directory in directories)
            {
                // �� ���� ���� ��� .txt ������ �����ɴϴ�.
                string[] files = Directory.GetFiles(directory, "*.txt");

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    comboBox.Items.Add(fileInfo.Name); // ���� �̸��� ComboBox�� �߰��մϴ�.
                }
            }
        }


        // �׸����2�� ���μ��� �������� ������ �ؽ�Ʈ ���Ͽ� �����ϴ� �Լ�
        static void SaveProcessPathToFile(string filePath, DataGridView gridView2)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (DataGridViewRow row in gridView2.Rows)
                {
                    if (row.Cells["App Name"].Value != null) // "MainWindowTitle"�� ���� ������ Ÿ��Ʋ�� ǥ�õǴ� �÷��� �̸��Դϴ�.
                    {
                        string windowTitle = row.Cells["App Name"].Value.ToString();

                        // �ý��ۿ��� ���� ���� ��� ���μ����� �����ɴϴ�.
                        Process[] processes = Process.GetProcesses();

                        foreach (Process prs in processes)
                        {
                            // ���� ������ Ÿ��Ʋ�� ����ڰ� ������ ���� ��ġ�ϴ��� Ȯ���մϴ�.
                            if (prs.MainWindowTitle == windowTitle)
                            {
                                try
                                {
                                    // ���μ����� ���� ���� ��θ� �����ɴϴ�.
                                    string processPath = prs.MainModule.FileName;

                                    // ���� ���� ��θ� ���Ͽ� ���ϴ�.
                                    sw.WriteLine($"{processPath}");
                                    break; // ��ġ�ϴ� ù ��° ���μ����� ã�����Ƿ� �ݺ��� �ߴ��մϴ�.
                                }
                                catch (Exception ex)
                                {
                                    // ������ �� ���� ���μ����� "���� �Ұ�"�� ǥ���մϴ�.
                                    sw.WriteLine($"WindowTitle: {windowTitle}, ���: ���� �Ұ� - {ex.Message}");
                                }
                            }
                        }
                    }
                }
                // �� ������ ���Ͽ� �߰��մϴ�.
                sw.WriteLine("\n\n");

                foreach (DataGridViewRow row in gridView2.Rows)
                {
                    if (row.Cells["App Name"].Value != null)
                    {

                        string AppName = row.Cells["App Name"].Value.ToString();
                        string ProcessName = row.Cells["Process Name"].Value.ToString();
                        string X = row.Cells["X"].Value.ToString();
                        string Y = row.Cells["Y"].Value.ToString();
                        string Width = row.Cells["Width"].Value.ToString();
                        string Height = row.Cells["Height"].Value.ToString();
                        string State = row.Cells["State"].Value.ToString();

                        sw.WriteLine("//" + AppName + "//" + ProcessName + "//" + X + "//" + Y + "//" + Width + "//" + Height + "//" + State);

                    }
                }
            }
        }


        // ���Ͽ� �������� �����ϴ� �Լ�
        static void SaveProcessIconToFile(string folderPath, DataGridView gridView2)
        {
            int imageIndex = 0; // ���ϸ� ���� �ε����Դϴ�.

            foreach (DataGridViewRow row in gridView2.Rows)
            {
                // DataGridView�� ù ��° ������ Ư�� ���� �� �̹����� �����ɴϴ�.
                var cellValue = row.Cells[0].Value;

                // ���� ���� null�� �ƴϰ�, Image Ÿ������ Ȯ���մϴ�.
                if (cellValue != null && cellValue is System.Drawing.Image)
                {
                    // �̹��� ���Ϸ� �����մϴ�.
                    System.Drawing.Image img = (System.Drawing.Image)cellValue;

                    string imagePath = Path.Combine(folderPath, $"{imageIndex}.png"); // PNG ���Ϸ� ����
                    img.Save(imagePath, ImageFormat.Png); // ImageFormat�� ���� �ٸ� �������� ������ �� �ֽ��ϴ�.

                    imageIndex++; // ���� �̹����� ���� �ε����� ������ŵ�ϴ�.
                }
                else
                {
                    // ������ ���� ó���� �⺻�� ��ȯ
                }
            }
        }



        // ���� ��ư�� ������ ���μ����� ��ΰ� �ؽ�Ʈ ���� ���·� ����Ǵ� �Լ�
        private void btnSavePath_Click(object sender, EventArgs e)
        {
            // ���μ��� �̸��� ������� ���� �� ���� �̸��� �����մϴ�.
            string folderName = "[FILE]_" + txtProcessName.Text;
            string fileName = folderName + ".txt";
            string folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath, folderName); // ���ø����̼� ���� ��ο� ���� ����
            string filePath = Path.Combine(folderPath, fileName); // ���� ���� ���

            // ������ ��ο� ������ ���ٸ� �����մϴ�.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // ���μ��� ��θ� ���Ͽ� �����ϴ� �޼��带 ȣ���մϴ�.
            SaveProcessPathToFile(filePath, dataGridView2); // ���� ���ڷ� �־�� �ϳ�? ���߿� Ȯ��
            SaveProcessIconToFile(folderPath, dataGridView2);

            MessageBox.Show("������ �Ϸ�Ǿ����ϴ�.");
        }



        // ���������� ��θ� �����Ͽ� ������ �����Ű�� �Լ�
        // �����Լ���. ���߿� ������ ��ɿ� �߰� �ʿ�
        private void btnRunPath_Click(object sender, EventArgs e)
        {
            try
            {
                // ���Ͽ��� �� ���� �о�´�.
                string[] applicationPaths = File.ReadAllLines(selectFile);
                int blankLineCount = 0;

                foreach (string appPath in applicationPaths)
                {
                    // ���� ������ Ȯ��
                    if (string.IsNullOrWhiteSpace(appPath))
                    {
                        blankLineCount++;
                    }
                    else
                    {
                        // ������ �ƴ� ���� ã���� ���� �� ī��Ʈ�� �ʱ�ȭ
                        blankLineCount = 0;
                    }

                    // ���� ���� 3���� �Ǹ� �ٸ� �۾� ����
                    if (blankLineCount == 3)
                    {
                        PerformOtherTask();
                        break;
                    }

                    // ���� ���� 3���� �ƴ� ���� ���ø����̼� ����
                    if (blankLineCount < 3)
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = appPath,
                            UseShellExecute = true // Shell�� ����Ͽ� ���� ����
                        };

                        // ���ø����̼� ����
                        Process.Start(psi);
                    }
                }
            }
            catch (Exception ex)
            {
                // ���� ó��
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void PerformOtherTask()
        {
            // ���� ���� 3���� ���� �� ������ �۾�
            Console.WriteLine("Performing other tasks...");
            // ���⿡ �ٸ� �۾��� �����ϼ���.
        }



        // 1�� ���� ���� ����Ŭ�� �ϸ� 2�� ��� ������ �ű�� �Լ�
        private void gridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // ��ȿ�� �� �ε������� Ȯ���մϴ�.
            {
                // ù ��° �׸��� ���� ���õ� ���� �����ɴϴ�.
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // �� ��° �׸��� �信 ������ ���� �ִ��� Ȯ���մϴ�.
                bool isDuplicate = false;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    bool allCellsMatch = true;
                    foreach (DataGridViewCell cell in selectedRow.Cells)
                    {
                        // �ش� ���� ���� �ٸ���, �� ���� �ߺ����� �ʽ��ϴ�.
                        if (row.Cells[cell.ColumnIndex].Value.ToString() != cell.Value.ToString())
                        {
                            allCellsMatch = false;
                            break;
                        }
                    }

                    if (allCellsMatch)
                    {
                        // ��� ���� ��ġ�ϸ�, �ߺ��� ���Դϴ�.
                        isDuplicate = true;
                        break;
                    }
                }

                // �ߺ����� ���� ��쿡�� �� ���� �߰��մϴ�.
                if (!isDuplicate)
                {
                    int newRow = dataGridView2.Rows.Add();
                    foreach (DataGridViewCell cell in selectedRow.Cells)
                    {
                        dataGridView2.Rows[newRow].Cells[cell.ColumnIndex].Value = cell.Value;
                    }
                }
            }
        }


        // �������� ���¸� �������� �Լ�
        private string GetWindowState(IntPtr hWnd)
        {
            User33.WINDOWPLACEMENT placement = new User33.WINDOWPLACEMENT();
            if (User33.GetWindowPlacement(hWnd, ref placement))
            {
                switch (placement.showCmd)
                {
                    case User33.SW_SHOWMAXIMIZED:
                        return "Max";
                    case User33.SW_SHOWMINIMIZED:
                        return "Min";
                    case User33.SW_SHOWNORMAL:
                        return "Normal";
                    default:
                        return "Unknown";
                }
            }
            return "Error";
        }

        // �ʿ��� �ܺ� �Լ� ����
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // P/Invoke ����
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Shell32.dll", SetLastError = true)]
        static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        const uint PROCESS_QUERY_INFORMATION = 0x0400;
        const uint PROCESS_VM_READ = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;
        }

        private void SaveIconFromWindow(IntPtr hWnd, string windowTitle)
        {
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);

            if (processHandle != IntPtr.Zero)
            {
                StringBuilder buffer = new StringBuilder(1024);
                uint bufferSize = (uint)buffer.Capacity + 1; // ���� ũ�� ����
                if (QueryFullProcessImageName(processHandle, 0, buffer, ref bufferSize))
                {
                    string processPath = buffer.ToString();
                    ExtractAndAddIconToImageList(processPath, windowTitle);
                }

                CloseHandle(processHandle);
            }
        }

        private void ExtractAndAddIconToImageList(string filePath, string windowTitle)
        {
            IntPtr largeIcon, smallIcon;
            if (ExtractIconEx(filePath, 0, out largeIcon, out smallIcon, 1) > 0)
            {
                using (Icon icon = Icon.FromHandle(largeIcon))
                {
                    // Icon�� ImageList�� �߰�
                    imageList1.Images.Add(icon.ToBitmap());
                    // ����������, windowTitle�� ����Ͽ� �� �̹����� Ű�� �Ҵ��� �� �ֽ��ϴ�.
                    // ��: imageList.Images.Add(windowTitle, icon.ToBitmap());

                    Console.WriteLine($"Icon added to ImageList: {windowTitle}");
                }

                // ������ �ڵ� ����
                if (largeIcon != IntPtr.Zero) DestroyIcon(largeIcon);
                if (smallIcon != IntPtr.Zero) DestroyIcon(smallIcon);
            }
        }


        private bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {

            if (IsWindowVisible(hWnd))
            {


                StringBuilder sb = new StringBuilder(256);
                if (GetWindowText(hWnd, sb, sb.Capacity) > 0)
                {


                    RECT rect;
                    GetWindowRect(hWnd, out rect);

                    WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                    placement.Length = Marshal.SizeOf(placement);
                    GetWindowPlacement(hWnd, out placement);

                    string windowState = placement.ShowCmd switch
                    {
                        1 => "Normal",
                        2 => "Minimized",
                        3 => "Maximized",
                        _ => "Unknown"
                    };

                    // ���μ��� ID ��������
                    GetWindowThreadProcessId(hWnd, out int processId);

                    // ���μ��� �̸� ��������
                    string processName = "Unknown";
                    try
                    {
                        Process process = Process.GetProcessById(processId);
                        processName = process.ProcessName;

                    }
                    catch (Exception ex)
                    {
                        processName = $"Error: {ex.Message}";
                    }

                    SaveIconFromWindow(hWnd, processName);

                    var row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);

                    int imageIndex;
                    imageIndex = imageList1.Images.Count - 1;
                    row.Cells[0].Value = imageList1.Images[imageIndex];
                    row.Cells[1].Value = sb;
                    row.Cells[2].Value = processName;
                    row.Cells[3].Value = rect.Left;
                    row.Cells[4].Value = rect.Top;
                    row.Cells[5].Value = rect.Right - rect.Left;
                    row.Cells[6].Value = rect.Bottom - rect.Top;
                    row.Cells[7].Value = windowState;

                    dataGridView1.Rows.Add(row);

                    Console.WriteLine($"Handle: {hWnd}, Title: {sb}, Process Name: {processName}, X: {rect.Left}, Y: {rect.Top}, Width: {rect.Right - rect.Left}, Height: {rect.Bottom - rect.Top}, State: {windowState}");
                }
            }
            return true; // Return true to continue enumerating the next window
        }


        private void LoadProcesses()
        {
            dataGridView1.Rows.Clear(); // DataGridView ������ Ŭ����
            EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);
        }


        //private void LoadProcesses()
        //{

        //    EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);


        //    dataGridView1.Rows.Clear(); // DataGridView ������ Ŭ����
        //    Process[] processes = Process.GetProcesses();
        //    foreach (Process prs in processes)
        //    {
        //        // MainWindowTitle�� ������� ���� ���μ����� ���͸�
        //        if (!string.IsNullOrEmpty(prs.MainWindowTitle))
        //        {
        //            int imageIndex;


        //            try
        //            {
        //                // ���μ����� ���� ���κ��� ������ ���� �õ�
        //                Icon icon = Icon.ExtractAssociatedIcon(prs.MainModule.FileName);
        //                // Icon�� Image ���·� ��ȯ
        //                Bitmap iconBitmap = icon.ToBitmap();
        //                imageList1.Images.Add(iconBitmap); // ImageList�� ������ �߰�
        //                imageIndex = imageList1.Images.Count - 1; // �߰��� �������� �ε���

        //            }
        //            catch (Exception ex)
        //            {
        //                // ���� ���� �� �⺻ Error ������ ���
        //                imageList1.Images.Add(SystemIcons.Error);
        //                imageIndex = imageList1.Images.Count - 1; // �߰��� Error �������� �ε���

        //            }

        //            // ������ �׸���信 �� �߰�
        //            var row = new DataGridViewRow();
        //            row.CreateCells(dataGridView1);

        //            // �������� �̹��� ���� �߰�
        //            row.Cells[0].Value = imageList1.Images[imageIndex];

        //            // ���μ��� �̸��� ������ �߰�
        //            row.Cells[1].Value = prs.MainWindowTitle;
        //            row.Cells[2].Value = prs.ProcessName;

        //            // ������ ���� ���� �߰�
        //            string windowState = GetWindowState(prs.MainWindowHandle);
        //            row.Cells[7].Value = windowState;



        //            // ������ ��ġ �� ũ�� ���� �߰�
        //            try
        //            {
        //                var rect = new User32.Rect();
        //                User32.GetWindowRect(prs.MainWindowHandle, ref rect);
        //                row.Cells[3].Value = rect.Left.ToString(); // X�� ��ġ
        //                row.Cells[4].Value = rect.Top.ToString(); // Y�� ��ġ
        //                row.Cells[5].Value = (rect.Right - rect.Left).ToString(); // ���� ũ��
        //                row.Cells[6].Value = (rect.Bottom - rect.Top).ToString(); // ���� ũ��
        //            }
        //            catch
        //            {
        //                row.Cells[3].Value = "N/A"; // X�� ��ġ
        //                row.Cells[4].Value = "N/A"; // Y�� ��ġ
        //                row.Cells[5].Value = "N/A"; // ���� ũ��
        //                row.Cells[6].Value = "N/A"; // ���� ũ��
        //            }
        //            dataGridView1.Rows.Add(row);
        //        }
        //    }
        //}



        //    private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        //    {
        //        if (IsWindowVisible(hWnd))
        //        {
        //            StringBuilder sb = new StringBuilder(256);
        //            if (GetWindowText(hWnd, sb, sb.Capacity) > 0)
        //            {
        //                RECT rect;
        //                GetWindowRect(hWnd, out rect);

        //                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
        //                placement.Length = Marshal.SizeOf(placement);
        //                GetWindowPlacement(hWnd, out placement);

        //                string windowState = placement.ShowCmd switch
        //                {
        //                    1 => "Normal",
        //                    2 => "Minimized",
        //                    3 => "Maximized",
        //                    _ => "Unknown"
        //                };

        //                // ���μ��� ID ��������
        //                GetWindowThreadProcessId(hWnd, out int processId);

        //                // ���μ��� �̸� ��������
        //                string processName = "Unknown";
        //                try
        //                {
        //                    Process process = Process.GetProcessById(processId);
        //                    processName = process.ProcessName;
        //                }
        //                catch (Exception ex)
        //                {
        //                    processName = $"Error: {ex.Message}";
        //                }

        //                Console.WriteLine($"Handle: {hWnd}, Title: {sb}, Process Name: {processName}, X: {rect.Left}, Y: {rect.Top}, Width: {rect.Right - rect.Left}, Height: {rect.Bottom - rect.Top}, State: {windowState}");
        //            }
        //        }
        //        return true; // Return true to continue enumerating the next window
        //    }

        private void InitializeMouseTabComponents(TabPage tabPageMouse) // ��ü���� ���콺 �� ����
        {
            // ���콺 �ӵ� ���� ����
            System.Windows.Forms.Label lblMouseSpeed = new System.Windows.Forms.Label();
            lblMouseSpeed.Text = "���콺 �ӵ� ����:";
            lblMouseSpeed.Location = new Point(20, 20);     // ��ġ
            lblMouseSpeed.Size = new Size(120, 20);         // ũ��

            TrackBar trackBarMouseSpeed = new TrackBar();
            trackBarMouseSpeed.Location = new Point(150, 20); // ��ġ
            trackBarMouseSpeed.Size = new Size(250, 45);        // ũ��
            trackBarMouseSpeed.Minimum = 1;
            trackBarMouseSpeed.Maximum = 20;
            trackBarMouseSpeed.TickFrequency = 1;
            trackBarMouseSpeed.ValueChanged += TrackBarMouseSpeed_ValueChanged;

            // �ؽ�Ʈ �ڽ��� Ʈ���� �� ǥ��
            txtSpeedValue = new TextBox();
            txtSpeedValue.ReadOnly = true; // �б� �������� �����Ͽ� ����ڰ� �ؽ�Ʈ�� �������� ���ϵ��� ��
            txtSpeedValue.Location = new Point(410, 20);
            txtSpeedValue.Size = new Size(20, 20);
            txtSpeedValue.TextAlign = HorizontalAlignment.Center;
            txtSpeedValue.Text = trackBarMouseSpeed.Value.ToString(); // �ʱⰪ ����

            // ���콺 ���� ��� (���� ���� ����)
            CheckBox chkInvertMouse = new CheckBox();
            chkInvertMouse.Text = "���콺 ����";
            chkInvertMouse.Location = new Point(20, 60);
            chkInvertMouse.Size = new Size(100, 20);
            chkInvertMouse.CheckedChanged += new EventHandler(InvertMouse);

            // ���콺 ������ �θ� Ŀ�� �������� ��� (���� ���� ����)
            CheckBox chkHideCursor = new CheckBox();
            chkHideCursor.Text = "Ŀ�� ����";
            chkHideCursor.Location = new Point(20, 100);
            chkHideCursor.Size = new Size(100, 20);
            chkHideCursor.CheckedChanged += new EventHandler(HideCursor);

            // ���콺 �� ���� ��� (���� ���� ����)
            System.Windows.Forms.Label lblWheelSensitivity = new System.Windows.Forms.Label();
            lblWheelSensitivity.Text = "�� ����:";
            lblWheelSensitivity.Location = new Point(20, 140);
            lblWheelSensitivity.Size = new Size(100, 20);

            TrackBar trackBarWheelSensitivity = new TrackBar();
            trackBarWheelSensitivity.Location = new Point(130, 140);
            trackBarWheelSensitivity.Size = new Size(170, 45);
            trackBarWheelSensitivity.Minimum = 1;
            trackBarWheelSensitivity.Maximum = 10;
            trackBarWheelSensitivity.TickFrequency = 1;
            trackBarWheelSensitivity.ValueChanged += new EventHandler((sender, e) => SetWheelSensitivity(trackBarWheelSensitivity.Value));

            // UI ��Ҹ� �� �������� �߰�
            tabPageMouse.Controls.Add(lblMouseSpeed);
            tabPageMouse.Controls.Add(trackBarMouseSpeed);
            tabPageMouse.Controls.Add(txtSpeedValue);
            tabPageMouse.Controls.Add(chkInvertMouse);
            tabPageMouse.Controls.Add(chkHideCursor);
            tabPageMouse.Controls.Add(lblWheelSensitivity);
            tabPageMouse.Controls.Add(trackBarWheelSensitivity);


        }
        private void InvertMouse(object sender, EventArgs e) // ���콺 ���� (���� ���� ����)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                // ���콺 ���� ���� ����
                MessageBox.Show($"���콺 ����: {chk.Checked}");
            }
        }

        private void HideCursor(object sender, EventArgs e) // Ŀ�� ����� (���� ���� ����)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                // Ŀ�� ���� ���� ����
                MessageBox.Show($"Ŀ�� ����: {chk.Checked}");
            }
        }

        private void SetWheelSensitivity(int sensitivity) // �� ���� ���� (���� ���� ����)
        {
            // �� ���� ���� ���� ����
            MessageBox.Show($"�� ����: {sensitivity}");
        }

        private void TrackBarMouseSpeed_ValueChanged(object sender, EventArgs e)
        {
            TrackBar trackBar = sender as TrackBar;
            if (trackBar != null)
            {

                // ���� ���� �� �� ������Ʈ
                txtSpeedValue.Text = trackBar.Value.ToString();

                // ���콺 �ӵ� ����
                MouseControl mouseControl = new MouseControl();
                mouseControl.SetMouseSpeed(trackBar.Value);
            }
        }

        public class MouseControl
        {
            // SystemParametersInfo �Լ��� ȣ���ϱ� ���� ��� �� DLLImport ����
            private const uint SPI_SETLOGICALDPI = 0x007E;
            private const uint SPI_SETMOUSESPEED = 0x0071;
            private const uint SPIF_SENDCHANGE = 0x02;

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, int pvParam, uint fWinIni);

            public void SetMouseSpeed(int speed)
            {
                // SystemParametersInfo �Լ� ȣ���Ͽ� ���콺 �ӵ� ���� ����
                if (!SystemParametersInfo(SPI_SETMOUSESPEED, 0, speed, SPIF_SENDCHANGE))
                {
                    MessageBox.Show("���콺 �ӵ��� ������ �� �����ϴ�.");
                }
            }

        }

        // ������ ���� �������µ� �ʿ��� Ŭ����
        public class User33
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

            public struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public int showCmd;
                public System.Drawing.Point ptMinPosition;
                public System.Drawing.Point ptMaxPosition;
                public System.Drawing.Rectangle rcNormalPosition;
            }

            public const int SW_SHOWMAXIMIZED = 3;
            public const int SW_SHOWMINIMIZED = 2;
            public const int SW_SHOWNORMAL = 1;
        }



        // ������ ũ�� �������µ� �ʿ��� Ŭ����
        public class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);
        }
    }
}