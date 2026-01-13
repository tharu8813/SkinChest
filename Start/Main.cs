using CmlLib.Core.Auth.Microsoft;
using SkinChest.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkinChest.Start {
    public partial class Main : DevForm {
        private List<SkinData> allSkins = new List<SkinData>();
        private List<SkinData> filteredSkins = new List<SkinData>();

        public Main() {
            InitializeComponent();
            comboBoxSort.SelectedIndex = 0; // 최신순 기본 선택
        }

        private async Task UpdateAsync() {
            try {
                // 로딩 표시
                flowLayoutPanel1.Controls.Clear();
                var loadingLabel = new Label {
                    Text = "스킨을 불러오는 중...",
                    AutoSize = true,
                    Padding = new Padding(10)
                };
                flowLayoutPanel1.Controls.Add(loadingLabel);

                // 비동기로 스킨 데이터 로드
                await Task.Run(() => {
                    allSkins = Tool.LoadAllSkins();
                });

                // 정렬 적용
                ApplySort();

                // 검색 필터 적용
                ApplyFilter();

                // 스킨 개수 업데이트
                UpdateSkinCount();

            } catch (Exception ex) {
                Tool.ShowError($"스킨을 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void ApplySort() {
            switch (comboBoxSort.SelectedIndex) {
                case 0: // 최신순
                    allSkins = allSkins.OrderByDescending(s => s.AddedDate).ToList();
                    break;
                case 1: // 이름순
                    allSkins = allSkins.OrderBy(s => s.Name).ToList();
                    break;
                case 2: // 오래된순
                    allSkins = allSkins.OrderBy(s => s.AddedDate).ToList();
                    break;
            }
        }

        private void ApplyFilter() {
            string searchText = textBoxSearch.Text;

            // placeholder 텍스트면 전체 표시
            if (searchText == "🔍 스킨 이름으로 검색..." || string.IsNullOrWhiteSpace(searchText)) {
                filteredSkins = allSkins;
            } else {
                filteredSkins = allSkins.Where(s =>
                    s.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }

            DisplaySkins();
        }

        private async void DisplaySkins() {
            flowLayoutPanel1.SuspendLayout(); // 레이아웃 업데이트 일시 중지 (성능 향상)
            flowLayoutPanel1.Controls.Clear();

            if (filteredSkins.Count == 0) {
                var emptyLabel = new Label {
                    Text = "스킨이 없습니다. '➕ 스킨 추가' 버튼을 눌러 스킨을 추가하세요.",
                    AutoSize = true,
                    Padding = new Padding(10),
                    ForeColor = Color.Gray,
                    Font = new Font("맑은 고딕", 9.5F)
                };
                flowLayoutPanel1.Controls.Add(emptyLabel);
                flowLayoutPanel1.ResumeLayout();
                return;
            }

            // 배치 처리로 성능 향상
            var controls = new List<Control>(); // Control 타입으로 변경

            foreach (var item in filteredSkins) {
                var listControl = new SkinListItem {
                    SkinDataValue = item,
                    Margin = new Padding(5)
                };

                // 라벨 설정
                listControl.label2.Text = item.Name;
                listControl.label1.Text = item.Type == 0 ? "스킨 형식: 스티브" : "스킨 형식: 알렉스";
                listControl.label3.Text = item.AddedDate.ToString("yyyy-MM-dd HH:mm:ss");

                // 이벤트 핸들러
                listControl.NoHandler += async (se, ex) => {
                    await UpdateAsync();
                };

                controls.Add(listControl);
            }

            // 한 번에 모든 컨트롤 추가
            flowLayoutPanel1.Controls.AddRange(controls.ToArray());
            flowLayoutPanel1.ResumeLayout(); // 레이아웃 업데이트 재개

            // UI 반응성을 위한 양보
            await Task.Yield();
        }

        private void UpdateSkinCount() {
            labelSkinCount.Text = filteredSkins.Count == allSkins.Count
                ? $"스킨 {allSkins.Count}개"
                : $"스킨 {filteredSkins.Count}/{allSkins.Count}개";
        }

        private async void Main_Load(object sender, EventArgs e) {
            await UpdateAsync();

            // 저장된 로그인 정보가 있다면 복원 시도
            await TryRestoreSession();
        }

        private async Task TryRestoreSession() {
            // 세션 복원 로직 (선택사항)
            // CmlLib에서 세션 저장/복원을 지원한다면 구현
        }

        private async void button3_Click(object sender, EventArgs e) {
            var addSkinForm = new AddSkin();
            if (addSkinForm.ShowDialog() == DialogResult.OK) {
                await UpdateAsync();
            }
        }

        private void AllControlsEnabled(bool isEnabled) {
            foreach (Control item in Controls) {
                if (item != label3) // 로그인 상태 텍스트는 항상 표시
                    item.Enabled = isEnabled;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            LoginStart();
        }

        async void LoginStart() {
            try {
                AllControlsEnabled(false);
                btnLogin.Text = "로그인 중...";

                var loginHandler = JELoginHandlerBuilder.BuildDefault();
                Tool.MSession = await loginHandler.Authenticate();

                if (Tool.MSession == null) {
                    Tool.ShowError("로그인에 실패했습니다. 다시 시도해주세요.");
                    btnLogin.Text = "🔐 로그인";
                    return;
                }

                // 프로필 이미지 로드 (선택사항)
                await LoadPlayerHead(Tool.MSession.UUID);

                label3.Text = $"✓ {Tool.MSession.Username} (UUID: {Tool.MSession.UUID})";
                label3.ForeColor = Color.FromArgb(76, 175, 80); // 초록색
                btnLogin.Text = "✓ 로그인됨";
                btnLogin.BackColor = Color.FromArgb(76, 175, 80);

                Tool.ShowInfo($"환영합니다, {Tool.MSession.Username}님!");
            } catch (Exception ex) {
                Tool.ShowError($"로그인 중 오류가 발생했습니다: {ex.Message}");
                btnLogin.Text = "🔐 로그인";
            } finally {
                AllControlsEnabled(true);
            }
        }

        private async Task LoadPlayerHead(string uuid) {
            try {
                // Crafatar API를 사용하여 플레이어 얼굴 이미지 로드
                string url = $"https://crafatar.com/avatars/{uuid}?size=32&overlay";
                using (var client = new System.Net.WebClient()) {
                    byte[] imageData = await client.DownloadDataTaskAsync(url);
                    using (var ms = new MemoryStream(imageData)) {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
            } catch {
                // 이미지 로드 실패 시 무시
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (Tool.MSession == null) {
                Tool.ShowError("로그인 후 다시 시도해주세요.");
                return;
            }
            Process.Start($"https://ko.namemc.com/profile/{Tool.MSession.Username}");
        }

        private void button4_Click(object sender, EventArgs e) {
            if (!Directory.Exists(Tool.SkinDataPath)) {
                Directory.CreateDirectory(Tool.SkinDataPath);
            }
            Process.Start(Tool.SkinDataPath);
        }

        // 검색 기능
        private void textBoxSearch_TextChanged(object sender, EventArgs e) {
            ApplyFilter();
            UpdateSkinCount();
        }

        private void textBoxSearch_Enter(object sender, EventArgs e) {
            if (textBoxSearch.Text == "🔍 스킨 이름으로 검색...") {
                textBoxSearch.Text = "";
                textBoxSearch.ForeColor = Color.Black;
            }
        }

        private void textBoxSearch_Leave(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text)) {
                textBoxSearch.Text = "🔍 스킨 이름으로 검색...";
                textBoxSearch.ForeColor = Color.Gray;
            }
        }

        // 정렬 기능
        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e) {
            ApplySort();
            ApplyFilter();
        }
    }
}