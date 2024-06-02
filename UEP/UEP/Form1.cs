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

            this.AutoScaleMode = AutoScaleMode.Font; // 폰트크기 자동조절이라는데 되는건지 모르겠다
            this.Resize += new EventHandler(Form1_Resize); // 폰트 크기 조절용
        }

        // 디자인 코드
        private void InitializeComponents()
        {
            imageList1 = new ImageList();
            imageList1.ImageSize = new Size(100, 100);  // 아이콘 크기 설정

            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();

            // 탭 컨트롤 생성
            tabControl1 = new TabControl();

            // 탭 컨트롤의 위치 및 크기 설정
            tabControl1.Location = new Point(30, 500); // 위치
            tabControl1.Size = new Size(1100, 300); // 크기

            // 탭 컨트롤을 폼에 추가
            this.Controls.Add(tabControl1);

            // 탭 페이지 추가
            TabPage tabPage1 = new TabPage("SoftWare");
            TabPage tabPage2 = new TabPage("Mouse");
            TabPage tabPage3 = new TabPage("Display");
            TabPage tabPage4 = new TabPage("Sound");

            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);

            // 그리드뷰2를 첫 번째 탭 페이지에 추가
            tabPage1.Controls.Add(dataGridView2);

            // DataGridView 설정
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


            // 컬럼 추가 및 초기 너비 설정
            AddImageColumn(".", 60, 1); // 이미지 컬럼 추가
            AddColumn("App Name", 300, 1);
            AddColumn("Process Name", 200, 1);
            AddColumn("X", 100, 1);
            AddColumn("Y", 100, 1);
            AddColumn("Width", 100, 1);
            AddColumn("Height", 100, 1);
            AddColumn("State", 100, 1);

            // 컬럼 추가 및 초기 너비 설정
            AddImageColumn(".", 60, 2); // 이미지 컬럼 추가
            AddColumn("App Name", 300, 2);
            AddColumn("Process Name", 200, 2);
            AddColumn("X", 100, 2);
            AddColumn("Y", 100, 2);
            AddColumn("Width", 100, 2);
            AddColumn("Height", 100, 2);
            AddColumn("State", 100, 2);

            // 폼에 DataGridView 추가
            this.Controls.Add(this.dataGridView1);



            // 프로세스 로드 (예시: 데이터를 직접 추가)
            LoadProcesses();

            dataGridView1.CellDoubleClick += new DataGridViewCellEventHandler(gridView1_CellClick); // 이거 위치 옮겨도 되는지 확인해보자

            // 텍스트 박스 설정
            txtProcessName = new TextBox();
            txtProcessName.Location = new Point(30, 350); // 위치 설정
            txtProcessName.Size = new Size(200, 20); // 크기 설정

            // 버튼 설정
            btnSavePath = new Button();
            btnSavePath.Location = new Point(240, 350); // 위치 설정
            btnSavePath.Size = new Size(80, 40); // 크기 설정
            btnSavePath.Text = "Save"; // 버튼 텍스트 설정
            btnSavePath.Click += new EventHandler(btnSavePath_Click); // 클릭 이벤트 핸들러 연결

            // 프로세스목록을 새로고침하는 버튼
            btnProcessRefresh = new Button();
            btnProcessRefresh.Location = new Point(800, 350); // 위치 설정
            btnProcessRefresh.Size = new Size(80, 40); // 크기 설정
            btnProcessRefresh.Text = "리로드"; // 버튼 텍스트 설정
            btnProcessRefresh.Click += new EventHandler(btnProcessRefresh_Click); // 클릭 이벤트 핸들러 연결

            // 버튼 설정            
            btnRunPath = new Button();
            btnRunPath.Location = new Point(240, 450); // 위치 설정
            btnRunPath.Size = new Size(80, 40); // 크기 설정
            btnRunPath.Text = "실행"; // 버튼 텍스트 설정
            btnRunPath.Click += new EventHandler(btnRunPath_Click); // 클릭 이벤트 핸들러 연결

            // 프리셋 목록 보여주기
            comboBox = new ComboBox();
            comboBox.Location = new Point(30, 450);
            comboBox.Size = new Size(200, 20);
            comboBox.Text = "프리셋 목록";
            comboBox.Click += new EventHandler(LoadTextFilesToComboBox);
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);


            // 텍스트 박스와 버튼을 폼에 추가
            this.Controls.Add(txtProcessName);
            this.Controls.Add(btnSavePath);
            this.Controls.Add(btnRunPath);
            this.Controls.Add(comboBox);
            this.Controls.Add(btnProcessRefresh);

            // Mouse tab UI 요소 추가
            InitializeMouseTabComponents(tabPage2);
        }


        // 그리드뷰에 문자열 추가
        private void AddColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.MinimumWidth = 50; // 최소 너비 설정
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // 자동 크기 조정 비활성화
            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }

        }


        // 그리드뷰에 이미지 추가
        private void AddImageColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewImageColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.ImageLayout = DataGridViewImageCellLayout.Zoom; // 이미지 레이아웃 설정
            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }
        }


        // 폰트 관련 함수
        private void Form1_Resize(object sender, EventArgs e)
        {
            float scaleFactor = Math.Min((float)this.ClientSize.Width / 1800, (float)this.ClientSize.Height / 1600);
            AdjustControlFonts(this, scaleFactor);
        }


        // 폰트 관련 함수
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


        // 프로세스를 새로고침 하는 버튼의 이벤트
        private void btnProcessRefresh_Click(object sender, EventArgs e)
        {
            LoadProcesses();
        }


        // 콤보박스의 메뉴를 선택했을 경우, 경로를 찾고 텍스트파일을 읽어서 프로세스 저장 정보를 가져온다
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex != -1) // 선택된 항목이 있는지 확인합니다.
            {
                string selectedFile = comboBox.SelectedItem.ToString();
                Console.WriteLine(selectedFile);


                string modifiedFile = selectedFile.Substring(0, selectedFile.Length - 4);

                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exeLocation);
                string filePath = Path.Combine(exeDirectory, modifiedFile + "\\" + selectedFile);
                Console.WriteLine(filePath);

                selectFile = filePath;

                List<string> processInfoLines = new List<string>(); // 처리할 정보를 저장할 리스트입니다.
                int blankLineCount = 0; // 공백 행의 카운트를 저장합니다.

                foreach (var line in File.ReadLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        blankLineCount++; // 공백 행이면 카운트를 증가시킵니다.
                    }
                    else if (blankLineCount >= 3)
                    {
                        processInfoLines.Add(line); // 3개의 공백 행 이후의 데이터를 리스트에 추가합니다.
                    }

                    if (!string.IsNullOrWhiteSpace(line) && blankLineCount < 3)
                    {
                        blankLineCount = 0; // 공백이 아닌 행을 만나고 3개 미만의 공백 행이었으면 카운트를 리셋합니다.
                    }
                }

                DisplayProcessInfoInGridView(processInfoLines, modifiedFile); // 데이터를 그리드 뷰에 표시합니다.
            }
        }


        // 콤보박스의 메뉴를 선택했을 경우, 그리드뷰2에 프로세스 정보를 업로드한다
        private void DisplayProcessInfoInGridView(List<string> processInfoLines, string modifiedFile)
        {
            dataGridView2.Rows.Clear(); // 기존의 내용을 지우고 새 정보를 표시합니다.
            int imageNum = 0;
            foreach (string line in processInfoLines)
            {
                // line 변수에 저장된 텍스트를 "//" 기준으로 분할
                string[] data = line.Split(new string[] { "//" }, StringSplitOptions.None);

                // DataGridViewRow 객체 생성
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView2);


                // 현재 실행 중인 어셈블리의 위치를 얻음
                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // 어셈블리의 디렉터리를 얻음
                string exeDirectory = Path.GetDirectoryName(exeLocation);
                // 이미지 파일의 상대 경로를 추가하여 전체 경로를 구성
                string imagePath = Path.Combine(exeDirectory, modifiedFile, imageNum.ToString() + ".png");
                imageNum++;

                // 이미지 파일을 로드
                System.Drawing.Image icon = System.Drawing.Image.FromFile(imagePath);

                // 로드한 이미지를 DataGridView의 셀에 할당
                row.Cells[0].Value = icon;


                // 나머지 열에 텍스트 데이터 할당
                for (int i = 1; i < row.Cells.Count; i++)
                {
                    // data 배열의 길이를 초과하지 않도록 조건 확인
                    if (i < data.Length)
                    {
                        row.Cells[i].Value = data[i];
                    }
                }

                // 설정된 행을 DataGridView에 추가
                dataGridView2.Rows.Add(row);
            }
        }


        // 콤보박스에 지정 디렉토리에 있는 모든 텍스트 파일을 가져오는 함수
        private void LoadTextFilesToComboBox(object sender, EventArgs e)
        {
            comboBox.Items.Clear(); // 기존 목록을 비웁니다.

            // 현재 실행 중인 어셈블리의 위치를 얻음
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // 어셈블리의 디렉터리를 얻음
            string exeDirectory = Path.GetDirectoryName(exeLocation);
            // 이미지 파일의 상대 경로를 추가하여 전체 경로를 구성
            string directoryPath = Path.Combine(exeDirectory);


            // "[FILE]"로 시작하는 모든 폴더를 가져옵니다.
            string[] directories = Directory.GetDirectories(directoryPath, "[FILE]*");

            foreach (string directory in directories)
            {
                // 각 폴더 내의 모든 .txt 파일을 가져옵니다.
                string[] files = Directory.GetFiles(directory, "*.txt");

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    comboBox.Items.Add(fileInfo.Name); // 파일 이름을 ComboBox에 추가합니다.
                }
            }
        }


        // 그리드뷰2의 프로세스 실행파일 정보를 텍스트 파일에 저장하는 함수
        static void SaveProcessPathToFile(string filePath, DataGridView gridView2)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (DataGridViewRow row in gridView2.Rows)
                {
                    if (row.Cells["App Name"].Value != null) // "MainWindowTitle"은 메인 윈도우 타이틀이 표시되는 컬럼의 이름입니다.
                    {
                        string windowTitle = row.Cells["App Name"].Value.ToString();

                        // 시스템에서 실행 중인 모든 프로세스를 가져옵니다.
                        Process[] processes = Process.GetProcesses();

                        foreach (Process prs in processes)
                        {
                            // 메인 윈도우 타이틀이 사용자가 지정한 값과 일치하는지 확인합니다.
                            if (prs.MainWindowTitle == windowTitle)
                            {
                                try
                                {
                                    // 프로세스의 실행 파일 경로를 가져옵니다.
                                    string processPath = prs.MainModule.FileName;

                                    // 실행 파일 경로를 파일에 씁니다.
                                    sw.WriteLine($"{processPath}");
                                    break; // 일치하는 첫 번째 프로세스를 찾았으므로 반복을 중단합니다.
                                }
                                catch (Exception ex)
                                {
                                    // 접근할 수 없는 프로세스는 "접근 불가"로 표시합니다.
                                    sw.WriteLine($"WindowTitle: {windowTitle}, 경로: 접근 불가 - {ex.Message}");
                                }
                            }
                        }
                    }
                }
                // 행 정보를 파일에 추가합니다.
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


        // 파일에 아이콘을 저장하는 함수
        static void SaveProcessIconToFile(string folderPath, DataGridView gridView2)
        {
            int imageIndex = 0; // 파일명에 사용될 인덱스입니다.

            foreach (DataGridViewRow row in gridView2.Rows)
            {
                // DataGridView의 첫 번째 열에서 특정 행의 셀 이미지를 가져옵니다.
                var cellValue = row.Cells[0].Value;

                // 셀의 값이 null이 아니고, Image 타입인지 확인합니다.
                if (cellValue != null && cellValue is System.Drawing.Image)
                {
                    // 이미지 파일로 저장합니다.
                    System.Drawing.Image img = (System.Drawing.Image)cellValue;

                    string imagePath = Path.Combine(folderPath, $"{imageIndex}.png"); // PNG 파일로 저장
                    img.Save(imagePath, ImageFormat.Png); // ImageFormat에 따라 다른 형식으로 저장할 수 있습니다.

                    imageIndex++; // 다음 이미지에 대한 인덱스를 증가시킵니다.
                }
                else
                {
                    // 적절한 예외 처리나 기본값 반환
                }
            }
        }



        // 저장 버튼을 누르면 프로세스의 경로가 텍스트 파일 형태로 저장되는 함수
        private void btnSavePath_Click(object sender, EventArgs e)
        {
            // 프로세스 이름을 기반으로 폴더 및 파일 이름을 지정합니다.
            string folderName = "[FILE]_" + txtProcessName.Text;
            string fileName = folderName + ".txt";
            string folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath, folderName); // 애플리케이션 실행 경로에 폴더 생성
            string filePath = Path.Combine(folderPath, fileName); // 최종 파일 경로

            // 지정된 경로에 폴더가 없다면 생성합니다.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // 프로세스 경로를 파일에 저장하는 메서드를 호출합니다.
            SaveProcessPathToFile(filePath, dataGridView2); // 굳이 인자로 넣어야 하나? 나중에 확인
            SaveProcessIconToFile(folderPath, dataGridView2);

            MessageBox.Show("저장이 완료되었습니다.");
        }



        // 실행파일의 경로를 참조하여 파일을 실행시키는 함수
        // 실험함수임. 나중에 프리셋 기능에 추가 필요
        private void btnRunPath_Click(object sender, EventArgs e)
        {
            try
            {
                // 파일에서 각 줄을 읽어온다.
                string[] applicationPaths = File.ReadAllLines(selectFile);
                int blankLineCount = 0;

                foreach (string appPath in applicationPaths)
                {
                    // 공백 행인지 확인
                    if (string.IsNullOrWhiteSpace(appPath))
                    {
                        blankLineCount++;
                    }
                    else
                    {
                        // 공백이 아닌 행을 찾으면 공백 행 카운트를 초기화
                        blankLineCount = 0;
                    }

                    // 공백 행이 3개가 되면 다른 작업 수행
                    if (blankLineCount == 3)
                    {
                        PerformOtherTask();
                        break;
                    }

                    // 공백 행이 3개가 아닐 때만 애플리케이션 실행
                    if (blankLineCount < 3)
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = appPath,
                            UseShellExecute = true // Shell을 사용하여 파일 실행
                        };

                        // 애플리케이션 실행
                        Process.Start(psi);
                    }
                }
            }
            catch (Exception ex)
            {
                // 오류 처리
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void PerformOtherTask()
        {
            // 공백 행이 3개가 나온 후 수행할 작업
            Console.WriteLine("Performing other tasks...");
            // 여기에 다른 작업을 구현하세요.
        }



        // 1번 뷰의 행을 더블클릭 하면 2번 뷰로 정보를 옮기는 함수
        private void gridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // 유효한 행 인덱스인지 확인합니다.
            {
                // 첫 번째 그리드 뷰의 선택된 행을 가져옵니다.
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // 두 번째 그리드 뷰에 동일한 행이 있는지 확인합니다.
                bool isDuplicate = false;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    bool allCellsMatch = true;
                    foreach (DataGridViewCell cell in selectedRow.Cells)
                    {
                        // 해당 열의 값이 다르면, 이 행은 중복되지 않습니다.
                        if (row.Cells[cell.ColumnIndex].Value.ToString() != cell.Value.ToString())
                        {
                            allCellsMatch = false;
                            break;
                        }
                    }

                    if (allCellsMatch)
                    {
                        // 모든 셀이 일치하면, 중복된 행입니다.
                        isDuplicate = true;
                        break;
                    }
                }

                // 중복되지 않은 경우에만 새 행을 추가합니다.
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


        // 윈도우의 상태를 가져오는 함수
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

        // 필요한 외부 함수 선언
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

        // P/Invoke 선언
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
                uint bufferSize = (uint)buffer.Capacity + 1; // 버퍼 크기 수정
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
                    // Icon을 ImageList에 추가
                    imageList1.Images.Add(icon.ToBitmap());
                    // 선택적으로, windowTitle을 사용하여 각 이미지에 키를 할당할 수 있습니다.
                    // 예: imageList.Images.Add(windowTitle, icon.ToBitmap());

                    Console.WriteLine($"Icon added to ImageList: {windowTitle}");
                }

                // 아이콘 핸들 정리
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

                    // 프로세스 ID 가져오기
                    GetWindowThreadProcessId(hWnd, out int processId);

                    // 프로세스 이름 가져오기
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
            dataGridView1.Rows.Clear(); // DataGridView 내용을 클리어
            EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);
        }


        //private void LoadProcesses()
        //{

        //    EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);


        //    dataGridView1.Rows.Clear(); // DataGridView 내용을 클리어
        //    Process[] processes = Process.GetProcesses();
        //    foreach (Process prs in processes)
        //    {
        //        // MainWindowTitle이 비어있지 않은 프로세스만 필터링
        //        if (!string.IsNullOrEmpty(prs.MainWindowTitle))
        //        {
        //            int imageIndex;


        //            try
        //            {
        //                // 프로세스의 메인 모듈로부터 아이콘 추출 시도
        //                Icon icon = Icon.ExtractAssociatedIcon(prs.MainModule.FileName);
        //                // Icon을 Image 형태로 변환
        //                Bitmap iconBitmap = icon.ToBitmap();
        //                imageList1.Images.Add(iconBitmap); // ImageList에 아이콘 추가
        //                imageIndex = imageList1.Images.Count - 1; // 추가된 아이콘의 인덱스

        //            }
        //            catch (Exception ex)
        //            {
        //                // 추출 실패 시 기본 Error 아이콘 사용
        //                imageList1.Images.Add(SystemIcons.Error);
        //                imageIndex = imageList1.Images.Count - 1; // 추가된 Error 아이콘의 인덱스

        //            }

        //            // 데이터 그리드뷰에 행 추가
        //            var row = new DataGridViewRow();
        //            row.CreateCells(dataGridView1);

        //            // 아이콘을 이미지 셀에 추가
        //            row.Cells[0].Value = imageList1.Images[imageIndex];

        //            // 프로세스 이름과 아이콘 추가
        //            row.Cells[1].Value = prs.MainWindowTitle;
        //            row.Cells[2].Value = prs.ProcessName;

        //            // 윈도우 상태 정보 추가
        //            string windowState = GetWindowState(prs.MainWindowHandle);
        //            row.Cells[7].Value = windowState;



        //            // 윈도우 위치 및 크기 정보 추가
        //            try
        //            {
        //                var rect = new User32.Rect();
        //                User32.GetWindowRect(prs.MainWindowHandle, ref rect);
        //                row.Cells[3].Value = rect.Left.ToString(); // X축 위치
        //                row.Cells[4].Value = rect.Top.ToString(); // Y축 위치
        //                row.Cells[5].Value = (rect.Right - rect.Left).ToString(); // 가로 크기
        //                row.Cells[6].Value = (rect.Bottom - rect.Top).ToString(); // 세로 크기
        //            }
        //            catch
        //            {
        //                row.Cells[3].Value = "N/A"; // X축 위치
        //                row.Cells[4].Value = "N/A"; // Y축 위치
        //                row.Cells[5].Value = "N/A"; // 가로 크기
        //                row.Cells[6].Value = "N/A"; // 세로 크기
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

        //                // 프로세스 ID 가져오기
        //                GetWindowThreadProcessId(hWnd, out int processId);

        //                // 프로세스 이름 가져오기
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

        private void InitializeMouseTabComponents(TabPage tabPageMouse) // 전체적인 마우스 탭 관리
        {
            // 마우스 속도 조절 관련
            System.Windows.Forms.Label lblMouseSpeed = new System.Windows.Forms.Label();
            lblMouseSpeed.Text = "마우스 속도 조절:";
            lblMouseSpeed.Location = new Point(20, 20);     // 위치
            lblMouseSpeed.Size = new Size(120, 20);         // 크기

            TrackBar trackBarMouseSpeed = new TrackBar();
            trackBarMouseSpeed.Location = new Point(150, 20); // 위치
            trackBarMouseSpeed.Size = new Size(250, 45);        // 크기
            trackBarMouseSpeed.Minimum = 1;
            trackBarMouseSpeed.Maximum = 20;
            trackBarMouseSpeed.TickFrequency = 1;
            trackBarMouseSpeed.ValueChanged += TrackBarMouseSpeed_ValueChanged;

            // 텍스트 박스로 트랙바 값 표시
            txtSpeedValue = new TextBox();
            txtSpeedValue.ReadOnly = true; // 읽기 전용으로 설정하여 사용자가 텍스트를 편집하지 못하도록 함
            txtSpeedValue.Location = new Point(410, 20);
            txtSpeedValue.Size = new Size(20, 20);
            txtSpeedValue.TextAlign = HorizontalAlignment.Center;
            txtSpeedValue.Text = trackBarMouseSpeed.Value.ToString(); // 초기값 설정

            // 마우스 반전 기능 (아직 구현 안함)
            CheckBox chkInvertMouse = new CheckBox();
            chkInvertMouse.Text = "마우스 반전";
            chkInvertMouse.Location = new Point(20, 60);
            chkInvertMouse.Size = new Size(100, 20);
            chkInvertMouse.CheckedChanged += new EventHandler(InvertMouse);

            // 마우스 가만히 두면 커서 없어지는 기능 (아직 구현 안함)
            CheckBox chkHideCursor = new CheckBox();
            chkHideCursor.Text = "커서 숨김";
            chkHideCursor.Location = new Point(20, 100);
            chkHideCursor.Size = new Size(100, 20);
            chkHideCursor.CheckedChanged += new EventHandler(HideCursor);

            // 마우스 휠 감도 기능 (아직 구현 안함)
            System.Windows.Forms.Label lblWheelSensitivity = new System.Windows.Forms.Label();
            lblWheelSensitivity.Text = "휠 감도:";
            lblWheelSensitivity.Location = new Point(20, 140);
            lblWheelSensitivity.Size = new Size(100, 20);

            TrackBar trackBarWheelSensitivity = new TrackBar();
            trackBarWheelSensitivity.Location = new Point(130, 140);
            trackBarWheelSensitivity.Size = new Size(170, 45);
            trackBarWheelSensitivity.Minimum = 1;
            trackBarWheelSensitivity.Maximum = 10;
            trackBarWheelSensitivity.TickFrequency = 1;
            trackBarWheelSensitivity.ValueChanged += new EventHandler((sender, e) => SetWheelSensitivity(trackBarWheelSensitivity.Value));

            // UI 요소를 탭 페이지에 추가
            tabPageMouse.Controls.Add(lblMouseSpeed);
            tabPageMouse.Controls.Add(trackBarMouseSpeed);
            tabPageMouse.Controls.Add(txtSpeedValue);
            tabPageMouse.Controls.Add(chkInvertMouse);
            tabPageMouse.Controls.Add(chkHideCursor);
            tabPageMouse.Controls.Add(lblWheelSensitivity);
            tabPageMouse.Controls.Add(trackBarWheelSensitivity);


        }
        private void InvertMouse(object sender, EventArgs e) // 마우스 반전 (아직 구현 안함)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                // 마우스 반전 로직 구현
                MessageBox.Show($"마우스 반전: {chk.Checked}");
            }
        }

        private void HideCursor(object sender, EventArgs e) // 커서 숨기기 (아직 구현 안함)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                // 커서 숨김 로직 구현
                MessageBox.Show($"커서 숨김: {chk.Checked}");
            }
        }

        private void SetWheelSensitivity(int sensitivity) // 휠 감도 조절 (아직 구현 안함)
        {
            // 휠 감도 설정 로직 구현
            MessageBox.Show($"휠 감도: {sensitivity}");
        }

        private void TrackBarMouseSpeed_ValueChanged(object sender, EventArgs e)
        {
            TrackBar trackBar = sender as TrackBar;
            if (trackBar != null)
            {

                // 진행 막대 바 값 업데이트
                txtSpeedValue.Text = trackBar.Value.ToString();

                // 마우스 속도 조절
                MouseControl mouseControl = new MouseControl();
                mouseControl.SetMouseSpeed(trackBar.Value);
            }
        }

        public class MouseControl
        {
            // SystemParametersInfo 함수를 호출하기 위한 상수 및 DLLImport 선언
            private const uint SPI_SETLOGICALDPI = 0x007E;
            private const uint SPI_SETMOUSESPEED = 0x0071;
            private const uint SPIF_SENDCHANGE = 0x02;

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, int pvParam, uint fWinIni);

            public void SetMouseSpeed(int speed)
            {
                // SystemParametersInfo 함수 호출하여 마우스 속도 설정 변경
                if (!SystemParametersInfo(SPI_SETMOUSESPEED, 0, speed, SPIF_SENDCHANGE))
                {
                    MessageBox.Show("마우스 속도를 변경할 수 없습니다.");
                }
            }

        }

        // 윈도우 상태 가져오는데 필요한 클래스
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



        // 윈도우 크기 가져오는데 필요한 클래스
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