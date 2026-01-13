namespace SkinChest.Start {
    partial class SkinViewerForm {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent() {
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblSkinName = new System.Windows.Forms.Label();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.lblHint = new System.Windows.Forms.Label();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(206)))), ((int)(((byte)(250)))));
            this.topPanel.Controls.Add(this.lblSkinName);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.topPanel.Size = new System.Drawing.Size(484, 50);
            this.topPanel.TabIndex = 0;
            // 
            // lblSkinName
            // 
            this.lblSkinName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSkinName.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSkinName.ForeColor = System.Drawing.Color.White;
            this.lblSkinName.Location = new System.Drawing.Point(15, 10);
            this.lblSkinName.Name = "lblSkinName";
            this.lblSkinName.Size = new System.Drawing.Size(454, 30);
            this.lblSkinName.TabIndex = 0;
            this.lblSkinName.Text = "로딩중...";
            this.lblSkinName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 50);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(484, 471);
            this.webView.TabIndex = 1;
            this.webView.ZoomFactor = 1D;
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.Color.White;
            this.bottomPanel.Controls.Add(this.lblHint);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 521);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.bottomPanel.Size = new System.Drawing.Size(484, 40);
            this.bottomPanel.TabIndex = 2;
            // 
            // lblHint
            // 
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHint.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblHint.Location = new System.Drawing.Point(15, 10);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(454, 20);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "💡 스킨 3D 모델을 확인하세요";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SkinViewerForm
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.bottomPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "SkinViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "스킨 3D 뷰어";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkinViewerForm_FormClosing);
            this.Load += new System.EventHandler(this.SkinViewerForm_Load);
            this.topPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblSkinName;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label lblHint;
    }
}
