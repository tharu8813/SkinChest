using CmlLib.Core.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SkinChest.Tools {
    internal class Tool {
        public static string SkinDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SkinChest");

        public static void ShowInfo(string message) {
            MessageBox.Show(message, "스킨상자", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowError(string message) {
            MessageBox.Show(message, "스킨상자", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool ShowConfirm(string message) {
            return MessageBox.Show(message, "스킨상자", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes;
        }

        public static MSession MSession { get; set; }

        /// <summary>
        /// 모든 스킨 데이터를 로드합니다.
        /// </summary>
        public static List<SkinData> LoadAllSkins() {
            var skinList = new List<SkinData>();

            try {
                if (!Directory.Exists(SkinDataPath)) {
                    Directory.CreateDirectory(SkinDataPath);
                    return skinList;
                }

                var directories = Directory.GetDirectories(SkinDataPath);

                foreach (var directory in directories) {
                    try {
                        string jsonFile = Path.Combine(directory, "info.json");
                        string skinFile = Path.Combine(directory, "skin.png");

                        if (!File.Exists(jsonFile) || !File.Exists(skinFile)) {
                            Console.WriteLine($"스킵: {directory} - 필수 파일 누락");
                            continue;
                        }

                        string json = File.ReadAllText(jsonFile);
                        var skinInfo = JsonConvert.DeserializeObject<SkinData>(json);

                        if (skinInfo != null) {
                            skinInfo.Image = File.ReadAllBytes(skinFile);
                            skinInfo.ImagePath = skinFile;
                            skinList.Add(skinInfo);

                            Console.WriteLine($"로드됨: {skinInfo.Name} ({skinInfo.Type}) - {skinInfo.AddedDate}");
                        }
                    } catch (Exception ex) {
                        Console.WriteLine($"스킨 로드 실패 ({directory}): {ex.Message}");
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"스킨 데이터 로드 중 오류: {ex.Message}");
            }

            return skinList;
        }

        /// <summary>
        /// 이미지를 바이트 배열로 변환합니다.
        /// </summary>
        public static byte[] ConvertImageToByteArray(string imagePath) {
            if (!File.Exists(imagePath)) {
                throw new FileNotFoundException("이미지 파일을 찾을 수 없습니다.", imagePath);
            }
            return File.ReadAllBytes(imagePath);
        }

        /// <summary>
        /// 마인크래프트 스킨 이미지를 2D 평면도로 변환합니다.
        /// </summary>
        public static Bitmap Extract2DSkin(Bitmap skinImage) {
            if (skinImage == null) {
                Console.WriteLine("스킨 이미지가 null입니다.");
                return null;
            }

            if (skinImage.Width != 64 || (skinImage.Height != 64 && skinImage.Height != 32)) {
                Console.WriteLine($"유효하지 않은 스킨 크기: {skinImage.Width}x{skinImage.Height}");
                return null;
            }

            try {
                Bitmap flatSkin = new Bitmap(64, 64);
                using (Graphics g = Graphics.FromImage(flatSkin)) {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    // 스킨 파트 정의 (소스, 대상)
                    var parts = new (Rectangle src, Rectangle dest)[] {
                        // Head
                        (new Rectangle(8, 8, 8, 8),   new Rectangle(24, 0, 8, 8)),

                        // Body
                        (new Rectangle(20, 20, 8, 12), new Rectangle(24, 16, 8, 12)),

                        // Left Arm
                        (new Rectangle(44, 20, 4, 12), new Rectangle(16, 16, 4, 12)),

                        // Right Arm
                        (new Rectangle(36, 52, 4, 12), new Rectangle(44, 16, 4, 12)),

                        // Left Leg
                        (new Rectangle(4, 20, 4, 12), new Rectangle(24, 28, 4, 12)),

                        // Right Leg
                        (new Rectangle(20, 52, 4, 12), new Rectangle(28, 28, 4, 12))
                    };

                    foreach (var (src, dest) in parts) {
                        g.DrawImage(skinImage, dest, src, GraphicsUnit.Pixel);
                    }
                }

                return flatSkin;
            } catch (Exception ex) {
                Console.WriteLine($"2D 스킨 추출 오류: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 스킨의 얼굴 부분만 추출합니다.
        /// </summary>
        public static Bitmap ExtractFace(Bitmap skinImage, int size = 32) {
            if (skinImage == null || skinImage.Width != 64) {
                return null;
            }

            try {
                // 얼굴 영역 (8x8 픽셀)
                Rectangle faceRect = new Rectangle(8, 8, 8, 8);
                Bitmap face = new Bitmap(size, size);

                using (Graphics g = Graphics.FromImage(face)) {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(skinImage,
                        new Rectangle(0, 0, size, size),
                        faceRect,
                        GraphicsUnit.Pixel);
                }

                return face;
            } catch (Exception ex) {
                Console.WriteLine($"얼굴 추출 오류: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 파일 크기를 읽기 쉬운 형식으로 변환합니다.
        /// </summary>
        public static string GetReadableFileSize(long bytes) {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1) {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 디렉토리 크기를 계산합니다.
        /// </summary>
        public static long GetDirectorySize(string path) {
            if (!Directory.Exists(path)) return 0;

            try {
                var dirInfo = new DirectoryInfo(path);
                return dirInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
            } catch {
                return 0;
            }
        }

        /// <summary>
        /// 안전하게 디렉토리를 삭제합니다.
        /// </summary>
        public static bool SafeDeleteDirectory(string path, bool showConfirm = true) {
            try {
                if (!Directory.Exists(path)) return true;

                if (showConfirm) {
                    if (!ShowConfirm($"'{Path.GetFileName(path)}'를 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.")) {
                        return false;
                    }
                }

                Directory.Delete(path, true);
                return true;
            } catch (Exception ex) {
                ShowError($"삭제 중 오류가 발생했습니다: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 데이터 폴더의 총 크기를 가져옵니다.
        /// </summary>
        public static string GetTotalSkinDataSize() {
            long size = GetDirectorySize(SkinDataPath);
            return GetReadableFileSize(size);
        }
    }
}