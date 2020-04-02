using LinqToDB;
using Punto_de_ventas.Connection;
using Punto_de_ventas.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Punto_de_ventas.modelsclass
{
    public class Venta: Conexion
    {
        private Decimal importe = 0, totalPagar = 0m, ingresos, ingresosTotales;
        private string dia = DateTime.Now.ToString("dd");
        private string mes = DateTime.Now.ToString("MMM");
        private string año = DateTime.Now.ToString("yyy");
        private string fecha = DateTime.Now.ToString("dd/MMM/yyy");
        private bool verificar = false, suCambio = false;
        private int caja, idUsuario;

        public void start(int caja, int idUsuario)
        {
            this.caja = caja;
            this.idUsuario = idUsuario;
        }
        public List<Productos> buscarProductos(string codigo)
        {
            return Producto.Where(p => p.Codigo.Equals(codigo)).ToList();
        }

        public void guardarVentasTempo(string codigo, int funcion, int caja, int idUsuario)
        {
            string importe, precios;
            int cantidad = 1, existencia;
            Decimal precio, importes;

            var ventaTempo = TempoVentas.Where(t => t.Codigo.Equals(codigo) && t.Caja.Equals(caja) 
            && t.IdUsuario.Equals(idUsuario)).ToList();
            var product = Producto.Where(p => p.Codigo.Equals(codigo)).ToList();
            precio = Convert.ToDecimal(product[0].PrecioVenta);
            precios = String.Format("${0:#,###,###,##0.00####}", precio);
            if (ventaTempo.Count() > 0)
            {
                cantidad = ventaTempo[0].Cantidad;
                if(funcion == 0)
                {
                    cantidad += 1;
                }
                else
                {
                    cantidad--;
                }
                importes = precio * cantidad;
                importe = String.Format("${0:#,###,###,##0.00####}", importes);
                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo[0].IdTempo) && t.Caja.Equals(caja))
                                .Set(t => t.Precio, precios)
                                .Set(t => t.Cantidad, cantidad)
                                .Set(t => t.Importe, importe)
                                .Set(t => t.Caja, caja)
                                .Set(t => t.IdUsuario, idUsuario)
                                .Update();
            }
            else
            {
                TempoVentas.Value(t => t.Codigo, product[0].Codigo)
                            .Value(t => t.Descripcion, product[0].Descripcion)
                            .Value(t => t.Precio, precios)
                            .Value(t => t.Cantidad, 1)
                            .Value(t => t.Importe, precios)
                            .Value(t => t.Caja, caja)
                            .Value(t => t.IdUsuario, idUsuario)
                            .Value(t => t.Costo, Convert.ToString(product[0].Costo))
                            .Value(t => t.Departamento, product[0].Departamento)
                            .Value(t => t.Categoria, product[0].Categoria)
                            .Insert();
            }
            existencia = product[0].Existencia;
            if (existencia == 0)
            {

            }
            else
            { 
                existencia--;
                Producto.Where(p => p.IdProducto.Equals(product[0].IdProducto))
                .Set(t => t.Existencia, existencia)
                .Update();
            }
            var ventaTempo2 = TempoVentas.Where(t => t.Codigo.Equals(codigo) && t.Caja.Equals(caja)
                   && t.IdUsuario.Equals(idUsuario)).ToList();
            var buscar = Promos.Where(p => p.Codigo.Equals(ventaTempo2[0].Codigo)).ToList();
            if (buscar.Count != 0)
            {
                buscarPromocion(Convert.ToDecimal(ventaTempo2[0].Precio.Replace("$", "")), ventaTempo2[0].Cantidad, buscar[0].Precio, 
                    buscar[0].CantidadProductos);
                buscarPromocionDos(ventaTempo2[0].Codigo, Convert.ToDecimal(ventaTempo2[0].Precio.Replace("$", "")), ventaTempo2[0].Cantidad);
            }
                


            void buscarPromocion(decimal preventa, decimal cantventa, decimal prepromo, decimal cantpromo)
            {
                Decimal precioVenta, precioPromo, cantidadVenta, CantidadPromo;
                string precioPromo2;

                precioVenta = preventa;
                cantidadVenta = cantventa;
                precioPromo = prepromo;
                CantidadPromo = cantpromo;

                precioPromo2 = String.Format("${0:#,###,###,##0.00####}", precioPromo);


                    if (CantidadPromo > cantidadVenta)
                    {

                    }
                    else
                    {
                        if (CantidadPromo == cantidadVenta)
                        {
                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                .Set(t => t.Importe, precioPromo2)
                                .Update();
                          
                        }
                        else
                        {
                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 2))
                            {
                                Decimal cantidad3, precio3;
                                string precio4;
                                cantidad3 = cantidadVenta - CantidadPromo;
                                precio3 = precioPromo + (precioVenta * cantidad3);
                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                    .Set(t => t.Importe, precio4)
                                    .Update();
                            }
                            else
                            {
                                if (cantidadVenta == (CantidadPromo * 2))
                                {
                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 2));
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precioPromo3)
                                        .Update();
                                    
                                }
                                else
                                {
                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 3))
                                    {
                                        Decimal cantidad3, precio3;
                                        string precio4;
                                        cantidad3 = cantidadVenta - (CantidadPromo * 2);
                                        precio3 = (precioPromo * 2) + (precioVenta * cantidad3);
                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                            .Set(t => t.Importe, precio4)
                                            .Update();
                                    }
                                    else
                                    {
                                        if (cantidadVenta == (CantidadPromo * 3))
                                        {
                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 3));
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precioPromo3)
                                                .Update();
                                            
                                        }
                                        else
                                        {
                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 4))
                                            {
                                                Decimal cantidad3, precio3;
                                                string precio4;
                                                cantidad3 = cantidadVenta - (CantidadPromo * 3);
                                                precio3 = (precioPromo * 3) + (precioVenta * cantidad3);
                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                    .Set(t => t.Importe, precio4)
                                                    .Update();
                                            }
                                            else
                                            {
                                                if (cantidadVenta == (CantidadPromo * 4))
                                                {
                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 4));
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precioPromo3)
                                                        .Update();
                                                 
                                                    
                                                }
                                                else
                                                {
                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 5))
                                                    {
                                                        Decimal cantidad3, precio3;
                                                        string precio4;
                                                        cantidad3 = cantidadVenta - (CantidadPromo * 4);
                                                        precio3 = (precioPromo * 4) + (precioVenta * cantidad3);
                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                            .Set(t => t.Importe, precio4)
                                                            .Update();
                                                       
                                                    }
                                                    else
                                                    {
                                                        if (cantidadVenta == (CantidadPromo * 5))
                                                        {
                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 5));
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precioPromo3)
                                                                .Update();
                                                            
                                                        }
                                                        else
                                                        {
                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 6))
                                                            {
                                                                Decimal cantidad3, precio3;
                                                                string precio4;
                                                                cantidad3 = cantidadVenta - (CantidadPromo * 5);
                                                                precio3 = (precioPromo * 5) + (precioVenta * cantidad3);
                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                    .Set(t => t.Importe, precio4)
                                                                    .Update();
                                                            }
                                                            else
                                                            {
                                                                if (cantidadVenta == (CantidadPromo * 6))
                                                                {
                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 6));
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precioPromo3)
                                                                        .Update();
                                                                    
                                                                }
                                                                else
                                                                {
                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 7))
                                                                    {
                                                                        Decimal cantidad3, precio3;
                                                                        string precio4;
                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 6);
                                                                        precio3 = (precioPromo * 6) + (precioVenta * cantidad3);
                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                            .Set(t => t.Importe, precio4)
                                                                            .Update();
                                                                    }
                                                                    else
                                                                    {
                                                                        if (cantidadVenta == (CantidadPromo * 7))
                                                                        {
                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 7));
                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                .Update();
                                                                           
                                                                        }
                                                                        else
                                                                        {
                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 8))
                                                                            {
                                                                                Decimal cantidad3, precio3;
                                                                                string precio4;
                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 7);
                                                                                precio3 = (precioPromo * 7) + (precioVenta * cantidad3);
                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                    .Set(t => t.Importe, precio4)
                                                                                    .Update();
                                                                            }
                                                                            else
                                                                            {
                                                                                if (cantidadVenta == (CantidadPromo * 8))
                                                                                {
                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 8));
                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                        .Update();
                                                                                   
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 9))
                                                                                    {
                                                                                        Decimal cantidad3, precio3;
                                                                                        string precio4;
                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 8);
                                                                                        precio3 = (precioPromo * 8) + (precioVenta * cantidad3);
                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                            .Set(t => t.Importe, precio4)
                                                                                            .Update();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (cantidadVenta == (CantidadPromo * 9))
                                                                                        {
                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 9));
                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                .Update();
                                                                                            
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 10))
                                                                                            {
                                                                                                Decimal cantidad3, precio3;
                                                                                                string precio4;
                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 9);
                                                                                                precio3 = (precioPromo * 9) + (precioVenta * cantidad3);
                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                    .Update();
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (cantidadVenta == (CantidadPromo * 10))
                                                                                                {
                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 10));
                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                        .Update();
                                                                                                    
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 11))
                                                                                                    {
                                                                                                        Decimal cantidad3, precio3;
                                                                                                        string precio4;
                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 10);
                                                                                                        precio3 = (precioPromo * 10) + (precioVenta * cantidad3);
                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                            .Update();
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (cantidadVenta == (CantidadPromo * 11))
                                                                                                        {
                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 11));
                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                .Update();
                                                                                                            
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 12))
                                                                                                            {
                                                                                                                Decimal cantidad3, precio3;
                                                                                                                string precio4;
                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 11);
                                                                                                                precio3 = (precioPromo * 11) + (precioVenta * cantidad3);
                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                    .Update();
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (cantidadVenta == (CantidadPromo * 12))
                                                                                                                {
                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 12));
                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                        .Update();
                                                                                                                    
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 13))
                                                                                                                    {
                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                        string precio4;
                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 12);
                                                                                                                        precio3 = (precioPromo * 12) + (precioVenta * cantidad3);
                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                            .Update();
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (cantidadVenta == (CantidadPromo * 13))
                                                                                                                        {
                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 13));
                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                .Update();
                                                                                                                            
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 14))
                                                                                                                            {
                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                string precio4;
                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 13);
                                                                                                                                precio3 = (precioPromo * 13) + (precioVenta * cantidad3);
                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                    .Update();
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (cantidadVenta == (CantidadPromo * 14))
                                                                                                                                {
                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 14));
                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                        .Update();
                                                                                                                                    
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 15))
                                                                                                                                    {
                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                        string precio4;
                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 14);
                                                                                                                                        precio3 = (precioPromo * 14) + (precioVenta * cantidad3);
                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                            .Update();
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (cantidadVenta == (CantidadPromo * 15))
                                                                                                                                        {
                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 15));
                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                .Update();
                                                                                                                                          
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 16))
                                                                                                                                            {
                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                string precio4;
                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 15);
                                                                                                                                                precio3 = (precioPromo * 15) + (precioVenta * cantidad3);
                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                    .Update();
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (cantidadVenta == (CantidadPromo * 16))
                                                                                                                                                {
                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 16));
                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                        .Update();
                                                                                                                                                   
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 17))
                                                                                                                                                    {
                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                        string precio4;
                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 16);
                                                                                                                                                        precio3 = (precioPromo * 16) + (precioVenta * cantidad3);
                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                            .Update();
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 17))
                                                                                                                                                        {
                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 17));
                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                .Update();
                                                                                                                                                            
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 18))
                                                                                                                                                            {
                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                string precio4;
                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 17);
                                                                                                                                                                precio3 = (precioPromo * 17) + (precioVenta * cantidad3);
                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                    .Update();
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 18))
                                                                                                                                                                {
                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 18));
                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                        .Update();
                                                                                                                                                                    
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 19))
                                                                                                                                                                    {
                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                        string precio4;
                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 18);
                                                                                                                                                                        precio3 = (precioPromo * 18) + (precioVenta * cantidad3);
                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                            .Update();
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 19))
                                                                                                                                                                        {
                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 19));
                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                .Update();
                                                                                                                                                                            
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 20))
                                                                                                                                                                            {
                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                string precio4;
                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 19);
                                                                                                                                                                                precio3 = (precioPromo * 19) + (precioVenta * cantidad3);
                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                    .Update();
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 20))
                                                                                                                                                                                {
                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 20));
                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                        .Update();
                                                                                                                                                                                    
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 21))
                                                                                                                                                                                    {
                                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                                        string precio4;
                                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 20);
                                                                                                                                                                                        precio3 = (precioPromo * 20) + (precioVenta * cantidad3);
                                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                                            .Update();
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 21))
                                                                                                                                                                                        {
                                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 21));
                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                .Update();
                                                                                                                                                                                            
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 22))
                                                                                                                                                                                            {
                                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                                string precio4;
                                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 21);
                                                                                                                                                                                                precio3 = (precioPromo * 21) + (precioVenta * cantidad3);
                                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                                    .Update();
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 22))
                                                                                                                                                                                                {
                                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 22));
                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                    
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 23))
                                                                                                                                                                                                    {
                                                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                                                        string precio4;
                                                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 22);
                                                                                                                                                                                                        precio3 = (precioPromo * 22) + (precioVenta * cantidad3);
                                                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                                                            .Update();
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 23))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 23));
                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                            
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 24))
                                                                                                                                                                                                            {
                                                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                                                string precio4;
                                                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 23);
                                                                                                                                                                                                                precio3 = (precioPromo * 23) + (precioVenta * cantidad3);
                                                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                    .Update();
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 24))
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 24));
                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                    
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 25))
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                                                                        string precio4;
                                                                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 24);
                                                                                                                                                                                                                        precio3 = (precioPromo * 24) + (precioVenta * cantidad3);
                                                                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                            .Update();
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 25))
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 25));
                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                            
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 26))
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                                                                string precio4;
                                                                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 25);
                                                                                                                                                                                                                                precio3 = (precioPromo * 25) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                    .Update();
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 26))
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 26));
                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 27))
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                                                                                        string precio4;
                                                                                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 26);
                                                                                                                                                                                                                                        precio3 = (precioPromo * 26) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                            .Update();
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 27))
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 27));
                                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 28))
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                string precio4;
                                                                                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 27);
                                                                                                                                                                                                                                                precio3 = (precioPromo * 27) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                    .Update();
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 28))
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 28));
                                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 29))
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                        string precio4;
                                                                                                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 28);
                                                                                                                                                                                                                                                        precio3 = (precioPromo * 28) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                            .Update();
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 29))
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 29));
                                                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 30))
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                                string precio4;
                                                                                                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 29);
                                                                                                                                                                                                                                                                precio3 = (precioPromo * 29) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                                    .Update();
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 30))
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 30));
                                                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 31))
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                                        string precio4;
                                                                                                                                                                                                                                                                        cantidad3 = cantidadVenta - (CantidadPromo * 30);
                                                                                                                                                                                                                                                                        precio3 = (precioPromo * 30) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                                        precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                            .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                                            .Update();
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        if (cantidadVenta == (CantidadPromo * 31))
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 31));
                                                                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                                .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 32))
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                                                string precio4;
                                                                                                                                                                                                                                                                                cantidad3 = cantidadVenta - (CantidadPromo * 31);
                                                                                                                                                                                                                                                                                precio3 = (precioPromo * 31) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                                                precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                                    .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                                                    .Update();
                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                if (cantidadVenta == (CantidadPromo * 32))
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 32));
                                                                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                                        .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                }

                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                            }

                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }

                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                
            }

            void buscarPromocionDos(string codigos, decimal preventa, decimal cantventa)
            {
                var buscar2 = PromosDos.Where(p => p.Codigo.Equals(codigos)).ToList();

                if (buscar2.Count != 0)
                {
                    Decimal precioVenta, precioPromo, cantidadVenta, CantidadPromo;
                    string precioPromo2;
                    int limite;

                    precioVenta = preventa;
                    cantidadVenta = cantventa;
                    precioPromo = buscar2[0].Precio;
                    CantidadPromo = buscar2[0].CantidadProductos;
                    limite = buscar2[0].LimiteVenta;

                    precioPromo2 = String.Format("${0:#,###,###,##0.00####}", precioPromo);
                    if (limite > 0)
                    {
                        if (CantidadPromo == cantidadVenta)
                        {
                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                .Set(t => t.Importe, precioPromo2)
                                .Update();
                            limite--;
                            PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                     .Set(p => p.LimiteVenta, limite)
                                     .Update();
                        }
                        else
                        {
                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 2))
                            {
                                Decimal cantidad3, precio3;
                                string precio4;
                                cantidad3 = cantidadVenta - CantidadPromo;
                                var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                if (buscarenventa.Count != 0)
                                {
                                    buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                    var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                    precio3 = precioPromo + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precio4)
                                        .Update();
                                }
                                else
                                {
                                    precio3 = precioPromo + (precioVenta * cantidad3);
                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precio4)
                                        .Update();
                                }

                            }
                            else
                            {
                                if (cantidadVenta == (CantidadPromo * 2))
                                {
                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 2));
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precioPromo3)
                                        .Update();
                                    limite--;
                                    PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                     .Set(p => p.LimiteVenta, limite)
                                     .Update();
                                }
                                else
                                {
                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 3))
                                    {
                                        Decimal cantidad3, precio3;
                                        string precio4;
                                        cantidad3 = cantidadVenta - (CantidadPromo * 2);
                                        var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                        if (buscarenventa.Count != 0)
                                        {
                                            buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                            var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                            precio3 = (precioPromo * 2) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precio4)
                                                .Update();
                                        }
                                        else
                                        {
                                            precio3 = (precioPromo * 2) + (precioVenta * cantidad3);
                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precio4)
                                                .Update();
                                        }

                                    }
                                    else
                                    {
                                        if (cantidadVenta == (CantidadPromo * 3))
                                        {
                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 3));
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precioPromo3)
                                                .Update();
                                            limite--;
                                            PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                             .Set(p => p.LimiteVenta, limite)
                                             .Update();
                                        }
                                        else
                                        {
                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 4))
                                            {
                                                Decimal cantidad3, precio3;
                                                string precio4;
                                                cantidad3 = cantidadVenta - (CantidadPromo * 3);
                                                var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                                if (buscarenventa.Count != 0)
                                                {
                                                    buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                                    var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                                    precio3 = (precioPromo * 3) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precio4)
                                                        .Update();
                                                }
                                                else
                                                {
                                                    precio3 = (precioPromo * 3) + (precioVenta * cantidad3);
                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precio4)
                                                        .Update();
                                                }

                                            }
                                            else
                                            {
                                                if (cantidadVenta == (CantidadPromo * 4))
                                                {
                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 4));
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precioPromo3)
                                                        .Update();
                                                    limite--;
                                                    PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                                     .Set(p => p.LimiteVenta, limite)
                                                     .Update();
                                                }
                                                else
                                                {
                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 5))
                                                    {
                                                        Decimal cantidad3, precio3;
                                                        string precio4;
                                                        cantidad3 = cantidadVenta - (CantidadPromo * 4);
                                                        var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                                        if (buscarenventa.Count != 0)
                                                        {
                                                            buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                                            var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                                            precio3 = (precioPromo * 4) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precio4)
                                                                .Update();
                                                        }
                                                        else
                                                        {
                                                            precio3 = (precioPromo * 4) + (precioVenta * cantidad3);
                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precio4)
                                                                .Update();
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (cantidadVenta == (CantidadPromo * 5))
                                                        {
                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 5));
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precioPromo3)
                                                                .Update();
                                                            limite--;
                                                            PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                                             .Set(p => p.LimiteVenta, limite)
                                                             .Update();
                                                        }
                                                        else
                                                        {
                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 6))
                                                            {
                                                                Decimal cantidad3, precio3;
                                                                string precio4;
                                                                cantidad3 = cantidadVenta - (CantidadPromo * 5);
                                                                var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                                                if (buscarenventa.Count != 0)
                                                                {
                                                                    buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                                                    var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                                                    precio3 = (precioPromo * 5) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precio4)
                                                                        .Update();
                                                                }
                                                                else
                                                                {
                                                                    precio3 = (precioPromo * 5) + (precioVenta * cantidad3);
                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precio4)
                                                                        .Update();
                                                                }

                                                            }
                                                            else
                                                            {
                                                                if (cantidadVenta == (CantidadPromo * 6))
                                                                {
                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 6));
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precioPromo3)
                                                                        .Update();
                                                                    limite--;
                                                                    PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                                                     .Set(p => p.LimiteVenta, limite)
                                                                     .Update();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        public void guardarVentasTempoDos(string codigo, int cant, int caja, int idUsuario)
        {
            string importe, precios;
            int cantidad = 1, existencia, cant2;
            Decimal precio, importes;

            var ventaTempo = TempoVentas.Where(t => t.Codigo.Equals(codigo) && t.Caja.Equals(caja)
            && t.IdUsuario.Equals(idUsuario)).ToList();
            var product = Producto.Where(p => p.Codigo.Equals(codigo)).ToList();
            precio = Convert.ToDecimal(product[0].PrecioVenta);
            precios = String.Format("${0:#,###,###,##0.00####}", precio);
            if (ventaTempo.Count() > 0)
            {
                cantidad = ventaTempo[0].Cantidad;
                cant2 = cantidad + cant;
                importes = precio * cant2;
                importe = String.Format("${0:#,###,###,##0.00####}", importes);
                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo[0].IdTempo) && t.Caja.Equals(caja))
                                .Set(t => t.Precio, precios)
                                .Set(t => t.Cantidad, cant2)
                                .Set(t => t.Importe, importe)
                                .Set(t => t.Caja, caja)
                                .Set(t => t.IdUsuario, idUsuario)
                                .Update();
            }
            else
            {
                TempoVentas.Value(t => t.Codigo, product[0].Codigo)
                            .Value(t => t.Descripcion, product[0].Descripcion)
                            .Value(t => t.Precio, precios)
                            .Value(t => t.Cantidad, cant)
                            .Value(t => t.Importe, String.Format("${0:#,###,###,##0.00####}", precio * cant))
                            .Value(t => t.Caja, caja)
                            .Value(t => t.IdUsuario, idUsuario)
                            .Value(t => t.Costo, Convert.ToString(product[0].Costo))
                            .Value(t => t.Departamento, product[0].Departamento)
                            .Value(t => t.Categoria, product[0].Categoria)
                            .Insert();
            }
            existencia = product[0].Existencia;
            if (existencia == 0)
            {

            }
            else
            {
                Producto.Where(p => p.IdProducto.Equals(product[0].IdProducto))
                .Set(t => t.Existencia, existencia - cant)
                .Update();
            }
            var ventaTempo2 = TempoVentas.Where(t => t.Codigo.Equals(codigo) && t.Caja.Equals(caja)
                   && t.IdUsuario.Equals(idUsuario)).ToList();
            var buscar = Promos.Where(p => p.Codigo.Equals(ventaTempo2[0].Codigo)).ToList();
            if (buscar.Count != 0)
            {
                buscarPromocion(Convert.ToDecimal(ventaTempo2[0].Precio.Replace("$", "")), ventaTempo2[0].Cantidad, buscar[0].Precio,
                    buscar[0].CantidadProductos);
                buscarPromocionDos(ventaTempo2[0].Codigo, Convert.ToDecimal(ventaTempo2[0].Precio.Replace("$", "")), ventaTempo2[0].Cantidad);
            }



            void buscarPromocion(decimal preventa, decimal cantventa, decimal prepromo, decimal cantpromo)
            {
                Decimal precioVenta, precioPromo, cantidadVenta, CantidadPromo;
                string precioPromo2;

                precioVenta = preventa;
                cantidadVenta = cantventa;
                precioPromo = prepromo;
                CantidadPromo = cantpromo;

                precioPromo2 = String.Format("${0:#,###,###,##0.00####}", precioPromo);


                if (CantidadPromo > cantidadVenta)
                {

                }
                else
                {
                    if (CantidadPromo == cantidadVenta)
                    {
                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                            .Set(t => t.Importe, precioPromo2)
                            .Update();

                    }
                    else
                    {
                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 2))
                        {
                            Decimal cantidad3, precio3;
                            string precio4;
                            cantidad3 = cantidadVenta - CantidadPromo;
                            precio3 = precioPromo + (precioVenta * cantidad3);
                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                .Set(t => t.Importe, precio4)
                                .Update();
                        }
                        else
                        {
                            if (cantidadVenta == (CantidadPromo * 2))
                            {
                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 2));
                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                    .Set(t => t.Importe, precioPromo3)
                                    .Update();

                            }
                            else
                            {
                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 3))
                                {
                                    Decimal cantidad3, precio3;
                                    string precio4;
                                    cantidad3 = cantidadVenta - (CantidadPromo * 2);
                                    precio3 = (precioPromo * 2) + (precioVenta * cantidad3);
                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precio4)
                                        .Update();
                                }
                                else
                                {
                                    if (cantidadVenta == (CantidadPromo * 3))
                                    {
                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 3));
                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                            .Set(t => t.Importe, precioPromo3)
                                            .Update();

                                    }
                                    else
                                    {
                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 4))
                                        {
                                            Decimal cantidad3, precio3;
                                            string precio4;
                                            cantidad3 = cantidadVenta - (CantidadPromo * 3);
                                            precio3 = (precioPromo * 3) + (precioVenta * cantidad3);
                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precio4)
                                                .Update();
                                        }
                                        else
                                        {
                                            if (cantidadVenta == (CantidadPromo * 4))
                                            {
                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 4));
                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                    .Set(t => t.Importe, precioPromo3)
                                                    .Update();


                                            }
                                            else
                                            {
                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 5))
                                                {
                                                    Decimal cantidad3, precio3;
                                                    string precio4;
                                                    cantidad3 = cantidadVenta - (CantidadPromo * 4);
                                                    precio3 = (precioPromo * 4) + (precioVenta * cantidad3);
                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precio4)
                                                        .Update();

                                                }
                                                else
                                                {
                                                    if (cantidadVenta == (CantidadPromo * 5))
                                                    {
                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 5));
                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                            .Set(t => t.Importe, precioPromo3)
                                                            .Update();

                                                    }
                                                    else
                                                    {
                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 6))
                                                        {
                                                            Decimal cantidad3, precio3;
                                                            string precio4;
                                                            cantidad3 = cantidadVenta - (CantidadPromo * 5);
                                                            precio3 = (precioPromo * 5) + (precioVenta * cantidad3);
                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precio4)
                                                                .Update();
                                                        }
                                                        else
                                                        {
                                                            if (cantidadVenta == (CantidadPromo * 6))
                                                            {
                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 6));
                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                    .Set(t => t.Importe, precioPromo3)
                                                                    .Update();

                                                            }
                                                            else
                                                            {
                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 7))
                                                                {
                                                                    Decimal cantidad3, precio3;
                                                                    string precio4;
                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 6);
                                                                    precio3 = (precioPromo * 6) + (precioVenta * cantidad3);
                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precio4)
                                                                        .Update();
                                                                }
                                                                else
                                                                {
                                                                    if (cantidadVenta == (CantidadPromo * 7))
                                                                    {
                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 7));
                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                            .Set(t => t.Importe, precioPromo3)
                                                                            .Update();

                                                                    }
                                                                    else
                                                                    {
                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 8))
                                                                        {
                                                                            Decimal cantidad3, precio3;
                                                                            string precio4;
                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 7);
                                                                            precio3 = (precioPromo * 7) + (precioVenta * cantidad3);
                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                .Set(t => t.Importe, precio4)
                                                                                .Update();
                                                                        }
                                                                        else
                                                                        {
                                                                            if (cantidadVenta == (CantidadPromo * 8))
                                                                            {
                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 8));
                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                    .Update();

                                                                            }
                                                                            else
                                                                            {
                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 9))
                                                                                {
                                                                                    Decimal cantidad3, precio3;
                                                                                    string precio4;
                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 8);
                                                                                    precio3 = (precioPromo * 8) + (precioVenta * cantidad3);
                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                        .Set(t => t.Importe, precio4)
                                                                                        .Update();
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (cantidadVenta == (CantidadPromo * 9))
                                                                                    {
                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 9));
                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                            .Update();

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 10))
                                                                                        {
                                                                                            Decimal cantidad3, precio3;
                                                                                            string precio4;
                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 9);
                                                                                            precio3 = (precioPromo * 9) + (precioVenta * cantidad3);
                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                .Set(t => t.Importe, precio4)
                                                                                                .Update();
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (cantidadVenta == (CantidadPromo * 10))
                                                                                            {
                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 10));
                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                    .Update();

                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 11))
                                                                                                {
                                                                                                    Decimal cantidad3, precio3;
                                                                                                    string precio4;
                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 10);
                                                                                                    precio3 = (precioPromo * 10) + (precioVenta * cantidad3);
                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                        .Update();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (cantidadVenta == (CantidadPromo * 11))
                                                                                                    {
                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 11));
                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                            .Update();

                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 12))
                                                                                                        {
                                                                                                            Decimal cantidad3, precio3;
                                                                                                            string precio4;
                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 11);
                                                                                                            precio3 = (precioPromo * 11) + (precioVenta * cantidad3);
                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                .Update();
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (cantidadVenta == (CantidadPromo * 12))
                                                                                                            {
                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 12));
                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                    .Update();

                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 13))
                                                                                                                {
                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                    string precio4;
                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 12);
                                                                                                                    precio3 = (precioPromo * 12) + (precioVenta * cantidad3);
                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                        .Update();
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (cantidadVenta == (CantidadPromo * 13))
                                                                                                                    {
                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 13));
                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                            .Update();

                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 14))
                                                                                                                        {
                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                            string precio4;
                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 13);
                                                                                                                            precio3 = (precioPromo * 13) + (precioVenta * cantidad3);
                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                .Update();
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (cantidadVenta == (CantidadPromo * 14))
                                                                                                                            {
                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 14));
                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                    .Update();

                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 15))
                                                                                                                                {
                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                    string precio4;
                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 14);
                                                                                                                                    precio3 = (precioPromo * 14) + (precioVenta * cantidad3);
                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                        .Update();
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (cantidadVenta == (CantidadPromo * 15))
                                                                                                                                    {
                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 15));
                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                            .Update();

                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 16))
                                                                                                                                        {
                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                            string precio4;
                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 15);
                                                                                                                                            precio3 = (precioPromo * 15) + (precioVenta * cantidad3);
                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                .Update();
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (cantidadVenta == (CantidadPromo * 16))
                                                                                                                                            {
                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 16));
                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                    .Update();

                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 17))
                                                                                                                                                {
                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                    string precio4;
                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 16);
                                                                                                                                                    precio3 = (precioPromo * 16) + (precioVenta * cantidad3);
                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                        .Update();
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 17))
                                                                                                                                                    {
                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 17));
                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                            .Update();

                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 18))
                                                                                                                                                        {
                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                            string precio4;
                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 17);
                                                                                                                                                            precio3 = (precioPromo * 17) + (precioVenta * cantidad3);
                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                .Update();
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 18))
                                                                                                                                                            {
                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 18));
                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                    .Update();

                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 19))
                                                                                                                                                                {
                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                    string precio4;
                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 18);
                                                                                                                                                                    precio3 = (precioPromo * 18) + (precioVenta * cantidad3);
                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                        .Update();
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 19))
                                                                                                                                                                    {
                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 19));
                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                            .Update();

                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 20))
                                                                                                                                                                        {
                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                            string precio4;
                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 19);
                                                                                                                                                                            precio3 = (precioPromo * 19) + (precioVenta * cantidad3);
                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                .Update();
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 20))
                                                                                                                                                                            {
                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 20));
                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                    .Update();

                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 21))
                                                                                                                                                                                {
                                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                                    string precio4;
                                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 20);
                                                                                                                                                                                    precio3 = (precioPromo * 20) + (precioVenta * cantidad3);
                                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                                        .Update();
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 21))
                                                                                                                                                                                    {
                                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 21));
                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                            .Update();

                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 22))
                                                                                                                                                                                        {
                                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                                            string precio4;
                                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 21);
                                                                                                                                                                                            precio3 = (precioPromo * 21) + (precioVenta * cantidad3);
                                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                                .Update();
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 22))
                                                                                                                                                                                            {
                                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 22));
                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                    .Update();

                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 23))
                                                                                                                                                                                                {
                                                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                                                    string precio4;
                                                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 22);
                                                                                                                                                                                                    precio3 = (precioPromo * 22) + (precioVenta * cantidad3);
                                                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 23))
                                                                                                                                                                                                    {
                                                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 23));
                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                            .Update();

                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 24))
                                                                                                                                                                                                        {
                                                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                                                            string precio4;
                                                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 23);
                                                                                                                                                                                                            precio3 = (precioPromo * 23) + (precioVenta * cantidad3);
                                                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 24))
                                                                                                                                                                                                            {
                                                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 24));
                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                    .Update();

                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 25))
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                                                                    string precio4;
                                                                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 24);
                                                                                                                                                                                                                    precio3 = (precioPromo * 24) + (precioVenta * cantidad3);
                                                                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 25))
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 25));
                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                            .Update();

                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 26))
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                                                                            string precio4;
                                                                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 25);
                                                                                                                                                                                                                            precio3 = (precioPromo * 25) + (precioVenta * cantidad3);
                                                                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 26))
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 26));
                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                    .Update();

                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 27))
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                                                                                    string precio4;
                                                                                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 26);
                                                                                                                                                                                                                                    precio3 = (precioPromo * 26) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 27))
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 27));
                                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                            .Update();

                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 28))
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                                                                                            string precio4;
                                                                                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 27);
                                                                                                                                                                                                                                            precio3 = (precioPromo * 27) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 28))
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 28));
                                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                    .Update();


                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 29))
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                    string precio4;
                                                                                                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 28);
                                                                                                                                                                                                                                                    precio3 = (precioPromo * 28) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 29))
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 29));
                                                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                            .Update();


                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 30))
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                            string precio4;
                                                                                                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 29);
                                                                                                                                                                                                                                                            precio3 = (precioPromo * 29) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 30))
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 30));
                                                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                    .Update();

                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 31))
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                                    string precio4;
                                                                                                                                                                                                                                                                    cantidad3 = cantidadVenta - (CantidadPromo * 30);
                                                                                                                                                                                                                                                                    precio3 = (precioPromo * 30) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                        .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                                        .Update();
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    if (cantidadVenta == (CantidadPromo * 31))
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 31));
                                                                                                                                                                                                                                                                        TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                            .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                            .Update();

                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 32))
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            Decimal cantidad3, precio3;
                                                                                                                                                                                                                                                                            string precio4;
                                                                                                                                                                                                                                                                            cantidad3 = cantidadVenta - (CantidadPromo * 31);
                                                                                                                                                                                                                                                                            precio3 = (precioPromo * 31) + (precioVenta * cantidad3);
                                                                                                                                                                                                                                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                                                                                                                                                                                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                                .Set(t => t.Importe, precio4)
                                                                                                                                                                                                                                                                                .Update();
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            if (cantidadVenta == (CantidadPromo * 32))
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 32));
                                                                                                                                                                                                                                                                                TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                                                                                                                                                                                                                                    .Set(t => t.Importe, precioPromo3)
                                                                                                                                                                                                                                                                                    .Update();

                                                                                                                                                                                                                                                                            }

                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                        }

                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }

                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            void buscarPromocionDos(string codigos, decimal preventa, decimal cantventa)
            {
                var buscar2 = PromosDos.Where(p => p.Codigo.Equals(codigos)).ToList();

                if (buscar2.Count != 0)
                {
                    Decimal precioVenta, precioPromo, cantidadVenta, CantidadPromo;
                    string precioPromo2;
                    int limite;

                    precioVenta = preventa;
                    cantidadVenta = cantventa;
                    precioPromo = buscar2[0].Precio;
                    CantidadPromo = buscar2[0].CantidadProductos;
                    limite = buscar2[0].LimiteVenta;

                    precioPromo2 = String.Format("${0:#,###,###,##0.00####}", precioPromo);
                    if (limite > 0)
                    {
                        if (CantidadPromo == cantidadVenta)
                        {
                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                .Set(t => t.Importe, precioPromo2)
                                .Update();
                            limite--;
                            PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                     .Set(p => p.LimiteVenta, limite)
                                     .Update();
                        }
                        else
                        {
                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 2))
                            {
                                Decimal cantidad3, precio3;
                                string precio4;
                                cantidad3 = cantidadVenta - CantidadPromo;
                                var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                if (buscarenventa.Count != 0)
                                {
                                    buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                    var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                    precio3 = precioPromo + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precio4)
                                        .Update();
                                }
                                else
                                {
                                    precio3 = precioPromo + (precioVenta * cantidad3);
                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precio4)
                                        .Update();
                                }

                            }
                            else
                            {
                                if (cantidadVenta == (CantidadPromo * 2))
                                {
                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 2));
                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                        .Set(t => t.Importe, precioPromo3)
                                        .Update();
                                    limite--;
                                    PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                     .Set(p => p.LimiteVenta, limite)
                                     .Update();
                                }
                                else
                                {
                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 3))
                                    {
                                        Decimal cantidad3, precio3;
                                        string precio4;
                                        cantidad3 = cantidadVenta - (CantidadPromo * 2);
                                        var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                        if (buscarenventa.Count != 0)
                                        {
                                            buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                            var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                            precio3 = (precioPromo * 2) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precio4)
                                                .Update();
                                        }
                                        else
                                        {
                                            precio3 = (precioPromo * 2) + (precioVenta * cantidad3);
                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precio4)
                                                .Update();
                                        }

                                    }
                                    else
                                    {
                                        if (cantidadVenta == (CantidadPromo * 3))
                                        {
                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 3));
                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                .Set(t => t.Importe, precioPromo3)
                                                .Update();
                                            limite--;
                                            PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                             .Set(p => p.LimiteVenta, limite)
                                             .Update();
                                        }
                                        else
                                        {
                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 4))
                                            {
                                                Decimal cantidad3, precio3;
                                                string precio4;
                                                cantidad3 = cantidadVenta - (CantidadPromo * 3);
                                                var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                                if (buscarenventa.Count != 0)
                                                {
                                                    buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                                    var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                                    precio3 = (precioPromo * 3) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precio4)
                                                        .Update();
                                                }
                                                else
                                                {
                                                    precio3 = (precioPromo * 3) + (precioVenta * cantidad3);
                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precio4)
                                                        .Update();
                                                }

                                            }
                                            else
                                            {
                                                if (cantidadVenta == (CantidadPromo * 4))
                                                {
                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 4));
                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                        .Set(t => t.Importe, precioPromo3)
                                                        .Update();
                                                    limite--;
                                                    PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                                     .Set(p => p.LimiteVenta, limite)
                                                     .Update();
                                                }
                                                else
                                                {
                                                    if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 5))
                                                    {
                                                        Decimal cantidad3, precio3;
                                                        string precio4;
                                                        cantidad3 = cantidadVenta - (CantidadPromo * 4);
                                                        var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                                        if (buscarenventa.Count != 0)
                                                        {
                                                            buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                                            var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                                            precio3 = (precioPromo * 4) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precio4)
                                                                .Update();
                                                        }
                                                        else
                                                        {
                                                            precio3 = (precioPromo * 4) + (precioVenta * cantidad3);
                                                            precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precio4)
                                                                .Update();
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (cantidadVenta == (CantidadPromo * 5))
                                                        {
                                                            string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 5));
                                                            TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                .Set(t => t.Importe, precioPromo3)
                                                                .Update();
                                                            limite--;
                                                            PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                                             .Set(p => p.LimiteVenta, limite)
                                                             .Update();
                                                        }
                                                        else
                                                        {
                                                            if (CantidadPromo < cantidadVenta && cantidadVenta < (CantidadPromo * 6))
                                                            {
                                                                Decimal cantidad3, precio3;
                                                                string precio4;
                                                                cantidad3 = cantidadVenta - (CantidadPromo * 5);
                                                                var buscarenventa = Promos.Where(v => v.Codigo.Equals(codigos) && v.CantidadProductos.Equals(cantidad3)).ToList();
                                                                if (buscarenventa.Count != 0)
                                                                {
                                                                    buscarPromocion(precioVenta, cantidad3, buscarenventa[0].Precio, buscarenventa[0].CantidadProductos);
                                                                    var buscarenventados = TempoVentas.Where(t => t.Codigo.Equals(codigos)).ToList();
                                                                    precio3 = (precioPromo * 5) + (Convert.ToDecimal(buscarenventados[0].Importe.Replace("$", "")));
                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precio4)
                                                                        .Update();
                                                                }
                                                                else
                                                                {
                                                                    precio3 = (precioPromo * 5) + (precioVenta * cantidad3);
                                                                    precio4 = String.Format("${0:#,###,###,##0.00####}", precio3);
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precio4)
                                                                        .Update();
                                                                }

                                                            }
                                                            else
                                                            {
                                                                if (cantidadVenta == (CantidadPromo * 6))
                                                                {
                                                                    string precioPromo3 = String.Format("${0:#,###,###,##0.00####}", (precioPromo * 6));
                                                                    TempoVentas.Where(t => t.IdTempo.Equals(ventaTempo2[0].IdTempo) && t.Caja.Equals(caja))
                                                                        .Set(t => t.Importe, precioPromo3)
                                                                        .Update();
                                                                    limite--;
                                                                    PromosDos.Where(p => p.Codigo.Equals(buscar2[0].Codigo))
                                                                     .Set(p => p.LimiteVenta, limite)
                                                                     .Update();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        public void buscarVentaTempo(DataGridView dataGridView, int num_pag, int reg_por_pag)
        {
            var query = TempoVentas.Where(t => t.Caja.Equals(caja) && t.IdUsuario.Equals(idUsuario)).ToList();
            int inicio = (num_pag - 1) * reg_por_pag;
            dataGridView.DataSource = query.Skip(inicio).Take(reg_por_pag).ToList();
            dataGridView.Columns[0].Visible = false;
            dataGridView.Columns[6].Visible = false;
            dataGridView.Columns[7].Visible = false;
            dataGridView.Columns[8].Visible = false;
            dataGridView.Columns[9].Visible = false;
            dataGridView.Columns[10].Visible = false;
            dataGridView.Columns[3].DefaultCellStyle.ForeColor = Color.Green;
            dataGridView.Columns[5].DefaultCellStyle.ForeColor = Color.Green;
        }

        internal void importes (Label label, int caja, int idUsuario)
        {
            importe = 0;
            var ventaTempo = TempoVentas.Where(t => t.Caja.Equals(caja) && t.IdUsuario.Equals(idUsuario)).ToList();
            if (ventaTempo.Count > 0)
            {
                ventaTempo.ForEach(item => {
                    importe += Convert.ToDecimal(item.Importe.Replace("$", ""));
                });
                label.Text = String.Format("${0:#,###,###,##0.00####}", importe);
            }
            else
            {
                label.Text = "$0.00";
            }
        }

        public void borrarVenta(string codigo, int cant, int caja, int idUsuario)
        {
            int cantidad = 0, existencia = 0;
            var ventatemp = TempoVentas.Where(t => t.Codigo.Equals(codigo) && t.Caja.Equals(caja)
            && t.IdUsuario.Equals(idUsuario)).ToList();
            cantidad = ventatemp[0].Cantidad;
            var producto = Producto.Where(p => p.Codigo.Equals(codigo)).ToList();
            existencia = producto[0].Existencia;

            if(cant == 1)
            {
                existencia += cantidad;
                TempoVentas.Where(t => t.IdTempo == ventatemp[0].IdTempo && t.Caja.Equals(caja)
                && t.IdUsuario.Equals(idUsuario)).Delete();
            }
            else
            {
                existencia++;
                guardarVentasTempo(codigo, 1, caja, idUsuario);
            }
            Producto.Where(p => p.IdProducto.Equals(producto[0].IdProducto))
                        .Set(t => t.Existencia, existencia)
                        .Update();
        }

        public void agregarVenta(string codigo, int cant, int caja, int idusuario)
        {
            int cantidad = 0, existencia = 0;
            var ventatemp = TempoVentas.Where(t => t.Codigo.Equals(codigo) && t.Caja.Equals(caja)
            && t.IdUsuario.Equals(idusuario)).ToList();
            cantidad = ventatemp[0].Cantidad;
            var producto = Producto.Where(p => p.Codigo.Equals(codigo)).ToList();
            existencia = producto[0].Existencia;

            if (existencia <= 0)
            {

            }
            else
            {
                guardarVentasTempo(codigo, 0, caja, idusuario);
            }
        }

        public void pagosCliente(TextBox textBox, Label label1, Label label2, Label label3, CheckBox checkBox)
        {
            Decimal pago, pagar;
            if (textBox.Text == "")
            {
                label1.Text = "Su cambio";
                label2.Text = "$0.00";
            }
            else
            {
                pagar = importe;
                pago = Convert.ToDecimal(textBox.Text);
                if (pago >= pagar)
                {
                    totalPagar = pago - pagar; 
                    if (totalPagar > ingresosTotales)
                    {
                        label1.Text = "No hay ingresos en caja";
                        label1.ForeColor = Color.Red;
                        verificar = false;
                        suCambio = false;
                    }
                    else
                    {
                        if (checkBox.Checked == true)
                        {
                            label1.Text = "Desmarque la opcion credito";
                            label1.ForeColor = Color.Red;
                            verificar = false;
                            suCambio = false;
                        }
                        else
                        {
                            label1.Text = "Su cambio";
                            label1.ForeColor = Color.Green;
                            totalPagar = pago - pagar;
                            verificar = true;
                            suCambio = true;
                        }
                    }
                }
                if (pago < pagar)
                {
                    label1.Text = "Pago insuficiente";
                    label1.ForeColor = Color.Red;
                    totalPagar = pagar - pago;
                    suCambio = false;
                    if (checkBox.Checked == true)
                    {
                        verificar = true;
                    }
                    else
                    {
                        verificar = false;
                    }
                }
                label2.Text = String.Format("${0:#,###,###,##0.00####}", totalPagar);
            }
            label3.Text = "Pago con";
            label3.ForeColor = Color.Teal;
        }

        public void buscarCliente(DataGridView dataGridView, string campo)
        {
            if (campo == "")
            {
                var query = Cliente.Join(ReportesClientes, 
                    c => c.IdCliente,
                    t => 0,
                    (c,t) => new {
                        c.IdCliente,
                        c.ID,   
                        c.Nombre,
                        c.Apellido,
                        c.LimiteCredito,
                        t.SaldoActual,
                        t.FechaActual,
                        t.UltimoPago,
                        t.FechaPago
                    }).Where(c => c.ID.StartsWith(campo)); ;
                dataGridView.DataSource = query.ToList();
            }
            else
            {
                var query = Cliente.Join(ReportesClientes,
                    c => c.IdCliente,
                    t => t.IdCliente,
                    (c, t) => new {
                        c.IdCliente,
                        c.ID,
                        c.Nombre,
                        c.Apellido,
                        c.LimiteCredito,
                        t.SaldoActual,
                        t.FechaActual,
                        t.UltimoPago,
                        t.FechaPago
                    }).Where(c => c.ID.StartsWith(campo) || c.Nombre.StartsWith(campo));
                dataGridView.DataSource = query.ToList();
            }
            dataGridView.Columns[0].Visible = false;
        }

        public void datosCliente(CheckBox checkBox, TextBox textBox_pagos, TextBox textBox_Buscar,
            DataGridView dataGridView, List<Label> labels)
        {
            string deuda1, nombre, apelldio;
            Decimal deuda2, deudaTotal;
            if (checkBox.Checked == true)
            {
                if (textBox_pagos.Text == "")
                {
                    if (checkBox.Checked == false)
                    {
                        labels[0].Text = "$0.00";
                        labels[1].Text = "$0.00";
                        labels[2].Text = labels[0].Text;
                        labels[3].Text = "";
                        labels[4].Text = "$0.00";
                        labels[5].Text = "$0.00";
                        labels[6].Text = "--/--/--";
                    }
                        
                }
                else
                {
                    if (verificar)
                    {
                        if (textBox_Buscar.Text != "")
                        {
                            deuda1 = Convert.ToString(dataGridView.CurrentRow.Cells[5].Value);
                            deuda2 = Convert.ToDecimal(deuda1.Replace("$", ""));
                            deudaTotal = deuda2 + totalPagar;
                            labels[0].Text = string.Format("${0:#,###,###,##0.00####}", deudaTotal);
                            nombre = Convert.ToString(dataGridView.CurrentRow.Cells[2].Value);
                            apelldio = Convert.ToString(dataGridView.CurrentRow.Cells[3].Value);

                            labels[3].Text = nombre + "" + apelldio;
                            labels[1].Text = string.Format("${0:#,###,###,##0.00####}", totalPagar);
                            labels[4].Text = deuda1;
                            labels[2].Text = string.Format("${0:#,###,###,##0.00####}", deudaTotal);
                            labels[5].Text = Convert.ToString(dataGridView.CurrentRow.Cells[6].Value);
                            labels[6].Text = fecha;
                        }
                    }   
                }
            }
            else
            {
                labels[0].Text = "$0.00";
                labels[1].Text = "$0.00";
                labels[2].Text = labels[0].Text;
                labels[3].Text = "";
                labels[4].Text = "$0.00";
                labels[5].Text = "$0.00";
                labels[6].Text = "--/--/--";
                textBox_Buscar.Text = "";
            }
        }

        public bool cobrar(CheckBox checkBox, TextBox textBox_pagos, DataGridView dataGridView, List<Label> labels,
            int caja, int idUsuario)
        {
            var ultimoTicket = Ventas.OrderByDescending(v => v.NumeroTicket).ToList();
            int ultimo = ultimoTicket[0].NumeroTicket;
            bool valor = false;
            if (textBox_pagos.Text == "")
            {
                labels[7].Text = "Ingrese el pago";
                labels[7].ForeColor = Color.Red;
                textBox_pagos.Focus();
            }
            else
            {
                if (verificar) { 
                String saldoActual, IDCliente= null;
                Decimal deuda= 0, deudaActual = 0m, pagos, ingresosInicial;
                int idCliente= 0, limite;
                pagos = Convert.ToDecimal(textBox_pagos.Text);
                if (checkBox.Checked == true)
                {
                    if (dataGridView.CurrentRow != null)
                    {
                        idCliente = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);
                        IDCliente = Convert.ToString(dataGridView.CurrentRow.Cells[1].Value);
                        saldoActual = Convert.ToString(dataGridView.CurrentRow.Cells[5].Value);
                        deudaActual = Convert.ToDecimal(saldoActual.Replace("$",""));
                        deuda = totalPagar + deudaActual;
                        limite = Convert.ToInt16(dataGridView.CurrentRow.Cells[4].Value);
                        if (limite == 0)
                            {
                                insertVentas();
                            }
                        else
                            {
                                if (limite >= deuda)
                                {
                                    insertVentas();
                                }
                                else
                                {
                                    labels[8].Text = "Sobrepasa el limite de credito";
                                    valor = false;
                                }
                            }
                        
                        
                        
                    }
                    else
                    {
                            if (verificar)
                            {
                                labels[8].Text = "Seleccione un cliente";
                                valor = false;
                            }
                    }
                }
                else
                {
                    if (verificar)
                    {
                        if (pagos >= importe)
                        {
                            insertVentas();
                        }
                    }
                    else
                    {

                    }
                }
                void insertVentas()
                    {
                        var ventaTempo = TempoVentas.Where(t => t.Caja.Equals(caja)
                                && t.IdUsuario.Equals(idUsuario)).ToList();
                        ultimo++;
                        if (ventaTempo.Count > 0)
                        {
                            
                            if (checkBox.Checked == true)
                            {
                                ventaTempo.ForEach(item =>
                                {
                                    Ventasclientes.Value(v => v.Codigo, item.Codigo)
                                            .Value(v => v.Descripcion, item.Descripcion)
                                            .Value(v => v.Precio, item.Precio)
                                            .Value(v => v.Cantidad, item.Cantidad)
                                            .Value(v => v.Importe, item.Importe)
                                            .Value(v => v.Fecha, fecha)
                                            .Value(v => v.Caja, caja)
                                            .Value(v => v.IdUsuario, idUsuario)
                                            .Value(v => v.IdCliente, idCliente)
                                            .Value(v => v.Costo, item.Costo)
                                            .Insert();
                                });

                                var reporte = ReportesClientes.Where(r => r.IdCliente.Equals(idCliente)).ToList();

                                ReportesClientes.Where(r => r.IdRegistro.Equals(reporte[0].IdRegistro))
                                .Set(r => r.SaldoActual, string.Format("${0:#,###,###,##0.00####}", deuda))
                                .Set(r => r.FechaActual, fecha)
                                .Update();

                                CreditosVentas.Value(b => b.Total, string.Format("${0:#,###,###,##0.00####}", importe))
                                          .Value(b => b.Pago, string.Format("${0:#,###,###,##0.00####}", pagos))
                                          .Value(b => b.Credito, string.Format("${0:#,###,###,##0.00####}", totalPagar))
                                          .Value(b => b.Dia, Convert.ToInt16(dia))
                                          .Value(b => b.Mes, mes)
                                          .Value(b => b.Año, año)
                                          .Value(b => b.Fecha, fecha)
                                          .Value(b => b.Cliente, IDCliente)
                                          .Value(b => b.Caja, caja)
                                          .Value(b => b.IdUsuario, idUsuario)
                                          .Insert();
                            }
                            else
                            {
                                ventaTempo.ForEach(item =>
                                {
                                    Ventas.Value(v => v.Codigo, item.Codigo)
                                            .Value(v => v.Descripcion, item.Descripcion)
                                            .Value(v => v.Precio, item.Precio)
                                            .Value(v => v.Cantidad, item.Cantidad)
                                            .Value(v => v.Importe, item.Importe)
                                            .Value(v => v.Dia, Convert.ToInt16(dia))
                                            .Value(v => v.Mes, mes)
                                            .Value(v => v.Año, año)
                                            .Value(v => v.Fecha, fecha)
                                            .Value(v => v.Caja, caja)
                                            .Value(v => v.IdUsuario, idUsuario)
                                            .Value(v => v.NumeroTicket, ultimo)
                                            .Value(v => v.Costo, item.Costo)
                                            .Value(v => v.Departamento, item.Departamento)
                                            .Value(v => v.Categoria, item.Categoria)
                                            .Insert();
                                });
                            }

                            var cajaIngreso = CajasIngresos.Where(t => t.Caja.Equals(caja) && t.IdUsuario.Equals(idUsuario)
                                    && t.Type.Equals("Ventas") && t.Fecha.Equals(fecha)).ToList();
                            if (cajaIngreso.Count > 0)
                            {
                                ingresos = pagos + Convert.ToDecimal(cajaIngreso[0].Ingreso.Replace("$", ""));
                                CajasIngresos.Where(c => c.Id.Equals(cajaIngreso[0].Id) && c.Caja.Equals(caja)
                                && c.IdUsuario.Equals(idUsuario) && c.Type.Equals("Ventas") && c.Fecha.Equals(fecha))
                                    .Set(c => c.Ingreso, string.Format("${0:#,###,###,##0.00####}", ingresos))
                                    .Update();
                            }
                            else
                            {
                                CajasIngresos.Value(v => v.Caja, caja)
                                        .Value(v => v.Ingreso, string.Format("${0:#,###,###,##0.00####}", pagos))
                                        .Value(v => v.Type, "Ventas")
                                        .Value(v => v.Dia, Convert.ToInt16(dia))
                                        .Value(v => v.Mes, mes)
                                        .Value(v => v.Año, año)
                                        .Value(v => v.IdUsuario, idUsuario)
                                        .Value(v => v.Fecha, fecha)
                                        .Insert();
                            }
                            valor = true;
                        }
                        if (suCambio)
                        {
                            ingresosInicial = 0;
                            var cajaIngresoInicial = CajasIngresos.Where(t => t.Caja.Equals(caja)
                            && t.IdUsuario.Equals(idUsuario)
                            && t.Type.Equals("Inicial") && t.Fecha.Equals(fecha)).ToList();
                            if (cajaIngresoInicial.Count > 0)
                            {
                                cajaIngresoInicial.ForEach(item => {

                                    ingresosInicial += Convert.ToDecimal(item.Ingreso.Replace("$", ""));
                                });
                                if (ingresosInicial > 0)
                                {
                                    if (ingresosInicial > totalPagar || ingresosInicial == totalPagar)
                                    {
                                        ingresosInicial -= totalPagar;
                                        CajasIngresos.Where(t => t.Caja.Equals(caja) 
                                        && t.Id.Equals(cajaIngresoInicial[0].Id)
                                        && t.IdUsuario.Equals(idUsuario)
                                        && t.Type.Equals("Inicial") 
                                        && t.Fecha.Equals(fecha))
                                        .Set(t => t.Ingreso, string.Format("${0:#,###,###,##0.00####}", ingresosInicial))
                                        .Update();
                                        valor = true;
                                    }
                                    else
                                    {
                                        ingresosVentas();
                                    }
                                }
                                else
                                {
                                    ingresosVentas();
                                }
                            }
                            else
                            {
                                ingresosVentas();
                            }
                            void ingresosVentas()
                            {
                                Decimal ingresosVenta = 0;
                                ingresosInicial = 0;
                                var cajaIngresoVentas = CajasIngresos.Where(c => c.Caja.Equals(caja) && c.IdUsuario.Equals(idUsuario)
                                && c.Type.Equals("Ventas") && c.Fecha.Equals(fecha)).ToList();
                                if (cajaIngresoVentas.Count > 0)
                                {
                                    var data = cajaIngresoVentas[0].Ingreso;
                                    ingresosVenta = Convert.ToDecimal(data.Replace("$", ""));
                                    if (totalPagar < ingresosVenta || ingresosVenta == totalPagar)
                                    {
                                        if (0 < cajaIngresoInicial.Count)
                                        {
                                            var ingresoIni = cajaIngresoInicial[0].Ingreso;
                                            ingresosInicial = Convert.ToDecimal(ingresoIni.Replace("$", ""));
                                            totalPagar -= ingresosInicial;

                                            CajasIngresos.Where(r => r.Id.Equals(cajaIngresoInicial[0].Id) 
                                            && r.Caja.Equals(caja) && r.IdUsuario.Equals(idUsuario) 
                                            && r.Type.Equals("Inicial") && r.Fecha.Equals(fecha))
                                            .Set(r => r.Ingreso, "$0.00")
                                            .Update();
                                        }
                                        ingresosVenta -= totalPagar;
                                        CajasIngresos.Where(t => t.Caja.Equals(caja)
                                            && t.Id.Equals(cajaIngresoVentas[0].Id)
                                            && t.IdUsuario.Equals(idUsuario)
                                            && t.Type.Equals("Ventas")
                                            && t.Fecha.Equals(fecha))
                                            .Set(t => t.Ingreso, string.Format("${0:#,###,###,##0.00####}", ingresosVenta))
                                            .Update();
                                        valor = true;
                                    }
                                    else
                                    {
                                        if (totalPagar < ingresosTotales || ingresosTotales == totalPagar)
                                        {
                                            if (0 < cajaIngresoInicial.Count)
                                            {
                                                var ingresoIni = cajaIngresoInicial[0].Ingreso;
                                                ingresosInicial = Convert.ToDecimal(ingresoIni.Replace("$", ""));
                                                totalPagar -= ingresosInicial;

                                                CajasIngresos.Where(t => t.Caja.Equals(caja)
                                                && t.Id.Equals(cajaIngresoInicial[0].Id)
                                                && t.IdUsuario.Equals(idUsuario)
                                                && t.Type.Equals("Inicial")
                                                && t.Fecha.Equals(fecha))
                                                .Set(t => t.Ingreso, "$0.00")
                                                .Update();
                                            }

                                                ingresosVenta -= totalPagar;
                                            CajasIngresos.Where(t => t.Caja.Equals(caja)
                                                && t.Id.Equals(cajaIngresoVentas[0].Id)
                                                && t.IdUsuario.Equals(idUsuario)
                                                && t.Type.Equals("Ventas")
                                                && t.Fecha.Equals(fecha))
                                                .Set(t => t.Ingreso, string.Format("${0:#,###,###,##0.00####}", ingresosVenta))
                                                .Update();
                                            valor = true;
                                        }
                                        else
                                        {
                                            labels[9].Text = "No hay ingresos";
                                            labels[9].ForeColor = Color.Red;
                                            valor = false;
                                        }
                                    }
                                }
                                else
                                {
                                    labels[9].Text = "No hay ingresos";
                                    labels[9].ForeColor = Color.Red;
                                    valor = false;
                                }
                            }

                        }
                    }
                }
            }
            return valor;
        }
        // termina el metodo cobrar

        public void ingresosCajas(Label label1, Label label2, Label label3, int caja, int idUsuario)
        {
            ingresosTotales = 0;
            var cajasIngresosInicial = CajasIngresos.Where(c => c.Caja.Equals(caja) && c.IdUsuario.Equals(idUsuario)
                    && c.Type.Equals("Inicial") && c.Fecha.Equals(fecha)).ToList();
            if (cajasIngresosInicial.Count > 0)
            {
                //var data = cajasIngresosInicial[0].Ingreso;
                //label1.Text = data;
                //ingresosTotales = Convert.ToDecimal(data.Replace("$",""));
                cajasIngresosInicial.ForEach(item => {
                    ingresosTotales += Convert.ToDecimal(item.Ingreso.Replace("$", ""));
                });

                label1.Text = "$" + ingresosTotales.ToString();
                label1.ForeColor = Color.LightSlateGray;
            }
            else
            {
                label1.Text = "$0.00";
                label1.ForeColor = Color.Red;
            }
            var cajasIngresosVentas = CajasIngresos.Where(c => c.Caja.Equals(caja) && c.IdUsuario.Equals(idUsuario)
                    && c.Type.Equals("Ventas") && c.Fecha.Equals(fecha)).ToList();
            if (cajasIngresosVentas.Count > 0)
            {
                var data = cajasIngresosVentas[0].Ingreso;
                label2.Text = data;
                ingresosTotales += Convert.ToDecimal(data.Replace("$", ""));
            }
            else
            {
                label2.Text = "$0.00";
                label2.ForeColor = Color.Red;
            }
            label3.Text = string.Format("${0:#,###,###,##0.00####}", ingresosTotales);
        }
        public List<Tempo_Ventas> getTempoVentas()
        {
            return TempoVentas.Where(t => t.Caja.Equals(caja) && t.IdUsuario.Equals(idUsuario)).ToList();
        }
    }
}
