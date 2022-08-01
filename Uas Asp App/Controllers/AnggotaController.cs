using Microsoft.AspNetCore.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using Uas_Asp_App.Models;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Uas_Asp_App.Controllers
{
    public class AnggotaController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "fxCxN5UljVB75RoZhbTmhQtsmMu0IUQSgJSTvHjp",
            BasePath = "https://uasaspapp-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Anggota");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Anggota>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Anggota>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Anggota/" + id);
            Anggota data = JsonConvert.DeserializeObject<Anggota>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Anggota anggota)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Anggota/" + anggota.npm, anggota);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Anggota/" + id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Anggota anggota)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = anggota;
                PushResponse response = client.Push("Anggota/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Anggota/" + data.Id, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Added Succesfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }


    }
}
