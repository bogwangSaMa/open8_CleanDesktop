using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UEP
{
    public partial class Form2 : Form
    {
        private DataGridViewRow row;

        private Label sequenceLabel;
        private Label appNameLabel; // 수정 불가능
        private Label processNameLabel; // 수정 불가능
        private Label xLabel;
        private Label yLabel;
        private Label widthLabel;
        private Label heightLabel;
        private Label stateLabel;
        private Label processPathLabel;

        private TextBox sequence;
        private TextBox appName; // 수정 불가능
        private TextBox processName; // 수정 불가능
        private TextBox x;
        private TextBox y;
        private TextBox width;
        private TextBox height;
        private ComboBox state;
        private TextBox processPath;

        private Button btnSave;

        public Form2(DataGridViewRow selectedRow)
        {

            InitializeComponent();
            InitializeComponents();
            row = selectedRow;
            LoadRowData();

            this.Size = new Size(800, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponents()
        {
            // 
            sequenceLabel = new Label();
            sequenceLabel.Location = new Point(30, 30); // 위치 설정
            sequenceLabel.Size = new Size(100, 40); // 크기 설정
            sequenceLabel.Text = "순서";
            // 텍스트박스
            sequence = new TextBox();
            sequence.Location = new Point(150, 30); // 위치 설정
            sequence.Size = new Size(200, 40); // 크기 설정


            // 
            appNameLabel = new Label();
            appNameLabel.Location = new Point(30, 80); // 위치 설정
            appNameLabel.Size = new Size(100, 40); // 크기 설정
            appNameLabel.Text = "앱 이름"; // 버튼 텍스트 설정
            // 텍스트박스
            appName = new TextBox();
            appName.Location = new Point(150, 80); // 위치 설정
            appName.Size = new Size(200, 40); // 크기 설정


            // 
            processNameLabel = new Label();
            processNameLabel.Location = new Point(30, 130); // 위치 설정
            processNameLabel.Size = new Size(100, 40); // 크기 설정
            processNameLabel.Text = "프로세스 이름"; // 버튼 텍스트 설정
            // 텍스트박스
            processName = new TextBox();
            processName.Location = new Point(150, 130); // 위치 설정
            processName.Size = new Size(200, 40); // 크기 설정


            // 
            stateLabel = new Label();
            stateLabel.Location = new Point(30, 180); // 위치 설정
            stateLabel.Size = new Size(100, 40); // 크기 설정
            stateLabel.Text = "윈도우 상태"; // 버튼 텍스트 설정
            // 텍스트박스
            state = new ComboBox();
            state.Location = new Point(150, 180); // 위치 설정
            state.Size = new Size(200, 40); // 크기 설정


            // 
            processPathLabel = new Label();
            processPathLabel.Location = new Point(30, 230); // 위치 설정
            processPathLabel.Size = new Size(100, 40); // 크기 설정
            processPathLabel.Text = "실행파일 경로"; // 버튼 텍스트 설정
            // 텍스트박스
            processPath = new TextBox();
            processPath.Location = new Point(150, 230); // 위치 설정
            processPath.Size = new Size(400, 40); // 크기 설정


            // 
            xLabel = new Label();
            xLabel.Location = new Point(450, 30); // 위치 설정
            xLabel.Size = new Size(100, 40); // 크기 설정
            xLabel.Text = "X축 좌표"; // 버튼 텍스트 설정
            // 텍스트박스
            x = new TextBox();
            x.Location = new Point(550, 30); // 위치 설정
            x.Size = new Size(200, 40); // 크기 설정


            // 
            yLabel = new Label();
            yLabel.Location = new Point(450, 80); // 위치 설정
            yLabel.Size = new Size(100, 40); // 크기 설정
            yLabel.Text = "Y축 좌표"; // 버튼 텍스트 설정
            // 텍스트박스
            y = new TextBox();
            y.Location = new Point(550, 80); // 위치 설정
            y.Size = new Size(200, 40); // 크기 설정


            // 
            widthLabel = new Label();
            widthLabel.Location = new Point(450, 130); // 위치 설정
            widthLabel.Size = new Size(100, 40); // 크기 설정
            widthLabel.Text = "가로 길이"; // 버튼 텍스트 설정
            // 텍스트박스
            width = new TextBox();
            width.Location = new Point(550, 130); // 위치 설정
            width.Size = new Size(200, 40); // 크기 설정


            // 
            heightLabel = new Label();
            heightLabel.Location = new Point(450, 180); // 위치 설정
            heightLabel.Size = new Size(100, 40); // 크기 설정
            heightLabel.Text = "세로 길이"; // 버튼 텍스트 설정
            // 텍스트박스
            height = new TextBox();
            height.Location = new Point(550, 180); // 위치 설정
            height.Size = new Size(200, 40); // 크기 설정


            

            btnSave = new Button();
            btnSave.Location = new Point(650, 300); // 위치 설정
            btnSave.Size = new Size(100, 40); // 크기 설정
            btnSave.Text = "설정값 변경"; // 버튼 텍스트 설정
            btnSave.Click += new EventHandler(btnSave_Click);  


            this.Controls.Add(sequenceLabel);
            this.Controls.Add(appNameLabel);
            this.Controls.Add(processNameLabel);
            this.Controls.Add(xLabel);
            this.Controls.Add(yLabel);
            this.Controls.Add(widthLabel);
            this.Controls.Add(heightLabel);
            this.Controls.Add(stateLabel);
            this.Controls.Add(processPathLabel);

            this.Controls.Add(sequence);
            this.Controls.Add(appName);
            this.Controls.Add(processName);
            this.Controls.Add(x);
            this.Controls.Add(y);
            this.Controls.Add(width);
            this.Controls.Add(height);
            this.Controls.Add(state);
            this.Controls.Add(processPath);

            this.Controls.Add(btnSave);
        }

        private void LoadRowData()
        {
            sequence.Text = row.Cells[0].Value.ToString();
            sequence.ReadOnly = true; // appName 텍스트박스를 읽기 전용으로 설정
            appName.Text = row.Cells[2].Value.ToString();
            appName.ReadOnly = true; // appName 텍스트박스를 읽기 전용으로 설정
            processName.Text = row.Cells[3].Value.ToString();
            processName.ReadOnly = true; // appName 텍스트박스를 읽기 전용으로 설정
            x.Text = row.Cells[4].Value.ToString();
            y.Text = row.Cells[5].Value.ToString();
            width.Text = row.Cells[6].Value.ToString();
            height.Text = row.Cells[7].Value.ToString();

            state.Items.Clear(); // 기존 항목 초기화
            state.Items.Add("Min");
            state.Items.Add("Normal");
            state.Items.Add("Max");

            // 행의 값을 콤보박스에 설정
            state.SelectedIndex = state.Items.IndexOf(row.Cells[8].Value.ToString());

            processPath.Text = row.Cells[9].Value.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            row.Cells[0].Value = sequence.Text;
            row.Cells[2].Value = appName.Text;
            row.Cells[3].Value = processName.Text;
            row.Cells[4].Value = x.Text;
            row.Cells[5].Value = y.Text;
            row.Cells[6].Value = width.Text;
            row.Cells[7].Value = height.Text;
            row.Cells[8].Value = state.Text;
            row.Cells[9].Value = processPath.Text;

            this.Close();
        }
    }
}

















