using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab5_Montenegro.Models;
using Lab5_Consola;
namespace Lab5_Montenegro.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View(new List<Estampa>());
        }
        static Estampa[] sobre = new Estampa[205];
        static string[] claves = new string[205];
        static Dictionary<string, Estampa> dic2 = new Dictionary<string, Estampa>();
        static int contador = 0;
        static Dictionary<Guid, Estampa> dic = new Dictionary<Guid, Estampa>();
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            if (dic.Count == 0)
            {
                int muestra = 0;
                string filePath = string.Empty;
                if (postedFile != null)
                {
                    //dirección del archivo
                    string path = Server.MapPath("~/archivo/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    string csvData = System.IO.File.ReadAllText(filePath);
                    foreach (string row in csvData.Split('\n'))
                    {
                        if ((!string.IsNullOrEmpty(row)) && (contador != 0))
                        {
                            Estampa Estampas = new Estampa();
                            Estampa est = new Estampa();
                            Estampa estam = new Estampa();
                            Guid g;
                            Estampas.EOE = row.Split(';')[0];
                            Estampas.NumeroDeEstampa = Convert.ToInt32(" " + row.Split(';')[1]);
                            Estampas.Nombre = row.Split(';')[2];
                            Estampas.cantidad = Convert.ToInt32(row.Split(';')[3]);
                            
                            g = Guid.NewGuid();
                            dic.Add(g, Estampas);
                            
                            est.EOE = row.Split(';')[0];
                            est.NumeroDeEstampa = Convert.ToInt32(" " + row.Split(';')[1]);
                            est.Nombre = row.Split(';')[2];
                            est.cantidad = Convert.ToInt32(row.Split(';')[3]);
                            string t = Convert.ToString(g);
                            dic2.Add(t, est);
                            claves[muestra] = t;

                            estam.EOE = row.Split(';')[0];
                            estam.NumeroDeEstampa = Convert.ToInt32(" " + row.Split(';')[1]);
                            estam.Nombre = row.Split(';')[2];
                            estam.cantidad = Convert.ToInt32(row.Split(';')[3]);
                            sobre[muestra] = estam;
                            muestra++;
                        }
                        else
                        {
                            contador++;
                        }
                    }
                }
            }
            return View();
        }
        public ActionResult Menú()
        {
            return View();
        }
        public ActionResult ComprarSobre()
        {
            List<Estampa> lista = new List<Estampa>();
            Random nuevosobre = new Random();
            while (lista.Count != 5)
            {
                int num = nuevosobre.Next(0, 203);
                sobre[num].cantidad = 1;
                lista.Add(sobre[num]);
            }
            for (int i = 0; i < 5; i++)
            {
                foreach (Estampa kvp in dic.Values)
                {
                    if (lista[i].NumeroDeEstampa==kvp.NumeroDeEstampa)
                    {
                        kvp.cantidad += 1;
                    }
                }
            }
            var model = from puntos in lista
                        select puntos;
            return View("ComprarSobre", model);
        }
        public ActionResult ObservarAlbumInicial()
        {
            List<Estampa> listas = new List<Estampa>();
            foreach (KeyValuePair<string, Estampa> kvp in dic2)
            {
                listas.Add(kvp.Value);
            }
            var model = from puntos in listas
                        select puntos;
            return View("ObservarAlbumInicial", model);
        }
        public ActionResult ObservarAlbumActual()
        {
            List<Estampa> listad = new List<Estampa>();
            foreach (Estampa k in dic.Values)
            {
                listad.Add(k);
            }
            var model = from puntos in listad
                        select puntos;
            return View("ObservarAlbumActual", model);
        }
        private string ruta = AppDomain.CurrentDomain.BaseDirectory + "Archivos Que se Van a Utilizar//Album Actual.csv";
        public ActionResult Actualizar()
        {
            StreamWriter writer = new StreamWriter(ruta);
            string contenido=null;
            
            foreach(string clave in claves)
            {
                if (clave != null)
                {
                    Guid g;
                    Guid.TryParse(clave, out g);
                    contenido = string.Format("{0},{1},{2}", dic[g].EOE, dic[g].NumeroDeEstampa, dic[g].cantidad);
                    writer.WriteLine(contenido);
                }
            }
            writer.Close();
            return View();
        }
        public ActionResult BusquedaPorClave()
        {
            List<Estampa> dato = new List<Estampa>();
            for(int i = 0; i < claves.Count(); i++)
            {
                Estampa e = new Estampa();
                e.Claves = claves[i];
                dato.Add(e);
            }
            return View(dato);
        }
        public ActionResult Busqueda()
        {
            List<Estampa> l = new List<Estampa>();
            string clave = Request.Form["nombre"].ToString();
            foreach (KeyValuePair<Guid, Estampa> k in dic)
            {
                if(clave == " "+k.Key)
                {
                    Estampa extra = new Estampa();
                    extra = k.Value;
                    l.Add(extra);
                }
                else
                {
                    if (clave == k.Key.ToString())

                    {
                        Estampa extra = new Estampa();
                        extra = k.Value;
                        l.Add(extra);
                    }
                }
            }
            return View(l);
        }
        public ActionResult BusquedaPorEquipo()
        {
            List<Estampa> l = new List<Estampa>();
            string EoE = Request.Form["equipo"].ToString();
            foreach (KeyValuePair<Guid, Estampa> k in dic)
            {
                if (EoE == " " + k.Value.EOE)
                {
                    Estampa extra = new Estampa();
                    extra = k.Value;
                    l.Add(extra);
                }
                else
                {
                    if (EoE == k.Key.ToString())
                    {
                        Estampa extra = new Estampa();
                        extra = k.Value;
                        l.Add(extra);
                    }
                }
            }
            return View(l);
        }
        public ActionResult ComprobarCualesHacenFalta()
        {
            List<Estampa> l = new List<Estampa>();
            foreach (KeyValuePair<Guid, Estampa> k in dic)
            {
                if (k.Value.cantidad==0)
                {
                    Estampa extra = new Estampa();
                    extra = k.Value;
                    l.Add(extra);
                }
            }
            return View(l);
        }
        public ActionResult Intercambio()
        {
            List<Estampa> l = new List<Estampa>();
            int obtener = Convert.ToInt32(Request.Form["NrEstampa1"].ToString());
            int cambiar = Convert.ToInt32(Request.Form["NrEstampa2"].ToString());
            Estampa estampa = new Estampa();
            Estampa estampa2 = new Estampa();
            bool verdadero = false;
            foreach (KeyValuePair<Guid, Estampa> k in dic)
            {
                if (cambiar == k.Value.NumeroDeEstampa)
                {
                    if (k.Value.cantidad > 1)
                    {
                        k.Value.cantidad -= 1;
                        estampa2 = k.Value;
                        ViewData["Excepción"] = "Seleccionó una estampa que poseía una cantidad mayor a una";
                    }
                    else
                    {
                        verdadero = true;
                        ViewData["Excepción"] = "La estampa que intentó cambiar solamente posee la cantidad de 1 regrese y revise su lista actual para verificar otra estampa que poseea mayor cantidad e intente cambiarla nuevamente";
                    }
                }
            }

            if (verdadero != true)
            {
                foreach (KeyValuePair<Guid, Estampa> k in dic)
                {
                    if (obtener == k.Value.NumeroDeEstampa)
                    {
                        k.Value.cantidad += 1;
                        estampa = k.Value;
                    }
                }
            }

            l.Add(estampa);
            l.Add(estampa2);
            if (verdadero == false)
            {
                return View(l);
            }
            else
            {
                int i = 0;
                while(l.Count!=0)
                { 
                    l.Remove(l[i]);
                }
                return View(l);
            }
        }
    }
}