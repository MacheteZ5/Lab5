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
        //Un vector de tipo estampa que contendá los diferentes tipos de estampas. Se utilizó para crear un random de todas las estampas para simular la compra de un sobre de estampas
        static Estampa[] sobre = new Estampa[205];
        //un vector que contendrá todas las claves del diccionario
        static string[] claves = new string[205];
        static int contador = 0;
        //Este es el primer diccionario que contendrá un guid como clave y un valor estampa 
        //Por cada estampa del album existe una llave diferente
        static Dictionary<Guid, Estampa> dic = new Dictionary<Guid, Estampa>();
        //Este es el segundo diccionario que contendrá las claves de tipo string y los valores serán los tipo estampa del album
        static Dictionary<string, Estampa> dic2 = new Dictionary<string, Estampa>();
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            //carga de archivo
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
        //Menú, aquí se podrá acceder a todos los tipos de ActionResult del programa
        public ActionResult Menú()
        {
            return View();
        }
        //Aquí se podrá crear un nuevo sobre que contendrá 5 diferentes tipos de estampas
        public ActionResult ComprarSobre()
        {
            // al momento de obtener las cinco estampas, se actualiza la cantidad automáticamente en el diccionario
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
        //Aquí se podrá observar el diccionario 2 el cual contendrá los datos iniciales del album
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
        // Aquí se podra observar el diccionario 1 el cual contendrá los nuevos valores de las estampillas, al momento de comprar un sobre o cambiarla por una nueva estampilla
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
        //Actualizar sirve para crear un nuevo archivo csv que contendrá los valores del album actual
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
        //En pantalla aparecerán las diferentes claves del diccionario, con las cuales podrá buscar los valores de las respectivas claves
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
        // Funciona de la misma manera que la Busqueda, sin embargo en este, se mostrarán todas las estampas del equipo o si es en caso especial.
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
        //Al momento de cambiar una estampa por otra o comprar una estampa nueva, se actualiza el listado de comprobarcualeshacenfalta. 
        //En esta lista solamente apareceran las estampas que le hacen falta para completar el album
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