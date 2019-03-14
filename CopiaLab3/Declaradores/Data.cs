using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CopiaLab3.Models;
using Estructuras.NoLinearStructures.Trees;

namespace CopiaLab3.Declaradores
{
    public class Data
    {
        private static Data _instance = null;

        public static Data Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Data();
                }
                return _instance;
            }
        }

        public List<Medicina> ListaMedicinas = new List<Medicina>();
        public List<Medicina> ListaPedido = new List<Medicina>();
        public ArbolBinario<Indice> ArbolIndice = new ArbolBinario<Indice>();
    }
}