namespace MSP.Presentacion
{
    partial class FrmLogin
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel pnlLogin;
        private Label lblLoginTitulo;
        private Label lblUsuario;
        private TextBox txtUser;
        private Label lblPass;
        private TextBox txtPass;
        private Button btnEntrar;
        private LinkLabel lnkSetup;
        private Panel pnlSetup;
        private Label lblSetupTitulo;
        private Label lblRegNombre;
        private TextBox txtRegNombre;
        private Label lblRegUser;
        private TextBox txtRegUser;
        private Label lblRegPass;
        private TextBox txtRegPass;
        private Button btnCrearAdmin;
        private LinkLabel lnkVolver;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            pnlLogin = new Panel();
            lnkSetup = new LinkLabel();
            btnEntrar = new Button();
            txtPass = new TextBox();
            lblPass = new Label();
            txtUser = new TextBox();
            lblUsuario = new Label();
            lblLoginTitulo = new Label();
            pnlSetup = new Panel();
            lnkVolver = new LinkLabel();
            btnCrearAdmin = new Button();
            txtRegPass = new TextBox();
            lblRegPass = new Label();
            txtRegUser = new TextBox();
            lblRegUser = new Label();
            txtRegNombre = new TextBox();
            lblRegNombre = new Label();
            lblSetupTitulo = new Label();
            pnlHeader.SuspendLayout();
            pnlLogin.SuspendLayout();
            pnlSetup.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(13, 92, 99);
            pnlHeader.Controls.Add(lblSubtitulo);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(596, 115);
            pnlHeader.TabIndex = 2;
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.Dock = DockStyle.Fill;
            lblSubtitulo.Font = new Font("Segoe UI", 9.5F);
            lblSubtitulo.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtitulo.Location = new Point(0, 65);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(596, 50);
            lblSubtitulo.TabIndex = 0;
            lblSubtitulo.Text = "Plataforma Nacional Multi-Tenant de Establecimientos";
            lblSubtitulo.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(596, 65);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "MINISTERIO DE SALUD PÚBLICA";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlLogin
            // 
            pnlLogin.BackColor = Color.White;
            pnlLogin.Controls.Add(lnkSetup);
            pnlLogin.Controls.Add(btnEntrar);
            pnlLogin.Controls.Add(txtPass);
            pnlLogin.Controls.Add(lblPass);
            pnlLogin.Controls.Add(txtUser);
            pnlLogin.Controls.Add(lblUsuario);
            pnlLogin.Controls.Add(lblLoginTitulo);
            pnlLogin.Location = new Point(35, 140);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(380, 315);
            pnlLogin.TabIndex = 1;
            // 
            // lnkSetup
            // 
            lnkSetup.AutoSize = true;
            lnkSetup.LinkColor = Color.FromArgb(13, 92, 99);
            lnkSetup.Location = new Point(25, 265);
            lnkSetup.Name = "lnkSetup";
            lnkSetup.Size = new Size(442, 28);
            lnkSetup.TabIndex = 0;
            lnkSetup.TabStop = true;
            lnkSetup.Text = "¿Primera vez? Configurar Administrador Nacional";
            lnkSetup.Click += LnkSetup_Click;
            // 
            // btnEntrar
            // 
            btnEntrar.BackColor = Color.FromArgb(13, 92, 99);
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.Location = new Point(25, 208);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(325, 40);
            btnEntrar.TabIndex = 1;
            btnEntrar.Text = "Ingresar al Sistema";
            btnEntrar.UseVisualStyleBackColor = false;
            btnEntrar.Click += BtnEntrar_Click;
            // 
            // txtPass
            // 
            txtPass.Location = new Point(25, 158);
            txtPass.Name = "txtPass";
            txtPass.PlaceholderText = "••••••••";
            txtPass.Size = new Size(325, 34);
            txtPass.TabIndex = 2;
            txtPass.UseSystemPasswordChar = true;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.ForeColor = Color.FromArgb(71, 85, 105);
            lblPass.Location = new Point(25, 134);
            lblPass.Name = "lblPass";
            lblPass.Size = new Size(114, 28);
            lblPass.TabIndex = 3;
            lblPass.Text = "Contraseña:";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(25, 92);
            txtUser.Name = "txtUser";
            txtUser.PlaceholderText = "Ej: admin.nacional";
            txtUser.Size = new Size(325, 34);
            txtUser.TabIndex = 4;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.ForeColor = Color.FromArgb(71, 85, 105);
            lblUsuario.Location = new Point(25, 68);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(83, 28);
            lblUsuario.TabIndex = 5;
            lblUsuario.Text = "Usuario:";
            // 
            // lblLoginTitulo
            // 
            lblLoginTitulo.AutoSize = true;
            lblLoginTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblLoginTitulo.ForeColor = Color.FromArgb(30, 41, 59);
            lblLoginTitulo.Location = new Point(25, 20);
            lblLoginTitulo.Name = "lblLoginTitulo";
            lblLoginTitulo.Size = new Size(191, 38);
            lblLoginTitulo.TabIndex = 6;
            lblLoginTitulo.Text = "Iniciar Sesión";
            // 
            // pnlSetup
            // 
            pnlSetup.BackColor = Color.White;
            pnlSetup.Controls.Add(lnkVolver);
            pnlSetup.Controls.Add(btnCrearAdmin);
            pnlSetup.Controls.Add(txtRegPass);
            pnlSetup.Controls.Add(lblRegPass);
            pnlSetup.Controls.Add(txtRegUser);
            pnlSetup.Controls.Add(lblRegUser);
            pnlSetup.Controls.Add(txtRegNombre);
            pnlSetup.Controls.Add(lblRegNombre);
            pnlSetup.Controls.Add(lblSetupTitulo);
            pnlSetup.Location = new Point(35, 140);
            pnlSetup.Name = "pnlSetup";
            pnlSetup.Size = new Size(527, 382);
            pnlSetup.TabIndex = 0;
            pnlSetup.Visible = false;
            // 
            // lnkVolver
            // 
            lnkVolver.AutoSize = true;
            lnkVolver.Location = new Point(133, 340);
            lnkVolver.Name = "lnkVolver";
            lnkVolver.Size = new Size(247, 28);
            lnkVolver.TabIndex = 0;
            lnkVolver.TabStop = true;
            lnkVolver.Text = "← Volver al inicio de sesión";
            lnkVolver.Click += LnkVolver_Click;
            // 
            // btnCrearAdmin
            // 
            btnCrearAdmin.BackColor = Color.FromArgb(16, 124, 65);
            btnCrearAdmin.Cursor = Cursors.Hand;
            btnCrearAdmin.FlatAppearance.BorderSize = 0;
            btnCrearAdmin.FlatStyle = FlatStyle.Flat;
            btnCrearAdmin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCrearAdmin.ForeColor = Color.White;
            btnCrearAdmin.Location = new Point(83, 286);
            btnCrearAdmin.Name = "btnCrearAdmin";
            btnCrearAdmin.Size = new Size(330, 38);
            btnCrearAdmin.TabIndex = 1;
            btnCrearAdmin.Text = "Crear Administrador Nacional";
            btnCrearAdmin.UseVisualStyleBackColor = false;
            btnCrearAdmin.Click += BtnCrearAdmin_Click;
            // 
            // txtRegPass
            // 
            txtRegPass.Location = new Point(83, 228);
            txtRegPass.Name = "txtRegPass";
            txtRegPass.Size = new Size(330, 34);
            txtRegPass.TabIndex = 2;
            txtRegPass.UseSystemPasswordChar = true;
            // 
            // lblRegPass
            // 
            lblRegPass.AutoSize = true;
            lblRegPass.Location = new Point(83, 195);
            lblRegPass.Name = "lblRegPass";
            lblRegPass.Size = new Size(276, 28);
            lblRegPass.TabIndex = 3;
            lblRegPass.Text = "Contraseña (mín. 6 caracteres):";
            // 
            // txtRegUser
            // 
            txtRegUser.Location = new Point(83, 158);
            txtRegUser.Name = "txtRegUser";
            txtRegUser.PlaceholderText = "Ej: admin.nacional";
            txtRegUser.Size = new Size(330, 34);
            txtRegUser.TabIndex = 4;
            // 
            // lblRegUser
            // 
            lblRegUser.AutoSize = true;
            lblRegUser.Location = new Point(83, 127);
            lblRegUser.Name = "lblRegUser";
            lblRegUser.Size = new Size(83, 28);
            lblRegUser.TabIndex = 5;
            lblRegUser.Text = "Usuario:";
            // 
            // txtRegNombre
            // 
            txtRegNombre.Location = new Point(83, 92);
            txtRegNombre.Name = "txtRegNombre";
            txtRegNombre.PlaceholderText = "Ej: Dra. Katherine Ramírez";
            txtRegNombre.Size = new Size(330, 34);
            txtRegNombre.TabIndex = 6;
            // 
            // lblRegNombre
            // 
            lblRegNombre.AutoSize = true;
            lblRegNombre.Location = new Point(83, 61);
            lblRegNombre.Name = "lblRegNombre";
            lblRegNombre.Size = new Size(181, 28);
            lblRegNombre.TabIndex = 7;
            lblRegNombre.Text = "Nombre Completo:";
            // 
            // lblSetupTitulo
            // 
            lblSetupTitulo.AutoSize = true;
            lblSetupTitulo.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            lblSetupTitulo.ForeColor = Color.FromArgb(13, 92, 99);
            lblSetupTitulo.Location = new Point(34, 19);
            lblSetupTitulo.Name = "lblSetupTitulo";
            lblSetupTitulo.Size = new Size(433, 31);
            lblSetupTitulo.TabIndex = 8;
            lblSetupTitulo.Text = "Configuración Inicial (Admin Nacional)";
            // 
            // FrmLogin
            // 
            AcceptButton = btnEntrar;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(596, 557);
            Controls.Add(pnlSetup);
            Controls.Add(pnlLogin);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MSP · Acceso Multi-Tenant";
            Load += FrmLogin_Load;
            pnlHeader.ResumeLayout(false);
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            pnlSetup.ResumeLayout(false);
            pnlSetup.PerformLayout();
            ResumeLayout(false);
        }
    }
}