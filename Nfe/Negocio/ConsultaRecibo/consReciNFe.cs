﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.34014
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace Nfe.ConsReciNFe {
    
    public partial class TConsReciNFe
    {
        public TConsReciNFe()
        {

        }
        public TConsReciNFe(string Recibo, string Amb)
        {
            this.versao = "3.10";
            this.nRec = Recibo;
            if (Amb == "PROD")
                this.tpAmb = TAmb.Item1;
            else
                this.tpAmb = TAmb.Item2;
        }
    }
}
