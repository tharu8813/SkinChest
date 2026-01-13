using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkinChest.Start {
    public partial class SkinViewerForm : Tools.DevForm {
        private static SkinViewerForm instance;
        private bool isWebViewInitialized = false;

        public SkinViewerForm() {
            InitializeComponent();
        }

        private async void SkinViewerForm_Load(object sender, EventArgs e) {
            await InitializeWebView();
        }

        private async Task InitializeWebView() {
            if (isWebViewInitialized) return;

            try {
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "3d-view", "index.html");
                if (!File.Exists(htmlPath)) {
                    MessageBox.Show("3D 뷰어 파일(index.html)을 찾을 수 없습니다.\n프로그램 폴더에 index.html 파일이 있는지 확인하세요.",
                        "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await webView.EnsureCoreWebView2Async(null);
                webView.Source = new Uri($"file:///{htmlPath}");
                isWebViewInitialized = true;

                // WebView 로드 완료 대기
                await Task.Delay(1000);
            } catch (Exception ex) {
                MessageBox.Show($"3D 뷰어 초기화 실패: {ex.Message}",
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task LoadSkin(string skinPath, string skinName) {
            if (!isWebViewInitialized) {
                await InitializeWebView();
            }

            lblSkinName.Text = skinName;

            if (File.Exists(skinPath)) {
                try {
                    // 경로의 백슬래시를 슬래시로 변환
                    string normalizedPath = skinPath.Replace("\\", "/");

                    // JavaScript 함수 호출하여 스킨 로드
                    await webView.ExecuteScriptAsync($"handleImageSelect(\"{normalizedPath}\")");
                } catch (Exception ex) {
                    MessageBox.Show($"스킨 로드 실패: {ex.Message}",
                        "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else {
                MessageBox.Show($"스킨 파일을 찾을 수 없습니다:\n{skinPath}",
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SkinViewerForm_FormClosing(object sender, FormClosingEventArgs e) {
            // 폼을 닫지 않고 숨기기 (재사용을 위해)
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                this.Hide();
            }
        }

        // 정적 메서드로 뷰어 표시
        public static async void Show3DViewer(string skinPath, string skinName) {
            if (instance == null || instance.IsDisposed) {
                instance = new SkinViewerForm();
            }

            if (!instance.Visible) {
                instance.Show();
            } else {
                instance.BringToFront();
                instance.Activate();
            }

            await instance.LoadSkin(skinPath, skinName);
        }
    }
}