using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte
    {
        private CD_Reporte objcd_reporte = new CD_Reporte();

        public List<ReporteCompra> Compra (string fechaInicio, string fechaFinal, int idProveedor)
        {
            return objcd_reporte.Compra(fechaInicio, fechaFinal, idProveedor);
        }

        public List<ReporteVenta> Venta(string fechaInicio, string fechaFinal)
        {
            return objcd_reporte.Venta(fechaInicio, fechaFinal);
        }
    }
}
