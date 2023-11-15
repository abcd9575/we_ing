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
        private Size originalSize; // 창의 원래 크기를 저장할 변수
        private Point originalLocation; // 창의 원래 위치를 저장할 변수
        private Bitmap markedImage; // 빨간색 크로스가 그려진 이미지를 저장할 변수
        private Point initialMousePosition;
        private Point currentMousePosition;


        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form_KeyDown;
            pictureBox.MouseClick += new MouseEventHandler(pictureBox_MouseClick);
            pictureBox.Paint += new PaintEventHandler(pictureBox_Paint);
            pictureBox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);

            // PictureBox의 배경색을 회색으로 설정
            pictureBox.BackColor = Color.Gray;

            // PictureBox의 이미지 배치 모드를 중앙으로 설정
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

            // 프로그램이 시작할 때의 창의 크기를 저장
            originalSize = this.Size;
            
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // ESC 키를 누르면 PictureBox를 비우고 창의 상태를 원래대로 복원
                pictureBox.Image = null;
                this.Size = originalSize; // 창의 크기를 원래 크기로 복원
                this.Location = originalLocation; // 창의 위치를 원래 위치로 복원
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                CaptureButton.Visible = true;
            }
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            // 저장한 좌표 초기화
            firstClickPoint = Point.Empty;
            secondClickPoint = Point.Empty;
            originalLocation = this.Location;
            CaptureButton.Visible = false;
            // 현재 화면 캡쳐
            screenshot = CaptureScreen();
            markedImage = (Bitmap)screenshot.Clone(); // 크로스를 그릴 이미지 복사


            // 캡쳐한 화면을 PictureBox에 표시

            pictureBox.Bounds = Screen.PrimaryScreen.Bounds;
            pictureBox.Image = screenshot;

            // 창의 상태를 전체화면으로 변경
            this.WindowState = FormWindowState.Normal; // 현재 창의 상태를 "Normal"로 설정
            this.FormBorderStyle = FormBorderStyle.None; // 창 테두리를 없애는 코드
            this.Bounds = Screen.PrimaryScreen.Bounds; // 창의 크기를 주 모니터의 해상도에 맞게 설정

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

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // 첫 번째 클릭 좌표 저장
            if (firstClickPoint == Point.Empty && CaptureButton.Visible == false)
            {
                firstClickPoint = e.Location;
                // PictureBox에 빨간색 크로스 그리기
                using (Graphics g = Graphics.FromImage(markedImage))
                {
                    using (Pen pen = new Pen(Color.Red, 3))
                    {
                        g.DrawLine(pen, firstClickPoint.X - 100000, firstClickPoint.Y, firstClickPoint.X + 100000, firstClickPoint.Y);
                        g.DrawLine(pen, firstClickPoint.X, firstClickPoint.Y - 100000, firstClickPoint.X, firstClickPoint.Y + 100000);
                    }
                }
                // 클릭한 지점의 이미지를 세 배 확대
                using (Bitmap zoomedImage = new Bitmap(markedImage, markedImage.Width * 3, markedImage.Height * 3))
                {
                    // 크로스가 그려진 이미지를 PictureBox에 표시
                    pictureBox.Image = (Bitmap)zoomedImage.Clone();
                }
                
                // 마우스 포인터를 화면 중앙으로 이동
                Cursor.Position = this.PointToScreen(new Point(this.Width / 2, this.Height / 2));

                // 초기 마우스 위치 저장
                initialMousePosition = Cursor.Position;

                // PictureBox 이미지 갱신
                pictureBox.Invalidate();
            }
            // 두 번째 클릭 좌표 저장
            else if (secondClickPoint == Point.Empty && CaptureButton.Visible == false)
            {
                // 확대된 이미지 상에서의 클릭 좌표를 원본 이미지에 대응하는 좌표로 변환
                int x = e.X / 3 + firstClickPoint.X - pictureBox.Width / (2 * 3);
                int y = e.Y / 3 + firstClickPoint.Y - pictureBox.Height / (2 * 3);
                secondClickPoint = new Point(x, y);
                //secondClickPoint = e.Location;

                // 클릭한 두 지점을 기준으로 사각형 만들기
                int left = Math.Min(firstClickPoint.X, secondClickPoint.X);
                int top = Math.Min(firstClickPoint.Y, secondClickPoint.Y);
                int width = Math.Abs(firstClickPoint.X - secondClickPoint.X);
                int height = Math.Abs(firstClickPoint.Y - secondClickPoint.Y);
                Rectangle rect = new Rectangle(left, top, width, height);

                // 사각형 영역의 이미지 추출
                using (Bitmap croppedImage = screenshot.Clone(rect, screenshot.PixelFormat))
                {
                    // 확대할 이미지의 크기 계산
                    int newWidth = croppedImage.Width * 3;
                    int newHeight = croppedImage.Height * 3;

                    // 확대할 이미지 생성
                    using (Bitmap zoomedImage = new Bitmap(croppedImage, newWidth, newHeight))
                    {
                        // 확대한 이미지를 PictureBox에 표시
                        pictureBox.Image = (Bitmap)zoomedImage.Clone();
                    }
                }

                // PictureBox 이미지 갱신
                pictureBox.Invalidate();

                // 두 좌표를 반환하고 PictureBox를 비우며 창의 상태를 원래대로 복원
                MessageBox.Show($"첫 번째 좌표: ({firstClickPoint.X}, {firstClickPoint.Y}), 두 번째 좌표: ({secondClickPoint.X}, {secondClickPoint.Y})");
                pictureBox.Image = null;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;


                // 저장한 좌표 초기화
                firstClickPoint = Point.Empty;
                secondClickPoint = Point.Empty;

                this.Size = originalSize; // 창의 크기를 원래 크기로 복원
                this.Location = originalLocation; // 창의 위치를 원래 위치로 복원
                CaptureButton.Visible = true;

                // 가비지 컬렉션 강제 실행
                GC.Collect();

            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!firstClickPoint.IsEmpty)
            {
                // 현재 마우스 위치 저장
                currentMousePosition = Cursor.Position;

                // PictureBox 이미지 갱신
                pictureBox.Invalidate();
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox.Image != null && !firstClickPoint.IsEmpty)
            {
                // 마우스 이동에 따른 그림 이동 계산
                int dx = initialMousePosition.X - currentMousePosition.X;
                int dy = initialMousePosition.Y - currentMousePosition.Y;


                // 클릭한 지점을 중심으로 그림을 그림
                int startX = dx - (firstClickPoint.X * 3 - pictureBox.Width / 2);
                int startY = dy - (firstClickPoint.Y * 3 - pictureBox.Height / 2);

                // 그림을 dx, dy만큼 이동시키기
                e.Graphics.Clear(Color.Gray);
                e.Graphics.DrawImage(pictureBox.Image, startX, startY);
            }
        }
    }
}