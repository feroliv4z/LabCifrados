﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab_3_1251518_1229918.Models;
namespace Lab_3_1251518_1229918.Controllers
{
    public class CifradoSDESController : Controller
    {
        static string RutaArchivos = string.Empty;
        static int bufferLengt = 100000;
        // GET: CifradoSDES
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LecturaCifrado()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LecturaCifrado(HttpPostedFileBase postedFile)
        {
            var ArchivoLeido = string.Empty;
            var Key = string.Empty;
            //le permite corroborar si la carpeta Files ya existe en la solución
            bool Exists;
            string Paths = Server.MapPath("~/Files/");
            Exists = Directory.Exists(Paths);
            if (!Exists)
            {
                Directory.CreateDirectory(Paths);
            }
            //el siguiente if permite seleccionar un archivo en específico
            if (postedFile != null)
            {
                string rutaDirectorioUsuario = Server.MapPath(string.Empty);
                //se toma la ruta y nombre del archivo
                ArchivoLeido = rutaDirectorioUsuario + Path.GetFileName(postedFile.FileName);
                // se añade la extensión del archivo
                RutaArchivos = rutaDirectorioUsuario;
                postedFile.SaveAs(ArchivoLeido);
                var valor = Convert.ToInt32(Request.Form["clave"].ToString());
                Key = Convert.ToString(valor,2);
                Key = Key.PadLeft(10,'0');
            }
            return RedirectToAction("Cifrado", new { ArchivoLeido,Key});
        }
        public ActionResult Cifrado(string ArchivoLeido, string Key)
        {
            //permutaciones
            var P10 = string.Empty;
            var P8 = string.Empty;
            var P4 = string.Empty;
            var EP = string.Empty;
            var IP = string.Empty;
            CifradoSDES cifradoSDES = new CifradoSDES();
            var NoESValido = cifradoSDES.GenerarPermutaciones(bufferLengt, ref P10, ref P8, ref P4, ref EP, ref IP);
            if (!NoESValido)
            {
                //generar claves
                var resultanteLS1 = string.Empty;
                var K1 = cifradoSDES.GenerarK1(Key, P10, P8, ref resultanteLS1);
                var K2 = cifradoSDES.GenerarK2(resultanteLS1, P8);
                //cifrar
                List<string> BinaryList = cifradoSDES.LecturaArchivo(ArchivoLeido,bufferLengt);
                foreach (string binary in BinaryList)
                {
                    cifradoSDES.Cifrar(binary,IP,EP,K1,P4);
                }
            }
            else
            {
                return RedirectToAction("LecturaCifrado");
            }
            return View();
        }

    }
}
