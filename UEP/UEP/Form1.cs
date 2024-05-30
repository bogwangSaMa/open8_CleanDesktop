using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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
            imageList1.ImageSize = new Size(150, 150);  // 아이콘 크기 설정

            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();

            // DataGridView 설정
            this.dataGridView1.Location = new Point(30, 30);
            this.dataGridView1.Size = new Size(1100, 300);

            this.dataGridView2.Location = new Point(30, 500);
            this.dataGridView2.Size = new Size(1100, 300);

            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = true;
            this.dataGridView1.RowHeadersVisible = false;

            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = true;
            this.dataGridView2.RowHeadersVisible = false;

            //this.dataGridView2.AllowDrop = true;
            //this.dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //this.dataGridView2.TabIndex = 0;
            //this.dataGridView2.MouseDown += new MouseEventHandler(this.dataGridView2_MouseDown);
            //this.dataGridView2.MouseMove += new MouseEventHandler(this.dataGridView2_MouseMove);
            //this.dataGridView2.DragOver += new DragEventHandler(this.dataGridView2_DragOver);
            //this.dataGridView2.DragDrop += new DragEventHandler(this.dataGridView2_DragDrop);

            //// 
            //// Form1
            //// 
            //this.AutoScaleDimensions = new SizeF(6F, 13F);
            //this.AutoScaleMode = AutoScaleMode.Font;
            //this.ClientSize = new Size(800, 450);
            //this.Controls.Add(this.dataGridView1);
            //this.Name = "Form1";
            //this.Text = "Form1";
            //((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            //this.ResumeLayout(false);



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
            this.Controls.Add(this.dataGridView2);

            // 프로세스 로드 (예시: 데이터를 직접 추가)
            LoadProcesses();

            dataGridView1.CellDoubleClick += new DataGridViewCellEventHandler(gridView1_CellClick); // 이거 위치 옮겨도 되는지 확인해보자

            // 텍스트 박스 설정
            txtProcessName = new TextBox();
            txtProcessName.Location = new System.Drawing.Point(30, 350); // 위치 설정
            txtProcessName.Size = new System.Drawing.Size(200, 20); // 크기 설정

            // 버튼 설정
            btnSavePath = new Button();
            btnSavePath.Location = new System.Drawing.Point(240, 350); // 위치 설정
            btnSavePath.Size = new System.Drawing.Size(80, 40); // 크기 설정
            btnSavePath.Text = "Save"; // 버튼 텍스트 설정
            btnSavePath.Click += new EventHandler(btnSavePath_Click); // 클릭 이벤트 핸들러 연결



            // 프로세스목록을 새로고침하는 버튼
            btnProcessRefresh = new Button();
            btnProcessRefresh.Location = new System.Drawing.Point(800, 350); // 위치 설정
            btnProcessRefresh.Size = new System.Drawing.Size(80, 40); // 크기 설정
            btnProcessRefresh.Text = "리로드"; // 버튼 텍스트 설정
            btnProcessRefresh.Click += new EventHandler(btnProcessRefresh_Click); // 클릭 이벤트 핸들러 연결



            // 버튼 설정
            btnRunPath = new Button();
            btnRunPath.Location = new System.Drawing.Point(240, 450); // 위치 설정
            btnRunPath.Size = new System.Drawing.Size(80, 40); // 크기 설정
            btnRunPath.Text = "실행"; // 버튼 텍스트 설정
            btnRunPath.Click += new EventHandler(btnRunPath_Click); // 클릭 이벤트 핸들러 연결

            // 프리셋 목록 보여주기
            comboBox = new ComboBox();
            comboBox.Location = new System.Drawing.Point(30, 450);
            comboBox.Size = new System.Drawing.Size(200, 20);
            comboBox.Text = "프리셋 목록";
            comboBox.Click += new EventHandler(LoadTextFilesToComboBox);
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);


            // 텍스트 박스와 버튼을 폼에 추가
            this.Controls.Add(txtProcessName);
            this.Controls.Add(btnSavePath);
            this.Controls.Add(btnRunPath);
            this.Controls.Add(comboBox);
            this.Controls.Add(btnProcessRefresh);


        }

        //private void dataGridView2_MouseDown(object sender, MouseEventArgs e)
        //{
        //    rowIndexFromMouseDown = dataGridView1.HitTest(e.X, e.Y).RowIndex;

        //    if (rowIndexFromMouseDown != -1)
        //    {
        //        dataGridView1.DoDragDrop(dataGridView1.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
        //    }
        //}

        //private void dataGridView2_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
        //    {
        //        // Drag the row if the left mouse button is pressed
        //        if (rowIndexFromMouseDown != -1)
        //        {
        //            DragDropEffects dropEffect = dataGridView1.DoDragDrop(dataGridView1.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
        //        }
        //    }
        //}

        //private void dataGridView2_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Move;

        //    Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));
        //    rowIndexOfItemUnderMouseToDrop = dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
        //}

        //private void dataGridView2_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (rowIndexOfItemUnderMouseToDrop != -1 && rowIndexFromMouseDown != -1)
        //    {
        //        DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
        //        dataGridView1.Rows.RemoveAt(rowIndexFromMouseDown);
        //        dataGridView1.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
        //    }
        //}

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
            } else
            {
                this.dataGridView2.Columns.Add(column);
            }

        }

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
            } else
            {
                this.dataGridView2.Columns.Add(column);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            float scaleFactor = Math.Min((float)this.ClientSize.Width / 1800, (float)this.ClientSize.Height / 1600);
            AdjustControlFonts(this, scaleFactor);
        }

        private void AdjustControlFonts(Control ctrl, float scaleFactor)
        {
            foreach (Control c in ctrl.Controls)
            {
                c.Font = new Font(c.Font.FontFamily, 10 * scaleFactor, c.Font.Style);
                if (c.Controls.Count > 0)
                {
                    AdjustControlFonts(c, scaleFactor);
                }
            }
        }


        private void btnProcessRefresh_Click(object sender, EventArgs e)
        {
            LoadProcesses();
        }


        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex != -1) // 선택된 항목이 있는지 확인합니다.
            {
                string selectedFile = comboBox.SelectedItem.ToString();
                Console.WriteLine(selectedFile);


                string modifiedFile = selectedFile.Substring(0, selectedFile.Length - 4);

                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = System.IO.Path.GetDirectoryName(exeLocation);
                string filePath = System.IO.Path.Combine(exeDirectory, modifiedFile + "\\" + selectedFile);
                Console.WriteLine(filePath);

                selectFile = filePath;

                List<string> processInfoLines = new List<string>(); // 처리할 정보를 저장할 리스트입니다.
                int blankLineCount = 0; // 공백 행의 카운트를 저장합니다.

                foreach (var line in File.ReadLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        blankLineCount++; // 공백 행이면 카운트를 증가시킵니다.
                    } else if (blankLineCount >= 3)
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
                string exeDirectory = System.IO.Path.GetDirectoryName(exeLocation);
                // 이미지 파일의 상대 경로를 추가하여 전체 경로를 구성
                string imagePath = System.IO.Path.Combine(exeDirectory, modifiedFile, imageNum.ToString() + ".png"); //]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]이렇게 하면 실행파일 있는곳의 경로 가질수있음
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



        // 콤보박스에 지정 디렉토리에 있는 모든 텍스트 파일 가져오는 함수
        private void LoadTextFilesToComboBox(object sender, EventArgs e)
        {
            comboBox.Items.Clear(); // 기존 목록을 비웁니다.

            // 현재 실행 중인 어셈블리의 위치를 얻음
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // 어셈블리의 디렉터리를 얻음
            string exeDirectory = System.IO.Path.GetDirectoryName(exeLocation);
            // 이미지 파일의 상대 경로를 추가하여 전체 경로를 구성
            string directoryPath = System.IO.Path.Combine(exeDirectory);


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
                                } catch (Exception ex)
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
                } else
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
                    } else
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
            } catch (Exception ex)
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





        private void LoadProcesses()
        {
            dataGridView1.Rows.Clear(); // DataGridView 내용을 클리어
            Process[] processes = Process.GetProcesses();
            foreach (Process prs in processes)
            {
                // MainWindowTitle이 비어있지 않은 프로세스만 필터링
                if (!string.IsNullOrEmpty(prs.MainWindowTitle))
                {
                    int imageIndex;

                    try
                    {
                        // 프로세스의 메인 모듈로부터 아이콘 추출 시도
                        Icon icon = Icon.ExtractAssociatedIcon(prs.MainModule.FileName);
                        // Icon을 Image 형태로 변환
                        Bitmap iconBitmap = icon.ToBitmap();
                        imageList1.Images.Add(iconBitmap); // ImageList에 아이콘 추가
                        imageIndex = imageList1.Images.Count - 1; // 추가된 아이콘의 인덱스

                    } catch (Exception ex)
                    {
                        // 추출 실패 시 기본 Error 아이콘 사용
                        imageList1.Images.Add(SystemIcons.Error);
                        imageIndex = imageList1.Images.Count - 1; // 추가된 Error 아이콘의 인덱스

                    }

                    // 데이터 그리드뷰에 행 추가
                    var row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);

                    // 아이콘을 이미지 셀에 추가
                    row.Cells[0].Value = imageList1.Images[imageIndex];

                    // 프로세스 이름과 아이콘 추가
                    row.Cells[1].Value = prs.MainWindowTitle;
                    row.Cells[2].Value = prs.ProcessName;

                    // 윈도우 상태 정보 추가
                    string windowState = GetWindowState(prs.MainWindowHandle);
                    row.Cells[7].Value = windowState;



                    // 윈도우 위치 및 크기 정보 추가
                    try
                    {
                        var rect = new User32.Rect();
                        User32.GetWindowRect(prs.MainWindowHandle, ref rect);
                        row.Cells[3].Value = rect.Left.ToString(); // X축 위치
                        row.Cells[4].Value = rect.Top.ToString(); // Y축 위치
                        row.Cells[5].Value = (rect.Right - rect.Left).ToString(); // 가로 크기
                        row.Cells[6].Value = (rect.Bottom - rect.Top).ToString(); // 세로 크기
                    } catch
                    {
                        row.Cells[3].Value = "N/A"; // X축 위치
                        row.Cells[4].Value = "N/A"; // Y축 위치
                        row.Cells[5].Value = "N/A"; // 가로 크기
                        row.Cells[6].Value = "N/A"; // 세로 크기
                    }
                    dataGridView1.Rows.Add(row);
                }
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