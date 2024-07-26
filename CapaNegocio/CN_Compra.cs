using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Compra
    {
        private CD_Compra objCdCompra = new CD_Compra();

        public int ObtenerCorrelativo()
        {
            return objCdCompra.ObtenerCorrelativo();
        }

        public bool Registrar(Compra obj,DataTable detalle_compra, out string Mensaje)
        {
            return objCdCompra.Registrar(obj, detalle_compra, out Mensaje);
        }

        public Compra ObtenerCompra(string numero)
        {
            Compra objCompra = objCdCompra.ObtenerCompra(numero);

            if(objCompra.IdCompra != 0)
            {
                List<Detalle_Compra> oDetalleCompra = objCdCompra.ObtenerDetalleCompra(objCompra.IdCompra);
                objCompra.oDetalleCompra = oDetalleCompra;
            }

            return objCompra;
        }
    }
}
