using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.MVC.ComercioDigital.Models;

namespace Web.MVC.ComercioDigital.Controllers
{
    public class ItensPedidoController : Controller
    {
        readonly string apiBaseAddress = ConfigurationManager.AppSettings["EnderecoApi"];

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> NovoItem(string pedidoId)
        {
            IEnumerable<Produto> produtos;
            ItemPedido itemPedido = new ItemPedido();
            itemPedido.PedidoId = new Guid(pedidoId);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync("produtos");

                if (result.IsSuccessStatusCode)
                {
                    string conteudoRetorno = await result.Content.ReadAsStringAsync();
                    produtos = JsonConvert.DeserializeObject<List<Produto>>(conteudoRetorno);
                    itemPedido.Produtos = produtos.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome });
                }
                else
                {
                    produtos = Enumerable.Empty<Produto>();
                    ModelState.AddModelError(string.Empty, "Erro ao listar produtos.");
                }
            }
            return View(itemPedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NovoItem([Bind(Include = "PedidoId, ProdutoId, Quantidade")] ItemPedido itemPedido)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(itemPedido);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PostAsync("itens", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("../Pedido/Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao cadastrar item.");
                    }
                }
            }
            return View(itemPedido);
        }

        public async Task<ActionResult> Detalhes(string pedidoId)
        {
            if (pedidoId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IEnumerable<ItemPedido> itensPedido = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync($"itens/pedido/{pedidoId}");

                if (result.IsSuccessStatusCode)
                {
                    string conteudoRetorno = await result.Content.ReadAsStringAsync();
                    itensPedido = JsonConvert.DeserializeObject<List<ItemPedido>>(conteudoRetorno);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao listar detalhes do pedido.");
                }
            }

            if (itensPedido == null)
            {
                return HttpNotFound();
            }

            return View(itensPedido);
        }

        [HttpGet]
        public async Task<ActionResult> AtualizaItemPedido(string id)
        {
            return await ObterItemPedido(id);
        }

        [HttpGet]
        public async Task<ActionResult> ExcluiItemPedido(string id)
        {
            return await ObterItemPedido(id);
        }

        [HttpPost, ActionName("ExcluiItemPedido")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarExclusao(string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var response = await client.DeleteAsync($"itens/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("../Pedido/Index");
                }
                else
                    ModelState.AddModelError(string.Empty, "Erro ao excluir pedido.");
            }
            return View();
        }

        private async Task<ActionResult> ObterItemPedido(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ItemPedido itemPedido = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync($"itens/{id}");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    itemPedido = JsonConvert.DeserializeObject<ItemPedido>(retorno);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao obter dados do pedido.");
                }
            }

            if (itemPedido == null)
            {
                return HttpNotFound();
            }

            return View(itemPedido);
        }
    }
}