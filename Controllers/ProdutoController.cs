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
    public class ProdutoController : Controller
    {
        readonly string apiBaseAddress = ConfigurationManager.AppSettings["EnderecoApi"];

        public async Task<ActionResult> Index()
        {
            IEnumerable<Produto> produtos = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync("produtos");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    produtos = JsonConvert.DeserializeObject<List<Produto>>(retorno);
                }
                else
                {
                    produtos = Enumerable.Empty<Produto>();
                    ModelState.AddModelError(string.Empty, "Erro ao listar produtos.");
                }
            }
            return View(produtos);
        }

        public ActionResult NovoProduto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NovoProduto([Bind(Include = "Nome, Valor")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(produto);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PostAsync("produtos", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao cadastrar novo produto.");
                    }
                }
            }
            return View(produto);
        }

        [HttpGet]
        public async Task<ActionResult> AtualizaProduto(string id)
        {
            return await ObterProduto(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AtualizaProduto([Bind(Include = "Id, Nome, Valor")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(produto);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PutAsync($"produtos/{produto.Id}", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao atualizar dados do produto.");
                    }
                }
                return RedirectToAction("Index");
            }
            return View(produto);
        }

        [HttpGet, ActionName("ExcluiProduto")]
        public async Task<ActionResult> ExcluiProduto(string id)
        {
            return await ObterProduto(id);
        }

        [HttpPost, ActionName("ExcluiProduto")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarExclusao(string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var response = await client.DeleteAsync($"produtos/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError(string.Empty, "Erro ao excluir produto.");
            }
            return View();
        }

        private async Task<ActionResult> ObterProduto(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync($"produtos/{id}");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    produto = JsonConvert.DeserializeObject<Produto>(retorno);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao obter dados do produto.");
                }
            }
            if (produto == null)
            {
                return HttpNotFound();
            }

            return View(produto);
        }
    }
}