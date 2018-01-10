using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Timers;
using System.Diagnostics;

namespace AppRelojChecadorCurso
{
    public partial class Form1 : Form
    {

        string servidor = "localhost";
        string baseDatos = "rjchcseiio";
        string usuario = "root";
        string password = "";
        string ipdispositivo, pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9, pos10, pos11, pos12, pos13, pos14, pos15;
        private DataGridViewButtonColumn con;
        private DataGridViewButtonColumn desc;
        private bool bandera = false;
        static System.Timers.Timer _timer;
        Thread[] hilos;

        public Form1()
        {
            
            InitializeComponent();
            ModeloAsistencias modelo = new ModeloAsistencias();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mostrar();
            mostrarip();
            cargarCombo();
            llenarTablaConexiones();
            CreadorBotones();
       
            string sql = "select count(ip) from controlip;";

            //Creamos la conexion para consultar el numero de dispositivos existentes
            MySqlConnection _con = new MySqlConnection();
            _con.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
            _con.Open();

            MySqlCommand MyCon = new MySqlCommand(sql, _con);
            int cant = Convert.ToInt32(MyCon.ExecuteScalar());
            MySqlDataReader MyReader;
            MyReader = MyCon.ExecuteReader();
            Console.WriteLine(" IP´s encontradas en la BD: '" + cant + "'" + "\n");
            _con.Close();
            //creamos los hilos
            hilos = new Thread[cant];

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {           
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btn_actualizar_Click(object sender, EventArgs e)
        {
            marcajes.Refresh();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        void mostrar()
        {
            // Se crea la conexión a la base de datos
            MySqlConnection _conexion= new MySqlConnection();
            _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password + "; max pool size=" + 100;

            // Se abre la conexion
            _conexion.Open();

            // Se crea un DataTable que almacenará los datos desde donde se cargaran los datos al DataGridView
            DataTable dtDatos = new DataTable();

            // Se crea un MySqlAdapter para obtener los datos de la base
            MySqlDataAdapter mdaDatos = new MySqlDataAdapter("select a.id_asistencia, a.id_credencial, e.nombre, esc.clave_bic, a.hora, a.fecha, a.dispositivo, a.tipo_marcaje  from asistencias_mapeo as a inner join empleados as e on a.id_credencial = e.id_empleado  inner join escuelas as esc on e.bic = esc.idescuela; ", _conexion);

            // Con la información del adaptador se rellena el DataTable
            mdaDatos.Fill(dtDatos);

            // Se asigna el DataTable como origen de datos del DataGridView
            marcajes.DataSource = dtDatos;
             
            marcajes.Sort(marcajes.Columns["id_asistencia"], ListSortDirection.Descending);
            marcajes.Columns["id_asistencia"].Visible = false;
            marcajes.Columns["id_credencial"].HeaderText = "ID de Asesor";
            marcajes.Columns["nombre"].HeaderText = "Nombre de Asesor";
            marcajes.Columns["clave_bic"].HeaderText = "BIC";
            marcajes.Columns["hora"].HeaderText = "Hora de Registro";
            marcajes.Columns["fecha"].HeaderText = "Fecha de Registro";
            marcajes.Columns["dispositivo"].HeaderText = "Reloj Checador de Origen";
            marcajes.Columns["tipo_marcaje"].HeaderText = "Entrada= 0 - Salida= 1";
            // Se cierra la conexión a la base de datos
            _conexion.Close();
        }


        void mostrarip()
        {
            // Se crea la conexión a la base de datos
            MySqlConnection _conexionip = new MySqlConnection();
            _conexionip.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password + "; max pool size=" + 100;

            // Se abre la conexion
            _conexionip.Open();

            // Se crea un DataTable que almacenará los datos desde donde se cargaran los datos al DataGridView
            DataTable dtDatos = new DataTable();

            // Se crea un MySqlAdapter para obtener los datos de la base
            MySqlDataAdapter mdaDatos = new MySqlDataAdapter("select c.id_registro, e.clave_bic, c.ip from  controlip as c inner join escuelas as e on c.bic = e.idescuela; ", _conexionip);

            // Con la información del adaptador se rellena el DataTable
            mdaDatos.Fill(dtDatos);

            // Se asigna el DataTable como origen de datos del DataGridView
            ips.DataSource = dtDatos;

            ips.Sort(ips.Columns["id_registro"], ListSortDirection.Ascending);
            ips.Columns["id_registro"].HeaderText = "#";
            ips.Columns["clave_bic"].HeaderText = "BIC";
            ips.Columns["ip"].HeaderText = "Dirección IP del Dispositivo";
            // Se cierra la conexión a la base de datos
            _conexionip.Close();

        }


        void cargarCombo()
        {

            MySqlConnection _conexion = new MySqlConnection();
            _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password + "; max pool size=" + 100;

            // Se abre la conexion
            _conexion.Open();

            // Se crea un DataTable que almacenará los datos desde donde se cargaran los datos al DataGridView
            DataTable dtDatos = new DataTable();

            // Se crea un MySqlAdapter para obtener los datos de la base
            MySqlDataAdapter mdaDatos = new MySqlDataAdapter("select * from escuelas", _conexion);

            // Con la información del adaptador se rellena el DataTable
            mdaDatos.Fill(dtDatos);
            cmbBic.ValueMember = "idescuela";
            cmbBic.DisplayMember = "clave_bic";
            // Se asigna el DataTable como origen de datos del DataGridView
            cmbBic.DataSource = dtDatos;

            _conexion.Close();
        }



        // Metodo para crear un datagridview que tome las ips de la base de datos y añada los botones en la tabla para conexion
        private void llenarTablaConexiones()
        {
            // Se crea la conexión a la base de datos
            MySqlConnection _conexionip = new MySqlConnection();
            _conexionip.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password + "; max pool size=" + 100;

            // Se abre la conexion
            _conexionip.Open();

            // Se crea un DataTable que almacenará los datos desde donde se cargaran los datos al DataGridView
            DataTable dtDatos = new DataTable();

            // Se crea un MySqlAdapter para obtener los datos de la base
            MySqlDataAdapter mdaDatos = new MySqlDataAdapter("select c.id_registro, e.clave_bic, c.ip from  controlip as c inner join escuelas as e on c.bic = e.idescuela; ", _conexionip);

            // Con la información del adaptador se rellena el DataTable
            mdaDatos.Fill(dtDatos);

            // Se asigna el DataTable como origen de datos del DataGridView
            dtConexiones.DataSource = dtDatos;
            //Definicion del Datagridview con valores de cabecera y la propiedad nombre para ser utilizada en las operaciones de conexion con el reloj checador 
            dtConexiones.Columns["id_registro"].HeaderText = "#";
         
            dtConexiones.Columns["clave_bic"].HeaderText = "BIC";
       
            dtConexiones.Columns["ip"].HeaderText = "Dirección IP del Dispositivo";
           

            // Se cierra la conexión a la base de datos
            _conexionip.Close();


        }

        private void CreadorBotones()
        {
            //---------------- Boton Conectar --------------
            DataGridViewButtonColumn con = new DataGridViewButtonColumn();
            DataGridViewButtonColumn desc = new DataGridViewButtonColumn();
            con.HeaderText = "Conectar";
            con.Text = "Conectar";
            con.Name = "conectar";
            con.UseColumnTextForButtonValue = true;
            dtConexiones.Columns.Add(con);
            //--------------- Boton Desconectar --------------
            desc.HeaderText = "Desconectar";
            desc.Text = "Desconectar";
            desc.Name = "desconectar";
            desc.UseColumnTextForButtonValue = true;
            dtConexiones.Columns.Add(desc);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        //Agregar una IP
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            IPs ip = new IPs();
            ip.Bic = Convert.ToInt32(cmbBic.SelectedValue.ToString());
            ip.Ip = txtIP.Text.Trim();

            int resultado = OperacionesIPs.Agregar(ip);
            if (resultado > 0)
            {
                MessageBox.Show("IP Guardada Con Exito!!", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                mostrarip();
                llenarTablaConexiones();
                txtID.Text = "";
                cmbBic.Text = "";
                txtIP.Text = "";
            }
            else
            {
                MessageBox.Show("No se pudo guardar la IP", "Fallo!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                mostrarip();
                llenarTablaConexiones();
                txtID.Text = "";
                cmbBic.Text = "";
                txtIP.Text = "";
            }
        }
        //Modificar la informacion de una IP
        private void btnModificar_Click(object sender, EventArgs e)
        {
            IPs ip = new IPs();
            ip.Id_registro = Convert.ToInt32(txtID.Text.ToString());
            ip.Bic = Convert.ToInt32(cmbBic.SelectedValue.ToString());
            ip.Ip = txtIP.Text.Trim();

            if (OperacionesIPs.Actualizar(ip) > 0)
            {
                MessageBox.Show("Los datos de la IP se actualizaron correctamente", "Datos Actualizados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                mostrarip();
                llenarTablaConexiones();
                txtID.Text = "";
                cmbBic.Text = "";
                txtIP.Text = "";
            }
            else
            {
                MessageBox.Show("No se pudo actualizar", "Error al Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                mostrarip();
                llenarTablaConexiones();
                txtID.Text = "";
                cmbBic.Text = "";
                txtIP.Text = "";
            }
        }

        //Eliminar una IP
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            IPs ip = new IPs();
            ip.Id_registro = Convert.ToInt32(txtID.Text.ToString());

            if (MessageBox.Show("Esta Seguro que desea eliminar la IP Actual", "¿Estas Seguro?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (OperacionesIPs.Eliminar(ip) > 0)
                {
                    MessageBox.Show("IP Eliminada Correctamente!", "IP Eliminada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mostrarip();
                    llenarTablaConexiones();
                    txtID.Text = "";
                    cmbBic.Text = "";
                    txtIP.Text = "";
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar la IP", "IP No Eliminada", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    mostrarip();
                    llenarTablaConexiones();
                    txtID.Text = "";
                    cmbBic.Text = "";
                    txtIP.Text = "";
                }
            }
            else
                MessageBox.Show("Se cancelo la eliminacion", "Eliminacion Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            mostrarip();
            llenarTablaConexiones();
            txtID.Text = "";
            cmbBic.Text = "";
            txtIP.Text = "";
        }

        private void ips_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = this.ips.CurrentRow.Cells[0].Value.ToString();
            cmbBic.Text = this.ips.CurrentRow.Cells[1].Value.ToString();
            txtIP.Text = this.ips.CurrentRow.Cells[2].Value.ToString();
        }

        private void dtConexiones_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentRow = int.Parse(e.RowIndex.ToString());//Obtiene la fila actual
            if (e.ColumnIndex >= 0 && this.dtConexiones.Columns[e.ColumnIndex].Name == "conectar" && e.RowIndex >= 0)
            {
                Procesos proceso = new Procesos();
                dtConexiones[0, currentRow].Style.BackColor = Color.Green;
                dtConexiones[1, currentRow].Style.BackColor = Color.Green;
                dtConexiones[2, currentRow].Style.BackColor = Color.Green;
                dtConexiones[3, currentRow].Style.BackColor = Color.Green;
                dtConexiones[4, currentRow].Style.BackColor = Color.Green;

                String update;
                string servidor = "localhost";
                string baseDatos = "rjchcseiio";
                string usuario = "root";
                string password = "";

                // ---------------- ACTUALIZAR BANDERA DE ENCENDIDO --------------------------
                MessageBox.Show("Conectandose al dispositivo: " + Convert.ToString(dtConexiones[4, currentRow].Value) + " del BIC: " + Convert.ToString(dtConexiones[3, currentRow].Value), "Obteniendo IP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Despues de seleccionar el boton de conectar en la tabla, se crea el proceso con la ip que se obtiene de la tabla.

                int id = Convert.ToInt32(dtConexiones[2, currentRow].Value);
                ipdispositivo = Convert.ToString(dtConexiones[4, currentRow].Value);
                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;
                btnModificar.Enabled = false;

                String banderaon = "on";
                update = string.Format("Update controlip set bandera='{0}' where id_registro={1}", banderaon, id);

                //Creamos la conexión a la base de datos
                MySqlConnection _conexion = new MySqlConnection();
                _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                _conexion.Open();
                //Se muestra un mensaje de Asistencia Agregada
                MySqlCommand C = new MySqlCommand(update, _conexion);

                C.ExecuteNonQuery();
                _conexion.Close();
                // ---------------- FINALIZACIÓN DE ACTUALIZAR BANDERA DE ENCENCIDO --------------------------
                // ----- SWITCHEO BASADO EN EL ID ASOCIADO A LA POSICION DE LA IP EN EL FORMULARIO -----
                switch (id)
                {
                    case 3:
                     Thread hilo1 = new Thread(new ParameterizedThreadStart(crearTimer));
                        pos1 = Convert.ToString(dtConexiones[4, 0].Value);
                        hilo1.Start(pos1);
                        break;

                    case 4:
                        Thread hilo2 = new Thread(new ParameterizedThreadStart(crearTimer2));
                        pos2 = Convert.ToString(dtConexiones[4, 1].Value);
                        hilo2.Start(pos2);
                        break;

                    case 5:
                        Thread hilo3 = new Thread(new ParameterizedThreadStart(crearTimer3));
                        pos3 = Convert.ToString(dtConexiones[4, 2].Value);
                        hilo3.Start(pos3);
                        break;

                    case 6:
                        Thread hilo4 = new Thread(new ParameterizedThreadStart(crearTimer4));
                        pos4 = Convert.ToString(dtConexiones[4, 3].Value);
                        hilo4.Start(pos4);
                        break;

                    case 14:
                        Thread hilo5 = new Thread(new ParameterizedThreadStart(crearTimer5));
                        pos5 = Convert.ToString(dtConexiones[4, 4].Value);
                        hilo5.Start(pos5);
                        break;

                    case 15:
                        Thread hilo6 = new Thread(new ParameterizedThreadStart(crearTimer6));
                        pos6 = Convert.ToString(dtConexiones[4, 5].Value);
                        hilo6.Start(pos6);
                        break;

                    case 16:
                        Thread hilo7 = new Thread(new ParameterizedThreadStart(crearTimer7));
                        pos7 = Convert.ToString(dtConexiones[4, 6].Value);
                        hilo7.Start(pos7);
                        break;

                    case 17:
                        Thread hilo8 = new Thread(new ParameterizedThreadStart(crearTimer8));
                        pos8 = Convert.ToString(dtConexiones[4, 7].Value);
                        hilo8.Start(pos8);
                        break;

                    case 18:
                        Thread hilo9 = new Thread(new ParameterizedThreadStart(crearTimer9));
                        pos9 = Convert.ToString(dtConexiones[4, 8].Value);
                        hilo9.Start(pos9);
                        break;

                    case 19:
                        Thread hilo10 = new Thread(new ParameterizedThreadStart(crearTimer10));
                        pos10 = Convert.ToString(dtConexiones[4, 9].Value);
                        hilo10.Start(pos10);
                        break;

                    case 20:
                        Thread hilo11 = new Thread(new ParameterizedThreadStart(crearTimer11));
                        pos11 = Convert.ToString(dtConexiones[4, 10].Value);
                        hilo11.Start(pos11);
                        break;

                    case 21:
                        Thread hilo12 = new Thread(new ParameterizedThreadStart(crearTimer12));
                        pos12 = Convert.ToString(dtConexiones[4, 11].Value);
                        hilo12.Start(pos12);
                        break;

                    case 22:
                        Thread hilo13= new Thread(new ParameterizedThreadStart(crearTimer13));
                        pos13 = Convert.ToString(dtConexiones[4, 12].Value);
                        hilo13.Start(pos13);
                        break;

                    case 23:
                        Thread hilo14 = new Thread(new ParameterizedThreadStart(crearTimer14));
                        pos14 = Convert.ToString(dtConexiones[4, 13].Value);
                        hilo14.Start(pos14);
                        break;

                    case 24:
                        Thread hilo15 = new Thread(new ParameterizedThreadStart(crearTimer15));
                        pos15 = Convert.ToString(dtConexiones[4, 14].Value);
                        hilo15.Start(pos15);
                        break;

                    default:
                        break;
                }

                /* 
                 * 1.- Bandera de conexion desde la base, comprobar si estan conectados desde la base de datos con una bandera OFF / ON
                 * 2.- Combrobar si la ip esta conectada 
                 * 3.- Posteriormente ejecutar el proceso.procesocseiio con la IP que tenga la bandera ON
                 * 4.- Si un IP(Dispositivo) se agrega se comprueba si esta activada
                 * 5.- Se crea un hilo por cada dispositivo que se encuentre en ON
                 * 6.- Se le asigna un id a cada hilo y este aumenta en uno cuando la bandera esta en ON
                 */
                // Aqui se ejecutaria la cosulta mediante un while, dentro de ese while se crearia el proceso de obtencion de informacion
                //1.- Hacer una consulta a la base de datos para verificar cuantos registros hay

            }

            if (e.ColumnIndex >= 0 && this.dtConexiones.Columns[e.ColumnIndex].Name == "desconectar" && e.RowIndex >= 0)
            {
                bandera = false;
                dtConexiones[0, currentRow].Style.BackColor = Color.Red;
                dtConexiones[1, currentRow].Style.BackColor = Color.Red;
                dtConexiones[2, currentRow].Style.BackColor = Color.Red;
                dtConexiones[3, currentRow].Style.BackColor = Color.Red;
                dtConexiones[4, currentRow].Style.BackColor = Color.Red;

                String update;
                string servidor = "localhost";
                string baseDatos = "rjchcseiio";
                string usuario = "root";
                string password = "";


                MessageBox.Show("Desconectandose al dispositivo: " + Convert.ToString(dtConexiones[4, currentRow].Value) + " del BIC: " + Convert.ToString(dtConexiones[3, currentRow].Value), "Obteniendo IP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                int id = Convert.ToInt32(dtConexiones[2, currentRow].Value);
                ipdispositivo = Convert.ToString(dtConexiones[4, currentRow].Value);
                String banderaoff = "off";
                update = string.Format("Update controlip set bandera='{0}' where id_registro={1}", banderaoff, id);

                //Creamos la conexión a la base de datos
                MySqlConnection _conexion = new MySqlConnection();
                _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                _conexion.Open();
                //Se muestra un mensaje de Asistencia Agregada
                MySqlCommand C = new MySqlCommand(update, _conexion);

                C.ExecuteNonQuery();
                _conexion.Close();

                btnAgregar.Enabled = true;
                btnEliminar.Enabled = true;
                btnModificar.Enabled = true;
            }
        }
      // ----------------------- SECCION DE TEMPORIZADORES -----------------------------
        private void crearTimer(Object ip)
        {
            pos1 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos1);
        }

        private void crearTimer2(Object ip)
        {
            pos2 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed2);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed2(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos2);

        }

        private void crearTimer3(Object ip)
        {
            pos3 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed3);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed3(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos3);

        }

        private void crearTimer4(Object ip)
        {
            pos4 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed4);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed4(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos4);

        }

        private void crearTimer5(Object ip)
        {
            pos5 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed5);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed5(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos5);

        }

        private void crearTimer6(Object ip)
        {
            pos6 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed6);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed6(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos6);

        }

        private void crearTimer7(Object ip)
        {
            pos7 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed7);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed7(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos7);

        }

        private void crearTimer8(Object ip)
        {
            pos8 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed8);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed8(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos8);

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://drive.google.com/file/d/0B44qibjqiACVaGh3elJOT242Q3M/view");
        }

        private void crearTimer9(Object ip)
        {
            pos9 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed9);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed9(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos9);

        }

        private void crearTimer10(Object ip)
        {
            pos10 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed10);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed10(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos10);

        }

        private void crearTimer11(Object ip)
        {
            pos11 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed11);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed11(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos11);

        }

        private void crearTimer12(Object ip)
        {
            pos12 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed12);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed12(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos12);

        }

        private void crearTimer13(Object ip)
        {
            pos13 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed13);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed13(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos13);
          
        }

        private void crearTimer14(Object ip)
        {
            pos14 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed14);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed14(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos14);

        }

        private void crearTimer15(Object ip)
        {
            pos15 = ip as String;
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed15);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed15(object sender, ElapsedEventArgs e)
        {
            Procesos proceso = new Procesos();
            proceso.proceso_cseiio(pos15);

        }
        // ----------------------- TERMINA SECCION DE TEMPORIZADORES -----------------------------

        private void actualizarTabla_Tick(object sender, EventArgs e)
        {
            mostrar();
            marcajes.Refresh();
        }
    }


}
