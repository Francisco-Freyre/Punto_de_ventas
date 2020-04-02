using Punto_de_ventas.models;
using Punto_de_ventas.modelsclass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Punto_de_ventas
{
    public partial class Entrada : Form
    {
        private string codigo = "", tipo, usuariocorte, pagocon, sucambio;
        private int cantidad = 0, idVenta = 0, ticket = 0, idusuario = 0;
        GroupBox groupBox;
        DateTimePicker dateTimePicker;
        private string rol, usuario;
        private int idUsuario, caja;
        private string dia = DateTime.Now.ToString("dd");
        private string mes = DateTime.Now.ToString("MMM");
        private string año = DateTime.Now.ToString("yyy");
        private string fecha = DateTime.Now.ToString("dd/MMM/yyy");

        public static Caja Caja = new Caja();

       

        public Entrada(List<usuarios> listUsuario, List<Cajas> listCaja)
        {
            InitializeComponent();
            textBox_Dinero.Focus();

            if (null != listUsuario)
            {
                rol = listUsuario[0].Rol;
                idUsuario = listUsuario[0].IdUsuario;
                usuario = listUsuario[0].Usuario;
                if ("Admin" == rol)
                {
                    caja = 0;
                }
                else
                {
                    caja = listCaja[0].Caja;
                }
            }

        }

        private void textBox_Dinero_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClassModels.evento.numberKeyPress(e);
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox_Dinero.Text == "")
                {
                    MessageBox.Show("Favor de insertar un valor numerico.", "Punto Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Caja.guardarIngresosEntrada(caja, textBox_Dinero.Text, Convert.ToInt16(dia), mes, año, idUsuario, fecha);
                    Caja.guardarDineroCaja(caja, textBox_Dinero.Text, idUsuario, fecha);
                    Visible = false;
                }
            }
        }

        private void textBox_Dinero_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox_Dinero.Text = "";
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            ClassModels.imprimir.printDocument(e, groupBox, caja, idUsuario, tipo, dateTimePicker, usuariocorte, pagocon, sucambio);
        }

        private void textBox_Dinero_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox_Dinero.Text == "")
            {
                MessageBox.Show("Favor de insertar un valor numerico.", "Punto Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Caja.guardarIngresosEntrada(caja, textBox_Dinero.Text, Convert.ToInt16(dia), mes, año, idUsuario, fecha);
                Caja.guardarDineroCaja(caja, textBox_Dinero.Text, idUsuario, fecha);
                Visible = false;
            }
            idUsuario = 0;
            caja = 0;
            groupBox = null;
            tipo = "Imprimir";
            dateTimePicker = null;
            printDocument1.Print();
        }
    }
}
