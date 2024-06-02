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

        private TextBox sequence;
        private TextBox appName; // 수정 불가능
        private TextBox processName; // 수정 불가능
        private TextBox x;
        private TextBox y;
        private TextBox width;
        private TextBox height;
        private TextBox state;

        public Form2(DataGridViewRow selectedRow)
        {
            InitializeComponent();
            InitializeComponents();
            row = selectedRow;
            LoadRowData();
        }

        private void InitializeComponents()
        {
            // 
            sequenceLabel = new Label();
            sequenceLabel.Location = new Point(30, 30); // 위치 설정
            sequenceLabel.Size = new Size(100, 40); // 크기 설정
            sequenceLabel.Text = "순서";

            // 
            appNameLabel = new Label();
            appNameLabel.Location = new Point(30, 80); // 위치 설정
            appNameLabel.Size = new Size(100, 40); // 크기 설정
            appNameLabel.Text = "앱 이름"; // 버튼 텍스트 설정

            // 
            processNameLabel = new Label();
            processNameLabel.Location = new Point(30, 130); // 위치 설정
            processNameLabel.Size = new Size(100, 40); // 크기 설정
            processNameLabel.Text = "프로세스 이름"; // 버튼 텍스트 설정

            // 
            xLabel = new Label();
            xLabel.Location = new Point(30, 180); // 위치 설정
            xLabel.Size = new Size(100, 40); // 크기 설정
            xLabel.Text = "X축 좌표"; // 버튼 텍스트 설정

            // 
            yLabel = new Label();
            yLabel.Location = new Point(30, 230); // 위치 설정
            yLabel.Size = new Size(100, 40); // 크기 설정
            yLabel.Text = "Y축 좌표"; // 버튼 텍스트 설정

            // 
            widthLabel = new Label();
            widthLabel.Location = new Point(30, 280); // 위치 설정
            widthLabel.Size = new Size(100, 40); // 크기 설정
            widthLabel.Text = "가로 길이"; // 버튼 텍스트 설정

            // 
            heightLabel = new Label();
            heightLabel.Location = new Point(30, 330); // 위치 설정
            heightLabel.Size = new Size(100, 40); // 크기 설정
            heightLabel.Text = "세로 길이"; // 버튼 텍스트 설정

            // 
            stateLabel = new Label();
            stateLabel.Location = new Point(30, 380); // 위치 설정
            stateLabel.Size = new Size(100, 40); // 크기 설정
            stateLabel.Text = "윈도우 상태"; // 버튼 텍스트 설정



            this.Controls.Add(sequenceLabel);
            this.Controls.Add(appNameLabel);
            this.Controls.Add(processNameLabel);
            this.Controls.Add(xLabel);
            this.Controls.Add(yLabel);
            this.Controls.Add(widthLabel);
            this.Controls.Add(heightLabel);
            this.Controls.Add(stateLabel);
        }

        private void LoadRowData()
        {
            //textBoxCell1.Text = row.Cells[0].Value.ToString();
            //textBoxCell2.Text = row.Cells[1].Value.ToString();
            // 필요한 만큼 추가합니다.
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //row.Cells[0].Value = textBoxCell1.Text;
            //row.Cells[1].Value = textBoxCell2.Text;
            // 필요한 만큼 추가합니다.

            this.Close();
        }
    }
}

















