using MojangAPI;
using MojangAPI.Model;
using SkinChest.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace SkinChest.Start {
    public partial class SkinListItem : UserControl {
        public SkinListItem() {
            InitializeComponent();
        }

        public EventHandler YesHandler;
        public EventHandler NoHandler;

        private SkinData _skinDataValue;
        public SkinData SkinDataValue {
            get => _skinDataValue;
            set {
                _skinDataValue = value;
                LoadSkinImage();
            }
        }

        private void LoadSkinImage() {
            if (_skinDataValue == null) return;

            try {
                if (File.Exists(_skinDataValue.ImagePath)) {
                    // 기존 이미지 해제
                    if (pictureBoxSkin.Image != null) {
                        var oldImage = pictureBoxSkin.Image;
                        pictureBoxSkin.Image = null;
                        oldImage.Dispose();
                    }

                    // 새 이미지 로드 (파일 스트림을 사용하여 파일 잠금 방지)
                    using (var fs = new FileStream(_skinDataValue.ImagePath, FileMode.Open, FileAccess.Read)) {
                        pictureBoxSkin.Image = Image.FromStream(fs);
                    }
                }
            } catch (Exception ex) {
                // 로드 실패시 기본 이미지 또는 에러 표시
                pictureBoxSkin.Image = null;
                Console.WriteLine($"스킨 이미지 로드 실패: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, System.EventArgs e) {
            if (Tool.MSession != null) {
                try {
                    button1.Enabled = false;
                    button1.Text = "적용 중...";

                    Mojang mojang = new Mojang(new HttpClient());
                    await mojang.UploadSkin(
                        Tool.MSession.AccessToken,
                        SkinDataValue.Type == 0 ? SkinType.Steve : SkinType.Alex,
                        SkinDataValue.ImagePath
                    );

                    Tool.ShowInfo("스킨이 적용되었습니다.");
                    button1.Text = "✓ 스킨 적용";
                } catch (Exception ex) {
                    Tool.ShowError($"스킨 업로드 실패: {ex.Message}");
                    button1.Text = "✓ 스킨 적용";
                } finally {
                    button1.Enabled = true;
                }
            } else {
                Tool.ShowError("로그인 후에 시도해주세요.");
            }
            YesHandler?.Invoke(sender, e);
        }

        private void button2_Click(object sender, EventArgs e) {
            var result = MessageBox.Show(
                $"'{SkinDataValue.Name}' 스킨을 삭제하시겠습니까?",
                "스킨 삭제",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes) return;

            try {
                string skinPath = Path.Combine(Tool.SkinDataPath, SkinDataValue.Name);
                DirectoryInfo dir1 = new DirectoryInfo(skinPath);
                dir1.Attributes = FileAttributes.Normal;
                dir1.Delete(true);
                Tool.ShowInfo($"스킨 '{SkinDataValue.Name}'가 삭제되었습니다.");
                NoHandler?.Invoke(sender, e);
            } catch (Exception ex) {
                Tool.ShowError($"스킨 삭제 실패: {ex.Message}");
            }
        }

        private void panel1_DoubleClick(object sender, EventArgs e) {
            if (_skinDataValue != null && File.Exists(_skinDataValue.ImagePath)) {
                SkinViewerForm.Show3DViewer(_skinDataValue.ImagePath, _skinDataValue.Name);
            }
        }
    }
}