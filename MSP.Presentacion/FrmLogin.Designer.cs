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
        private Panel pnlHeaderAcento;
        private Panel pnlLoginBody;
        private Panel pnlLoginBarra;
        private Panel pnlLoginAcento;
        private Panel pnlLoginDivisor;
        private Panel pnlUserBox;
        private Panel pnlUserInner;
        private Panel pnlPassBox;
        private Panel pnlPassInner;
        private Panel pnlSetupBody;
        private Panel pnlSetupBarra;
        private Panel pnlSetupAcento;
        private Panel pnlSetupDivisor;
        private Panel pnlRegNombreBox;
        private Panel pnlRegNombreInner;
        private Panel pnlRegUserBox;
        private Panel pnlRegUserInner;
        private Panel pnlRegPassBox;
        private Panel pnlRegPassInner;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            pnlLogin = new Panel();
            lblLoginTitulo = new Label();
            lblUsuario = new Label();
            txtUser = new TextBox();
            lblPass = new Label();
            txtPass = new TextBox();
            btnEntrar = new Button();
            lnkSetup = new LinkLabel();
            pnlSetup = new Panel();
            lblSetupTitulo = new Label();
            lblRegNombre = new Label();
            txtRegNombre = new TextBox();
            lblRegUser = new Label();
            txtRegUser = new TextBox();
            lblRegPass = new Label();
            txtRegPass = new TextBox();
            btnCrearAdmin = new Button();
            lnkVolver = new LinkLabel();
            pnlHeaderAcento = new Panel();
            pnlLoginBody = new Panel();
            pnlLoginBarra = new Panel();
            pnlLoginAcento = new Panel();
            pnlLoginDivisor = new Panel();
            pnlUserBox = new Panel();
            pnlUserInner = new Panel();
            pnlPassBox = new Panel();
            pnlPassInner = new Panel();
            pnlSetupBody = new Panel();
            pnlSetupBarra = new Panel();
            pnlSetupAcento = new Panel();
            pnlSetupDivisor = new Panel();
            pnlRegNombreBox = new Panel();
            pnlRegNombreInner = new Panel();
            pnlRegUserBox = new Panel();
            pnlRegUserInner = new Panel();
            pnlRegPassBox = new Panel();
            pnlRegPassInner = new Panel();
            pnlHeader.SuspendLayout();
            pnlLogin.SuspendLayout();
            pnlLoginBody.SuspendLayout();
            pnlUserBox.SuspendLayout();
            pnlUserInner.SuspendLayout();
            pnlPassBox.SuspendLayout();
            pnlPassInner.SuspendLayout();
            pnlSetup.SuspendLayout();
            pnlSetupBody.SuspendLayout();
            pnlRegNombreBox.SuspendLayout();
            pnlRegNombreInner.SuspendLayout();
            pnlRegUserBox.SuspendLayout();
            pnlRegUserInner.SuspendLayout();
            pnlRegPassBox.SuspendLayout();
            pnlRegPassInner.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(13, 92, 99);
            pnlHeader.Controls.Add(lblSubtitulo);
            pnlHeader.Controls.Add(pnlHeaderAcento);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(480, 108);
            pnlHeader.TabIndex = 2;
            // 
            // pnlHeaderAcento
            // 
            pnlHeaderAcento.BackColor = Color.FromArgb(204, 251, 241);
            pnlHeaderAcento.Dock = DockStyle.Bottom;
            pnlHeaderAcento.Location = new Point(0, 104);
            pnlHeaderAcento.Name = "pnlHeaderAcento";
            pnlHeaderAcento.Size = new Size(480, 4);
            pnlHeaderAcento.TabIndex = 2;
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.Dock = DockStyle.Fill;
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.FromArgb(204, 251, 241);
            lblSubtitulo.Location = new Point(0, 56);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Padding = new Padding(0, 3, 0, 0);
            lblSubtitulo.Size = new Size(480, 48);
            lblSubtitulo.TabIndex = 0;
            lblSubtitulo.Text = "Plataforma Nacional Multi-Tenant de Establecimientos";
            lblSubtitulo.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(480, 56);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "MINISTERIO DE SALUD PÚBLICA";
            lblTitulo.TextAlign = ContentAlignment.BottomCenter;
            // 
            // pnlLogin
            // 
            pnlLogin.BackColor = Color.FromArgb(203, 213, 225);
            pnlLogin.Controls.Add(pnlLoginBody);
            pnlLogin.Location = new Point(40, 165);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Padding = new Padding(1);
            pnlLogin.Size = new Size(400, 358);
            pnlLogin.TabIndex = 1;
            // 
            // pnlLoginBody
            // 
            pnlLoginBody.BackColor = Color.White;
            pnlLoginBody.Controls.Add(lnkSetup);
            pnlLoginBody.Controls.Add(pnlLoginDivisor);
            pnlLoginBody.Controls.Add(btnEntrar);
            pnlLoginBody.Controls.Add(pnlPassBox);
            pnlLoginBody.Controls.Add(lblPass);
            pnlLoginBody.Controls.Add(pnlUserBox);
            pnlLoginBody.Controls.Add(lblUsuario);
            pnlLoginBody.Controls.Add(pnlLoginAcento);
            pnlLoginBody.Controls.Add(lblLoginTitulo);
            pnlLoginBody.Controls.Add(pnlLoginBarra);
            pnlLoginBody.Dock = DockStyle.Fill;
            pnlLoginBody.Location = new Point(1, 1);
            pnlLoginBody.Name = "pnlLoginBody";
            pnlLoginBody.Size = new Size(398, 356);
            pnlLoginBody.TabIndex = 0;
            // 
            // pnlLoginBarra
            // 
            pnlLoginBarra.BackColor = Color.FromArgb(13, 92, 99);
            pnlLoginBarra.Dock = DockStyle.Top;
            pnlLoginBarra.Location = new Point(0, 0);
            pnlLoginBarra.Name = "pnlLoginBarra";
            pnlLoginBarra.Size = new Size(398, 4);
            pnlLoginBarra.TabIndex = 9;
            // 
            // lblLoginTitulo
            // 
            lblLoginTitulo.AutoSize = true;
            lblLoginTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblLoginTitulo.ForeColor = Color.FromArgb(30, 41, 59);
            lblLoginTitulo.Location = new Point(28, 24);
            lblLoginTitulo.Name = "lblLoginTitulo";
            lblLoginTitulo.Size = new Size(190, 22);
            lblLoginTitulo.TabIndex = 8;
            lblLoginTitulo.Text = "Iniciar Sesión";
            // 
            // pnlLoginAcento
            // 
            pnlLoginAcento.BackColor = Color.FromArgb(13, 92, 99);
            pnlLoginAcento.Location = new Point(28, 52);
            pnlLoginAcento.Name = "pnlLoginAcento";
            pnlLoginAcento.Size = new Size(40, 3);
            pnlLoginAcento.TabIndex = 7;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI Semibold", 9.5F);
            lblUsuario.ForeColor = Color.FromArgb(71, 85, 105);
            lblUsuario.Location = new Point(28, 78);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(60, 17);
            lblUsuario.TabIndex = 0;
            lblUsuario.Text = "Usuario:";
            // 
            // pnlUserBox
            // 
            pnlUserBox.BackColor = Color.FromArgb(203, 213, 225);
            pnlUserBox.Controls.Add(pnlUserInner);
            pnlUserBox.Location = new Point(28, 100);
            pnlUserBox.Name = "pnlUserBox";
            pnlUserBox.Padding = new Padding(1);
            pnlUserBox.Size = new Size(342, 36);
            pnlUserBox.TabIndex = 1;
            // 
            // pnlUserInner
            // 
            pnlUserInner.BackColor = Color.White;
            pnlUserInner.Controls.Add(txtUser);
            pnlUserInner.Dock = DockStyle.Fill;
            pnlUserInner.Location = new Point(1, 1);
            pnlUserInner.Name = "pnlUserInner";
            pnlUserInner.Padding = new Padding(10, 8, 10, 6);
            pnlUserInner.Size = new Size(340, 34);
            pnlUserInner.TabIndex = 0;
            // 
            // txtUser
            // 
            txtUser.BackColor = Color.White;
            txtUser.BorderStyle = BorderStyle.None;
            txtUser.Dock = DockStyle.Fill;
            txtUser.ForeColor = Color.FromArgb(30, 41, 59);
            txtUser.Location = new Point(10, 8);
            txtUser.Name = "txtUser";
            txtUser.PlaceholderText = "Ej: admin.nacional";
            txtUser.Size = new Size(320, 20);
            txtUser.TabIndex = 0;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.Font = new Font("Segoe UI Semibold", 9.5F);
            lblPass.ForeColor = Color.FromArgb(71, 85, 105);
            lblPass.Location = new Point(28, 148);
            lblPass.Name = "lblPass";
            lblPass.Size = new Size(78, 17);
            lblPass.TabIndex = 2;
            lblPass.Text = "Contraseña:";
            // 
            // pnlPassBox
            // 
            pnlPassBox.BackColor = Color.FromArgb(203, 213, 225);
            pnlPassBox.Controls.Add(pnlPassInner);
            pnlPassBox.Location = new Point(28, 170);
            pnlPassBox.Name = "pnlPassBox";
            pnlPassBox.Padding = new Padding(1);
            pnlPassBox.Size = new Size(342, 36);
            pnlPassBox.TabIndex = 3;
            // 
            // pnlPassInner
            // 
            pnlPassInner.BackColor = Color.White;
            pnlPassInner.Controls.Add(txtPass);
            pnlPassInner.Dock = DockStyle.Fill;
            pnlPassInner.Location = new Point(1, 1);
            pnlPassInner.Name = "pnlPassInner";
            pnlPassInner.Padding = new Padding(10, 8, 10, 6);
            pnlPassInner.Size = new Size(340, 34);
            pnlPassInner.TabIndex = 0;
            // 
            // txtPass
            // 
            txtPass.BackColor = Color.White;
            txtPass.BorderStyle = BorderStyle.None;
            txtPass.Dock = DockStyle.Fill;
            txtPass.ForeColor = Color.FromArgb(30, 41, 59);
            txtPass.Location = new Point(10, 8);
            txtPass.Name = "txtPass";
            txtPass.PlaceholderText = "••••••••";
            txtPass.Size = new Size(320, 20);
            txtPass.TabIndex = 0;
            txtPass.UseSystemPasswordChar = true;
            // 
            // btnEntrar
            // 
            btnEntrar.BackColor = Color.FromArgb(13, 92, 99);
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.FlatAppearance.MouseDownBackColor = Color.FromArgb(7, 58, 63);
            btnEntrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(9, 74, 80);
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.Location = new Point(28, 232);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(342, 42);
            btnEntrar.TabIndex = 4;
            btnEntrar.Text = "Ingresar al Sistema";
            btnEntrar.UseVisualStyleBackColor = false;
            btnEntrar.Click += BtnEntrar_Click;
            // 
            // pnlLoginDivisor
            // 
            pnlLoginDivisor.BackColor = Color.FromArgb(226, 232, 240);
            pnlLoginDivisor.Location = new Point(28, 298);
            pnlLoginDivisor.Name = "pnlLoginDivisor";
            pnlLoginDivisor.Size = new Size(342, 1);
            pnlLoginDivisor.TabIndex = 5;
            // 
            // lnkSetup
            // 
            lnkSetup.ActiveLinkColor = Color.FromArgb(7, 58, 63);
            lnkSetup.AutoSize = false;
            lnkSetup.Font = new Font("Segoe UI", 9.5F);
            lnkSetup.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkSetup.LinkColor = Color.FromArgb(13, 92, 99);
            lnkSetup.Location = new Point(28, 312);
            lnkSetup.Name = "lnkSetup";
            lnkSetup.Size = new Size(342, 22);
            lnkSetup.TabIndex = 6;
            lnkSetup.TabStop = true;
            lnkSetup.Text = "¿Primera vez? Configurar Administrador Nacional";
            lnkSetup.TextAlign = ContentAlignment.MiddleCenter;
            lnkSetup.VisitedLinkColor = Color.FromArgb(13, 92, 99);
            lnkSetup.Click += LnkSetup_Click;
            // 
            // pnlSetup
            // 
            pnlSetup.BackColor = Color.FromArgb(203, 213, 225);
            pnlSetup.Controls.Add(pnlSetupBody);
            pnlSetup.Location = new Point(40, 132);
            pnlSetup.Name = "pnlSetup";
            pnlSetup.Padding = new Padding(1);
            pnlSetup.Size = new Size(400, 424);
            pnlSetup.TabIndex = 0;
            pnlSetup.Visible = false;
            // 
            // pnlSetupBody
            // 
            pnlSetupBody.BackColor = Color.White;
            pnlSetupBody.Controls.Add(lnkVolver);
            pnlSetupBody.Controls.Add(pnlSetupDivisor);
            pnlSetupBody.Controls.Add(btnCrearAdmin);
            pnlSetupBody.Controls.Add(pnlRegPassBox);
            pnlSetupBody.Controls.Add(lblRegPass);
            pnlSetupBody.Controls.Add(pnlRegUserBox);
            pnlSetupBody.Controls.Add(lblRegUser);
            pnlSetupBody.Controls.Add(pnlRegNombreBox);
            pnlSetupBody.Controls.Add(lblRegNombre);
            pnlSetupBody.Controls.Add(pnlSetupAcento);
            pnlSetupBody.Controls.Add(lblSetupTitulo);
            pnlSetupBody.Controls.Add(pnlSetupBarra);
            pnlSetupBody.Dock = DockStyle.Fill;
            pnlSetupBody.Location = new Point(1, 1);
            pnlSetupBody.Name = "pnlSetupBody";
            pnlSetupBody.Size = new Size(398, 422);
            pnlSetupBody.TabIndex = 0;
            // 
            // pnlSetupBarra
            // 
            pnlSetupBarra.BackColor = Color.FromArgb(13, 92, 99);
            pnlSetupBarra.Dock = DockStyle.Top;
            pnlSetupBarra.Location = new Point(0, 0);
            pnlSetupBarra.Name = "pnlSetupBarra";
            pnlSetupBarra.Size = new Size(398, 4);
            pnlSetupBarra.TabIndex = 11;
            // 
            // lblSetupTitulo
            // 
            lblSetupTitulo.AutoSize = true;
            lblSetupTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSetupTitulo.ForeColor = Color.FromArgb(30, 41, 59);
            lblSetupTitulo.Location = new Point(28, 24);
            lblSetupTitulo.Name = "lblSetupTitulo";
            lblSetupTitulo.Size = new Size(190, 22);
            lblSetupTitulo.TabIndex = 10;
            lblSetupTitulo.Text = "Configuración Inicial (Admin Nacional)";
            // 
            // pnlSetupAcento
            // 
            pnlSetupAcento.BackColor = Color.FromArgb(13, 92, 99);
            pnlSetupAcento.Location = new Point(28, 52);
            pnlSetupAcento.Name = "pnlSetupAcento";
            pnlSetupAcento.Size = new Size(40, 3);
            pnlSetupAcento.TabIndex = 9;
            // 
            // lblRegNombre
            // 
            lblRegNombre.AutoSize = true;
            lblRegNombre.Font = new Font("Segoe UI Semibold", 9.5F);
            lblRegNombre.ForeColor = Color.FromArgb(71, 85, 105);
            lblRegNombre.Location = new Point(28, 78);
            lblRegNombre.Name = "lblRegNombre";
            lblRegNombre.Size = new Size(110, 17);
            lblRegNombre.TabIndex = 0;
            lblRegNombre.Text = "Nombre Completo:";
            // 
            // pnlRegNombreBox
            // 
            pnlRegNombreBox.BackColor = Color.FromArgb(203, 213, 225);
            pnlRegNombreBox.Controls.Add(pnlRegNombreInner);
            pnlRegNombreBox.Location = new Point(28, 100);
            pnlRegNombreBox.Name = "pnlRegNombreBox";
            pnlRegNombreBox.Padding = new Padding(1);
            pnlRegNombreBox.Size = new Size(342, 36);
            pnlRegNombreBox.TabIndex = 1;
            // 
            // pnlRegNombreInner
            // 
            pnlRegNombreInner.BackColor = Color.White;
            pnlRegNombreInner.Controls.Add(txtRegNombre);
            pnlRegNombreInner.Dock = DockStyle.Fill;
            pnlRegNombreInner.Location = new Point(1, 1);
            pnlRegNombreInner.Name = "pnlRegNombreInner";
            pnlRegNombreInner.Padding = new Padding(10, 8, 10, 6);
            pnlRegNombreInner.Size = new Size(340, 34);
            pnlRegNombreInner.TabIndex = 0;
            // 
            // txtRegNombre
            // 
            txtRegNombre.BackColor = Color.White;
            txtRegNombre.BorderStyle = BorderStyle.None;
            txtRegNombre.Dock = DockStyle.Fill;
            txtRegNombre.ForeColor = Color.FromArgb(30, 41, 59);
            txtRegNombre.Location = new Point(10, 8);
            txtRegNombre.Name = "txtRegNombre";
            txtRegNombre.PlaceholderText = "Ej: Dra. Katherine Ramírez";
            txtRegNombre.Size = new Size(320, 20);
            txtRegNombre.TabIndex = 0;
            // 
            // lblRegUser
            // 
            lblRegUser.AutoSize = true;
            lblRegUser.Font = new Font("Segoe UI Semibold", 9.5F);
            lblRegUser.ForeColor = Color.FromArgb(71, 85, 105);
            lblRegUser.Location = new Point(28, 148);
            lblRegUser.Name = "lblRegUser";
            lblRegUser.Size = new Size(60, 17);
            lblRegUser.TabIndex = 2;
            lblRegUser.Text = "Usuario:";
            // 
            // pnlRegUserBox
            // 
            pnlRegUserBox.BackColor = Color.FromArgb(203, 213, 225);
            pnlRegUserBox.Controls.Add(pnlRegUserInner);
            pnlRegUserBox.Location = new Point(28, 170);
            pnlRegUserBox.Name = "pnlRegUserBox";
            pnlRegUserBox.Padding = new Padding(1);
            pnlRegUserBox.Size = new Size(342, 36);
            pnlRegUserBox.TabIndex = 3;
            // 
            // pnlRegUserInner
            // 
            pnlRegUserInner.BackColor = Color.White;
            pnlRegUserInner.Controls.Add(txtRegUser);
            pnlRegUserInner.Dock = DockStyle.Fill;
            pnlRegUserInner.Location = new Point(1, 1);
            pnlRegUserInner.Name = "pnlRegUserInner";
            pnlRegUserInner.Padding = new Padding(10, 8, 10, 6);
            pnlRegUserInner.Size = new Size(340, 34);
            pnlRegUserInner.TabIndex = 0;
            // 
            // txtRegUser
            // 
            txtRegUser.BackColor = Color.White;
            txtRegUser.BorderStyle = BorderStyle.None;
            txtRegUser.Dock = DockStyle.Fill;
            txtRegUser.ForeColor = Color.FromArgb(30, 41, 59);
            txtRegUser.Location = new Point(10, 8);
            txtRegUser.Name = "txtRegUser";
            txtRegUser.PlaceholderText = "Ej: admin.nacional";
            txtRegUser.Size = new Size(320, 20);
            txtRegUser.TabIndex = 0;
            // 
            // lblRegPass
            // 
            lblRegPass.AutoSize = true;
            lblRegPass.Font = new Font("Segoe UI Semibold", 9.5F);
            lblRegPass.ForeColor = Color.FromArgb(71, 85, 105);
            lblRegPass.Location = new Point(28, 218);
            lblRegPass.Name = "lblRegPass";
            lblRegPass.Size = new Size(190, 17);
            lblRegPass.TabIndex = 4;
            lblRegPass.Text = "Contraseña (mín. 6 caracteres):";
            // 
            // pnlRegPassBox
            // 
            pnlRegPassBox.BackColor = Color.FromArgb(203, 213, 225);
            pnlRegPassBox.Controls.Add(pnlRegPassInner);
            pnlRegPassBox.Location = new Point(28, 240);
            pnlRegPassBox.Name = "pnlRegPassBox";
            pnlRegPassBox.Padding = new Padding(1);
            pnlRegPassBox.Size = new Size(342, 36);
            pnlRegPassBox.TabIndex = 5;
            // 
            // pnlRegPassInner
            // 
            pnlRegPassInner.BackColor = Color.White;
            pnlRegPassInner.Controls.Add(txtRegPass);
            pnlRegPassInner.Dock = DockStyle.Fill;
            pnlRegPassInner.Location = new Point(1, 1);
            pnlRegPassInner.Name = "pnlRegPassInner";
            pnlRegPassInner.Padding = new Padding(10, 8, 10, 6);
            pnlRegPassInner.Size = new Size(340, 34);
            pnlRegPassInner.TabIndex = 0;
            // 
            // txtRegPass
            // 
            txtRegPass.BackColor = Color.White;
            txtRegPass.BorderStyle = BorderStyle.None;
            txtRegPass.Dock = DockStyle.Fill;
            txtRegPass.ForeColor = Color.FromArgb(30, 41, 59);
            txtRegPass.Location = new Point(10, 8);
            txtRegPass.Name = "txtRegPass";
            txtRegPass.Size = new Size(320, 20);
            txtRegPass.TabIndex = 0;
            txtRegPass.UseSystemPasswordChar = true;
            // 
            // btnCrearAdmin
            // 
            btnCrearAdmin.BackColor = Color.FromArgb(16, 124, 65);
            btnCrearAdmin.Cursor = Cursors.Hand;
            btnCrearAdmin.FlatAppearance.BorderSize = 0;
            btnCrearAdmin.FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 86, 45);
            btnCrearAdmin.FlatAppearance.MouseOverBackColor = Color.FromArgb(13, 104, 54);
            btnCrearAdmin.FlatStyle = FlatStyle.Flat;
            btnCrearAdmin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCrearAdmin.ForeColor = Color.White;
            btnCrearAdmin.Location = new Point(28, 298);
            btnCrearAdmin.Name = "btnCrearAdmin";
            btnCrearAdmin.Size = new Size(342, 42);
            btnCrearAdmin.TabIndex = 6;
            btnCrearAdmin.Text = "Crear Administrador Nacional";
            btnCrearAdmin.UseVisualStyleBackColor = false;
            btnCrearAdmin.Click += BtnCrearAdmin_Click;
            // 
            // pnlSetupDivisor
            // 
            pnlSetupDivisor.BackColor = Color.FromArgb(226, 232, 240);
            pnlSetupDivisor.Location = new Point(28, 364);
            pnlSetupDivisor.Name = "pnlSetupDivisor";
            pnlSetupDivisor.Size = new Size(342, 1);
            pnlSetupDivisor.TabIndex = 7;
            // 
            // lnkVolver
            // 
            lnkVolver.ActiveLinkColor = Color.FromArgb(7, 58, 63);
            lnkVolver.AutoSize = false;
            lnkVolver.Font = new Font("Segoe UI", 9.5F);
            lnkVolver.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkVolver.LinkColor = Color.FromArgb(13, 92, 99);
            lnkVolver.Location = new Point(28, 378);
            lnkVolver.Name = "lnkVolver";
            lnkVolver.Size = new Size(342, 22);
            lnkVolver.TabIndex = 8;
            lnkVolver.TabStop = true;
            lnkVolver.Text = "← Volver al inicio de sesión";
            lnkVolver.TextAlign = ContentAlignment.MiddleCenter;
            lnkVolver.VisitedLinkColor = Color.FromArgb(13, 92, 99);
            lnkVolver.Click += LnkVolver_Click;
            // 
            // FrmLogin
            // 
            AcceptButton = btnEntrar;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(480, 580);
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
            pnlRegPassInner.ResumeLayout(false);
            pnlRegPassBox.ResumeLayout(false);
            pnlRegUserInner.ResumeLayout(false);
            pnlRegUserBox.ResumeLayout(false);
            pnlRegNombreInner.ResumeLayout(false);
            pnlRegNombreBox.ResumeLayout(false);
            pnlSetupBody.ResumeLayout(false);
            pnlSetup.ResumeLayout(false);
            pnlPassInner.ResumeLayout(false);
            pnlPassBox.ResumeLayout(false);
            pnlUserInner.ResumeLayout(false);
            pnlUserBox.ResumeLayout(false);
            pnlLoginBody.ResumeLayout(false);
            pnlLogin.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}