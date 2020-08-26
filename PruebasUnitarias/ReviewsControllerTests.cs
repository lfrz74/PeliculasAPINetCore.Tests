using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class ReviewsControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task UsuarioNoPuedeCrearDosReviewsParaLaMismaPelicula()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaId = contexto.Peliculas.Select(x => x.Id).First();
            var review1 = new Review() 
            { 
                PeliculaId = peliculaId, 
                UsuarioId = usuarioPorDefectoId, 
                Puntuacion = 5 
            };

            contexto.Add(review1);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var reviewCreacionDTO = new ReviewCreacionDTO { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDTO);

            var valor = respuesta as IStatusCodeActionResult;
            Assert.AreEqual(400, valor.StatusCode.Value);
        }
        [TestMethod]
        public async Task CrearReview()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaId = contexto.Peliculas.Select(x => x.Id).First();
            var contexto2 = ConstruirContext(nombreDB);

            var mapper = ConfigurarAutoMapper();
            var controller = new ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var reviewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDTO);

            var valor = respuesta as NoContentResult;
            Assert.IsNotNull(valor);

            var contexto3 = ConstruirContext(nombreDB);
            var reviewDB = contexto3.Reviews.First();
            Assert.AreEqual(usuarioPorDefectoId, reviewDB.UsuarioId);
        }
        private void CrearPeliculas(string nombreDB)
        {
            var contexto = ConstruirContext(nombreDB);
            contexto.Peliculas.Add(new Pelicula() { Titulo = "pelicula 1" });
            contexto.SaveChanges();
        }
    }
}
