﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.34011
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace Nfe.ConsStatServ {
    using System.Data;
    using System.Xml.Serialization;
    using Nfe.Entidade;
    using Nfe.Negocio.Geral;


    public partial class TConsStatServ
    {
        public TConsStatServ()
        {

        }
        public TConsStatServ(Entidade_Status eObjStatus)
        {
            this.cUF =  (TCodUfIBGE)NegocioFuncoesGerais.RetornoCodigoIbge(int.Parse(eObjStatus.cUf));
            if(eObjStatus.tpAmbiente == "PROD")
                this.tpAmb = TAmb.Item1;
            else
                this.tpAmb = TAmb.Item2;
            this.versao = eObjStatus.versao;
            this.xServ = TConsStatServXServ.STATUS;
        }
    }
    
}