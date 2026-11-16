using MSP.Negocio;

namespace MSP.Presentacion
{
    public partial class FrmLogin : Form
    {
        private readonly AuthService _auth = new();

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object? sender, EventArgs e)
        {
            try
            {
                if (_auth.SistemaRequiereConfiguracionInicial())
                {
                    MessageBox.Show("La base de datos no tiene usuarios registrados. Cree el Administrador Nacional inicial desde esta pantalla.", "Configuración Inicial", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MostrarPanel(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error conectando con SQL Server:\n{ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPanel(bool setup)
        {
            pnlSetup.Visible = setup;
            pnlLogin.Visible = !setup;
        }

        private void LnkSetup_Click(object? sender, EventArgs e) => MostrarPanel(true);
        private void LnkVolver_Click(object? sender, EventArgs e) => MostrarPanel(false);

        private void BtnEntrar_Click(object? sender, EventArgs e)
        {
            try
            {
                _auth.IniciarSesion(txtUser.Text, txtPass.Text);
                Hide();
                //using var principal = new FrmPrincipal();
                //principal.ShowDialog();
                txtPass.Clear();
                Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCrearAdmin_Click(object? sender, EventArgs e)
        {
            try
            {
                _auth.RegistrarPrimerAdminNacional(txtRegNombre.Text, txtRegUser.Text, txtRegPass.Text);
                MessageBox.Show("Administrador Nacional creado exitosamente. Ahora puede iniciar sesión.", "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUser.Text = txtRegUser.Text;
                MostrarPanel(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}