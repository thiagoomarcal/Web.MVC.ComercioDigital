
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.MVC.ComercioDigital.Models
{
    public class Cliente
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é óbrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é óbrigatório.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }
    }
}