using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class SalasDeCineControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task ObtenerSalasDeCineA11KilometrosOMenos()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var salasDeCine = new List<SalaDeCine>()
                {
                    new SalaDeCine{ Id = 1, Nombre = "Multicines Condado", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-78.490387, -0.103655)) }
                };

                context.AddRange(salasDeCine);
                await context.SaveChangesAsync();
            }

            var filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 5,
                Latitud = -78.482998,
                Longitud = -0.112414
            };

            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurarAutoMapper();
                var controller = new SalasDeCineController(context, mapper, geometryFactory);
                var respuesta = await controller.Cercanos(filtro);
                var valor = respuesta.Value;

                Assert.AreEqual(1, valor.Count);
            }

        }

    }
}
