using Newtonsoft.Json;
using SkinChest.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SkinChest.Start {
    public partial class AddSkin : DevForm {
        private Timer timer;
        private bool isValidSkin = false;

        public AddSkin() {
            InitializeComponent();
            InitializeWebView();
            comboBox1.SelectedIndex = 0; // 기본값 스티브 선택
        }

        private async void InitializeWebView() {
            try {
                webView21.Source = new Uri($"file:///{AppDomain.CurrentDomain.BaseDirectory}/3d-view/index.html");
                await webView21.EnsureCoreWebView2Async(null);

                timer = new Timer();
                timer.Interval = 2000;
                timer.Tick += Timer_Tick;
                timer.Start();

                button3.Visible = false; // 로딩 버튼 숨기기
            } catch (Exception ex) {
                Tool.ShowError($"3D 뷰어 초기화 실패: {ex.Message}");
                button3.Text = "미리보기 로드 실패";
            }
        }

        private async void Timer_Tick(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(textBox2.Text) &&
                textBox2.Text != "" &&
                File.Exists(textBox2.Text)) {
                try {
                    await webView21.ExecuteScriptAsync(
                        $"handleImageSelect(\"{textBox2.Text.Replace("\\", "/")}\")");
                } catch {
                    // 스크립트 실행 실패 무시
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                SetSkinFile(openFileDialog1.FileName);
            }
        }

        private void SetSkinFile(string filePath) {
            if (!File.Exists(filePath)) {
                Tool.ShowError("파일이 존재하지 않습니다.");
                return;
            }

            if (!Path.GetExtension(filePath).Equals(".png", StringComparison.OrdinalIgnoreCase)) {
                Tool.ShowError("PNG 파일만 선택할 수 있습니다.");
                return;
            }

            // 스킨 파일 유효성 검사
            if (!ValidateSkinFile(filePath)) {
                Tool.ShowError("유효하지 않은 스킨 파일입니다.\n64x64 또는 64x32 크기의 PNG 파일이어야 합니다.");
                return;
            }

            textBox2.Text = filePath;
            isValidSkin = true;
            panelDropZone.BackColor = Color.FromArgb(232, 245, 233); // 연한 초록색
        }

        private bool ValidateSkinFile(string filePath) {
            try {
                using (var img = Image.FromFile(filePath)) {
                    return (img.Width == 64 && (img.Height == 64 || img.Height == 32));
                }
            } catch {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            // 유효성 검사
            if (string.IsNullOrWhiteSpace(textBox1.Text)) {
                Tool.ShowError("스킨 이름을 입력해주세요.");
                textBox1.Focus();
                return;
            }

            if (comboBox1.SelectedIndex == -1) {
                Tool.ShowError("스킨 유형을 선택해주세요.");
                comboBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text)) {
                Tool.ShowError("스킨 파일을 선택해주세요.");
                return;
            }

            if (!isValidSkin) {
                Tool.ShowError("유효한 스킨 파일을 선택해주세요.");
                return;
            }

            // 특수문자 제거
            string skinName = RemoveInvalidChars(textBox1.Text.Trim());
            if (string.IsNullOrEmpty(skinName)) {
                Tool.ShowError("유효한 스킨 이름을 입력해주세요.");
                return;
            }

            AddSkinList(skinName, comboBox1.SelectedIndex, textBox2.Text);
        }

        private string RemoveInvalidChars(string filename) {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", filename.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        void AddSkinList(string name, int type, string skinFilePath) {
            try {
                string skinPath = Path.Combine(Tool.SkinDataPath, name);

                if (Directory.Exists(skinPath)) {
                    var result = MessageBox.Show(
                        $"\"{name}\" 라는 이름의 스킨이 이미 존재합니다.\n덮어쓰시겠습니까?",
                        "스킨상자",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No) {
                        return;
                    }

                    // 기존 파일 삭제
                    Directory.Delete(skinPath, true);
                }

                Directory.CreateDirectory(skinPath);

                // 스킨 파일 복사
                File.Copy(skinFilePath, Path.Combine(skinPath, "skin.png"), true);

                // 정보 저장
                var skinInfo = new {
                    Name = name,
                    Type = type,
                    AddedDate = DateTime.Now
                };
                string json = JsonConvert.SerializeObject(skinInfo, Formatting.Indented);
                File.WriteAllText(Path.Combine(skinPath, "info.json"), json);

                Tool.ShowInfo($"'{name}' 스킨이 성공적으로 추가되었습니다!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            } catch (Exception ex) {
                Tool.ShowError($"스킨 추가 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e) {
            try {
                panelDropZone.BackColor = Color.FromArgb(245, 245, 245);

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0) {
                    SetSkinFile(files[0]);
                }
            } catch (Exception ex) {
                Tool.ShowError($"파일을 불러올 수 없습니다: {ex.Message}");
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1 &&
                    Path.GetExtension(files[0]).Equals(".png", StringComparison.OrdinalIgnoreCase)) {
                    e.Effect = DragDropEffects.Copy;
                    panelDropZone.BackColor = Color.FromArgb(200, 230, 201); // 초록색
                    label7.Text = "📁\n\n놓으면 파일이 선택됩니다";
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
            panelDropZone.BackColor = Color.FromArgb(255, 205, 210); // 빨간색
            label7.Text = "📁\n\nPNG 파일만 가능합니다";
        }

        private void pictureBox1_DragLeave(object sender, EventArgs e) {
            panelDropZone.BackColor = Color.FromArgb(245, 245, 245);
            label7.Text = "📁\n\n여기에 PNG 파일을 드래그하세요";
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            timer?.Stop();
            timer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}