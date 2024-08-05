﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SefazLib.usuarios
{
    public class Usuario
    {
        [Display(Name = "Login")]
        public string login { get; set; }
        [Display(Name = "Nome")]
        public string nome { get; set; }
        [Display(Name = "Foto")]
        public string foto { get; set; }

        public string nomeCompleto { get; set; }
        public string cargo { get; }
        public string email { get; }
        public string id { get; }

        public Usuario(string Login, string Nome, string Foto)
        {
            login = Login;
            nome = Nome;
            foto = Foto;
        }

        public Usuario(string Id, string GivenName, string DisplayName, string JobTitle, string Mail, string Photo)
        {
            id = Id;
            nomeCompleto = DisplayName;
            nome = GivenName;
            cargo = JobTitle;
            email = Mail;
            login = Mail.Split('@')[0];
            foto = Photo;
        }

        public Usuario()
        {
        }

        public static string ObterIniciais(string nomeCompleto)
        {
            if (string.IsNullOrEmpty(nomeCompleto))
                return "";

            string[] palavrasExcluidas = { "de", "dos", "E" };

            string[] partesNome = nomeCompleto.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string iniciais = "";

            for (int i = 0; i < partesNome.Length; i++)
            {
                if (!palavrasExcluidas.Contains(partesNome[i], StringComparer.OrdinalIgnoreCase))
                {
                    iniciais += partesNome[i][0];
                }
            }

            return iniciais;
        }

        public StringContent ToJson()
        {
            return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
        }

        public Usuario ToModel(string UsuarioJson)
        {
            return JsonConvert.DeserializeObject<Usuario>(UsuarioJson);
        }

        public IEnumerable<Usuario> ToList(string UsuarioJson)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Usuario>>(UsuarioJson);
        }
    }
}