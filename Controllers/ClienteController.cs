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
    public class ClienteController : Controller
    {
        readonly string apiBaseAddress = ConfigurationManager.AppSettings["EnderecoApi"];

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Cliente> clientes = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync("clientes");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    clientes = JsonConvert.DeserializeObject<List<Cliente>>(retorno);
                }
                else
                {
                    clientes = Enumerable.Empty<Cliente>();
                    ModelState.AddModelError(string.Empty, "Erro ao listar clientes.");
                }
            }
            return View(clientes);
        }

        [HttpGet]
        public ActionResult NovoCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NovoCliente([Bind(Include = "Nome, Email, Telefone")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(cliente);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PostAsync("clientes", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao cadastrar novo cliente.");
                    }
                }
            }
            return View(cliente);
        }

        [HttpGet]
        public async Task<ActionResult> AtualizaCliente(string id)
        {
            return await ObterCliente(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AtualizaCliente([Bind(Include = "Id, Nome, Email, Telefone")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(cliente);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiBaseAddress);
                    var response = await client.PutAsync($"clientes/{cliente.Id}", data);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao atualizar dados do cliente.");
                    }
                }

                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        [HttpGet, ActionName("ExcluiCliente")]
        public async Task<ActionResult> ExcluiCliente(string id)
        {
            return await ObterCliente(id);
        }

        [HttpPost, ActionName("ExcluiCliente")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarExclusao(string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var response = await client.DeleteAsync($"clientes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError(string.Empty, "Erro ao excluir cliente.");
            }
            return View();
        }

        private async Task<ActionResult> ObterCliente(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseAddress);

                var result = await client.GetAsync($"clientes/{id}");

                if (result.IsSuccessStatusCode)
                {
                    string retorno = await result.Content.ReadAsStringAsync();
                    cliente = JsonConvert.DeserializeObject<Cliente>(retorno);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao obter dados do cliente.");
                }
            }

            if (cliente == null)
            {
                return HttpNotFound();
            }

            return View(cliente);
        }
    }
}