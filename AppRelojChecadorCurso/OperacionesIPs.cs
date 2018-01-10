using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace AppRelojChecadorCurso
{

    class OperacionesIPs
    {
        public static int Agregar(IPs pIPs)
        {
            String insert;
            string servidor = "localhost";
            string baseDatos = "rjchcseiio";
            string usuario = "root";
            string password = "";

            int retorno = 0;

            insert = "Insert into controlip(bic,ip) values('" + pIPs.Bic + "','" + pIPs.Ip +"');";

            //Creamos la conexión a la base de datos
            MySqlConnection _conexion = new MySqlConnection();
            _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
            _conexion.Open();
            //Se muestra un mensaje de Asistencia Agregada
            MySqlCommand C = new MySqlCommand(insert, _conexion);
            
            retorno = C.ExecuteNonQuery();
            _conexion.Close();
            return retorno;
  
        }

        public static int Actualizar(IPs pIPs)
        {
            String update;
            string servidor = "localhost";
            string baseDatos = "rjchcseiio";
            string usuario = "root";
            string password = "";

            int retorno = 0;

            update = string.Format("Update controlip set bic={0}, ip='{1}' where id_registro={2}", pIPs.Bic,  pIPs.Ip, pIPs.Id_registro);

            //Creamos la conexión a la base de datos
            MySqlConnection _conexion = new MySqlConnection();
            _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
            _conexion.Open();
            //Se muestra un mensaje de Asistencia Agregada
            MySqlCommand C = new MySqlCommand(update, _conexion);

            retorno = C.ExecuteNonQuery();
            _conexion.Close();
            return retorno;


        }


        public static int Eliminar(IPs pIPs)
        {
            String delete;
            string servidor = "localhost";
            string baseDatos = "rjchcseiio";
            string usuario = "root";
            string password = "";

            int retorno = 0;

            delete = string.Format("Delete from controlip where id_registro={0}", pIPs.Id_registro);

            //Creamos la conexión a la base de datos
            MySqlConnection _conexion = new MySqlConnection();
            _conexion.ConnectionString = "Data Source=" + servidor + "; Database=" + baseDatos + "; Uid=" + usuario + "; Pwd=" + password;
            _conexion.Open();
            //Se muestra un mensaje de Asistencia Agregada
            MySqlCommand C = new MySqlCommand(delete, _conexion);

            retorno = C.ExecuteNonQuery();
            _conexion.Close();
            return retorno;


        }
    }
}
