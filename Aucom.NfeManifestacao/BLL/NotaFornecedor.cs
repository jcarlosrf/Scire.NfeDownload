using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.BLL
{
    public class NotaFornecedor
    {
        public string chave { get; set; }
        public string cnpj { get; set; }
        public string razao { get; set; }
        public string data { get; set; }
        public string cnpjdestinatario { get; set; }
    }
}
