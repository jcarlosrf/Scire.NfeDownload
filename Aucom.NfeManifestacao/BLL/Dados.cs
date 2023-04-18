using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.BLL
{
    public class Dados
    {
        public string razao { get; set; }
        public string chave { get; set; }
        public string cnpj { get; set; }
        public int codigo { get; set; }
        public string dataHoraSefaz { get; set; }
        public string ie { get; set; }
        public string nome { get; set; }
        public string emitente { get; set; }
        public string protocolo { get; set; }
        public string destinatario { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string bairro { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string pais { get; set; }
        public string complemento { get; set; }
        public string fone { get; set; }
        public string im { get; set; }
        public string cnae { get; set; }
        public string crt { get; set; }
        public double valor { get; set; }

        public Dados()
        {
             logradouro = "";
             numero = "";
             bairro = "";
             municipio = "";
             uf = "";
             cep = "";
             pais = "";
             complemento = "";
             fone = "";
             im = "";
             cnae = "";
             crt = "";
             valor = 0;
        }
    }
}
