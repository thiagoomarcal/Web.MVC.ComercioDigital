using System;
using System.ComponentModel.DataAnnotations;

namespace Web.MVC.ComercioDigital.Models
{
    public class Produto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é óbrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Valor é óbrigatório.")]
        public decimal Valor { get; set; }
    }
}