using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.DataFormats;
using Timer = System.Timers.Timer;



namespace UEP
{
    public partial class Form1 : Form
    {
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        delegate bool EnumThreadDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);


        const uint PROCESS_QUERY_INFORMATION = 0x0400;
        const uint PROCESS_VM_READ = 0x0010;

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

        private DataGridView dataGridView1; // ��ܿ� ���μ��� ���� ���
        private DataGridView dataGridView2; // �ϴܿ� ���μ��� ���� ���
        private ImageList imageList1; // ������ ������ ���� �̹��� ����Ʈ
        private TextBox txtProcessName;
        private Button btnSavePath;
        private Button btnRunPath;
        private Button btnProcessRefresh;
        private Button btnDeletePath;
        private Button deletePreset;
        private ComboBox comboBox;

        private TabControl tabControl1;
        private TabPage tabPage1;

        private TrackBar trackBarMouseSpeed;
        private TextBox txtSpeedValue;
        private Timer inactivityTimer;
        private bool isCursorHidden = false;
        private bool enableHideCursor = true;
        private Point lastMousePosition;



        private string folderPath;
        private string selectFile;

        public Form1()
        {
            InitializeComponent();
            InitializeComponents();



            // �������� ũ�� ���� ����
            this.Size = new Size(1200, 1200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoSize = true;
        }

        // ������ �ڵ�
        private void InitializeComponents()
        {
            imageList1 = new ImageList();
            imageList1.ImageSize = new Size(100, 100);  // ������ ũ�� ����

            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();

            // �׸������ ũ�� ������ ���ϵ��� �ϱ�
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.dataGridView1.AllowUserToResizeColumns = true; // �� ũ�� ���� ���
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.dataGridView2.AllowUserToResizeColumns = true; // �� ũ�� ���� ���
            this.dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // DataGridView ����
            this.dataGridView1.Location = new Point(30, 30);
            this.dataGridView1.Size = new Size(1100, 300);
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.ReadOnly = true;

            this.dataGridView2.Size = new Size(1100, 280);
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.ReadOnly = true;

            // �÷� �߰� �� �ʱ� �ʺ� ����
            AddImageColumn(".", 30, 1);
            AddColumn("App Name", 300, 1);
            AddColumn("Process Name", 200, 1);
            AddColumn("X", 100, 1);
            AddColumn("Y", 100, 1);
            AddColumn("Width", 100, 1);
            AddColumn("Height", 100, 1);
            AddColumn("State", 100, 1);
            AddHideColumn("ExePath", 0, 1);
            AddHideColumn("TxtPath", 0, 1);
            AddHideColumn("FolderPath", 0, 1);

            // �÷� �߰� �� �ʱ� �ʺ� ����
            AddColumn("����", 50, 2);
            AddImageColumn(".", 30, 2);
            AddColumn("App Name", 300, 2);
            AddColumn("Process Name", 200, 2);
            AddColumn("X", 100, 2);
            AddColumn("Y", 100, 2);
            AddColumn("Width", 100, 2);
            AddColumn("Height", 100, 2);
            AddColumn("State", 100, 2);
            AddHideColumn("ExePath", 0, 2);
            AddHideColumn("TxtPath", 0, 2);
            AddHideColumn("FolderPath", 0, 2);

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

            // ���μ��� �ε� (����: �����͸� ���� �߰�)
            LoadProcesses();

            dataGridView1.CellDoubleClick += new DataGridViewCellEventHandler(gridView1_CellClick); // �̰� ��ġ �Űܵ� �Ǵ��� Ȯ���غ���

            dataGridView2.CellDoubleClick += new DataGridViewCellEventHandler(dataGridView2_CellDoubleClick);

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

            // ������(�ؽ�Ʈ����)�� �����ϴ� ��ư            
            deletePreset = new Button();
            deletePreset.Location = new Point(400, 450); // ��ġ ����
            deletePreset.Size = new Size(140, 40); // ũ�� ����
            deletePreset.Text = "�����»���"; // ��ư �ؽ�Ʈ ����
            deletePreset.Click += new EventHandler(deletePreset_Click); // Ŭ�� �̺�Ʈ �ڵ鷯 ����

            // �׸����2 �� ���� ��ư
            btnDeletePath = new Button();
            btnDeletePath.Location = new Point(30, 800); // ��ġ ����
            btnDeletePath.Size = new Size(80, 40); // ũ�� ����
            btnDeletePath.Text = "����"; // ��ư �ؽ�Ʈ ����
            btnDeletePath.Click += new EventHandler(deleteButton_Click); // Ŭ�� �̺�Ʈ �ڵ鷯 ����

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
            this.Controls.Add(btnDeletePath);
            this.Controls.Add(deletePreset);
            // �׸����2�� ù ��° �� �������� �߰�
            tabPage1.Controls.Add(dataGridView2);

            // ���� DataGridView �߰�
            this.Controls.Add(this.dataGridView1);

            // Mouse tab UI ��� �߰�
            InitializeMouseTabComponents(tabPage2);
        }



        private void PrintGridViewData(DataGridView gridView)
        {
            // �׸������ ��� �� ���� ��������
            int rowCount = gridView.Rows.Count;
            int columnCount = gridView.Columns.Count;

            // �� ��� ���� �����͸� �ֿܼ� ���
            for (int row = 0; row < rowCount; row++)
            {
                string rowData = "";
                for (int col = 0; col < columnCount; col++)
                {
                    // ���� �� ��������
                    object cellValue = gridView.Rows[row].Cells[col].Value;

                    // ���� ���� ���ڿ��� ��ȯ�Ͽ� rowData�� �߰�
                    rowData += cellValue?.ToString() + "\t";
                }
                // �� ���� �����͸� �ֿܼ� ���
                Console.WriteLine(rowData);
            }
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
        // �׸���信 ������ ���ڿ� �߰�
        private void AddHideColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.MinimumWidth = 50; // �ּ� �ʺ� ����
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // �ڵ� ũ�� ���� ��Ȱ��ȭ
            column.Visible = false;
            column.Width = 0;

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

        // ������ ���� �Լ�
        private void deletePreset_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    (row.Cells[1].Value as System.Drawing.Image).Dispose();
                }
                dataGridView2.Rows.Clear();

                // ���� ���� ��� ���ϰ� ���� ������ ����
                Directory.Delete(folderPath, true);
                MessageBox.Show("������ �����Ǿ����ϴ�.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ �߻��߽��ϴ�: " + ex.Message);
            }
        }

        // ���μ����� ���ΰ�ħ �ϴ� ��ư�� �̺�Ʈ
        private void btnProcessRefresh_Click(object sender, EventArgs e)
        {
            LoadProcesses();
        }

        // �׸����2�� ���� ����Ŭ�� ������ ���� ���� ���� ����
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
            Form2 detailsForm = new Form2(selectedRow);
            detailsForm.ShowDialog();

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

                folderPath = Path.Combine(exeDirectory, modifiedFile);
                selectFile = filePath;

                List<string> processInfoLines = new List<string>(); // ���μ��� ������ ������ ����Ʈ
                List<string> processPathLines = new List<string>(); // ���μ��� ���� ��θ� ������ ����Ʈ

                foreach (var line in File.ReadLines(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains("//"))
                        {
                            // ���μ��� ���� ����
                            processInfoLines.Add(line);
                        }
                        else
                        {
                            // ���μ��� ���� ��� ����
                            processPathLines.Add(line);
                        }
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
                row.Cells[1].Value = icon;


                // ������ ���� �ؽ�Ʈ ������ �Ҵ�
                for (int i = 2; i < row.Cells.Count; i++)
                {
                    // data �迭�� ���̸� �ʰ����� �ʵ��� ���� Ȯ��
                    if (i < data.Length + 1)
                    {
                        row.Cells[i].Value = data[i - 1];
                    }
                }

                // ������ ���� DataGridView�� �߰�
                dataGridView2.Rows.Add(row);
                UpdateOrderColumn();
            }

            //PrintGridViewData(dataGridView2); //�ܼ��ϰ� �ֿܼ� ����ؼ� Ȯ���ϴ� �뵵
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


        // ���μ����� ��θ� �ؽ�Ʈ ���Ͽ� �����ϴ� �Լ� (���μ��� ������ �Լ��� ��)
        public void SaveProcessPathToFile(string filePath, DataGridView gridView2)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (DataGridViewRow row in gridView2.Rows)
                {
                    if (row.Cells["App Name"].Value != null)
                    {
                        string appName = row.Cells["App Name"].Value.ToString();
                        string processName = row.Cells["Process Name"].Value.ToString();
                        string x = row.Cells["X"].Value.ToString();
                        string y = row.Cells["Y"].Value.ToString();
                        string width = row.Cells["Width"].Value.ToString();
                        string height = row.Cells["Height"].Value.ToString();
                        string state = row.Cells["State"].Value.ToString();
                        string processPath = row.Cells["ExePath"].Value?.ToString(); // �갡 �������� ���

                        sw.WriteLine(processPath); // �������� ��� ����
                        sw.WriteLine($"//{appName}//{processName}//{x}//{y}//{width}//{height}//{state}//{processPath}"); // ���μ��� ����
                        sw.WriteLine(); // ��ĭ ���
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
                var cellValue = row.Cells[1].Value;

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
            // ���μ��� ���� ���� ��ȫ���̸� �������� �ʽ��ϴ�.
            List<string> pinkProcesses = new List<string>();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.Pink)
                {
                    string appName = row.Cells["app name"].Value.ToString();
                    pinkProcesses.Add(appName);
                }
            }

            if (pinkProcesses.Count > 0)
            {
                string message = "��ȿ���� ���� ����� ���μ��� �Դϴ�:\n\n";
                foreach (string appName in pinkProcesses)
                {
                    message += appName + "\n";
                }
                MessageBox.Show(message);
                return;
            }

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


        // �����ư�� ������ �ؽ�Ʈ������ �о���� �Լ�
        private void btnRunPath_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(selectFile))
            {
                string processPath;
                while ((processPath = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(processPath))
                    {
                        Process process = Process.Start(new ProcessStartInfo
                        {
                            FileName = processPath,
                            UseShellExecute = true
                        });

                        //process.WaitForInputIdle(); // ���μ��� â�� ������ ���� ������ ��ٸ�
                        Thread.Sleep(300);

                        string processInfo = sr.ReadLine(); // ���μ��� ���� �б�
                        UpdateProcessInfo(process, processInfo);
                        sr.ReadLine(); // �� �� �ǳʶٱ�
                    }
                }
            }
        }


        // ���μ��� �̸����� ������ â �ڵ鷯 ã�� �Լ�
        public static IntPtr GetWindowHandleByProcessName(string processName)
        {
            IntPtr windowHandle = IntPtr.Zero;

            foreach (Process p in Process.GetProcessesByName(processName))
            {
                foreach (ProcessThread thread in p.Threads)
                {
                    EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                    {
                        uint processId;
                        GetWindowThreadProcessId(hWnd, out processId);
                        if (processId == (uint)p.Id)
                        {
                            windowHandle = hWnd;
                            return false; // ������ �ڵ��� ã�����Ƿ� ���� �ߴ�
                        }
                        return true;
                    }, IntPtr.Zero);

                    if (windowHandle != IntPtr.Zero)
                        break; // ������ �ڵ��� ã�����Ƿ� ���μ��� ���� �ߴ�
                }
            }

            return windowHandle;
        }


        // ���α׷� ���� ��, ������ â�� ��ġ, ũ��, ���¸� �����ϴ� �Լ�
        private void UpdateProcessInfo(Process process, string processInfo)
        {

            if (process != null)
            {
                string[] info = processInfo.Split("//");

                if (info.Length == 9)
                {
                    int x = int.Parse(info[3]);
                    int y = int.Parse(info[4]);
                    int width = int.Parse(info[5]);
                    int height = int.Parse(info[6]);
                    string state = info[7]; // ���� ��

                    string name = info[2];


                    Console.WriteLine(" //x�� : " + x + " //y�� : " + y + " //���� : " + width + " //���� : " + height + " //���� : " + state);


                    Process[] processes = Process.GetProcessesByName(name);
                    Process p = processes[0];

                    List<IntPtr> handles = new List<IntPtr>();

                    foreach (ProcessThread thread in p.Threads)
                    {
                        EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

                    }

                    var processName = name;
                    var windowHandles = GetWindowHandleByProcessName(name); // �̰� ��¥�� �̰� ��¥ �ڵ鷯��

                    Console.WriteLine("���μ����̸� : " + processName);
                    Console.WriteLine("�ڵ鷯 : " + windowHandles);



                    // ���μ��� â ũ�� ����
                    SetWindowPos(windowHandles, IntPtr.Zero, x, y, width, height, 0);

                    // ���μ��� â ���� ����
                    switch (state)
                    {
                        case "Min":
                            ShowWindow(windowHandles, 6); // SW_MINIMIZE
                            break;
                        case "Normal":
                            ShowWindow(windowHandles, 1); // SW_SHOWNORMAL
                            break;
                        case "Max":
                            ShowWindow(windowHandles, 3); // SW_MAXIMIZE
                            break;
                    }
                }
            }
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
                    // �׸����1�� ���� ��ȸ�ϸ鼭 �׸����2�� �ε��� ���� ���մϴ�.
                    // ���⼭ i�� �׸����1�� �ε���, i+1�� �׸����2�� �ε����� �ش��մϴ�.
                    for (int i = 0; i < selectedRow.Cells.Count; i++)
                    {
                        // �׸����2�� �� �ε����� �׸����1�� �� �ε������� �ϳ� �� �����ϴ�.
                        int gridView2Index = i + 1;

                        // �ش� ���� ���� �ٸ���, �� ���� �ߺ����� �ʽ��ϴ�.
                        if (row.Cells[gridView2Index].Value?.ToString() != selectedRow.Cells[i].Value?.ToString())
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
                        dataGridView2.Rows[newRow].Cells[cell.ColumnIndex + 1].Value = cell.Value; // +1�� ���� ���� ����� ��
                    }

                    // 'App Name'�� ���� ���� ���� ���μ����� 'WindowTitle'�� ��ġ�ϴ��� Ȯ���մϴ�.
                    string appName = selectedRow.Cells["App Name"].Value.ToString();
                    bool isAppNameMatched = false;

                    // ���� ���� ���� ��� ���μ����� �˻��մϴ�.
                    foreach (Process process in Process.GetProcesses())
                    {
                        try
                        {
                            if (process.MainWindowTitle.Contains(appName) && process.MainModule.FileName != null)
                            {
                                isAppNameMatched = true;
                                break;
                            }
                        }
                        catch
                        {
                            // �׼��� �źο� ���� ���ܸ� ó���մϴ�.
                        }
                    }

                    // 'App Name'�� ���� ���� ���μ����� 'WindowTitle'�� ��ġ���� �ʴ� ���, ���� ��ȫ������ ��ĥ�մϴ�.
                    //if (!isAppNameMatched)
                    //{

                    //    dataGridView2.Rows[newRow].DefaultCellStyle.BackColor = Color.Pink;
                    //}

                    // �߰��� ���� ������ �����մϴ�.
                    UpdateOrderColumn();
                }
            }
        }



        // ������ư�� ������ �׸����2�� ���� �����ϴ� �Լ�
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // ���õ� ���� �ִ��� Ȯ���մϴ�.
            if (dataGridView2.SelectedCells.Count > 0)
            {
                // ������ ���� ������ ����Ʈ�� �����մϴ�.
                List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();

                // ���õ� ���� ���� ����Ʈ�� �߰��մϴ� (�� ���� ����).
                foreach (DataGridViewCell cell in dataGridView2.SelectedCells)
                {
                    DataGridViewRow row = cell.OwningRow;
                    if (!row.IsNewRow && !rowsToDelete.Contains(row))
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // ����Ʈ�� �ִ� ����� �����մϴ�.
                foreach (DataGridViewRow row in rowsToDelete)
                {
                    dataGridView2.Rows.Remove(row);
                }

                // ���� ������ �� ���� ���� ������Ʈ�մϴ�.
                UpdateOrderColumn();
            }
            else
            {
                MessageBox.Show("������ ���� ���� �������ּ���.", "�˸�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // �߰��� ���� ������ �����ϴ� �Լ�
        private void UpdateOrderColumn()
        {
            int order = 1;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow) // �� ���� �ƴ� ��쿡�� ������ ������Ʈ�մϴ�.
                {
                    row.Cells["����"].Value = order++;
                }
            }
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


        // �׸����1�� �������� �������� �Լ�
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

                }

                // ������ �ڵ� ����
                if (largeIcon != IntPtr.Zero) DestroyIcon(largeIcon);
                if (smallIcon != IntPtr.Zero) DestroyIcon(smallIcon);
            }
        }


        // �׸����1�� ���μ��� ������ �������� �Լ�
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
                        2 => "Min",
                        3 => "Max",
                        _ => "Unknown"
                    };


                    // â ���°� �ּ�ȭ�� �� ��ǥ���� 0���� ����
                    if (windowState == "Min")
                    {
                        rect.Left = 0;
                        rect.Top = 0;
                        rect.Right = 0; // �ʺ� 0���� �����ϰų� �ʿ信 ���� ����
                        rect.Bottom = 0; // ���̸� 0���� �����ϰų� �ʿ信 ���� ����
                    }


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

                    string processPath = "Unknown";
                    try
                    {
                        Process process = Process.GetProcessById(processId);
                        processPath = process.MainModule.FileName;
                    }
                    catch (Exception ex)
                    {
                        processPath = $"Error: {ex.Message}";
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
                    row.Cells[8].Value = processPath; // �������� ��� �ֱ�
                    row.Cells[9].Value = "�ؽ�Ʈ���"; // �������� ��� �ֱ�
                    row.Cells[10].Value = "�������"; // �������� ��� �ֱ�

                    dataGridView1.Rows.Add(row);

                    //Console.WriteLine($"Handle: {hWnd}, Title: {sb}, Process Name: {processName}, X: {rect.Left}, Y: {rect.Top}, Width: {rect.Right - rect.Left}, Height: {rect.Bottom - rect.Top}, State: {windowState}");
                }
            }
            return true; // Return true to continue enumerating the next window
        }


        // ���ʿ� ���α׷��� �����ϸ� �׸����1�� ������ �������� �Լ�
        private void LoadProcesses()
        {
            dataGridView1.Rows.Clear(); // DataGridView ������ Ŭ����
            EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);
        }







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
            CheckBox chkHideCursor = new CheckBox
            {
                Text = "Ŀ�� ����",
                Location = new Point(20, 100),
                Size = new Size(100, 20)
            };
            chkHideCursor.CheckedChanged += HideCursor;
            // Initialize timer
            inactivityTimer = new Timer(3000); // 3 seconds
            inactivityTimer.Elapsed += OnInactivityTimerElapsed;

            // Set up mouse hook
            HookManager.MouseMove += HookManager_MouseMove;
            HookManager.Start();

            inactivityTimer.Start();
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

        private void HideCursor(object sender, EventArgs e) // Ŀ�� �����
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                enableHideCursor = chk.Checked;
                if (!enableHideCursor && isCursorHidden)
                {
                    ShowCursor(true);
                    isCursorHidden = false;
                }
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
        private void OnInactivityTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (enableHideCursor && !isCursorHidden)
            {
                if (Cursor.Position == lastMousePosition)
                {
                    Invoke(new Action(() =>
                    {
                        ShowCursor(false);
                        isCursorHidden = true;
                    }));
                }
                else
                {
                    lastMousePosition = Cursor.Position;
                }
            }
        }
        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCursorHidden)
            {
                ShowCursor(true);
                isCursorHidden = false;
            }
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            HookManager.MouseMove -= HookManager_MouseMove;
            HookManager.Stop();
            inactivityTimer.Dispose();
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
    }
    public static class HookManager
    {
        // Hooking constants
        private const int WH_MOUSE_LL = 14;

        // Mouse events
        public static event MouseEventHandler MouseMove = delegate { };
        public static event MouseEventHandler MouseClick = delegate { };
        public static event MouseEventHandler MouseDown = delegate { };
        public static event MouseEventHandler MouseUp = delegate { };

        private static LowLevelMouseProc _mouseProc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static void Start()
        {
            _hookID = SetHook(_mouseProc);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var wmMouse = (MouseMessages)wParam;
                if (wmMouse == MouseMessages.WM_MOUSEMOVE)
                {
                    var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    MouseMove.Invoke(null, new MouseEventArgs(MouseButtons.None, 0, hookStruct.pt.x, hookStruct.pt.y, 0));
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private enum MouseMessages
        {
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }
    }
    public class ProcessUtility
    {
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static Process GetProcessByHandle(IntPtr handle)
        {
            uint processId;
            GetWindowThreadProcessId(handle, out processId);
            return Process.GetProcessById((int)processId);
        }
    }
}
