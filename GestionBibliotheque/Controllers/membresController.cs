using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionBibliotheque.Models;
using System.Data.SqlClient;

namespace GestionBibliotheque.Controllers
{
    public class membresController : Controller
    {
        private BaseDonneesBiblioEntities db = new BaseDonneesBiblioEntities();
        
        // GET: membres
        public ActionResult Index()
        {
            var membres = db.membres.Include(m => m.adresses);
            return View(membres.ToList());
        }

        // GET: membres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            membres membres = db.membres.Find(id);
            if (membres == null)
            {
                return HttpNotFound();
            }
            return View(membres);
        }

        // GET: membres/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Homme",
                Value = "H"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Femme",
                Value = "F"
            });

            ViewBag.genre = listItems;

            ViewBag.idAdresse = new SelectList(db.adresses, "idAdresse", "rue");
            return View();
        }

        // POST: membres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Enregistrer(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                adresses adresse = new adresses();
                adresse.noCivique = collection["noCivique"];
                adresse.rue = collection["rue"];
                adresse.ville = collection["ville"];
                adresse.province = collection["province"];
                adresse.codePostal = collection["codePostal"];
                db.adresses.Add(adresse);             
                int idadresse = db.adresses.Max(model => model.idAdresse);

                membres membre = new membres();             
                membre.idAdresse = idadresse;
                membre.nom = collection["nom"];
                membre.prenom = collection["prenom"];
                membre.genre = collection["genre"];
                membre.dateNaissance = Convert.ToDateTime(collection["dateNaissance"]);
                membre.telephone = collection["telephone"];
                membre.courriel = collection["courriel"];
                db.membres.Add(membre);
                
                db.SaveChanges();
              
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: membres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            membres membres = db.membres.Find(id);
            if (membres == null)
            {
                return HttpNotFound();
            }
            ViewBag.idAdresse = new SelectList(db.adresses, "idAdresse", "rue", membres.idAdresse);

            var uneAdresse = db.adresses.Where(u => u.idAdresse == membres.idAdresse)
                    .Select(u => new
                    {
                        rue = u.rue,
                        noCivique = u.noCivique,
                        ville = u.ville,
                        province = u.province,
                        codePostal = u.codePostal
                    }).Single();

            TempData["noCivique"] = uneAdresse.noCivique;
            TempData["rue"] = uneAdresse.rue;
            TempData["ville"] = uneAdresse.ville;
            TempData["province"] = uneAdresse.province;
            TempData["codePostal"] = uneAdresse.codePostal;
            
            
            return View(membres);
        }

        // POST: membres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                adresses adresse = new adresses();

                adresse.idAdresse = Convert.ToInt32(collection["idAdresse"]);
                adresse.noCivique = collection["noCivique"];
                adresse.rue = collection["rue"];
                adresse.ville = collection["ville"];
                adresse.province = collection["province"];
                adresse.codePostal = collection["codepostal"];

                var adresseUpdateCommand = "UPDATE Adresses SET noCivique=@noCivique, rue=@rue, ville=@ville, province=@province, codePostal=@codePostal WHERE idAdresse=@idAdresse";
                db.Database.ExecuteSqlCommand(adresseUpdateCommand, new SqlParameter("@noCivique", adresse.noCivique), new SqlParameter("@rue", adresse.rue), new SqlParameter("@ville", adresse.ville), new SqlParameter("@province", adresse.province), new SqlParameter("@codePostal", adresse.codePostal), new SqlParameter("@idAdresse", adresse.idAdresse));
                db.SaveChanges();

                membres membre = new membres();

                membre.nom = collection["nom"];
                membre.prenom = collection["prenom"];
                membre.genre = collection["genre"];
                membre.dateNaissance = Convert.ToDateTime(collection["datenaissance"]);
                membre.telephone = collection["telephone"];
                membre.courriel = collection["courriel"];

                var membreUpdateCommand = "UPDATE Membres SET nom=@nom, prenom=@prenom, genre=@genre, dateNaissance=@dateNaissance, telephone=@telephone, courriel=@courriel WHERE idMembre=@idMembre";
                db.Database.ExecuteSqlCommand(membreUpdateCommand, new SqlParameter("@nom", membre.nom), new SqlParameter("@prenom", membre.prenom), new SqlParameter("@genre", membre.genre), new SqlParameter("@dateNaissance", membre.dateNaissance), new SqlParameter("@telephone", membre.telephone), new SqlParameter("@courriel", membre.courriel), new SqlParameter("@idMembre", membre.idMembre));
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: membres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            membres membres = db.membres.Find(id);
            if (membres == null)
            {
                return HttpNotFound();
            }
            return View(membres);
        }

        // POST: membres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            membres membres = db.membres.Find(id);
            db.membres.Remove(membres);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Recherche(string prenomRecherche,
            string nomRecherche, string genreRecherche, string dateNaissanceRecherchee,
            string telephoneRecherche, string courrielRecherche,
            string noCiviqueRecherche, string rueRecherchee, string codeRecherche,
            string villeRecherchee, string provinceRecherchee
             )
        {
            var membres = from m in db.membres
                          select m;

            membres = membres.Where(
                   s => (s.prenom.Contains(prenomRecherche))
                    && (s.nom.Contains(nomRecherche))
                    && (s.genre.Contains(genreRecherche))
                    && (s.dateNaissance.ToString().Contains(dateNaissanceRecherchee))
                    && (s.telephone.Contains(telephoneRecherche))
                    && (s.courriel.Contains(courrielRecherche))
                    && (s.adresses.noCivique.Contains(noCiviqueRecherche))
                    && (s.adresses.rue.Contains(rueRecherchee))
                    && (s.adresses.codePostal.Contains(codeRecherche))
                    && (s.adresses.ville.Contains(villeRecherchee))
                    && (s.adresses.province.Contains(provinceRecherchee))
                    );

            return View(membres);
        }
    }
}
