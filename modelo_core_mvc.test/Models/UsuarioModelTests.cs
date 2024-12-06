using Newtonsoft.Json;
using SefazLib.usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;

namespace SefazLib.Tests
{
    public class UsuarioModelTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var login = "userLogin";
            var nome = "userName";
            var foto = "userPhoto";

            // Act
            var usuario = new Usuario(login, nome, foto);

            // Assert
            Assert.Equal(login, usuario.login);
            Assert.Equal(nome, usuario.nome);
            Assert.Equal(foto, usuario.foto);
        }

        [Fact]
        public void Constructor_SetsPropertiesCorrectly_WithAdditionalParameters()
        {
            // Arrange
            var id = "userId";
            var givenName = "userGivenName";
            var displayName = "userDisplayName";
            var jobTitle = "userJobTitle";
            var mail = "user@mail.com";
            var photo = "userPhoto";

            // Act
            var usuario = new Usuario(id, givenName, displayName, jobTitle, mail, photo);

            // Assert
            Assert.Equal(id, usuario.id);
            Assert.Equal(displayName, usuario.nomeCompleto);
            Assert.Equal(givenName, usuario.nome);
            Assert.Equal(jobTitle, usuario.cargo);
            Assert.Equal(mail, usuario.email);
            Assert.Equal(mail.Split('@')[0], usuario.login);
            Assert.Equal(photo, usuario.foto);
        }

        [Fact]
        public void ObterIniciais_ReturnsCorrectInitials()
        {
            // Arrange
            var nomeCompleto = "João da Silva";

            // Act
            var iniciais = Usuario.ObterIniciais(nomeCompleto);

            // Assert
            Assert.Equal("JS", iniciais);
        }

        [Fact]
        public void ObterIniciais_ReturnsEmptyStringForEmptyName()
        {
            // Arrange
            var nomeCompleto = "";

            // Act
            var iniciais = Usuario.ObterIniciais(nomeCompleto);

            // Assert
            Assert.Equal("", iniciais);
        }

        [Fact]
        public void ToJson_ReturnsCorrectJson()
        {
            // Arrange
            var usuario = new Usuario("userLogin", "userName", "userPhoto");

            // Act
            var jsonContent = usuario.ToJson();
            var jsonString = jsonContent.ReadAsStringAsync().Result;

            // Assert
            var expectedJson = JsonConvert.SerializeObject(usuario);
            Assert.Equal(expectedJson, jsonString);
        }

        [Fact]
        public void ToModel_ReturnsCorrectUsuario()
        {
            // Arrange
            var usuario = new Usuario("userLogin", "userName", "userPhoto");
            var json = JsonConvert.SerializeObject(usuario);

            // Act
            var result = usuario.ToModel(json);

            // Assert
            Assert.Equal(usuario.login, result.login);
            Assert.Equal(usuario.nome, result.nome);
            Assert.Equal(usuario.foto, result.foto);
        }

        [Fact]
        public void ToList_ReturnsCorrectUsuarioList()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario("userLogin1", "userName1", "userPhoto1"),
                new Usuario("userLogin2", "userName2", "userPhoto2")
            };
            var json = JsonConvert.SerializeObject(usuarios);

            // Act
            var result = new Usuario().ToList(json);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(usuarios[0].login, result.ElementAt(0).login);
            Assert.Equal(usuarios[1].login, result.ElementAt(1).login);
        }
    }
}
