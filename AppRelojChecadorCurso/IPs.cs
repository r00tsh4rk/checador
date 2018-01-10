using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRelojChecadorCurso
{
    class IPs
    {
        public int Id_registro { get; set; }
        public int Bic { get; set; }
        public string Ip { get; set; }


        public IPs() { }

        public IPs(int pId, int pBic, string pIp)

        {
            this.Id_registro = pId;
            this.Bic = pBic;
            this.Ip = pIp;
    
        }
    }
}
