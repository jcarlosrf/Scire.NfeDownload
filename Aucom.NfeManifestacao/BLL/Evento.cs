using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.BLL
{
    public class Evento
    {
        public int id_nfe { get; set; }
        public string operacao { get; set; }
        public string protocolo { get; set; }
        public string codigo { get; set; }
        public string tipo { get; set; }
        public string xml { get; set; }
    }
}
