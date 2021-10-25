using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.MVC.ComercioDigital.Models
{
    public class ItemPedido
    {
        public Guid Id { get; set; }
        public Guid PedidoId { get; set; }
        public string TituloPedido { get; set; }
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public IEnumerable<SelectListItem> Produtos { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é óbrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O campo Quantidade é óbrigatório.")]
        public int Quantidade { get; set; }
    }
}