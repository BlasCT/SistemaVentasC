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
    public partial class frmVerDetalleVenta : Form
    {
        public frmVerDetalleVenta()
        {
            InitializeComponent();
        }

        private void frmVerDetalleVenta_Load(object sender, EventArgs e)
        {
            txtNumeroDocumento.Select();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Venta oVenta = new CN_Venta().ObtenerVenta(txtNumeroDocumento.Text);

            if(oVenta.IdVenta != 0)
            {
                txtNumeroDeDocumento.Text = oVenta.NumeroDocumento;

                txtFecha.Text = oVenta.FechaCreacion;
                txtTipoDocumento.Text = oVenta.TipoDocumento;
                txtUsuario.Text = oVenta.oUsuario.NombreCompleto;

                txtDocumentoCliente.Text = oVenta.DocumentoCliente;
                txtNombreCliente.Text = oVenta.NombreCliente;

                dgvData.Rows.Clear();

                foreach(Detalle_Venta dv in oVenta.oDetalleVenta)
                {
                    dgvData.Rows.Add(new object[] { dv.oProducto.Nombre, dv.PrecioVenta, dv.Cantidad, dv.SubTotal });
                }

                txtMontoTotal.Text = oVenta.MontoTotal.ToString("0.00");
                txtMontoPago.Text = oVenta.MontoPago.ToString("0.00");
                txtMontoCambio.Text = oVenta.MontoCambio.ToString("0.00");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFecha.Text = "";
            txtDocumentoCliente.Text = "";
            txtUsuario.Text = "";
            txtDocumentoCliente.Text = "";
            txtNombreCliente.Text = "";

            dgvData.Rows.Clear();
            txtMontoCambio.Text = "0.00";
            txtMontoPago.Text = "0.00";
            txtMontoTotal.Text = "0.00";
        }

        private void btnDescargarPDF_Click(object sender, EventArgs e)
        {
            if (txtTipoDocumento.Text == "")
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string TextoHtml = Properties.Resources.PlantillaVenta.ToString();
            Negocio oDatos = new CN_Negocio().ObtenerDatos();

            TextoHtml = TextoHtml.Replace("@nombrenegocio", oDatos.Nombre.ToUpper());
            TextoHtml = TextoHtml.Replace("@docnegocio", oDatos.RUC);
            TextoHtml = TextoHtml.Replace("@direcnegocio", oDatos.Direccion);

            TextoHtml = TextoHtml.Replace("@tipodocumento", txtTipoDocumento.Text.ToUpper());
            TextoHtml = TextoHtml.Replace("@numerodocumento", txtNumeroDocumento.Text);

            TextoHtml = TextoHtml.Replace("@doccliente", txtDocumentoCliente.Text);
            TextoHtml = TextoHtml.Replace("@nombrecliente", txtNombreCliente.Text);
            TextoHtml = TextoHtml.Replace("@fecharegistro", txtFecha.Text);
            TextoHtml = TextoHtml.Replace("@usuarioregistro", txtUsuario.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Precio"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            TextoHtml = TextoHtml.Replace("@filas", filas);
            TextoHtml = TextoHtml.Replace("@montototal", txtMontoTotal.Text);
            TextoHtml = TextoHtml.Replace("@pagocon", txtMontoPago.Text);
            TextoHtml = TextoHtml.Replace("@cambio", txtMontoCambio.Text);

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Venta_{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "Pdf Files |*.pdf";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
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
                    MessageBox.Show("Documento generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
