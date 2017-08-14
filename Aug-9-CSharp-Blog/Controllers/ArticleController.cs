using Aug_9_CSharp_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace Aug_9_CSharp_Blog.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		// GET: Article/List
		public ActionResult List()
		{
			using (var db = new BlogDbContext())
			{
				var articles = db.Articles.Include(a => a.Author).ToList();
				return View(articles);
			}
			
		}

		// GET: Article/Details
		public ActionResult Details(int? id)
		{
			using (var db = new BlogDbContext())
			{
				var article = db.Articles.Where(a => a.Id == id).Include(a => a.Author).FirstOrDefault();
				if (article == null) return HttpNotFound();

				return View(article);
			}

		}

		// GET: Article/Create
		[Authorize]
		public ActionResult Create()
		{
				return View();
		}

		// POST: Article/Create
		[HttpPost]
		[Authorize]
		public ActionResult Create(Article article)
		{
			if (ModelState.IsValid)
			{
				using (var db = new BlogDbContext())
				{
					var authorId = db.Users.Where(u => u.UserName == this.User.Identity.Name)
						.FirstOrDefault().Id;
					article.AuthorId = authorId;

					db.Articles.Add(article);
					db.SaveChanges();

					return RedirectToAction("Index");
				}
			}
			return View(article);
		}

		// GET: Article/Delete
		[Authorize]
		public ActionResult Delete(int? id)
		{
			using (var db = new BlogDbContext())
			{
				var article = db.Articles.Find(id);

				if (article == null) return HttpNotFound();

				if (!isAuthorized(article))
				{
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				}

				return View(article);
			}
			
		}

		// POST: Article/Delete
		[HttpPost]
		[Authorize]
		[ActionName("Delete")]
		public ActionResult DeleteConfirm(int? id)
		{
			using (var db = new BlogDbContext())
			{
				var article = db.Articles.Find(id);

				if (article == null) return HttpNotFound();

				if (!isAuthorized(article))
				{
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				}

				db.Articles.Remove(article);
				db.SaveChanges();

				return RedirectToAction("Index");
			}

		}

		// GET: Article/Edit
		[Authorize]
		public ActionResult Edit(int? id)
		{
			using (var db = new BlogDbContext())
			{
				var article = db.Articles.Find(id);

				if (article == null) return HttpNotFound();

				if (!isAuthorized(article))
				{
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				}

				var model = new ArticleViewModel();
				model.Id = article.Id;
				model.Title = article.Title;
				model.Content = article.Content;

				return View(model);
			}

		}

		// POST: Article/Edit
		[HttpPost]
		[Authorize]
		public ActionResult Edit(ArticleViewModel model)
		{
			if (ModelState.IsValid)
			{
				using (var db = new BlogDbContext())
				{
					var article = db.Articles.FirstOrDefault(a => a.Id == model.Id);

					if (article == null) return HttpNotFound();

					if (!isAuthorized(article))
					{
						return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
					}

					article.Title = model.Title;
					article.Content = model.Content;

					db.Entry(article).State = EntityState.Modified;
					db.SaveChanges();

					return RedirectToAction("Index");
				}
			}
			return View(model);
		}

		public bool isAuthorized (Article article)
		{
			bool isAdmin = this.User.IsInRole("Admin");
			bool isAuthor = article.IsAuthor(this.User.Identity.Name);

			return isAdmin || isAuthor;
		}
	}
}