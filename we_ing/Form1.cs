using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace we_ing
{
    public partial class Form1 : Form
    {
        private Bitmap screenshot;
        private Point firstClickPoint;
        private Point secondClickPoint;


        private bool capturing = false;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown); // KeyDown 이벤트 핸들러 추가
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // 'ESC' 키가 눌린 경우
                // 창의 상태와 테두리 스타일을 원래대로 되돌리고, PictureBox를 비웁니다.
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                //pictureBox.Image = null;
            }
        }


        private void CaptureButton_Click(object sender, EventArgs e)
        {
            screenshot = CaptureScreen();
            this.WindowState = FormWindowState.Normal; // 현재 창의 상태를 "Normal"로 설정
            this.FormBorderStyle = FormBorderStyle.None; // 창 테두리를 없애는 코드
            this.Bounds = Screen.PrimaryScreen.Bounds; // 창의 크기를 주 모니터의 해상도에 맞게 설정

            if (capturing)
            {
                // 정지 화면을 해제하고 스크린샷을 표시
                
                capturing = false;
                CaptureButton.Text = "스크린샷 찍기";
                this.Cursor = Cursors.Default;
                pictureBox.Visible = true;
                pictureBox.Bounds = Screen.PrimaryScreen.Bounds;
                pictureBox.Image = screenshot;
            }
            else
            {
                // 스크린샷 찍고 정지 화면 표시
                CaptureButton.Text = "정지 화면 해제";
                capturing = true;
                this.Cursor = Cursors.Cross;
                pictureBox.Visible = true;
                pictureBox.Bounds = Screen.PrimaryScreen.Bounds;
                pictureBox.Image = screenshot;
            }
        }

        private Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return screenshot;
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (capturing)
            {
                if (firstClickPoint == Point.Empty)
                {
                    firstClickPoint = e.Location;
                    using (Graphics graphics = Graphics.FromImage(screenshot))
                    {
                        using (Pen pen = new Pen(Color.Red, 3))
                        {
                            graphics.DrawEllipse(pen, firstClickPoint.X - 5, firstClickPoint.Y - 5, 10, 10);
                        }
                    }
                    pictureBox.Image = screenshot;
                }
                else if (secondClickPoint == Point.Empty)
                {
                    secondClickPoint = e.Location;
                    // 두 번째 클릭한 후 작업 수행, 예를 들면, 두 점 사이의 거리를 계산
                    double distance = CalculateDistance(firstClickPoint, secondClickPoint);
                    MessageBox.Show($"두 점 사이의 거리: {distance}");

                    // 정지 화면 해제
                    CaptureButton_Click(null, null);
                }
            }
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

}
