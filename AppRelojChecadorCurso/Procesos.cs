using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKSoftwareAPI;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace AppRelojChecadorCurso
{
    //Crear un metodo que se sobrecargue 
    class Procesos
    {
        string servidor = "127.0.0.1";
        string baseDatos = "rjchcseiio";
        string usuario = "root";
        string password = "";
        String idcredencial, horac, fechac, idasistencia;
        int tipo_marcaje;

        public void proceso_cseiio(Object ip)
        {
            ModeloAsistencias modelo = new ModeloAsistencias();
            int nmarcajes;
            //conexion al reloj checador
            ZKSoftware dispositivo = new ZKSoftware(Modelo.X628C);
            //consulta de la ip del reloj checador 
            // PRUEBA DE CONEXION
         
            if (!dispositivo.DispositivoConectar(Convert.ToString(ip), 1, false))
            {
                //Console.WriteLine(" No hay conexion con el dispositivo" + "\n" + Convert.ToString(ip));
                MessageBox.Show(" No hay conexion con el dispositivo: " + "\n" + Convert.ToString(ip), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            else
            {
                //Console.WriteLine("Conexion establecida con el dispositivo" + "\n" + Convert.ToString(ip));
                 //MessageBox.Show("Conexion establecida con el dispositivo" + "\n" + Convert.ToString(ip));
            } 

            //conexion al reloj checador
            //consulta de la ip del reloj checador 
            //Obtencion del numero total de marcajes del reloj checador

            //Realizar conexion
          //  dispositivo.DispositivoConectar(Convert.ToString(ip), 1, false);
            dispositivo.DispositivoConsultar(NumeroDe.RegistrosDeAsistencias);
            nmarcajes = dispositivo.ResultadoConsulta;


            //Conteo de registros en la base de datos 
            string sql = "select count(*) from asistencias_mapeo;";

            //Creamos la conexion para consultar el numero de registros existentes
            MySqlConnection _con = new MySqlConnection();
            _con.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
            _con.Open();

            MySqlCommand MyCon = new MySqlCommand(sql, _con);
            int cant = Convert.ToInt32(MyCon.ExecuteScalar());
            MySqlDataReader MyReader;
            MyReader = MyCon.ExecuteReader();
           // Console.WriteLine("Registros iniciales en la base de datos: '" + cant + "'" + "\n");
            _con.Close();

            //Se obtiene el ultimo registro de la base de datos
            string consultaUltimo = "SELECT * FROM asistencias_mapeo ORDER BY id_asistencia DESC, hora DESC LIMIT 1;";
            MySqlConnection _conn = new MySqlConnection();
            _conn.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password + "; max pool size=" + 100;
            _conn.Open();
            MySqlCommand Con = new MySqlCommand(consultaUltimo, _conn);
            MySqlDataReader Reader;
            Reader = Con.ExecuteReader();

            //Obtengo el ultimo registro de entrada de la  base de datos
            while (Reader.Read())
            {
                idasistencia = Reader["id_asistencia"].ToString();
                idcredencial = Reader["id_credencial"].ToString();
                horac = Reader["hora"].ToString();
                fechac = Reader["fecha"].ToString();
            }
            _conn.Close();
            //Consulta de marcajes del ultimo al primero
            try
            {
                try
                { 
                if (dispositivo.DispositivoObtenerRegistrosAsistencias())
                {
                    for (int i = (dispositivo.ListaMarcajes.Count - 1); i >= 0; i--)
                    {
                        //Datos a consultar del reloj checador
                        modelo.Numcredencial = dispositivo.ListaMarcajes[i].NumeroCredencial.ToString();
                        modelo.Horac = dispositivo.ListaMarcajes[i].Hora.ToString() + ":" + dispositivo.ListaMarcajes[i].Minuto.ToString() + ":" + dispositivo.ListaMarcajes[i].Segundo.ToString();
                        modelo.Fecha = dispositivo.ListaMarcajes[i].Anio.ToString() + "-" + dispositivo.ListaMarcajes[i].Mes.ToString() + "-" + dispositivo.ListaMarcajes[i].Dia.ToString();

                        //Si la base de datos se encuentra vacia, se realiza la insercion del priemer dato, de esa manera llenara la abse con los
                        //registros que tenga pendiente por subir el reloj checador 

                        if (cant == 0)
                        {
                            // Validar que no haya mas de un marcaje

                            //Console.WriteLine("Llenado incial de la base de datos");

                            // comenzar validacion
                            string insert;

                            int hh = Convert.ToInt32(modelo.Horac.Substring(0, 1));

                            if (hh >= 6 && hh <= 9)
                            {
                                tipo_marcaje = 0; //entrada
                                insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                              "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                //Creamos la conexión a la base de datos
                                MySqlConnection _conexion = new MySqlConnection();
                                _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                _conexion.Open();
                                //Se muestra un mensaje de Asistencia Agregada
                                MySqlCommand C = new MySqlCommand(insert, _conexion);
                                MySqlDataReader lector;
                                lector = C.ExecuteReader();
                                //Console.WriteLine("Entrada registrada");
                                _conexion.Close();
                            }

                            else
                            {
                                tipo_marcaje = 1; //salida
                                insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                              "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                //Creamos la conexión a la base de datos
                                MySqlConnection _conexion = new MySqlConnection();
                                _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                _conexion.Open();
                                //Se muestra un mensaje de Asistencia Agregada
                                MySqlCommand C = new MySqlCommand(insert, _conexion);
                                MySqlDataReader lector;
                                lector = C.ExecuteReader();
                               // Console.WriteLine("Salida Registrada");
                                _conexion.Close();
                            }


                            string consultaUltimo0 = "SELECT * FROM asistencias_mapeo WHERE dispositivo='" + Convert.ToString(ip) + "' ORDER BY id_asistencia DESC, hora DESC LIMIT 1;";
                            MySqlConnection _conn0 = new MySqlConnection();
                            _conn0.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                            _conn0.Open();
                            //Se muestra un mensaje de Asistencia Agregada
                            MySqlCommand Con0 = new MySqlCommand(consultaUltimo0, _conn0);
                            MySqlDataReader Readern;
                            Readern = Con0.ExecuteReader();

                            //Obtengo el ultimo registro de entrada de la  base de datos
                            while (Readern.Read())
                            {
                                idasistencia = Readern["id_asistencia"].ToString();
                                idcredencial = Readern["id_credencial"].ToString();
                                horac = Readern["hora"].ToString();
                                fechac = Readern["fecha"].ToString();

                            }
                            _conn0.Close();

                        }
                        else
                        //Cuando la base de datos tiene registros entonces comenzamos la comparacion 
                        //Compara si los marcajes son mayores a la cantidad de registros en labase de datos entonces inicia el proceso de insercion
                        if (nmarcajes > cant)
                        {


                            //Si la hora y fecha del ultimo registro de la bd es igual a la hora y fehca del ultimo registro del checador
                            // Ya no hay mas entradas o asistencias por registrar 
                            if (horac.Equals(modelo.Horac) == true && fechac.Equals(modelo.Fecha) == true)
                            {
                              //  Console.WriteLine("No se han detectado entradas al reloj checador");
                                break;
                            }
                            else
                            {
                                //Console.WriteLine("Se he detectado una nueva entrada al reloj checador");
                                //Creamos la cadena sql a ejecutar la inserción de los marcajes

                                string insert;
                                int hh = Convert.ToInt32(modelo.Horac.Substring(0, 1));

                                if (hh >= 6 && hh <= 9)
                                {
                                    tipo_marcaje = 0; //entrada
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                   // Console.WriteLine("Entrada agregada a la base de datos con el id: " + modelo.Numcredencial);
                                    _conexion.Close();
                                    cant++;
                                    /*Si ocurre un problema con la maquina o esta apagada, todos los marcajes pendientes por agregar
                                     * a la base de datos, se agregaran en cuanto haya conexion con la bd
                                     */
                                }

                                else
                                {
                                    tipo_marcaje = 1; //salida
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                  //  Console.WriteLine("Entrada agregada a la base de datos con el id: " + modelo.Numcredencial);
                                    _conexion.Close();
                                    cant++;
                                    /*Si ocurre un problema con la maquina o esta apagada, todos los marcajes pendientes por agregar
                                     * a la base de datos, se agregaran en cuanto haya conexion con la bd
                                     */
                                }

                                if (cant == nmarcajes)
                                {
                                   // Console.WriteLine("Se han insertado todas las asistencias pendientes a la base de datos ");
                                    break;
                                }
                            }

                        }
                        else
                        if (cant > nmarcajes)
                        {

                            string consultaUltimo0 = "SELECT * FROM asistencias_mapeo WHERE dispositivo='" + Convert.ToString(ip) + "' ORDER BY id_asistencia DESC, hora DESC LIMIT 1;";
                            MySqlConnection _conn0 = new MySqlConnection();
                            _conn0.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                            _conn0.Open();
                            //Se muestra un mensaje de Asistencia Agregada
                            MySqlCommand Con0 = new MySqlCommand(consultaUltimo0, _conn0);
                            MySqlDataReader Readern;
                            Readern = Con0.ExecuteReader();

                            //Obtengo el ultimo registro de entrada de la  base de datos
                            while (Readern.Read())
                            {
                                idasistencia = Readern["id_asistencia"].ToString();
                                idcredencial = Readern["id_credencial"].ToString();
                                horac = Readern["hora"].ToString();
                                fechac = Readern["fecha"].ToString();

                            }
                            _conn0.Close();

                            if (horac.Equals(modelo.Horac))
                            {
                                //Console.WriteLine("Son iguales, no hay registros");
                                break;
                            }
                            else
                            {
                               // Console.WriteLine("No son iguales");
                                //Creamos la cadena sql a ejecutar la inserción de los marcajes
                                string insert;
                                int hh = Convert.ToInt32(modelo.Horac.Substring(0, 1));

                                if (hh >= 6 && hh <= 9)
                                {
                                    tipo_marcaje = 0; //entrada
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                   // Console.WriteLine("Asistencia Agregada");
                                    _conexion.Close();
                                    //cant++;

                                    //Console.WriteLine("Se han insertado todos los marcajes restantes ");
                                    break;
                                }
                                else
                                {
                                    tipo_marcaje = 1; //entrada
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                   // Console.WriteLine("Asistencia Agregada");
                                    _conexion.Close();
                                    //cant++;

                                   // Console.WriteLine("Se han insertado todos los marcajes restantes ");
                                    break;
                                }
                            }

                        }
                        else
                        {
                           // Console.WriteLine("No se han registrado asistencias");
                            break;
                        }
                    }
                }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Los checadores requieren reinicio de mantenimiento, para seguir funcionando ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                    Application.Exit();
                }
            }
            catch(AccessViolationException e)
            {
                MessageBox.Show("Se genero la siguiente Excepción: "+e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
                //Console.WriteLine(e);
            }
        
            finally
            {
                if (dispositivo.DispositivoObtenerRegistrosAsistencias())
                {
                    for (int i = (dispositivo.ListaMarcajes.Count - 1); i >= 0; i--)
                    {
                        //Datos a consultar del reloj checador
                        modelo.Numcredencial = dispositivo.ListaMarcajes[i].NumeroCredencial.ToString();
                        modelo.Horac = dispositivo.ListaMarcajes[i].Hora.ToString() + ":" + dispositivo.ListaMarcajes[i].Minuto.ToString() + ":" + dispositivo.ListaMarcajes[i].Segundo.ToString();
                        modelo.Fecha = dispositivo.ListaMarcajes[i].Anio.ToString() + "-" + dispositivo.ListaMarcajes[i].Mes.ToString() + "-" + dispositivo.ListaMarcajes[i].Dia.ToString();

                        //Si la base de datos se encuentra vacia, se realiza la insercion del priemer dato, de esa manera llenara la abse con los
                        //registros que tenga pendiente por subir el reloj checador 

                        if (cant == 0)
                        {
                            // Validar que no haya mas de un marcaje

                           // Console.WriteLine("Llenado incial de la base de datos");

                            // comenzar validacion
                            string insert;

                            int hh = Convert.ToInt32(modelo.Horac.Substring(0, 1));

                            if (hh >= 6 && hh <= 9)
                            {
                                tipo_marcaje = 0; //entrada
                                insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                              "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                //Creamos la conexión a la base de datos
                                MySqlConnection _conexion = new MySqlConnection();
                                _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                _conexion.Open();
                                //Se muestra un mensaje de Asistencia Agregada
                                MySqlCommand C = new MySqlCommand(insert, _conexion);
                                MySqlDataReader lector;
                                lector = C.ExecuteReader();
                               // Console.WriteLine("Entrada registrada");
                                _conexion.Close();
                            }

                            else
                            {
                                tipo_marcaje = 1; //salida
                                insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                              "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                //Creamos la conexión a la base de datos
                                MySqlConnection _conexion = new MySqlConnection();
                                _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                _conexion.Open();
                                //Se muestra un mensaje de Asistencia Agregada
                                MySqlCommand C = new MySqlCommand(insert, _conexion);
                                MySqlDataReader lector;
                                lector = C.ExecuteReader();
                              //  Console.WriteLine("Salida Registrada");
                                _conexion.Close();
                            }


                            string consultaUltimo0 = "SELECT * FROM asistencias_mapeo WHERE dispositivo='" + Convert.ToString(ip) + "' ORDER BY id_asistencia DESC, hora DESC LIMIT 1;";
                            MySqlConnection _conn0 = new MySqlConnection();
                            _conn0.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                            _conn0.Open();
                            //Se muestra un mensaje de Asistencia Agregada
                            MySqlCommand Con0 = new MySqlCommand(consultaUltimo0, _conn0);
                            MySqlDataReader Readern;
                            Readern = Con0.ExecuteReader();

                            //Obtengo el ultimo registro de entrada de la  base de datos
                            while (Readern.Read())
                            {
                                idasistencia = Readern["id_asistencia"].ToString();
                                idcredencial = Readern["id_credencial"].ToString();
                                horac = Readern["hora"].ToString();
                                fechac = Readern["fecha"].ToString();

                            }
                            _conn0.Close();

                        }
                        else
                        //Cuando la base de datos tiene registros entonces comenzamos la comparacion 
                        //Compara si los marcajes son mayores a la cantidad de registros en labase de datos entonces inicia el proceso de insercion
                        if (nmarcajes > cant)
                        {


                            //Si la hora y fecha del ultimo registro de la bd es igual a la hora y fehca del ultimo registro del checador
                            // Ya no hay mas entradas o asistencias por registrar 
                            if (horac.Equals(modelo.Horac) == true && fechac.Equals(modelo.Fecha) == true)
                            {
                                //Console.WriteLine("No se han detectado entradas al reloj checador");
                                break;
                            }
                            else
                            {
                               // Console.WriteLine("Se he detectado una nueva entrada al reloj checador");
                                //Creamos la cadena sql a ejecutar la inserción de los marcajes

                                string insert;
                                int hh = Convert.ToInt32(modelo.Horac.Substring(0, 1));

                                if (hh >= 6 && hh <= 9)
                                {
                                    tipo_marcaje = 0; //entrada
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                    //Console.WriteLine("Entrada agregada a la base de datos con el id: " + modelo.Numcredencial);
                                    _conexion.Close();
                                    cant++;
                                    /*Si ocurre un problema con la maquina o esta apagada, todos los marcajes pendientes por agregar
                                     * a la base de datos, se agregaran en cuanto haya conexion con la bd
                                     */
                                }

                                else
                                {
                                    tipo_marcaje = 1; //salida
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                    //Console.WriteLine("Entrada agregada a la base de datos con el id: " + modelo.Numcredencial);
                                    _conexion.Close();
                                    cant++;
                                    /*Si ocurre un problema con la maquina o esta apagada, todos los marcajes pendientes por agregar
                                     * a la base de datos, se agregaran en cuanto haya conexion con la bd
                                     */
                                }

                                if (cant == nmarcajes)
                                {
                                    //Console.WriteLine("Se han insertado todas las asistencias pendientes a la base de datos ");
                                    break;
                                }
                            }

                        }
                        else
                        if (cant > nmarcajes)
                        {

                            string consultaUltimo0 = "SELECT * FROM asistencias_mapeo WHERE dispositivo='" + Convert.ToString(ip) + "' ORDER BY id_asistencia DESC, hora DESC LIMIT 1;";
                            MySqlConnection _conn0 = new MySqlConnection();
                            _conn0.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                            _conn0.Open();
                            //Se muestra un mensaje de Asistencia Agregada
                            MySqlCommand Con0 = new MySqlCommand(consultaUltimo0, _conn0);
                            MySqlDataReader Readern;
                            Readern = Con0.ExecuteReader();

                            //Obtengo el ultimo registro de entrada de la  base de datos
                            while (Readern.Read())
                            {
                                idasistencia = Readern["id_asistencia"].ToString();
                                idcredencial = Readern["id_credencial"].ToString();
                                horac = Readern["hora"].ToString();
                                fechac = Readern["fecha"].ToString();

                            }
                            _conn0.Close();

                            if (horac.Equals(modelo.Horac))
                            {
                                //Console.WriteLine("Son iguales, no hay registros");
                                break;
                            }
                            else
                            {
                                //Console.WriteLine("No son iguales");
                                //Creamos la cadena sql a ejecutar la inserción de los marcajes
                                string insert;
                                int hh = Convert.ToInt32(modelo.Horac.Substring(0, 1));

                                if (hh >= 6 && hh <= 9)
                                {
                                    tipo_marcaje = 0; //entrada
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                   // Console.WriteLine("Asistencia Agregada");
                                    _conexion.Close();
                                    //cant++;

                                   // Console.WriteLine("Se han insertado todos los marcajes restantes ");
                                    break;
                                }
                                else
                                {
                                    tipo_marcaje = 1; //entrada
                                    insert = "Insert into asistencias_mapeo(id_credencial,hora,fecha,dispositivo,tipo_marcaje) values('" + modelo.Numcredencial + "','" + modelo.Horac +
                                  "','" + modelo.Fecha + "','" + Convert.ToString(ip) + "','" + tipo_marcaje + "');";

                                    //Creamos la conexión a la base de datos
                                    MySqlConnection _conexion = new MySqlConnection();
                                    _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
                                    _conexion.Open();
                                    //Se muestra un mensaje de Asistencia Agregada
                                    MySqlCommand C = new MySqlCommand(insert, _conexion);
                                    MySqlDataReader lector;
                                    lector = C.ExecuteReader();
                                    //Console.WriteLine("Asistencia Agregada");
                                    _conexion.Close();
                                    //cant++;

                                   // Console.WriteLine("Se han insertado todos los marcajes restantes ");
                                    break;
                                }
                            }

                        }
                        else
                        {
                            //Console.WriteLine("No se han registrado asistencias");
                            break;
                        }
                    }
                }
            }
        }



        public void desconectar()
        {
            ZKSoftware dispositivo = new ZKSoftware(Modelo.X628C);
            dispositivo.DispositivoDesconectar();
        }
    }

}
