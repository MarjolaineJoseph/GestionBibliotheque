using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionBibliotheque.Models;

namespace GestionBibliotheque.Controllers
{
    public class pretsController : Controller
    {
        private BaseDonneesBiblioEntities db = new BaseDonneesBiblioEntities();

        // GET: prets
        public ActionResult Index()
        {
            var prets = db.prets.Include(p => p.livres).Include(p => p.membres);
            return View(prets.ToList());
        }

        // GET: prets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            prets prets = db.prets.Find(id);
            if (prets == null)
            {
                return HttpNotFound();
            }
            return View(prets);
        }

        // GET: prets/Create
        public ActionResult Create()
        {
            string queryMembres = "SELECT * FROM membres WHERE idMembre IN (SELECT idMembre FROM prets WHERE dateRetourPret IS NULL GROUP BY idMembre HAVING COUNT(idMembre) < 3) OR idMembre NOT IN (SELECT idMembre FROM prets)";
            var membresliste = db.Database.SqlQuery<membres>(queryMembres).ToList();
            
            /*changer ce query quand il y aura plus de prets*/
            string queryLivres = "SELECT * FROM livres WHERE idLivre NOT IN(SELECT idLivre FROM prets)";
            var livresliste = db.Database.SqlQuery<livres>(queryLivres).ToList();

            ViewBag.idLivre = new SelectList(livresliste, "idLivre", "titre");
            ViewBag.idMembre = new SelectList(membresliste, "idMembre", "prenom");

            return View();
        }

        // POST: prets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]       
        public ActionResult Enregistrer(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                prets pret = new prets();
                pret.idLivre = Convert.ToInt32(collection["idLivre"]);
                pret.idMembre = Convert.ToInt32(collection["idMembre"]);
                pret.datePret = Convert.ToDateTime(collection["datePret"]);

                db.prets.Add(pret);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        // GET: prets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            prets prets = db.prets.Find(id);
            if (prets == null)
            {
                return HttpNotFound();
            }
            ViewBag.idLivre = new SelectList(db.livres, "idLivre", "titre", prets.idLivre);
            ViewBag.idMembre = new SelectList(db.membres, "idMembre", "prenom", prets.idMembre);
            return View(prets);
        }

        // POST: prets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idPret,idMembre,idLivre,datePret,dateRetourPret")] prets prets)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prets).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idLivre = new SelectList(db.livres, "idLivre", "titre", prets.idLivre);
            ViewBag.idMembre = new SelectList(db.membres, "idMembre", "prenom", prets.idMembre);
            return View(prets);
        }

        // GET: prets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            prets prets = db.prets.Find(id);
            if (prets == null)
            {
                return HttpNotFound();
            }
            return View(prets);
        }

        // POST: prets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            prets prets = db.prets.Find(id);
            db.prets.Remove(prets);
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

        public ActionResult Recherche(string nomMembreRecherche,
            string prenomMembreRecherche, string titreLivreRecherche,
            string datePretRecherchee, string dateRetourRecherchee)
        {
            var prets = from p in db.prets
                        select p;

            prets = prets.Where(o => (o.membres.prenom.Contains(prenomMembreRecherche) &&
                                     (o.membres.nom.Contains(nomMembreRecherche) &&
                                     (o.livres.titre.Contains(titreLivreRecherche) &&
                                     (o.datePret.ToString().Contains(datePretRecherchee) &&
                                     (o.dateRetourPret.ToString().Contains(dateRetourRecherchee)))))));

            return View(prets);
        }

        public ActionResult Retards()
        {
            var prets = db.prets.Include(p => p.livres).Include(p => p.membres);

            var retardsPrets = prets.ToList();
            
            var retards = db.prets.SqlQuery("SELECT * FROM prets INNER JOIN membres ON membres.idmembre = prets.idmembre INNER JOIN livres ON livres.idlivre = prets.idlivre WHERE DATEDIFF(day, prets.datePret, GETDATE()) > 3 AND prets.dateRetourPret IS NULL").ToList();

            retardsPrets = retards;
            return View(retardsPrets);
        }
    }
}
