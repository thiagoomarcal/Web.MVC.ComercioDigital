using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.MVC.ComercioDigital.Models
{
    public class Pedido
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        public string Titulo { get; set; }
        public int Numero { get; set; }

        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        public Guid ClienteId { get; set; }
        public string NomeCliente { get; set; }
        public IEnumerable<SelectListItem> Clientes { get; set; }
    }
}