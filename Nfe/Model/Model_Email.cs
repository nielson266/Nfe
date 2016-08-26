using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nfe.Entidade;

namespace Nfe.Model
{
    public class Model_Email
    {
        public bool ExisteTipoEmail(string TpEmail)
        {
            return BancoDados.CodigoExiste("SELECT TpEmail FROM Email WHERE TpEmail ='" + TpEmail + "'");
        }
        public Entidade_Email PesquisaTipoEmail(string TpEmail)
        {
            Entidade_Email EntEmail = new Entidade_Email();

            var Dt = BancoDados.Consultar("SELECT * FROM Email WHERE TpEmail ='" + TpEmail + "'");

            EntEmail.Email = Dt.Rows[0]["NmEmail"].ToString();
            EntEmail.Usuario = Dt.Rows[0]["NmUsuario"].ToString();
            EntEmail.Senha = Dt.Rows[0]["Senha"].ToString();
            EntEmail.Smtp = Dt.Rows[0]["Smtp"].ToString();
            EntEmail.Porta = int.Parse(Dt.Rows[0]["Porta"].ToString());

            return EntEmail;
        }
    }
}
