using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class GenerosControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenenerTodosLosGeneros()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Género 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            //Prueba
            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get();

            //Verificación
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);

        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            //Prueba
            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Get(1);

            //Verificación
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            //Prueba
            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Género 2" });
            await contexto.SaveChangesAsync();

            //Verificación
            var context2 = ConstruirContext(nombreDB);
            var controller = new GenerosController(context2, mapper);

            var id = 1;
            var respuesta = await controller.Get(id);
            var resultado = respuesta.Value;
            Assert.AreEqual(id, resultado.Id);

        }

        [TestMethod]
        public async Task CrearGenero()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            //Prueba
            var nuevoGenero = new GeneroCreacionDTO() { Nombre = "nuevo género" };
            var controller = new GenerosController(contexto, mapper);

            //Verificación
            var respuesta = await controller.Post(nuevoGenero);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreDB);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1, cantidad);
        }
        [TestMethod]
        public async Task ActualizarGenero()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            //Prueba
            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();
            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GenerosController(contexto2, mapper);
            var generoCreacionDTO = new GeneroCreacionDTO() { Nombre = "Nuevo nombre" };

            //Verificación
            var id = 1;
            var respuesta = await controller.Put(id, generoCreacionDTO);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var existe = await contexto3.Generos.AnyAsync(g => g.Nombre == "Nuevo Nombre");
            Assert.IsTrue(existe);


        }
        [TestMethod]
        public async Task IntentaBorrarGeneroNoExistente()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            //Prueba
            var controller = new GenerosController(contexto, mapper);

            //Verificación
            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task IntentaBorrarGeneroExistente()
        {
            //Preparación
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            //Prueba
            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();
            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GenerosController(contexto2, mapper);

            //Verificación
            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var existe = await contexto3.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }

    }
}
