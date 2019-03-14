using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CopiaLab3.Declaradores;
using CopiaLab3.Models;

namespace CopiaLab3.Controllers
{
    public class HomeController : Controller
    {
        static int contador = 0;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BusquedaResultado()
        {
            ViewBag.Id = TempData["id"].ToString();
            ViewBag.Nombre = TempData["nombre"].ToString();
            ViewBag.Descripcion = TempData["descripcion"].ToString();
            ViewBag.Casa = TempData["casa"].ToString();
            ViewBag.Precio = TempData["precio"].ToString();
            ViewBag.Existencia = TempData["existencia"].ToString();
            return View();
        }

        public ActionResult BusquedaNombre()
        {
            if (TempData["AvisoMalo"] != null)
            {
                ViewBag.AvisoM = TempData["AvisoMalo"];
            }

            return View();
        }

        [HttpPost]
        public ActionResult BusquedaNombre(FormCollection coleccion)
        {
            var Nombre = coleccion["Nombre"];

            var Item = new Indice(0,Nombre);

            if (Data.Instance.ArbolIndice.Buscar(Data.Instance.ArbolIndice.Raiz, Item, Indice.OrdenarPorNombre))
            {
                var MedEncontrado = Data.Instance.ArbolIndice.DevolverValor(Data.Instance.ArbolIndice.Raiz, Item,Indice.OrdenarPorNombre);
                
                Predicate<Medicina> BuscarMedicamento = (Medicina med) => { return med.Nombre == MedEncontrado.Nombre; };
                
                var Medicamento = Data.Instance.ListaMedicinas.Find(BuscarMedicamento);

                TempData["id"] = Medicamento.Id;
                TempData["nombre"] = Medicamento.Nombre;
                TempData["descripcion"] = Medicamento.Descripcion;
                TempData["casa"] = Medicamento.Casa;
                TempData["precio"] = Medicamento.Precio;
                TempData["existencia"] = Medicamento.Existencia;

                TempData["AvisoBueno"] = "El medicamento ha sido agregado correctamente";

                return RedirectToAction("BusquedaResultado");
            }
            else
            {
                TempData["AvisoMalo"] = "El medicamento no se ha encontrado en el índice";
                return View();
            }
        }

        public ActionResult CrearPedido()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearPedido(FormCollection coleccion)
        {
            var nombre = coleccion["Nombre"];
            var direccion = coleccion["Direccion"];
            var nit = coleccion["Nit"];

            var PedidoNuevo = new Pedido(nombre,direccion,nit,Data.Instance.ListaPedido);
            Escribir(PedidoNuevo);
            return RedirectToAction("Index");
        }


        public ActionResult BuscarParaPedido()
        {
            if (TempData["AvisoBueno"] != null)
            {
                ViewBag.AvisoB = TempData["AvisoBueno"];
            }
            if (TempData["AvisoMalo"] != null)
            {
                ViewBag.AvisoM = TempData["AvisoMalo"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult BuscarParaPedido(FormCollection coleccion)
        {
            var Nombre = coleccion["Nombre"];

            var Item = new Indice(0, Nombre);

            if (Data.Instance.ArbolIndice.Buscar(Data.Instance.ArbolIndice.Raiz, Item, Indice.OrdenarPorNombre))
            {
                var MedEncontrado = Data.Instance.ArbolIndice.DevolverValor(Data.Instance.ArbolIndice.Raiz, Item, Indice.OrdenarPorNombre);

                Predicate<Medicina> BuscarMedicamento = (Medicina med) => { return med.Nombre == MedEncontrado.Nombre; };

                Data.Instance.ListaPedido.Add(Data.Instance.ListaMedicinas.Find(BuscarMedicamento));

                foreach (var item in Data.Instance.ListaMedicinas)
                {
                    if (item.Nombre == MedEncontrado.Nombre)
                    {
                        item.Existencia--;

                        if (item.Existencia == 0)
                        {
                            Data.Instance.ArbolIndice.Eliminar(Data.Instance.ArbolIndice.Raiz,MedEncontrado,Indice.OrdenarPorNombre);
                        }
                    }
                }

                TempData["AvisoBueno"] = "El medicamento ha sido agregado correctamente";

                return RedirectToAction("BuscarParaPedido");
            }
            else
            {
                TempData["AvisoMalo"] = "El medicamento no se ha encontrado en el índice";
                return View();
            }
        }

        private void Escribir(Pedido nuevoPedido)
        {
            contador++;
            var FilePath = string.Empty;
            var path = Server.MapPath("~/PedidosCSV/");
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FilePath = path + "Pedido" + contador.ToString() + ".txt";
            
            StreamWriter escritor = new StreamWriter(FilePath);

            escritor.Write(nuevoPedido.Nombre);
            escritor.Write(",");
            escritor.Write(nuevoPedido.Direccion);
            escritor.Write(",");
            escritor.Write(nuevoPedido.Nit);
            escritor.Write(",");
            foreach (var item in nuevoPedido.Medicinas)
            {
                escritor.Write("[");
                escritor.Write(item.Nombre);
                escritor.Write(",");
                escritor.Write(item.Descripcion);
                escritor.Write(",");
                escritor.Write(item.Casa);
                escritor.Write(",");
                escritor.Write(item.Precio);
                escritor.Write("]");
            }
            escritor.Write(",");
            escritor.Write("Q."+nuevoPedido.Total);
            escritor.Close();
        }
    }
}