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
    public class PedidoController : Controller
    {
        readonly string apiBaseAddress = ConfigurationManager.AppSettings["EnderecoApi"];

        public async Task<ActionResult> Index()
        {
            IEnumerable<Pedido> pedidos = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync("pedidos");

                if (result.IsSuccessStatusCode)
                {
                    string conteudoRetorno = await result.Content.ReadAsStringAsync();
                    pedidos = JsonConvert.DeserializeObject<List<Pedido>>(conteudoRetorno);
                }
                else
                {
                    pedidos = Enumerable.Empty<Pedido>();
                    ModelState.AddModelError(string.Empty, "Erro ao listar pedidos.");
                }
            }
            return View(pedidos);
        }

        public async Task<ActionResult> NovoPedido()
        {
            IEnumerable<Cliente> clientes = null;
            Pedido pedido = new Pedido();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);
                var result = await client.GetAsync("clientes");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    clientes = JsonConvert.DeserializeObject<List<Cliente>>(retorno);
                    pedido.Clientes = clientes.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome });
                }
                else
                {
                    clientes = Enumerable.Empty<Cliente>();
                    ModelState.AddModelError(string.Empty, "Erro ao listar clientes.");
                }
            }
            
            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NovoPedido([Bind(Include = "Titulo, ClienteId")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(pedido);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PostAsync("pedidos", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao cadastrar novo pedido.");
                    }
                }
            }
            return View(pedido);
        }

        [HttpGet]
        public async Task<ActionResult> AtualizaPedido(string id)
        {
            return await ObterPedido(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AtualizaPedido([Bind(Include = "Id, Titulo")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(pedido.Titulo);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PutAsync($"pedidos/{pedido.Id}", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao atualizar dados do pedido.");
                    }
                }
                return RedirectToAction("Index");
            }
            return View(pedido);
        }

        [HttpGet, ActionName("ExcluiPedido")]
        public async Task<ActionResult> ExcluiPedido(string id)
        {
            return await ObterPedido(id);
        }

        [HttpPost, ActionName("ExcluiPedido")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarExclusao(string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var response = await client.DeleteAsync($"pedidos/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError(string.Empty, "Erro ao excluir pedido.");
            }
            return View();
        }

        private async Task<ActionResult> ObterPedido(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync($"pedidos/{id}");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    pedido = JsonConvert.DeserializeObject<Pedido>(retorno);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao obter dados do pedido.");
                }
            }

            if (pedido == null)
            {
                return HttpNotFound();
            }

            return View(pedido);
        }
    }
}