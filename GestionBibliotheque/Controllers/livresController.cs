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
    public class livresController : Controller
    {
        private BaseDonneesBiblioEntities db = new BaseDonneesBiblioEntities();

        // GET: livres
        public ActionResult Index()
        {
            return View(db.livres.ToList());
        }

        // GET: livres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            livres livres = db.livres.Find(id);
            if (livres == null)
            {
                return HttpNotFound();
            }
            return View(livres);
        }

        // GET: livres/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Enfant",
                Value = "Enfant"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Jeunesse",
                Value = "Jeunesse"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Adolescent",
                Value = "Adolescent"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Adulte",
                Value = "Adulte"
            });
            ViewBag.categorie = listItems;

            return View();
        }

        // POST: livres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Enregistrer(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                livres livre = new livres();
  
                livre.titre = collection["titre"];
                livre.auteur = collection["auteur"];
                livre.nbPage = collection["nbPage"];
                livre.anneeeEdition = collection["anneeeEdition"];
                livre.categorie = collection["categorie"];
                livre.ISBN = collection["ISBN"];

                db.livres.Add(livre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        // GET: livres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            livres livres = db.livres.Find(id);
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Enfant",
                Value = "Enfant"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Jeunesse",
                Value = "Jeunesse"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Adolescent",
                Value = "Adolescent"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Adulte",
                Value = "Adulte"
            });

            ViewBag.categorie = listItems;

            if (livres == null)
            {
                return HttpNotFound();
            }
            return View(livres);
        }

        // POST: livres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public ActionResult Edit([Bind(Include = "idLivre,titre,auteur,nbPage,categorie,anneeeEdition,ISBN")] livres livres)
        {
            if (ModelState.IsValid)
            {
                db.Entry(livres).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(livres);
        }

        // GET: livres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            livres livres = db.livres.Find(id);
            if (livres == null)
            {
                return HttpNotFound();
            }
            return View(livres);
        }

        // POST: livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            livres livres = db.livres.Find(id);
            db.livres.Remove(livres);
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

        public ActionResult Recherche(string titreRecherche,
           string auteurRecherche, string nbPagesRecherche, string anneeEdRecherchee,
           string categorieRecherchee, string ISBNRecherche)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Enfant",
                Value = "Enfant"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Jeunesse",
                Value = "Jeunesse"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Adolescent",
                Value = "Adolescent"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Adulte",
                Value = "Adulte"
            });

            ViewBag.categorie = listItems;
            //Response.Write("Voyons l'auteur:" + auteurRecherche);
            var livres = from l in db.livres
                         select l;
            livres = livres.Where(
                   l => (l.titre.Contains(titreRecherche))
                    && (l.auteur.Contains(auteurRecherche))
                    && (l.nbPage.Contains(nbPagesRecherche))
                    && (l.anneeeEdition.Contains(anneeEdRecherchee))
                    && (l.categorie.Contains(categorieRecherchee))
                    && (l.ISBN.Contains(ISBNRecherche))
                    );
            //Response.Write("Voyons les livres:" + livres.ToList());

            /*Ajouter recherche avec id du livre si il est emprunter genre count where id = idlivre et la date de retour est null si count > 0 donne un string emprunter*/
            return View(livres);
        }
    }
}
