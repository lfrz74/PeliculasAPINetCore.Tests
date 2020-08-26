using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Moq;
using Microsoft.Extensions.Logging;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class PeliculasControllerTests : BasePruebas
    {
        private string CrearDataPrueba()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = ConstruirContext(databaseName);
            var genero = new Genero() { Nombre = "genero 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){ Titulo = "Película 1", FechaEstreno = new DateTime(2010, 1,1), EnCines = false },
                new Pelicula(){ Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false },
                new Pelicula(){ Titulo = "Película en Cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true }
            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Película con Género",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };
            peliculas.Add(peliculaConGenero);

            context.Add(genero);
            context.AddRange(peliculas);

            var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            //var context2 = ConstruirContext(databaseName);
            context.Add(peliculaConGenero);
            context.SaveChanges();

            return databaseName;

        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var tituloPelicula = "Película 1";

            var filtroDTO = new FiltroPeliculasDTO()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual(tituloPelicula, peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                EnCines = true
            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Película en Cines", peliculas[0].Titulo);
        }
        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                ProximosEstrenos = true
            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("No estrenada", peliculas[0].Titulo);

        }
        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(g => g.Id).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                GeneroId = generoId
            };


            //Este código de aquí no estaba en el curso, le puse y valió v. En el método CrearDataPrueba no me creó PeliculasGenero q también será v
            var peliculaId = contexto.Peliculas.FirstOrDefault(p => p.Titulo == "Película con Género").Id;
            var peliculaGenero = new PeliculasGeneros() { GeneroId = generoId, PeliculaId = peliculaId };
            var context2 = ConstruirContext(nombreDB);
            context2.Add(peliculaGenero);
            context2.SaveChanges();

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Película con Género", peliculas[0].Titulo);
        }
        [TestMethod]
        public async Task FiltrarOrdenaTituloAscendente()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDB);
            var peliculasDB = contexto2.Peliculas.OrderBy(p => p.Titulo).ToList();

            Assert.AreEqual(peliculasDB.Count, peliculas.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }
        }
        [TestMethod]
        public async Task FiltrarOrdenaTituloDescendente()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = false
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDB);
            var peliculasDB = contexto2.Peliculas.OrderByDescending(p => p.Titulo).ToList();

            Assert.AreEqual(peliculasDB.Count, peliculas.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }
        }
        [TestMethod]
        public async Task FiltrarPorCampoIncorrectoDevuelvePeliculas()
        {
            var nombreDB = CrearDataPrueba();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var mock = new Mock<ILogger<PeliculasController>>();

            var controller = new PeliculasController(contexto, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "abcd",
                OrdenAscendente = false
            };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDB);
            var peliculasDB = contexto2.Peliculas.ToList();
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
    }
}