using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            List<Usuario> Test = new CN_Usuario().listar();

            Usuario usuario = new CN_Usuario().listar().Where(u=>u.Documento == txtDocumento.Text && u.Clave== txtClave.Text).FirstOrDefault();

            if(usuario != null)
            {
                Inicio form = new Inicio(usuario);
                form.Show();
                this.Hide();
                form.FormClosing += frm_closing;
            }
            else
            {
                MessageBox.Show("DATOS INCORRECTOS","SISTEMABLAS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                txtClave.Text = "";
                txtDocumento.Text = "";
            }

            

            
        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {

            txtDocumento.Text = "";
            txtClave.Text = "";
            this.Show();
        }

       
    }
}
