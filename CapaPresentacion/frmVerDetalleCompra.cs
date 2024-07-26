using CapaEntidad;
using CapaNegocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmVerDetalleCompra : Form
    {
        public frmVerDetalleCompra()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtBusquedaDocumento.Text);
            if (oCompra.IdCompra != 0)
            {
                txtBusquedaDocumento.Text = oCompra.NumeroDocumento;

                txtFechaCompra.Text = oCompra.FechaCreacion;
                txtDocumentoCompra.Text = oCompra.TipoDocumento;
                txtUsuarioCompra.Text = oCompra.oUsuario.NombreCompleto;
                txtDocumentoProveedor.Text = oCompra.oProveedor.Documento;
                txtRazonSocialProveedor.Text = oCompra.oProveedor.RazonSocial;

                dgvData.Rows.Clear();

                foreach(Detalle_Compra dc in oCompra.oDetalleCompra)
                {
                    dgvData.Rows.Add(new object[] { dc.oProducto.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal });
                }

                txtMontoTotal.Text = oCompra.MontoTotal.ToString("0.00");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFechaCompra.Text = "";
            txtDocumentoCompra.Text = "";
            txtUsuarioCompra.Text = "";
            txtDocumentoProveedor.Text = "";
            txtRazonSocialProveedor.Text = "";

            dgvData.Rows.Clear();
            txtMontoTotal.Text = "0.00";
        }

        private void btnDescargarPDF_Click(object sender, EventArgs e)
        {
            if (txtDocumentoCompra.Text == "")
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string TextoHtml = Properties.Resources.PlantillaCompra.ToString();
            Negocio oDatos = new CN_Negocio().ObtenerDatos();
            
            TextoHtml = TextoHtml.Replace("@nombrenegocio", oDatos.Nombre.ToUpper());
            TextoHtml = TextoHtml.Replace("@docnegocio", oDatos.RUC);
            TextoHtml = TextoHtml.Replace("@direcnegocio", oDatos.Direccion);

            TextoHtml = TextoHtml.Replace("@tipodocumento", txtDocumentoCompra.Text.ToUpper());
            TextoHtml = TextoHtml.Replace("@numerodocumento", txtDocumento.Text);

            TextoHtml = TextoHtml.Replace("@docproveedor", txtDocumentoProveedor.Text);
            TextoHtml = TextoHtml.Replace("@nombreproveedor", txtRazonSocialProveedor.Text);
            TextoHtml = TextoHtml.Replace("@fecharegistro", txtFechaCompra.Text);
            TextoHtml = TextoHtml.Replace("@usuarioregistro", txtUsuarioCompra.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            TextoHtml = TextoHtml.Replace("@filas", filas);
            TextoHtml = TextoHtml.Replace("@montototal", txtMontoTotal.Text);

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Compra_{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "Pdf Files |*.pdf";

            if(savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4,25,25,25,25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    bool obtenido = true;
                    byte[] byteImage = new CN_Negocio().ObtenerLogo(out obtenido);

                    if (obtenido)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(byteImage);
                        img.ScaleToFit(60, 60);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        img.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                        pdfDoc.Add(img);
                    }

                    using (StringReader sr = new StringReader(TextoHtml))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    pdfDoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento generado", "Mensaje", MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }

        private void frmVerDetalleCompra_Load(object sender, EventArgs e)
        {

        }
    }
}
